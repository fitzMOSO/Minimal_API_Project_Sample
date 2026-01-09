# Minimal API Project Sample

A modern ASP.NET Core Minimal API project demonstrating **SOLID principles**, **Entity Framework Core**, **Scalar API Documentation**, and best practices for building lightweight, high-performance APIs with SQL Server.

## ?? SOLID Principles Implementation

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

## ? Features

- ? **Minimal API** - Clean, concise endpoint definitions
- ? **SOLID Architecture** - Maintainable and extensible design
- ? **Scalar API Documentation** - Modern, interactive API docs
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
- ? **Docker Support** - Ready for containerization
- ? **Database Seeding** - Pre-populated with sample data

## ?? Project Structure

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

## ?? Getting Started

### Prerequisites
- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)
- Docker (optional)

### Running the API

1. **Restore dependencies:**
```bash
dotnet restore
```

2. **Run the application:**
```bash
dotnet run
```

The application will:
- Create the database if it doesn't exist
- Apply migrations automatically
- Seed initial data
- Start listening on configured ports
- **Automatically open Scalar documentation in your browser!**

3. **Access Scalar API Documentation:**

**Windows Development:**
```
https://localhost:7016/scalar/v1  (HTTPS)
http://localhost:5145/scalar/v1   (HTTP)
```

**Docker:**
```
http://localhost:5145/scalar/v1
```

## ?? Scalar API Documentation

This project uses **Scalar** instead of traditional Swagger UI for a modern, beautiful API documentation experience.

### Features:
- ?? **Beautiful Purple Theme** with dark mode
- ? **Fast and Responsive** interface
- ?? **Interactive API Testing** directly in browser
- ?? **Code Generation** in multiple languages (C#, JavaScript, Python, etc.)
- ?? **Quick Search** with keyboard shortcuts (Ctrl/Cmd + K)
- ?? **Mobile Friendly** responsive design
- ?? **Auto-Opens** when you start the application

### Quick Access:
```
http://localhost:5145/scalar/v1
```

For detailed Scalar documentation, see **[SCALAR_DOCUMENTATION.md](SCALAR_DOCUMENTATION.md)**

## ?? API Endpoints

### Products

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{id}` | Get product by ID |
| `POST` | `/api/products` | Create a new product |
| `PUT` | `/api/products/{id}` | Update a product |
| `DELETE` | `/api/products/{id}` | Delete a product |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | API health status |

### API Information

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/` | API info and documentation link |

## ?? Testing Your API

### Option 1: Scalar (Recommended) ?
1. Start the application (F5 in Visual Studio)
2. Scalar opens automatically in your browser
3. Select an endpoint
4. Click "Try it"
5. Test directly in the browser

### Option 2: Postman
Use the provided HTTP examples in the repository

### Option 3: Visual Studio HTTP Client
Use the `.http` files in the project

## ?? Example Requests

### Create Product:
```json
POST /api/products
Content-Type: application/json

{
  "name": "Gaming Monitor",
  "description": "27-inch 144Hz gaming monitor",
  "price": 349.99,
  "stock": 20
}
```

**Response (201 Created):**
```json
{
  "id": 11,
  "name": "Gaming Monitor",
  "description": "27-inch 144Hz gaming monitor",
  "price": 349.99,
  "stock": 20
}
```

### Update Product:
```json
PUT /api/products/1
Content-Type: application/json

{
  "price": 299.99,
  "stock": 25
}
```

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop for developers",
  "price": 299.99,
  "stock": 25
}
```

## ??? Database

### Connection String
Located in `appsettings.Development.json`:
```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Minimal_API_DB
```

### Seeded Products
The database includes 10 pre-seeded products:
1. **Laptop** - $1,299.99 (15 in stock)
2. **Wireless Mouse** - $29.99 (50 in stock)
3. **Mechanical Keyboard** - $89.99 (30 in stock)
4. **27-inch Monitor** - $449.99 (20 in stock)
5. **USB-C Hub** - $49.99 (100 in stock)
6. **Webcam** - $79.99 (45 in stock)
7. **Noise-Cancelling Headphones** - $299.99 (25 in stock)
8. **External SSD** - $129.99 (60 in stock)
9. **Gaming Chair** - $349.99 (12 in stock)
10. **Standing Desk** - $599.99 (8 in stock)

See [DATABASE_SETUP.md](DATABASE_SETUP.md) for detailed database documentation.

## ?? Docker Support

### Running with Docker

```bash
docker-compose up --build
```

**Docker Access:**
```
http://localhost:5145/scalar/v1
```

**Note:** Docker uses in-memory repository by default to avoid LocalDB compatibility issues.

For detailed Docker instructions, see **[DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md)**

## ?? Documentation

- **[SCALAR_DOCUMENTATION.md](SCALAR_DOCUMENTATION.md)** - Comprehensive Scalar usage guide
- **[SOLID.md](SOLID.md)** - SOLID principles implementation details
- **[DATABASE_SETUP.md](DATABASE_SETUP.md)** - Database setup and migration guide
- **[DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md)** - Docker deployment guide
- **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Common issues and solutions
- **[EF_CORE_COMMANDS.md](EF_CORE_COMMANDS.md)** - Entity Framework Core CLI reference
- **[API_TEST_EXAMPLES.md](API_TEST_EXAMPLES.md)** - Copy-paste ready test examples

## ? Best Practices Implemented

1. ? **SOLID Principles** - All five principles thoroughly applied
2. ? **Scalar Documentation** - Modern, interactive API docs with auto-launch
3. ? **Entity Framework Core** - Code-first approach with migrations
4. ? **Database Seeding** - Consistent test data across environments
5. ? **Typed Results** - Using `TypedResults` for better performance and type safety
6. ? **Route Groups** - Organizing related endpoints with `MapGroup`
7. ? **Validation** - FluentValidation for declarative validation rules
8. ? **Logging** - Structured logging with proper log levels
9. ? **Error Handling** - Global exception handler with problem details
10. ? **Separation of Concerns** - Clear separation between layers
11. ? **Async/Await** - Proper async patterns throughout
12. ? **Nullable Reference Types** - Enabled for better null safety
13. ? **Record Types** - Using records for immutable DTOs
14. ? **XML Documentation** - Enhanced API documentation
15. ? **Dependency Injection** - Constructor injection following DIP

## ?? Testing Benefits

The SOLID architecture makes testing easy:

```csharp
// Mock the repository for testing the service
var mockRepo = new Mock<IProductRepository>();
var mockMapper = new Mock<IProductMapper>();
var mockValidator = new Mock<IValidator<CreateProductDto>>();
var service = new ProductService(
    mockRepo.Object, 
    mockMapper.Object, 
    mockValidator.Object,
    ...
);

