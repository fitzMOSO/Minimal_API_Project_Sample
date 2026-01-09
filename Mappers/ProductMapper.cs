using Minimal_API_Project_Sample.DTOs;
using Minimal_API_Project_Sample.Models;

namespace Minimal_API_Project_Sample.Mappers;

/// <summary>
/// Mapper interface - Dependency Inversion Principle
/// </summary>
public interface IProductMapper
{
    ProductDto ToDto(Product product);
    Product ToEntity(CreateProductDto dto);
    void UpdateEntity(Product entity, UpdateProductDto dto);
}

/// <summary>
/// Product mapper implementation - Single Responsibility Principle
/// Handles all mapping logic between Product and DTOs
/// </summary>
public class ProductMapper : IProductMapper
{
    public ProductDto ToDto(Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock
        );
    }

    public Product ToEntity(CreateProductDto dto)
    {
        return new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock
        };
    }

    public void UpdateEntity(Product entity, UpdateProductDto dto)
    {
        if (dto.Name is not null)
            entity.Name = dto.Name;
        
        if (dto.Description is not null)
            entity.Description = dto.Description;
        
        if (dto.Price.HasValue)
            entity.Price = dto.Price.Value;
        
        if (dto.Stock.HasValue)
            entity.Stock = dto.Stock.Value;
    }
}
