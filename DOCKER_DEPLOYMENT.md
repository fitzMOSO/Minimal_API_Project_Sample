# Docker Deployment Guide

## Building and Running with Docker

### Option 1: Using Docker Compose (Recommended)

#### Build and Run
```bash
docker-compose up --build
```

#### Run in Background
```bash
docker-compose up -d
```

#### Stop Containers
```bash
docker-compose down
```

#### View Logs
```bash
docker-compose logs -f
```

### Option 2: Using Docker CLI

#### Build the Image
```bash
docker build -t minimal-api-project-sample .
```

#### Run the Container
```bash
docker run -d -p 5145:5145 -p 7016:7016 --name minimal-api minimal-api-project-sample
```

#### Stop the Container
```bash
docker stop minimal-api
docker rm minimal-api
```

## Accessing the Application

Once the container is running, access the API at:

- **HTTP**: http://localhost:5145/
- **HTTPS**: https://localhost:7016/
- **Health Check**: http://localhost:5145/health
- **Products API**: http://localhost:5145/api/products

## Port Configuration

The Dockerfile and docker-compose are configured with the following ports:

| Service | Internal Port | External Port | Protocol |
|---------|---------------|---------------|----------|
| HTTP | 5145 | 5145 | HTTP |
| HTTPS | 7016 | 7016 | HTTPS |

## Environment Variables

### Development
```yaml
ASPNETCORE_ENVIRONMENT: Development
ASPNETCORE_HTTP_PORTS: 5145
ASPNETCORE_HTTPS_PORTS: 7016
```

### Production
```yaml
ASPNETCORE_ENVIRONMENT: Production
ASPNETCORE_HTTP_PORTS: 5145
ASPNETCORE_HTTPS_PORTS: 7016
ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=Minimal_API_DB;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
```

## Database Configuration for Docker

### Option 1: Use In-Memory Repository (Default)

For containerized environments, you may want to use the in-memory repository:

**Update `Program.cs`:**
```csharp
// Change from:
builder.Services.AddScoped<IProductRepository, DatabaseProductRepository>();

// To:
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();
```

### Option 2: Use SQL Server Container

Uncomment the SQL Server service in `docker-compose.yml` and update the connection string:

```yaml
services:
  minimal-api-project-sample:
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=Minimal_API_DB;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True
    depends_on:
      - sqlserver

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    # ... (see docker-compose.yml for full configuration)
```

### Option 3: Connect to External Database

Set the connection string as an environment variable:

```bash
docker run -d -p 5145:5145 -p 7016:7016 \
  -e "ConnectionStrings__DefaultConnection=Server=your-server;Database=Minimal_API_DB;User Id=sa;Password=YourPassword;TrustServerCertificate=True" \
  --name minimal-api minimal-api-project-sample
```

## HTTPS Configuration in Docker

### Development Certificate

For HTTPS to work in Docker, you need to provide a development certificate:

#### Generate and Export Certificate
```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p YourPassword
dotnet dev-certs https --trust
```

#### Update docker-compose.yml
```yaml
services:
  minimal-api-project-sample:
    environment:
      - ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
    volumes:
      - ~/.aspnet/https:/https:ro
```

## Testing the Docker Container

### Quick Test Commands

**PowerShell:**
```powershell
# Wait for container to start
Start-Sleep -Seconds 5

# Test HTTP endpoint
Invoke-RestMethod -Uri "http://localhost:5145/api/products" | ConvertTo-Json

# Test HTTPS endpoint (if configured)
Invoke-RestMethod -Uri "https://localhost:7016/api/products" -SkipCertificateCheck | ConvertTo-Json
```

**curl:**
```bash
# Test HTTP endpoint
curl http://localhost:5145/api/products

# Test health check
curl http://localhost:5145/health
```

**Browser:**
- Open: http://localhost:5145/api/products

## Troubleshooting Docker

### Container Won't Start

**Check logs:**
```bash
docker logs minimal-api
# or
docker-compose logs
```

