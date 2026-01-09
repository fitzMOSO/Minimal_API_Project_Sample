namespace Minimal_API_Project_Sample.Repositories;

/// <summary>
/// Generic repository interface following SOLID principles:
/// - Interface Segregation: Focused interface for common CRUD operations
/// - Open/Closed: Open for extension through generic type parameter
/// </summary>
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(int id, T entity);
    Task<bool> DeleteAsync(int id);
}
