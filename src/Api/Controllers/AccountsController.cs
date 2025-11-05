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
[Route("api/v1/accounts")]
public class AccountsController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly IAuditService _auditService;

    public AccountsController(AccountService accountService, IAuditService auditService)
    {
        _accountService = accountService;
        _auditService = auditService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetAsync(cancellationToken);
        return Ok(accounts);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _accountService.GetByIdAsync(id, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        Response.Headers.ETag = EtagGenerator.FromStrings(account.Id.ToString(), account.CreatedUtc.ToString("O"));
        return Ok(account);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var created = await _accountService.CreateAsync(request, cancellationToken);
        await _auditService.WriteAsync("Create", nameof(AccountDto), created.Id.ToString(), cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var updated = await _accountService.UpdateAsync(id, request, cancellationToken);
        if (updated is null)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Update", nameof(AccountDto), id.ToString(), cancellationToken);
        Response.Headers.ETag = EtagGenerator.FromStrings(updated.Id.ToString(), updated.CreatedUtc.ToString("O"), updated.IsArchived.ToString());
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _accountService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Delete", nameof(AccountDto), id.ToString(), cancellationToken);
        return NoContent();
    }
}
