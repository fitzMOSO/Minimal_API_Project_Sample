using Microsoft.EntityFrameworkCore;
using Minimal_API_Project_Sample.Data;
using Minimal_API_Project_Sample.Models;

namespace Minimal_API_Project_Sample.Repositories;

/// <summary>
/// Database repository implementation using Entity Framework Core
/// Follows SOLID principles:
/// - Single Responsibility: Handles only data access for products
/// - Liskov Substitution: Drop-in replacement for IProductRepository
/// - Dependency Inversion: Implements abstraction
/// </summary>
public class DatabaseProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseProductRepository> _logger;

    public DatabaseProductRepository(
        ApplicationDbContext context,
        ILogger<DatabaseProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all products from database");
        return await _context.Products
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving product with ID: {ProductId} from database", id);
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _logger.LogInformation("Creating product in database: {ProductName}", product.Name);
        
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = null;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        _logger.LogInformation("Updating product with ID: {ProductId} in database", id);
        
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for update", id);
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Product updated with ID: {ProductId}", id);
        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId} from database", id);
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for deletion", id);
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Product deleted with ID: {ProductId}", id);
        return true;
    }
}
