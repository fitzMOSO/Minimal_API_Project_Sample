using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Minimal_API_Project_Sample.Data;
using Minimal_API_Project_Sample.Endpoints;
using Minimal_API_Project_Sample.Mappers;
using Minimal_API_Project_Sample.Middleware;
using Minimal_API_Project_Sample.Repositories;
using Minimal_API_Project_Sample.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI/Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Exception handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Health checks
builder.Services.AddHealthChecks();

// Database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(30)));

// SOLID: Dependency Inversion - Register abstractions
// Repository layer - Data access (using DatabaseProductRepository now)
builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();

// Service layer - Business logic
builder.Services.AddScoped<IProductService, ProductService>();

// Mapper - Transformation logic
builder.Services.AddScoped<IProductMapper, ProductMapper>();

// Validators - Validation logic
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying database migrations");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    Name = "Minimal API Project Sample",
    Version = "1.0.0",
    Status = "Running",
    Database = "SQL Server LocalDB",
    Timestamp = DateTime.UtcNow
}))
.WithName("Root")
.WithTags("Info")
.ExcludeFromDescription();

app.MapProductEndpoints();

app.Run();
