using FluentValidation;
using Minimal_API_Project_Sample.DTOs;
using Minimal_API_Project_Sample.Mappers;
using Minimal_API_Project_Sample.Repositories;

namespace Minimal_API_Project_Sample.Services;

/// <summary>
/// Product service implementation following SOLID principles:
/// - Single Responsibility: Handles only product business logic
/// - Open/Closed: Open for extension through interfaces, closed for modification
/// - Liskov Substitution: Can be substituted with any IProductService implementation
/// - Interface Segregation: Implements focused interface
/// - Dependency Inversion: Depends on abstractions (IProductRepository, IProductMapper, IValidator)
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IProductMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository repository,
        IProductMapper mapper,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        ILogger<ProductService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        _logger.LogInformation("Getting all products");
        var products = await _repository.GetAllAsync();
        return products.Select(_mapper.ToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        var product = await _repository.GetByIdAsync(id);
        return product is not null ? _mapper.ToDto(product) : null;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        _logger.LogInformation("Creating new product: {ProductName}", dto.Name);
        
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var product = _mapper.ToEntity(dto);
        var created = await _repository.CreateAsync(product);
        
        _logger.LogInformation("Product created with ID: {ProductId}", created.Id);
        return _mapper.ToDto(created);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingProduct = await _repository.GetByIdAsync(id);
        if (existingProduct is null)
        {
            _logger.LogWarning("Product with ID: {ProductId} not found", id);
            return null;
        }

        _mapper.UpdateEntity(existingProduct, dto);
        var updated = await _repository.UpdateAsync(id, existingProduct);
        
        _logger.LogInformation("Product updated with ID: {ProductId}", id);
        return updated is not null ? _mapper.ToDto(updated) : null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        var result = await _repository.DeleteAsync(id);
        
        if (result)
        {
            _logger.LogInformation("Product deleted with ID: {ProductId}", id);
        }
        else
        {
            _logger.LogWarning("Product with ID: {ProductId} not found for deletion", id);
        }
        
        return result;
    }
}
