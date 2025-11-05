using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceZakatManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reportService;

    public ReportsController(ReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ReportSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummaryAsync([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
    {
        var summary = await _reportService.GetSummaryAsync(from, to, cancellationToken);
        return Ok(summary);
    }

    [HttpGet("cashflow")]
    [ProducesResponseType(typeof(CashflowReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCashflowAsync([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
    {
        var cashflow = await _reportService.GetCashflowAsync(from, to, cancellationToken);
        return Ok(cashflow);
    }

    [HttpGet("category-spend")]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategorySpendDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategorySpendAsync([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
    {
        var spend = await _reportService.GetCategorySpendAsync(from, to, cancellationToken);
        return Ok(spend);
    }
}
