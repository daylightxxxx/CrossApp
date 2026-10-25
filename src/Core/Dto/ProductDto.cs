namespace Core.Dto;

public record ProductDto(
    string Id,
    string Name,
    decimal Price);

public record CustomerDto(
    string Id,
    string Name,
    string? Phone = null);

public sealed record ImportResult<T>(IReadOnlyList<T> Items, IReadOnlyList<string> Errors);