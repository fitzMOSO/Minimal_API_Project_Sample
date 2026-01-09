using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Minimal_API_Project_Sample.Data;
using Minimal_API_Project_Sample.Endpoints;
using Minimal_API_Project_Sample.Mappers;
using Minimal_API_Project_Sample.Middleware;
using Minimal_API_Project_Sample.Repositories;
using Minimal_API_Project_Sample.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI/Swagger
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Exception handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Health checks
builder.Services.AddHealthChecks();

// SOLID: Dependency Inversion - Register abstractions
// Repository layer - Data access
// Choose repository based on environment
var useInMemory = builder.Environment.IsEnvironment("Docker") || 
                  builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

if (useInMemory)
{
    // Use in-memory repository for Docker or when explicitly configured
    builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
}
else
{
    // Database context for non-Docker environments
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlServerOptions => sqlServerOptions.CommandTimeout(30)));
    
    // Repository layer - Data access (using DatabaseProductRepository)
    builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();
}

// Service layer - Business logic
builder.Services.AddScoped<IProductService, ProductService>();

// Mapper - Transformation logic
builder.Services.AddScoped<IProductMapper, ProductMapper>();

// Validators - Validation logic
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Apply migrations and seed data on startup (only for database repository)
if (!useInMemory)
{
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
}

// Configure OpenAPI and Scalar
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Minimal API Project Sample")
            .WithTheme(ScalarTheme.Purple)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithDarkMode(true)
            .WithSearchHotKey("k");
    });
}

app.UseExceptionHandler();

// Only use HTTPS redirection when not in Docker (to avoid certificate issues)
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    Name = "Minimal API Project Sample",
    Version = "1.0.0",
    Status = "Running",
    DataStore = useInMemory ? "In-Memory" : "SQL Server LocalDB",
    Documentation = "/scalar/v1",
    Timestamp = DateTime.UtcNow
}))
.WithName("Root")
.WithTags("Info")
.ExcludeFromDescription();

app.MapProductEndpoints();

app.Run();
