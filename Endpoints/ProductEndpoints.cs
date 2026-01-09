using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Minimal_API_Project_Sample.DTOs;
using Minimal_API_Project_Sample.Services;

namespace Minimal_API_Project_Sample.Endpoints;

/// <summary>
/// Product endpoints following SOLID principles:
/// - Single Responsibility: Handles only HTTP concerns and routing
/// - Open/Closed: Easy to extend with new endpoints
/// - Dependency Inversion: Depends on IProductService abstraction
/// </summary>
public static class ProductEndpoints
{
    public static RouteGroupBuilder MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products")
            .WithTags("Products")
            .WithOpenApi();

        group.MapGet("/", GetAllProducts)
            .WithName("GetAllProducts")
            .WithSummary("Get all products")
            .WithDescription("Retrieves a list of all products in the system")
            .Produces<IEnumerable<ProductDto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get a product by ID")
            .WithDescription("Retrieves a specific product by its unique identifier")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product")
            .WithDescription("Creates a new product with the provided information. All fields are validated.")
            .Produces<ProductDto>(StatusCodes.Status201Created)
            .Produces<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:int}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product")
            .WithDescription("Updates an existing product. Supports partial updates - only provide the fields you want to change.")
            .Produces<ProductDto>(StatusCodes.Status200OK)
            .Produces<Dictionary<string, string[]>>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product")
            .WithDescription("Permanently deletes a product from the system")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<Ok<IEnumerable<ProductDto>>> GetAllProducts(
        IProductService productService)
    {
        var products = await productService.GetAllProductsAsync();
        return TypedResults.Ok(products);
    }

    private static async Task<Results<Ok<ProductDto>, NotFound>> GetProductById(
        int id,
        IProductService productService)
    {
        var product = await productService.GetProductByIdAsync(id);

        return product is not null
            ? TypedResults.Ok(product)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Created<ProductDto>, ValidationProblem>> CreateProduct(
        CreateProductDto dto,
        IProductService productService)
    {
        try
        {
            var created = await productService.CreateProductAsync(dto);
            return TypedResults.Created($"/api/products/{created.Id}", created);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            return TypedResults.ValidationProblem(errors);
        }
    }

    private static async Task<Results<Ok<ProductDto>, NotFound, ValidationProblem>> UpdateProduct(
        int id,
        UpdateProductDto dto,
        IProductService productService)
    {
        try
        {
            var updated = await productService.UpdateProductAsync(id, dto);
            
            return updated is not null
                ? TypedResults.Ok(updated)
                : TypedResults.NotFound();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
            return TypedResults.ValidationProblem(errors);
        }
    }

    private static async Task<Results<NoContent, NotFound>> DeleteProduct(
        int id,
        IProductService productService)
    {
        var deleted = await productService.DeleteProductAsync(id);

        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
