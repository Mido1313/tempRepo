using System.Globalization;
using System.Linq;
using System.Text;
using FinanceZakatManager.Api.Utils;
using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceZakatManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly IAuditService _auditService;

    public TransactionsController(TransactionService transactionService, IAuditService auditService)
    {
        _transactionService = transactionService;
        _auditService = auditService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] Guid? accountId, [FromQuery] Guid? categoryId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] string? q, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionService.GetAsync(accountId, categoryId, from, to, q, page, pageSize, cancellationToken);
        return Ok(transactions);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var transaction = await _transactionService.GetByIdAsync(id, cancellationToken);
        if (transaction is null)
        {
            return NotFound();
        }

        Response.Headers.ETag = EtagGenerator.FromStrings(transaction.Id.ToString(), transaction.CreatedUtc.ToString("O"));
        return Ok(transaction);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] UpsertTransactionRequest request, CancellationToken cancellationToken)
    {
        var created = await _transactionService.CreateAsync(request, cancellationToken);
        await _auditService.WriteAsync("Create", nameof(TransactionDto), created.Id.ToString(), cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpPost("import")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportAsync([FromForm] IFormFile file, [FromQuery] string? culture, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(Problem("CSV file is required."));
        }

        var requests = await ParseCsvAsync(file, culture);
        var imported = await _transactionService.ImportAsync(requests, cancellationToken);
        if (imported > 0)
        {
            await _auditService.WriteAsync("Import", nameof(TransactionDto), imported.ToString(CultureInfo.InvariantCulture), cancellationToken);
        }

        return Ok(new { imported });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertTransactionRequest request, CancellationToken cancellationToken)
    {
        var updated = await _transactionService.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Update", nameof(TransactionDto), id.ToString(), cancellationToken);
        Response.Headers.ETag = EtagGenerator.FromStrings(updated.Id.ToString(), updated.CreatedUtc.ToString("O"));
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _transactionService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Delete", nameof(TransactionDto), id.ToString(), cancellationToken);
        return NoContent();
    }

    private static async Task<List<UpsertTransactionRequest>> ParseCsvAsync(IFormFile file, string? culture)
    {
        var requests = new List<UpsertTransactionRequest>();
        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);
        var headerLine = await reader.ReadLineAsync() ?? throw new InvalidOperationException("CSV header missing");
        var delimiter = DetectDelimiter(headerLine);
        var headers = ParseCsvLine(headerLine, delimiter)
            .Select(NormalizeHeader)
            .ToArray();
        var headerLookup = headers
            .Select((value, index) => new { value, index })
            .GroupBy(x => x.value)
            .ToDictionary(x => x.Key, x => x.First().index);
        string? line;
        var format = string.IsNullOrEmpty(culture) ? CultureInfo.InvariantCulture : new CultureInfo(culture);
        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = ParseCsvLine(line, delimiter);
            string? GetValue(string key) => headerLookup.TryGetValue(NormalizeHeader(key), out var index) && index < parts.Length
                ? parts[index].Trim()
                : null;

            var request = new UpsertTransactionRequest
            {
                AccountId = Guid.Parse(GetValue("account") ?? throw new InvalidOperationException("Account column required")),
                CategoryId = Guid.TryParse(GetValue("category"), out var cat) ? cat : null,
                Amount = decimal.Parse(GetValue("amount") ?? "0", NumberStyles.Number | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowParentheses, format),
                Currency = (GetValue("currency") ?? "USD").ToUpperInvariant(),
                BookedOn = DateOnly.FromDateTime(DateTime.Parse(GetValue("date") ?? GetValue("bookedon") ?? throw new InvalidOperationException("Date column required"), format)),
                Note = GetValue("note"),
                Tags = (GetValue("tags") ?? string.Empty)
                    .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList(),
                ExternalId = GetValue("externalid"),
                IsTransfer = string.Equals(GetValue("istransfer"), "true", StringComparison.OrdinalIgnoreCase)
            };

            requests.Add(request);
        }

        return requests;
    }

    private static char DetectDelimiter(string headerLine)
    {
        if (headerLine.Contains(';') && !headerLine.Contains(','))
        {
            return ';';
        }

        return ',';
    }

    private static string[] ParseCsvLine(string line, char delimiter)
    {
        var values = new List<string>();
        var builder = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    builder.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }

                continue;
            }

            if (c == delimiter && !inQuotes)
            {
                values.Add(builder.ToString());
                builder.Clear();
                continue;
            }

            if (c != '\r')
            {
                builder.Append(c);
            }
        }

        values.Add(builder.ToString());
        return values.ToArray();
    }

    private static string NormalizeHeader(string header)
    {
        var span = header.AsSpan().Trim();
        Span<char> buffer = stackalloc char[span.Length];
        var count = 0;
        foreach (var c in span)
        {
            if (char.IsLetterOrDigit(c))
            {
                buffer[count++] = char.ToLowerInvariant(c);
            }
        }

        return new string(buffer[..count]);
    }
}
