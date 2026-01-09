# Entity Framework Core Quick Reference

## Essential EF Core Commands

All commands should be run from the project directory:
```bash
cd "C:\Users\My PC\Desktop\My Project\Minimal API\Minimal_API_Project_Sample"
```

## Migrations

### Create a New Migration
```bash
dotnet ef migrations add MigrationName
```

Examples:
```bash
dotnet ef migrations add InitialCreate
dotnet ef migrations add AddProductCategory
dotnet ef migrations add UpdateProductPriceColumn
```

### Apply Migrations to Database
```bash
dotnet ef database update
```

Apply specific migration:
```bash
dotnet ef database update MigrationName
```

### List All Migrations
```bash
dotnet ef migrations list
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

**?? Warning:** Only works if the migration hasn't been applied to the database yet!

### Generate SQL Script
```bash
dotnet ef migrations script
```

From specific migration to another:
```bash
dotnet ef migrations script FromMigration ToMigration
```

## Database Operations

### Drop Database
```bash
dotnet ef database drop
```

Force drop without confirmation:
```bash
dotnet ef database drop --force
```

### Update to Specific Migration
```bash
dotnet ef database update MigrationName
```

Roll back all migrations:
```bash
dotnet ef database update 0
```

## DbContext Operations

### List Available DbContexts
```bash
dotnet ef dbcontext list
```

### Generate DbContext from Database (Scaffold)
```bash
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer
```

### View DbContext Information
```bash
dotnet ef dbcontext info
```

## Common Workflows

### 1. Initial Database Setup
```bash
# Install EF Core tools (one time)
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Create database and apply migration
dotnet ef database update
```

### 2. Modify Existing Model
```bash
# 1. Modify your model class (e.g., Product.cs)
# 2. Create migration for changes
dotnet ef migrations add UpdateProductModel

# 3. Review the generated migration file
# 4. Apply to database
dotnet ef database update
```

### 3. Reset Database
```bash
# Drop existing database
dotnet ef database drop --force

# Recreate with all migrations
dotnet ef database update
```

### 4. Rollback Last Migration
```bash
# Remove from database and code
dotnet ef database update PreviousMigrationName
dotnet ef migrations remove
```

## Design-Time DbContext Creation

For migrations to work, EF Core needs to create your DbContext at design time. This project uses the automatic detection through `Program.cs`.

If you need explicit design-time factory, create this file:

```csharp
// Data/ApplicationDbContextFactory.cs
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer("Your-Connection-String");
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
```

## Troubleshooting

### Error: "Unable to create an object of type 'ApplicationDbContext'"
**Solution:** Ensure `Program.cs` properly configures the DbContext, or create a design-time factory.

### Error: "A connection was successfully established... existing connection was forcibly closed"
**Solution:** Check if SQL Server LocalDB is running:
```bash
sqllocaldb start MSSQLLocalDB
```

### Error: "Cannot drop database because it is currently in use"
**Solution:** Close all connections or force drop:
```bash
dotnet ef database drop --force
```

### Error: "Build failed"
**Solution:** Build your project first:
```bash
dotnet build
dotnet ef migrations add MigrationName
```

## Migration Best Practices

### ? DO:
- Create meaningful migration names: `AddProductImageUrl`, not `Update1`
- Review generated migration code before applying
- Test migrations on development environment first
- Keep migrations small and focused
- Commit migrations to source control
- Use `dotnet ef migrations script` for production deployments

### ? DON'T:
- Modify migrations after they've been applied
- Delete migrations that have been shared with team
- Apply migrations directly to production (use SQL scripts instead)
- Skip creating migrations for model changes
- Commit large data migrations to source control

## Package Information

Required packages for EF Core with SQL Server:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

## Connection Strings

### Development (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Minimal_API_DB;Integrated Security=True"
  }
}
```

### Production Example (appsettings.Production.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-server;Database=ProductionDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Using Environment Variables
```bash
export ConnectionStrings__DefaultConnection="Server=..."
# or in PowerShell:
$env:ConnectionStrings__DefaultConnection="Server=..."
```

## Automatic Migration on Startup

This project applies migrations automatically. In `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // Applies pending migrations
}
```

**?? Production Note:** Consider using explicit migration scripts instead of automatic migration for production environments.

## Useful Resources

- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Connection Strings](https://www.connectionstrings.com/sql-server/)
