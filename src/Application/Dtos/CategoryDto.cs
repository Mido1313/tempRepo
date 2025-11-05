using FinanceZakatManager.Domain.Enums;

namespace FinanceZakatManager.Application.Dtos;

public record CategoryDto(
    Guid Id,
    string Name,
    CategoryType Type,
    string ColorHex,
    DateTime CreatedUtc
);
