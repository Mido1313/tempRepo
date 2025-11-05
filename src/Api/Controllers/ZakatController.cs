using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceZakatManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/zakat")]
public class ZakatController : ControllerBase
{
    private readonly ZakatService _zakatService;

    public ZakatController(ZakatService zakatService)
    {
        _zakatService = zakatService;
    }

    [HttpPost("calculate")]
    [ProducesResponseType(typeof(ZakatHistoryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CalculateAsync([FromBody] ZakatCalculationRequest request, CancellationToken cancellationToken)
    {
        var result = await _zakatService.CalculateAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ZakatHistoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistoryAsync(CancellationToken cancellationToken)
    {
        var history = await _zakatService.GetHistoryAsync(cancellationToken);
        return Ok(history);
    }
}
