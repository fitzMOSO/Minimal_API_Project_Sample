
# Quick API Test Script
# Run this in PowerShell while your application is running

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Testing Minimal API" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: API Root
Write-Host "Test 1: API Info" -ForegroundColor Yellow
try {
    $result = Invoke-RestMethod -Uri "http://localhost:5145/" -Method Get -ErrorAction Stop
    Write-Host "? SUCCESS" -ForegroundColor Green
    $result | ConvertTo-Json
    Write-Host ""
} catch {
    Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Make sure the application is running on port 5145" -ForegroundColor Yellow
    Write-Host ""
}

# Test 2: Health Check
Write-Host "Test 2: Health Check" -ForegroundColor Yellow
try {
    $result = Invoke-WebRequest -Uri "http://localhost:5145/health" -Method Get -ErrorAction Stop
    Write-Host "? SUCCESS - Status: $($result.StatusCode)" -ForegroundColor Green
    Write-Host ""
} catch {
    Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 3: Get All Products
Write-Host "Test 3: Get All Products" -ForegroundColor Yellow
try {
    $products = Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Get -ErrorAction Stop
    Write-Host "? SUCCESS - Found $($products.Count) products" -ForegroundColor Green
    $products | Select-Object -First 3 | Format-Table Id, Name, Price, Stock
    Write-Host ""
} catch {
    Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 4: Get Single Product
Write-Host "Test 4: Get Product by ID (ID=1)" -ForegroundColor Yellow
try {
    $product = Invoke-RestMethod -Uri "http://localhost:5145/api/products/1" -Method Get -ErrorAction Stop
    Write-Host "? SUCCESS" -ForegroundColor Green
    $product | Format-List
    Write-Host ""
} catch {
    Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

# Test 5: Create Product
Write-Host "Test 5: Create New Product" -ForegroundColor Yellow
try {
    $newProduct = @{
        name = "PowerShell Test Product"
        description = "Created from PowerShell script"
        price = 49.99
        stock = 15
    } | ConvertTo-Json

    $created = Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Post -Body $newProduct -ContentType "application/json" -ErrorAction Stop
    Write-Host "? SUCCESS - Created product with ID: $($created.id)" -ForegroundColor Green
    $created | Format-List
    Write-Host ""
    
    # Store the ID for cleanup
    $testProductId = $created.id
    
    # Test 6: Update Product
    Write-Host "Test 6: Update Product (ID=$testProductId)" -ForegroundColor Yellow
    try {
        $updateData = @{
            price = 39.99
            stock = 20
        } | ConvertTo-Json
        
        $updated = Invoke-RestMethod -Uri "http://localhost:5145/api/products/$testProductId" -Method Put -Body $updateData -ContentType "application/json" -ErrorAction Stop
        Write-Host "? SUCCESS - Updated product" -ForegroundColor Green
        $updated | Format-List
        Write-Host ""
    } catch {
        Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }
    
    # Test 7: Delete Product
    Write-Host "Test 7: Delete Product (ID=$testProductId)" -ForegroundColor Yellow
    try {
        Invoke-RestMethod -Uri "http://localhost:5145/api/products/$testProductId" -Method Delete -ErrorAction Stop
        Write-Host "? SUCCESS - Product deleted" -ForegroundColor Green
        Write-Host ""
    } catch {
        Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
    }
    
} catch {
    Write-Host "? FAILED: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Testing Complete!" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "If tests failed, make sure:" -ForegroundColor Yellow
Write-Host "1. The application is running (dotnet run)" -ForegroundColor White
Write-Host "2. Listening on http://localhost:5145" -ForegroundColor White
Write-Host "3. No firewall blocking localhost" -ForegroundColor White
Write-Host ""
