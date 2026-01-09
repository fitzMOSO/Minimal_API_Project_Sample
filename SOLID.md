# SOLID Principles in This Project

This document explains how each SOLID principle is implemented in the Minimal API Project Sample.

## 1. Single Responsibility Principle (SRP)
**Definition:** A class should have only one reason to change.

### Examples in Our Project:

#### ? Before (Violates SRP):
```csharp
// One class doing everything - multiple responsibilities
public class ProductController
{
    public async Task<IActionResult> GetProduct(int id)
    {
        // HTTP concerns
        if (id <= 0) return BadRequest();
        
        // Data access
        var product = _products.FirstOrDefault(p => p.Id == id);
        
        // Mapping
        var dto = new ProductDto(product.Id, product.Name, ...);
        
        // Validation
        if (string.IsNullOrEmpty(dto.Name)) return BadRequest();
        
        return Ok(dto);
    }
}
```

#### ? After (Follows SRP):
```csharp
// ProductEndpoints.cs - Only HTTP concerns
public static class ProductEndpoints
{
    private static async Task<Results<Ok<ProductDto>, NotFound>> GetProductById(
        int id, IProductService productService)
    {
        var product = await productService.GetProductByIdAsync(id);
        return product is not null ? TypedResults.Ok(product) : TypedResults.NotFound();
    }
}

// ProductService.cs - Only business logic
public class ProductService : IProductService
{
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product is not null ? _mapper.ToDto(product) : null;
    }
}

// InMemoryProductRepository.cs - Only data access
public class InMemoryProductRepository : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);
    }
}

// ProductMapper.cs - Only mapping
public class ProductMapper : IProductMapper
{
    public ProductDto ToDto(Product product)
    {
        return new ProductDto(product.Id, product.Name, ...);
    }
}
```

---

## 2. Open/Closed Principle (OCP)
**Definition:** Software entities should be open for extension but closed for modification.

### Examples in Our Project:

#### ? Generic Repository Interface:
```csharp
// IRepository.cs - Base interface (closed for modification)
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(int id, T entity);
    Task<bool> DeleteAsync(int id);
}

// IProductRepository.cs - Extended interface (open for extension)
public interface IProductRepository : IRepository<Product>
{
    // Can add product-specific methods without changing IRepository
    // Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal min, decimal max);
}
```

#### ? Multiple Repository Implementations:
```csharp
// Add new implementations WITHOUT modifying existing code
public class InMemoryProductRepository : IProductRepository { }
public class DatabaseProductRepository : IProductRepository { }
public class ApiProductRepository : IProductRepository { }  // Can add this without changing anything
public class CachedProductRepository : IProductRepository { }  // Or this!

// Switch implementations in Program.cs:
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
// or
builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();
```

---

## 3. Liskov Substitution Principle (LSP)
**Definition:** Objects should be replaceable with instances of their subtypes without altering correctness.

### Examples in Our Project:

#### ? Interchangeable Repository Implementations:
```csharp
// ProductService works with ANY IProductRepository implementation
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;  // Uses abstraction
    
    public ProductService(IProductRepository repository)
    {
        _repository = repository;  // Any implementation works
    }
    
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        // This code works identically whether using:
        // - InMemoryProductRepository
        // - DatabaseProductRepository
        // - ApiProductRepository
        var product = await _repository.GetByIdAsync(id);
        return product is not null ? _mapper.ToDto(product) : null;
    }
}
```

#### ? All implementations honor the contract:
```csharp
// All implementations must:
// 1. Return null when product not found (not throw exception)
// 2. Return the product when found
// 3. Handle async properly

public class InMemoryProductRepository : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(product);  // Returns null if not found ?
    }
}

public class DatabaseProductRepository : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);  // Returns null if not found ?
    }
}
```

---

## 4. Interface Segregation Principle (ISP)
**Definition:** No client should be forced to depend on methods it doesn't use.

### Examples in Our Project:

#### ? Before (Violates ISP):
```csharp
// Fat interface - forces implementation of unused methods
public interface IProductOperations
{
    // CRUD operations
    Task<Product> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task CreateAsync(Product product);
    
    // Caching operations - not all implementations need these!
    void ClearCache();
    void WarmUpCache();
    
    // Export operations - not all implementations need these!
    Task<byte[]> ExportToCsvAsync();
    Task<byte[]> ExportToExcelAsync();
    
    // Analytics operations - not all implementations need these!
    Task<ProductStatistics> GetStatisticsAsync();
}
```

