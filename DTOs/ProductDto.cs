namespace Minimal_API_Project_Sample.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock
);

public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock
);

public record UpdateProductDto(
    string? Name,
    string? Description,
    decimal? Price,
    int? Stock
);
