# Database Setup Guide

## Prerequisites
- SQL Server LocalDB installed (comes with Visual Studio)
- .NET EF Core Tools installed globally

## Installing EF Core Tools

If you haven't installed the tools yet, run:
```bash
dotnet tool install --global dotnet-ef
```

Or update existing tools:
```bash
dotnet tool update --global dotnet-ef
```

## Creating the Database

The application is configured to automatically create and migrate the database on startup. However, you can also manually manage migrations.

### Automatic Migration (Recommended)
Simply run the application, and it will:
1. Create the database if it doesn't exist
2. Apply all pending migrations
3. Seed initial data

```bash
dotnet run
```

### Manual Migration

#### 1. Create Initial Migration
```bash
cd "C:\Users\My PC\Desktop\My Project\Minimal API\Minimal_API_Project_Sample"
dotnet ef migrations add InitialCreate
```

#### 2. Apply Migration to Database
```bash
dotnet ef database update
```

#### 3. View Migrations
```bash
dotnet ef migrations list
```

## Connection String

Located in `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minimal_API_DB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30"
  }
}
```

### Connection String Components:
- **Data Source**: `(localdb)\MSSQLLocalDB` - SQL Server LocalDB instance
- **Initial Catalog**: `Minimal_API_DB` - Database name
- **Integrated Security**: `True` - Uses Windows authentication
- **Encrypt**: `True` - Encrypts connection
- **Trust Server Certificate**: `False` - Validates server certificate

## Seeded Data

The database is automatically seeded with 10 sample products:

1. Laptop - $1,299.99 (15 in stock)
2. Wireless Mouse - $29.99 (50 in stock)
3. Mechanical Keyboard - $89.99 (30 in stock)
4. 27-inch Monitor - $449.99 (20 in stock)
5. USB-C Hub - $49.99 (100 in stock)
6. Webcam - $79.99 (45 in stock)
7. Noise-Cancelling Headphones - $299.99 (25 in stock)
8. External SSD - $129.99 (60 in stock)
9. Gaming Chair - $349.99 (12 in stock)
10. Standing Desk - $599.99 (8 in stock)

## Database Schema

### Products Table

| Column | Type | Constraints |
|--------|------|-------------|
| Id | int | Primary Key, Identity |
| Name | nvarchar(100) | Required |
| Description | nvarchar(500) | Nullable |
| Price | decimal(18,2) | Required |
| Stock | int | Required |
| CreatedAt | datetime2 | Required |
| UpdatedAt | datetime2 | Nullable |

## Common Operations

### Reset Database
To delete and recreate the database:
```bash
dotnet ef database drop
dotnet ef database update
```

### Add New Migration
After modifying the `Product` model or `ApplicationDbContext`:
```bash
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Remove Last Migration
If you haven't applied it yet:
```bash
dotnet ef migrations remove
```

### View SQL for Migration
```bash
dotnet ef migrations script
```

## Troubleshooting

### LocalDB Not Found
Install SQL Server Express LocalDB:
- Download from: https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
- Or install via Visual Studio Installer

### Connection String Issues
Verify LocalDB instance:
```bash
sqllocaldb info
sqllocaldb info MSSQLLocalDB
```

Start LocalDB if stopped:
```bash
sqllocaldb start MSSQLLocalDB
```

### Migration Errors
If migrations fail, try:
1. Clean and rebuild: `dotnet clean && dotnet build`
2. Delete `Migrations` folder and recreate: `dotnet ef migrations add InitialCreate`
3. Check connection string in `appsettings.Development.json`

## Switching Between Repositories

The application uses the **Repository Pattern** with **Dependency Injection**, making it easy to switch data sources.

### Current: Database Repository
```csharp
// In Program.cs
builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();
```

### Alternative: In-Memory Repository
For testing or development without a database:
```csharp
// In Program.cs
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
```

This demonstrates the **Open/Closed Principle** - you can switch implementations without modifying business logic!