### Port Already in Use

**Find and stop the process:**
```bash
# Windows
netstat -ano | findstr :5145
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :5145
kill -9 <PID>
```

**Or use different ports:**
```bash
docker run -d -p 8080:5145 -p 8443:7016 --name minimal-api minimal-api-project-sample
```
Then access at http://localhost:8080/api/products

### Database Connection Issues

If using SQL Server in Docker:

1. **Ensure SQL Server container is running:**
   ```bash
   docker ps
   ```

2. **Check SQL Server logs:**
   ```bash
   docker logs minimal-api-sqlserver
   ```

3. **Verify network connectivity:**
   ```bash
   docker network inspect minimal-api-network
   ```

### Cannot Access API from Host

**Check if ports are exposed:**
```bash
docker port minimal-api
```

**Check if container is running:**
```bash
docker ps
```

**Test from inside container:**
```bash
docker exec -it minimal-api curl http://localhost:5145/health
```

## Production Deployment

### Build for Production
```bash
docker build -t minimal-api-project-sample:latest -t minimal-api-project-sample:1.0.0 .
```

### Push to Registry
```bash
# Docker Hub
docker tag minimal-api-project-sample your-username/minimal-api-project-sample
docker push your-username/minimal-api-project-sample

# Azure Container Registry
docker tag minimal-api-project-sample yourregistry.azurecr.io/minimal-api-project-sample
docker push yourregistry.azurecr.io/minimal-api-project-sample
```

### Run in Production
```bash
docker run -d \
  --name minimal-api \
  --restart unless-stopped \
  -p 80:5145 \
  -p 443:7016 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "ConnectionStrings__DefaultConnection=Server=prod-server;Database=Minimal_API_DB;..." \
  minimal-api-project-sample:1.0.0
```

## Docker Compose Commands Reference

```bash
# Build and start
docker-compose up --build

# Start in background
docker-compose up -d

# Stop containers
docker-compose down

# Stop and remove volumes
docker-compose down -v

# View logs
docker-compose logs -f

# Restart a service
docker-compose restart minimal-api-project-sample

# Scale services (if applicable)
docker-compose up -d --scale minimal-api-project-sample=3

# Rebuild specific service
docker-compose build minimal-api-project-sample

# Execute command in running container
docker-compose exec minimal-api-project-sample bash
```

## Best Practices

1. **Use multi-stage builds** (already implemented in Dockerfile)
2. **Use non-root user** (already configured with `$APP_UID`)
3. **Use specific image versions** (using `10.0` tag)
4. **Store secrets securely** (use Docker secrets or environment variables)
5. **Use health checks** (already available at `/health`)
6. **Log to stdout/stderr** (already configured)
7. **Use .dockerignore** to exclude unnecessary files

## .dockerignore File

Create a `.dockerignore` file to exclude unnecessary files from the Docker build:

```
**/.dockerignore
**/.git
**/.gitignore
**/.vs
**/.vscode
**/*.*proj.user
**/bin
**/obj
**/Migrations
**/*.db
**/*.db-*
**/node_modules
**/.env
**/docker-compose.yml
**/docker-compose.*.yml
**/.DS_Store
**/README.md
**/TROUBLESHOOTING.md
**/QUICK_START.md
**/API_TEST_EXAMPLES.md
**/DATABASE_SETUP.md
**/EF_CORE_COMMANDS.md
**/SOLID.md
**/*.http
**/*.ps1
```

## Summary

- ? Dockerfile configured with ports 5145 (HTTP) and 7016 (HTTPS)
- ? Docker Compose ready for easy deployment
- ? Supports both in-memory and SQL Server databases
- ? Production-ready with health checks
- ? Multi-stage build for smaller image size
- ? Non-root user for security

For more information about the API itself, see:
- **README.md** - Project overview
- **QUICK_START.md** - Getting started guide
- **TROUBLESHOOTING.md** - Common issues and solutions
