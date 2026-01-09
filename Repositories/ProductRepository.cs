using Minimal_API_Project_Sample.Models;

namespace Minimal_API_Project_Sample.Repositories;

/// <summary>
/// Product-specific repository interface following Interface Segregation Principle
/// Inherits from generic repository and can be extended with product-specific methods
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    // Add product-specific methods here if needed in the future
    // Example: Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal min, decimal max);
}

/// <summary>
/// In-memory implementation of product repository following SOLID principles:
/// - Single Responsibility: Handles only data access for products
/// - Liskov Substitution: Can be replaced with any IProductRepository implementation
/// - Dependency Inversion: Implements abstraction
/// </summary>
public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;
    private readonly ILogger<InMemoryProductRepository> _logger;

    public InMemoryProductRepository(ILogger<InMemoryProductRepository> logger)
    {
        _logger = logger;
        SeedData();
    }

    private void SeedData()
    {
        _products.AddRange(new[]
        {
            new Product
            {
                Id = _nextId++,
                Name = "Laptop",
                Description = "High-performance laptop for developers",
                Price = 1299.99m,
                Stock = 15,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = _nextId++,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse",
                Price = 29.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = _nextId++,
                Name = "Mechanical Keyboard",
                Description = "RGB mechanical keyboard with blue switches",
                Price = 89.99m,
                Stock = 30,
                CreatedAt = DateTime.UtcNow
            }
        });
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all products");
        return Task.FromResult<IEnumerable<Product>>(_products);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving product with ID: {ProductId}", id);
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _nextId++;
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
        
        _logger.LogInformation("Created product with ID: {ProductId}", product.Id);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(int id, Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for update", id);
            return Task.FromResult<Product?>(null);
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Updated product with ID: {ProductId}", id);
        return Task.FromResult<Product?>(existingProduct);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for deletion", id);
            return Task.FromResult(false);
        }

        _products.Remove(product);
        _logger.LogInformation("Deleted product with ID: {ProductId}", id);
        return Task.FromResult(true);
    }
}
