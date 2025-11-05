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
[Route("api/v1/budgets")]
public class BudgetsController : ControllerBase
{
    private readonly BudgetService _budgetService;
    private readonly IAuditService _auditService;

    public BudgetsController(BudgetService budgetService, IAuditService auditService)
    {
        _budgetService = budgetService;
        _auditService = auditService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BudgetDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var budgets = await _budgetService.GetAsync(cancellationToken);
        return Ok(budgets);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var budget = await _budgetService.GetByIdAsync(id, cancellationToken);
        if (budget is null)
        {
            return NotFound();
        }

        Response.Headers.ETag = EtagGenerator.FromStrings(budget.Id.ToString(), budget.CreatedUtc.ToString("O"));
        return Ok(budget);
    }

    [HttpGet("{id:guid}/progress")]
    [ProducesResponseType(typeof(BudgetProgressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProgressAsync(Guid id, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken cancellationToken)
    {
        var progress = await _budgetService.GetProgressAsync(id, from, to, cancellationToken);
        if (progress is null)
        {
            return NotFound();
        }

        return Ok(progress);
    }

    [HttpPost]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] UpsertBudgetRequest request, CancellationToken cancellationToken)
    {
        var budget = await _budgetService.CreateAsync(request, cancellationToken);
        await _auditService.WriteAsync("Create", nameof(BudgetDto), budget.Id.ToString(), cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = budget.Id }, budget);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertBudgetRequest request, CancellationToken cancellationToken)
    {
        var budget = await _budgetService.UpdateAsync(id, request, cancellationToken);
        if (budget is null)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Update", nameof(BudgetDto), id.ToString(), cancellationToken);
        return Ok(budget);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _budgetService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Delete", nameof(BudgetDto), id.ToString(), cancellationToken);
        return NoContent();
    }
}
