using FinanceZakatManager.Api.Utils;
using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceZakatManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/prices")]
public class PricesController : ControllerBase
{
    private readonly PriceService _priceService;

    public PricesController(PriceService priceService)
    {
        _priceService = priceService;
    }

    [HttpGet("latest")]
    [ProducesResponseType(typeof(PriceQuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestAsync([FromQuery] string symbol, [FromQuery] string currency, CancellationToken cancellationToken)
    {
        var quote = await _priceService.GetLatestAsync(symbol, currency, cancellationToken);
        if (quote is null)
        {
            return NotFound();
        }

        Response.Headers.ETag = EtagGenerator.FromStrings(quote.Id.ToString(), quote.QuotedAtUtc.ToString("O"));
        return Ok(quote);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [ProducesResponseType(typeof(PriceQuoteDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] CreatePriceQuoteRequest request, CancellationToken cancellationToken)
    {
        var created = await _priceService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetLatestAsync), new { symbol = created.Symbol, currency = created.Currency }, created);
    }
}
