using FinanceZakatManager.Application.Dtos;
using FinanceZakatManager.Application.Interfaces;
using FinanceZakatManager.Application.Requests;
using FinanceZakatManager.Application.Services;
using FinanceZakatManager.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceZakatManager.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly IAuditService _auditService;

    public CategoriesController(CategoryService categoryService, IAuditService auditService)
    {
        _categoryService = categoryService;
        _auditService = auditService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var categories = await _categoryService.GetAsync(cancellationToken);
        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync([FromBody] UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.CreateAsync(request, cancellationToken);
        await _auditService.WriteAsync("Create", nameof(CategoryDto), category.Id.ToString(), cancellationToken);
        return CreatedAtAction(nameof(GetAsync), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpsertCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _categoryService.UpdateAsync(id, request, cancellationToken);
        if (category is null)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Update", nameof(CategoryDto), id.ToString(), cancellationToken);
        Response.Headers.ETag = EtagGenerator.FromStrings(category.Id.ToString(), category.CreatedUtc.ToString("O"));
        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        await _auditService.WriteAsync("Delete", nameof(CategoryDto), id.ToString(), cancellationToken);
        return NoContent();
    }
}
