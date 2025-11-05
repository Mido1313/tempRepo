using FinanceZakatManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinanceZakatManager.Application.Requests;

public class UpsertCategoryRequest
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public CategoryType Type { get; set; }

    [RegularExpression("^#?[0-9a-fA-F]{6}$")]
    public string ColorHex { get; set; } = "#4F9FD8";
}
