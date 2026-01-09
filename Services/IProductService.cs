using Minimal_API_Project_Sample.DTOs;
using Minimal_API_Project_Sample.Models;

namespace Minimal_API_Project_Sample.Services;

/// <summary>
/// Service interface for product business logic - Single Responsibility Principle
/// </summary>
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
}