#### ? After (Follows ISP):
```csharp
// Focused interfaces - clients only depend on what they need

// Base repository operations
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(int id, T entity);
    Task<bool> DeleteAsync(int id);
}

// Product-specific operations
public interface IProductRepository : IRepository<Product>
{
    // Only product-specific methods here
}

// Separate interface for caching (if needed)
public interface ICacheable
{
    void ClearCache();
    void WarmUpCache();
}

// Separate interface for exports (if needed)
public interface IExportable
{
    Task<byte[]> ExportToCsvAsync();
}

// Now implementations only implement what they need:
public class InMemoryProductRepository : IProductRepository { }  // Just CRUD

public class CachedProductRepository : IProductRepository, ICacheable { }  // CRUD + Caching
```

---

## 5. Dependency Inversion Principle (DIP)
**Definition:** High-level modules should not depend on low-level modules. Both should depend on abstractions.

### Examples in Our Project:

#### ? Before (Violates DIP):
```csharp
// High-level ProductService depends on low-level concrete class
public class ProductService
{
    private readonly InMemoryProductRepository _repository;  // Concrete dependency!
    
    public ProductService()
    {
        _repository = new InMemoryProductRepository();  // Tightly coupled!
    }
}
```

#### ? After (Follows DIP):
```csharp
// High-level ProductService depends on abstraction
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;      // Abstraction ?
    private readonly IProductMapper _mapper;              // Abstraction ?
    private readonly IValidator<CreateProductDto> _validator;  // Abstraction ?
    
    // Dependencies injected through constructor
    public ProductService(
        IProductRepository repository,      // Any implementation can be injected
        IProductMapper mapper,
        IValidator<CreateProductDto> validator)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
    }
}

// Dependency configuration in Program.cs
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductMapper, ProductMapper>();

// Easy to swap implementations:
// builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();
```

#### ? Dependency Flow:
```
???????????????????????????????????????
?      ProductEndpoints               ?
?  (High-level - Presentation)        ?
?                                     ?
?  Depends on: IProductService        ? ? Abstraction
???????????????????????????????????????
              ?
???????????????????????????????????????
?      ProductService                 ?
?  (High-level - Business Logic)      ?
?                                     ?
?  Depends on: IProductRepository     ? ? Abstraction
?              IProductMapper         ? ? Abstraction
?              IValidator             ? ? Abstraction
???????????????????????????????????????
              ?
???????????????????????????????????????
?  InMemoryProductRepository          ?
?  (Low-level - Data Access)          ?
?                                     ?
?  Implements: IProductRepository     ? ? Implements abstraction
???????????????????????????????????????
```

---

## Benefits of SOLID in This Project

### 1. **Testability**
```csharp
// Easy to test with mocks
var mockRepository = new Mock<IProductRepository>();
var mockMapper = new Mock<IProductMapper>();
var service = new ProductService(mockRepository.Object, mockMapper.Object, ...);

// Test service without hitting database
mockRepository.Setup(r => r.GetByIdAsync(1))
              .ReturnsAsync(new Product { Id = 1, Name = "Test" });
```

### 2. **Maintainability**
- Change data source? Just swap repository implementation
- Change validation rules? Only modify validators
- Change DTOs? Only modify mappers
- Each change is localized to one class

### 3. **Extensibility**
```csharp
// Add Redis caching WITHOUT changing existing code
public class CachedProductRepository : IProductRepository
{
    private readonly IProductRepository _innerRepository;
    private readonly IDistributedCache _cache;
    
    public async Task<Product?> GetByIdAsync(int id)
    {
        var cached = await _cache.GetStringAsync($"product:{id}");
        if (cached != null) return JsonSerializer.Deserialize<Product>(cached);
        
        var product = await _innerRepository.GetByIdAsync(id);
        await _cache.SetStringAsync($"product:{id}", JsonSerializer.Serialize(product));
        return product;
    }
}

// Register in Program.cs:
builder.Services.AddScoped<IProductRepository, CachedProductRepository>();
```

### 4. **Flexibility**
```csharp
// Switch between implementations based on configuration
if (builder.Configuration.GetValue<bool>("UseDatabase"))
{
    builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();
}
else
{
    builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
}
```

---

## Summary

| Principle | Implementation | Benefit |
|-----------|----------------|---------|
| **SRP** | Separate classes for endpoints, services, repositories, mappers, validators | Each class has one reason to change |
| **OCP** | Generic `IRepository<T>`, multiple implementations | Add new features without modifying existing code |
| **LSP** | All repository implementations honor the same contract | Implementations are interchangeable |
| **ISP** | Focused interfaces (`IProductService`, `IProductRepository`) | Clients only depend on what they need |
| **DIP** | Depend on interfaces, inject through constructors | Loose coupling, easy testing |

This architecture is **maintainable**, **testable**, **extensible**, and follows industry best practices! ??