// Mock the service for testing the endpoints
var mockService = new Mock<IProductService>();
// Test endpoints independently
```

## ?? Architecture Highlights

### Clean Architecture Layers
```
Presentation Layer (Endpoints)
    ? depends on
Business Logic Layer (Services)
    ? depends on
Data Access Layer (Repositories)
    ? depends on
Database Layer (EF Core + SQL Server)
```

### SOLID Benefits
- **Easy to Test** - Mock interfaces for unit tests
- **Easy to Extend** - Add new implementations without changing existing code
- **Easy to Maintain** - Each class has a single, clear purpose
- **Easy to Understand** - Clear separation of concerns

## ?? Quick Start Commands

```bash
# Clone the repository
git clone https://github.com/fitzMOSO/Minimal_API_Project_Sample.git

# Navigate to project
cd Minimal_API_Project_Sample

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# Your browser will automatically open Scalar!
```

## ?? Troubleshooting

### Port Already in Use?
Run the included PowerShell script:
```powershell
.\kill-port-5145.ps1
```

### Database Issues?
See [DATABASE_SETUP.md](DATABASE_SETUP.md) for:
- Migration commands
- Connection string configuration
- LocalDB troubleshooting

### Docker Issues?
See [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md) for:
- Certificate configuration
- Port mapping
- Environment setup

For more issues, check [TROUBLESHOOTING.md](TROUBLESHOOTING.md)

## ?? Project Stats

- **Language:** C# 14.0
- **Framework:** .NET 10
- **Database:** SQL Server LocalDB / In-Memory
- **API Style:** Minimal API
- **Documentation:** Scalar
- **Architecture:** SOLID Principles
- **Validation:** FluentValidation
- **ORM:** Entity Framework Core

## ?? Contributing

This is a sample/educational project demonstrating best practices. Feel free to:
- Fork the repository
- Create feature branches
- Submit pull requests
- Report issues
- Suggest improvements

## ?? License

This is a sample project for educational purposes demonstrating SOLID principles, Scalar API documentation, Entity Framework Core, and modern .NET best practices.

## ?? Key Takeaways

This project demonstrates:
- ? How to build a **production-ready** minimal API
- ? How to implement **SOLID principles** in .NET
- ? How to use **Scalar** for beautiful API documentation
- ? How to structure a **maintainable** codebase
- ? How to implement **proper testing** architecture
- ? How to configure **Docker** for deployment
- ? How to use **EF Core** with migrations and seeding

## ?? Features Summary

| Feature | Status | Notes |
|---------|--------|-------|
| Minimal API | ? | Clean, concise endpoints |
| SOLID Principles | ? | All 5 principles implemented |
| Scalar Documentation | ? | Auto-opens on start |
| Entity Framework Core | ? | Code-first with migrations |
| SQL Server LocalDB | ? | Auto-creates and seeds |
| Docker Support | ? | Docker Compose ready |
| FluentValidation | ? | Declarative validation |
| Global Exception Handling | ? | Centralized error handling |
| Health Checks | ? | `/health` endpoint |
| Structured Logging | ? | Throughout the app |

---

**?? Built with modern .NET best practices and SOLID principles**

**? Star this repository if you find it helpful!**

**?? Check out the [documentation files](.) for detailed guides**

**?? Happy coding!**
