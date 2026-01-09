# Minimal API Project Sample

A modern ASP.NET Core Minimal API project demonstrating **SOLID principles**, **Entity Framework Core**, and best practices for building lightweight, high-performance APIs with SQL Server.

## SOLID Principles Implementation

### ?? Single Responsibility Principle (SRP)
Each class has one reason to change:
- **ProductService** - Handles only business logic
- **ProductRepository** - Handles only data access
- **ProductMapper** - Handles only object mapping
- **ProductEndpoints** - Handles only HTTP concerns
- **Validators** - Handle only validation logic

### ?? Open/Closed Principle (OCP)
Open for extension, closed for modification:
- **Generic IRepository<T>** - Can be extended for new entity types
- **IProductRepository** - Can add new implementations (InMemory, Database, API client)
- **DatabaseProductRepository** - SQL Server implementation
- **InMemoryProductRepository** - In-memory implementation

### ?? Liskov Substitution Principle (LSP)
Interfaces can be replaced with any implementation:
- `IProductRepository` can be swapped between `InMemoryProductRepository` and `DatabaseProductRepository`
- `IProductService` can have multiple implementations
- All implementations honor the contract

### ?? Interface Segregation Principle (ISP)
Focused, specific interfaces:
- `IProductService` - Only product service methods
- `IProductRepository` - Only repository methods
- `IProductMapper` - Only mapping methods
- No client is forced to depend on methods it doesn't use

### ?? Dependency Inversion Principle (DIP)
Depend on abstractions, not concretions:
- **ProductService** depends on `IProductRepository`, `IProductMapper`, `IValidator`
- **ProductEndpoints** depends on `IProductService`
- All dependencies are injected through constructor
- Easy to test with mocks

## Features

- ? **Minimal API** - Clean, concise endpoint definitions
- ? **SOLID Architecture** - Maintainable and extensible design
- ? **Entity Framework Core** - Code-first database with migrations
- ? **SQL Server LocalDB** - Integrated development database
- ? **Repository Pattern** - Abstraction over data access
- ? **Service Layer** - Business logic separation
- ? **DTOs** - Separation of internal models from API contracts
- ? **Mapper Pattern** - Clean object transformations
- ? **FluentValidation** - Robust input validation
- ? **Dependency Injection** - Proper service lifetime management
- ? **Structured Logging** - Built-in logging throughout
- ? **Global Exception Handling** - Centralized error handling
- ? **Health Checks** - Monitoring endpoint
- ? **OpenAPI/Swagger** - Auto-generated API documentation
- ? **Docker Support** - Ready for containerization
- ? **Database Seeding** - Pre-populated with sample data

## Project Structure

```
??? Data/                  # Database context
?   ??? ApplicationDbContext.cs
??? DTOs/                  # Data Transfer Objects
?   ??? ProductDto.cs      # API contracts
??? Models/                # Domain models
?   ??? Product.cs         # Entity definitions
??? Repositories/          # Data access layer
?   ??? IRepository.cs     # Generic repository interface
?   ??? IProductRepository.cs
?   ??? InMemoryProductRepository.cs
?   ??? DatabaseProductRepository.cs
??? Services/              # Business logic layer
?   ??? IProductService.cs
?   ??? ProductService.cs
??? Mappers/               # Object mapping
?   ??? IProductMapper.cs
?   ??? ProductMapper.cs
??? Validators/            # FluentValidation validators
?   ??? ProductValidator.cs
??? Endpoints/             # API endpoint definitions
?   ??? ProductEndpoints.cs
??? Middleware/            # Custom middleware
?   ??? GlobalExceptionHandler.cs
??? Migrations/            # EF Core migrations
??? Program.cs             # Application entry point
```

## Architecture Layers

