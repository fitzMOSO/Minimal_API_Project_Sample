namespace Minimal_API_Project_Sample.DTOs;

/// <summary>
/// Represents a product in the system
/// </summary>
/// <param name="Id">Unique identifier for the product</param>
/// <param name="Name">Name of the product</param>
/// <param name="Description">Detailed description of the product</param>
/// <param name="Price">Price in USD</param>
/// <param name="Stock">Available quantity in stock</param>
public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock
);

/// <summary>
/// Data required to create a new product
/// </summary>
/// <param name="Name">Name of the product (required, max 100 characters)</param>
/// <param name="Description">Detailed description of the product (optional, max 500 characters)</param>
/// <param name="Price">Price in USD (must be greater than 0)</param>
/// <param name="Stock">Available quantity in stock (cannot be negative)</param>
public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int Stock
);

/// <summary>
/// Data to update an existing product. All fields are optional - only provide fields you want to update.
/// </summary>
/// <param name="Name">New name for the product (optional, max 100 characters)</param>
/// <param name="Description">New description for the product (optional, max 500 characters)</param>
/// <param name="Price">New price in USD (optional, must be greater than 0 if provided)</param>
/// <param name="Stock">New stock quantity (optional, cannot be negative if provided)</param>
public record UpdateProductDto(
    string? Name,
    string? Description,
    decimal? Price,
    int? Stock
);
