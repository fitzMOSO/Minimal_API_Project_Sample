# Troubleshooting Guide for HTTP Testing

## Common Issues and Solutions

### Issue 1: "Connection refused" or "Cannot connect to server"

**Problem:** The application is not running.

**Solution:**
1. Open a terminal in the project directory
2. Run the application:
   ```bash
   cd "C:\Users\My PC\Desktop\My Project\Minimal API\Minimal_API_Project_Sample"
   dotnet run
   ```
3. Wait for the message: "Now listening on: http://localhost:5145"
4. Try the HTTP requests again

---

### Issue 2: "SSL connection error" or "Certificate error"

**Problem:** HTTPS certificate issues in development.

**Solution:**
1. Trust the development certificate:
   ```bash
   dotnet dev-certs https --trust
   ```
2. Or use HTTP endpoints instead of HTTPS (port 5145 instead of 7016)

---

### Issue 3: "404 Not Found" for all endpoints

**Problem:** Wrong port or the API routes are not registered.

**Solution:**
1. Check the console output when running `dotnet run` for the actual listening ports
2. Verify the application is running with the correct profile:
   ```bash
   dotnet run --launch-profile https
   ```
3. Check if endpoints are mapped in `Program.cs`:
   ```csharp
   app.MapProductEndpoints();
   ```

---

### Issue 4: Database connection errors

**Problem:** SQL Server LocalDB is not running or database doesn't exist.

**Symptoms:**
- "Cannot open database"
- "A network-related or instance-specific error"

**Solution:**
1. Check if LocalDB is installed:
   ```bash
   sqllocaldb info
   ```
2. Start LocalDB:
   ```bash
   sqllocaldb start MSSQLLocalDB
   ```
3. Delete and recreate the database:
   ```bash
   dotnet ef database drop --force
   dotnet run
   ```
   The application will automatically create and seed the database.

---

### Issue 5: "Build failed" or "Cannot find type"

**Problem:** Dependencies not restored or compilation errors.

**Solution:**
1. Clean and restore:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```
2. Check for compilation errors in the output
3. If FluentValidation is missing:
   ```bash
   dotnet add package FluentValidation.DependencyInjectionExtensions
   ```

---

## Step-by-Step Testing Guide

### 1. Start the Application

**Option A: Using Visual Studio**
1. Open `Minimal_API_Project_Sample.sln` in Visual Studio
2. Press F5 or click the green "Run" button
3. Select "https" profile if prompted
4. Wait for the browser to open (you can close it)

**Option B: Using Command Line**
```bash
cd "C:\Users\My PC\Desktop\My Project\Minimal API\Minimal_API_Project_Sample"
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7016
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5145
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 2. Verify the Application is Running

**Test in Browser:**
- HTTP: http://localhost:5145/
- HTTPS: https://localhost:7016/

**Expected Response:**
```json
{
  "name": "Minimal API Project Sample",
  "version": "1.0.0",
  "status": "Running",
  "database": "SQL Server LocalDB",
  "timestamp": "2024-01-01T00:00:00Z"
}
```

### 3. Test with HTTP File in Visual Studio

1. Make sure the application is running (see step 1)
2. Open `Minimal_API_Project_Sample.http` in Visual Studio
3. Click "Send Request" above any request
4. View the response in the right panel

### 4. Test with curl (Alternative)

```bash
# Get all products
curl http://localhost:5145/api/products

# Get specific product
curl http://localhost:5145/api/products/1

# Create product
curl -X POST http://localhost:5145/api/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Test Product\",\"description\":\"Test\",\"price\":99.99,\"stock\":10}"

# Update product
curl -X PUT http://localhost:5145/api/products/1 ^
  -H "Content-Type: application/json" ^
  -d "{\"price\":299.99,\"stock\":20}"

# Delete product
curl -X DELETE http://localhost:5145/api/products/10
```

### 5. Test with Postman (Alternative)

1. Download and install Postman
2. Create a new request
3. Set URL: `http://localhost:5145/api/products`
4. Set method: GET
5. Click "Send"

---

## Checking Application Logs

The application logs provide useful debugging information:

```bash
# Run with verbose logging
dotnet run --verbosity detailed
```

Look for:
- ? "Now listening on:" - Application started successfully
- ? "Application started" - Ready to accept requests
- ? "Database migrations applied successfully" - Database is ready
- ? Any error messages or exceptions

---

## Port Configuration

If ports 5145 or 7016 are already in use, you can change them:

**Edit `Properties/launchSettings.json`:**
```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:7016;http://localhost:5145"
    }
  }
}
```

Change to different ports:
```json
{
  "profiles": {
    "https": {
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
  }
}
```

Then update the HTTP file:
```
@Minimal_API_Project_Sample_HostAddress = http://localhost:5000
@Minimal_API_Project_Sample_HostAddress_HTTPS = https://localhost:5001
```

---

## Quick Health Check Commands

```bash
# 1. Check if ports are in use
netstat -ano | findstr :5145
netstat -ano | findstr :7016

# 2. Check LocalDB status
sqllocaldb info
sqllocaldb info MSSQLLocalDB

# 3. Test basic connectivity
curl http://localhost:5145/health

# 4. View all products
curl http://localhost:5145/api/products
```

---

## Common Error Messages and Solutions

### "Unable to bind to https://localhost:7016"
**Solution:** Port is in use. Change port in `launchSettings.json` or kill the process using the port.

### "No connection could be made because the target machine actively refused it"
**Solution:** Application is not running. Start it with `dotnet run`.

### "Cannot open database 'Minimal_API_DB'"
**Solution:** Run `sqllocaldb start MSSQLLocalDB` and restart the application.

### "The certificate chain was issued by an authority that is not trusted"
**Solution:** Run `dotnet dev-certs https --trust` or use HTTP instead of HTTPS.

### "404 Not Found"
**Solution:** Check the URL path. Make sure it starts with `/api/products`.

### "400 Bad Request" with validation errors
**Solution:** Check your JSON payload. Ensure all required fields are present and valid.

---

## Testing Checklist

Before testing HTTP requests, verify:

- [ ] Application is running (`dotnet run`)
- [ ] Console shows "Now listening on" messages
- [ ] Database connection successful (check logs)
- [ ] LocalDB is running (`sqllocaldb info MSSQLLocalDB`)
- [ ] Correct port numbers in HTTP file (5145 for HTTP, 7016 for HTTPS)
- [ ] No firewall blocking localhost connections
- [ ] Visual Studio HTTP client is up to date

---

## Still Having Issues?

1. **Check the Build Output:**
   - View ? Output ? Show output from: Build
   - Look for compilation errors

2. **Check the Debug Output:**
   - View ? Output ? Show output from: Debug
   - Look for runtime errors

3. **Restart Everything:**
   ```bash
   # Stop the application (Ctrl+C if running in terminal)
   dotnet clean
   dotnet build
   sqllocaldb stop MSSQLLocalDB
   sqllocaldb start MSSQLLocalDB
   dotnet run
   ```

4. **Test Database Connection:**
   - Open SQL Server Object Explorer in Visual Studio
   - Connect to `(localdb)\MSSQLLocalDB`
   - Check if `Minimal_API_DB` database exists
   - Check if `Products` table has data

5. **Review Recent Changes:**
   - Undo recent code changes if the app was working before
   - Check Git history for breaking changes

---

## Need More Help?

Check these files for additional information:
- `README.md` - Project overview and setup
- `DATABASE_SETUP.md` - Database configuration
- `EF_CORE_COMMANDS.md` - Entity Framework commands
- `SOLID.md` - Architecture documentation

Or check the logs in the console output for specific error messages.