```
???????????????????????????????????????
?         Presentation Layer          ?
?     (ProductEndpoints.cs)           ?
?   - HTTP Request/Response           ?
?   - Route definitions               ?
???????????????????????????????????????
              ? depends on
              ?
???????????????????????????????????????
?        Business Logic Layer         ?
?     (IProductService)               ?
?   - Validation                      ?
?   - Business rules                  ?
?   - Orchestration                   ?
???????????????????????????????????????
              ? depends on
              ?
???????????????????????????????????????
?        Data Access Layer            ?
?     (IProductRepository)            ?
?   - CRUD operations                 ?
?   - Data persistence                ?
???????????????????????????????????????
              ?
              ?
???????????????????????????????????????
?          Database Layer             ?
?   (ApplicationDbContext)            ?
?   - Entity Framework Core           ?
?   - SQL Server LocalDB              ?
???????????????????????????????????????
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)
- Docker (optional)

### Database Setup

The application automatically creates and migrates the database on first run. See [DATABASE_SETUP.md](DATABASE_SETUP.md) for detailed instructions.

**Quick Start:**
1. The database will be created automatically on first run
2. Seeded with 10 sample products
3. Connection string in `appsettings.Development.json`

### Running the API

1. Restore dependencies:
```bash
dotnet restore
```

2. Run the application:
```bash
dotnet run
```

The application will:
- Create the database if it doesn't exist
- Apply migrations automatically
- Seed initial data
- Start listening on configured ports

3. Access Swagger UI (Development mode):
```
https://localhost:8081/openapi/v1.json
```

### Running with Docker

```bash
docker build -t minimal-api-sample .
docker run -p 8080:8080 -p 8081:8081 minimal-api-sample
```

**Note:** Docker configuration uses in-memory repository by default. Update `Program.cs` to use database repository with appropriate connection string for containerized environments.

## API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/{id}` | Update a product |
| DELETE | `/api/products/{id}` | Delete a product |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | API health status |

### Example Requests

**Create Product:**
```json
POST /api/products
{
  "name": "Gaming Monitor",
  "description": "27-inch 144Hz gaming monitor",
  "price": 349.99,
  "stock": 20
}
```

**Update Product:**
```json
PUT /api/products/1
{
  "price": 299.99,
  "stock": 25
}
```

## Database

### Connection String
Located in `appsettings.Development.json`:
```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Minimal_API_DB
```

### Seeded Products
The database includes 10 pre-seeded products:
- Laptop, Wireless Mouse, Mechanical Keyboard
- 27-inch Monitor, USB-C Hub, Webcam
- Noise-Cancelling Headphones, External SSD
- Gaming Chair, Standing Desk

See [DATABASE_SETUP.md](DATABASE_SETUP.md) for detailed database documentation.

## Extending the Application

### Adding a New Repository Implementation

Thanks to the **Open/Closed Principle**, you can add new data sources without modifying existing code:

```csharp
// In Program.cs, simply swap the implementation:
builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();
// or
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
// or create your own:
builder.Services.AddScoped<IProductRepository, CosmosDbProductRepository>();
```

### Adding a New Entity

1. Create the model in `Models/`
2. Add DbSet to `ApplicationDbContext`
3. Create DTOs in `DTOs/`
4. Create repository interface extending `IRepository<T>`
5. Create repository implementation
6. Create service interface and implementation
7. Create mapper
8. Create validators
9. Create endpoints
10. Create and apply migration

## Best Practices Implemented

1. **SOLID Principles** - All five principles thoroughly applied
2. **Entity Framework Core** - Code-first approach with migrations
3. **Database Seeding** - Consistent test data across environments
4. **Typed Results** - Using `TypedResults` for better performance and type safety
5. **Route Groups** - Organizing related endpoints with `MapGroup`
6. **Validation** - FluentValidation for declarative validation rules
7. **Logging** - Structured logging with proper log levels
8. **Error Handling** - Global exception handler with problem details
9. **Separation of Concerns** - Clear separation between layers
10. **Async/Await** - Proper async patterns throughout
11. **Nullable Reference Types** - Enabled for better null safety
12. **Record Types** - Using records for immutable DTOs
13. **OpenAPI Documentation** - Rich API documentation with summaries
14. **Dependency Injection** - Constructor injection following DIP

## Testing Benefits

The SOLID architecture makes testing easy:

```csharp
// Mock the repository for testing the service
var mockRepo = new Mock<IProductRepository>();
var mockMapper = new Mock<IProductMapper>();
var service = new ProductService(mockRepo.Object, mockMapper.Object, ...);

// Mock the service for testing the endpoints
var mockService = new Mock<IProductService>();
// Test endpoints independently
```

## Documentation

- **[SOLID.md](SOLID.md)** - Comprehensive SOLID principles guide with examples
- **[DATABASE_SETUP.md](DATABASE_SETUP.md)** - Database setup and migration guide

## License

This is a sample project for educational purposes demonstrating SOLID principles, Entity Framework Core, and modern .NET best practices.
