# Working API Test Examples

Copy and paste these commands directly into PowerShell or Command Prompt to test your API.

## Prerequisites
- Your application must be running
- Check console output shows: "Now listening on: http://localhost:5145"

---

## PowerShell Examples (Recommended for Windows)

### 1. Test API Root
```powershell
Invoke-RestMethod -Uri "http://localhost:5145/" -Method Get | ConvertTo-Json
```

### 2. Get All Products
```powershell
Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Get | ConvertTo-Json
```

### 3. Get Product by ID
```powershell
Invoke-RestMethod -Uri "http://localhost:5145/api/products/1" -Method Get | ConvertTo-Json
```

### 4. Create a New Product
```powershell
$product = @{
    name = "Gaming Mouse"
    description = "RGB gaming mouse with 16000 DPI"
    price = 79.99
    stock = 25
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Post -Body $product -ContentType "application/json" | ConvertTo-Json
```

### 5. Update a Product (Partial Update)
```powershell
$update = @{
    price = 69.99
    stock = 30
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5145/api/products/1" -Method Put -Body $update -ContentType "application/json" | ConvertTo-Json
```

### 6. Delete a Product
```powershell
Invoke-RestMethod -Uri "http://localhost:5145/api/products/10" -Method Delete
Write-Host "Product deleted successfully (204 No Content)"
```

### 7. Complete CRUD Test Sequence
```powershell
# Create
Write-Host "Creating product..." -ForegroundColor Cyan
$newProduct = @{
    name = "Test Product"
    description = "This is a test"
    price = 19.99
    stock = 5
} | ConvertTo-Json

$created = Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Post -Body $newProduct -ContentType "application/json"
Write-Host "Created product with ID: $($created.id)" -ForegroundColor Green
Write-Host ($created | ConvertTo-Json)

# Read
Write-Host "`nReading product..." -ForegroundColor Cyan
$retrieved = Invoke-RestMethod -Uri "http://localhost:5145/api/products/$($created.id)" -Method Get
Write-Host ($retrieved | ConvertTo-Json) -ForegroundColor Green

# Update
Write-Host "`nUpdating product..." -ForegroundColor Cyan
$updateData = @{
    price = 24.99
    stock = 10
} | ConvertTo-Json

$updated = Invoke-RestMethod -Uri "http://localhost:5145/api/products/$($created.id)" -Method Put -Body $updateData -ContentType "application/json"
Write-Host "Updated product:" -ForegroundColor Green
Write-Host ($updated | ConvertTo-Json)

# Delete
Write-Host "`nDeleting product..." -ForegroundColor Cyan
Invoke-RestMethod -Uri "http://localhost:5145/api/products/$($created.id)" -Method Delete
Write-Host "Product deleted successfully" -ForegroundColor Green

# Verify deletion
Write-Host "`nVerifying deletion..." -ForegroundColor Cyan
try {
    Invoke-RestMethod -Uri "http://localhost:5145/api/products/$($created.id)" -Method Get
} catch {
    Write-Host "Product not found (as expected) - 404 Not Found" -ForegroundColor Green
}
```

---

## cURL Examples (Cross-platform)

### Windows Command Prompt (cmd)

#### 1. Test API Root
```cmd
curl http://localhost:5145/
```

#### 2. Get All Products
```cmd
curl http://localhost:5145/api/products
```

#### 3. Get Product by ID
```cmd
curl http://localhost:5145/api/products/1
```

#### 4. Create a New Product
```cmd
curl -X POST http://localhost:5145/api/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Gaming Mouse\",\"description\":\"RGB gaming mouse with 16000 DPI\",\"price\":79.99,\"stock\":25}"
```

#### 5. Update a Product
```cmd
curl -X PUT http://localhost:5145/api/products/1 ^
  -H "Content-Type: application/json" ^
  -d "{\"price\":69.99,\"stock\":30}"
```

#### 6. Delete a Product
```cmd
curl -X DELETE http://localhost:5145/api/products/10
```

### PowerShell / Git Bash / Linux / macOS

#### 1. Test API Root
```bash
curl http://localhost:5145/
```

#### 2. Get All Products
```bash
curl http://localhost:5145/api/products
```

#### 3. Get Product by ID
```bash
curl http://localhost:5145/api/products/1
```

#### 4. Create a New Product
```bash
curl -X POST http://localhost:5145/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Gaming Mouse","description":"RGB gaming mouse with 16000 DPI","price":79.99,"stock":25}'
```

#### 5. Update a Product
```bash
curl -X PUT http://localhost:5145/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{"price":69.99,"stock":30}'
```

#### 6. Delete a Product
```bash
curl -X DELETE http://localhost:5145/api/products/10
```

---

## Visual Studio HTTP Client Examples

Copy these into your `.http` file:

```http
### Test Everything at Once

# 1. Get API Info
GET http://localhost:5145/
Accept: application/json

###

# 2. Get All Products
GET http://localhost:5145/api/products
Accept: application/json

###

# 3. Get Product by ID
GET http://localhost:5145/api/products/1
Accept: application/json

###

# 4. Create Product
POST http://localhost:5145/api/products
Content-Type: application/json

{
  "name": "Gaming Mouse",
  "description": "RGB gaming mouse with 16000 DPI",
  "price": 79.99,
  "stock": 25
}

###

# 5. Update Product (use ID from previous response)
PUT http://localhost:5145/api/products/1
Content-Type: application/json

{
  "price": 1099.99,
  "stock": 20
}

###

# 6. Delete Product
DELETE http://localhost:5145/api/products/10

###
```

---

## Browser Testing URLs

Just copy and paste these into your browser's address bar:

```
http://localhost:5145/
http://localhost:5145/health
http://localhost:5145/api/products
http://localhost:5145/api/products/1
http://localhost:5145/api/products/2
http://localhost:5145/api/products/3
```

---

## JavaScript Fetch API Examples

For testing in browser console (F12 ? Console):

### Get All Products
```javascript
fetch('http://localhost:5145/api/products')
  .then(response => response.json())
  .then(data => console.table(data))
  .catch(error => console.error('Error:', error));
```

### Get Product by ID
```javascript
fetch('http://localhost:5145/api/products/1')
  .then(response => response.json())
  .then(data => console.log(data))
  .catch(error => console.error('Error:', error));
```

### Create Product
```javascript
fetch('http://localhost:5145/api/products', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    name: 'Gaming Mouse',
    description: 'RGB gaming mouse with 16000 DPI',
    price: 79.99,
    stock: 25
  })
})
  .then(response => response.json())
  .then(data => console.log('Created:', data))
  .catch(error => console.error('Error:', error));
```

### Update Product
```javascript
fetch('http://localhost:5145/api/products/1', {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
  },
  body: JSON.stringify({
    price: 1099.99,
    stock: 20
  })
})
  .then(response => response.json())
  .then(data => console.log('Updated:', data))
  .catch(error => console.error('Error:', error));
```

### Delete Product
```javascript
fetch('http://localhost:5145/api/products/10', {
  method: 'DELETE'
})
  .then(response => {
    if (response.status === 204) {
      console.log('Product deleted successfully');
    } else {
      console.log('Failed to delete product');
    }
  })
  .catch(error => console.error('Error:', error));
```

---

## Python Examples

```python
import requests
import json

BASE_URL = "http://localhost:5145"

# Get all products
response = requests.get(f"{BASE_URL}/api/products")
print("All Products:", response.json())

# Get product by ID
response = requests.get(f"{BASE_URL}/api/products/1")
print("\nProduct 1:", response.json())

# Create product
new_product = {
    "name": "Gaming Mouse",
    "description": "RGB gaming mouse with 16000 DPI",
    "price": 79.99,
    "stock": 25
}
response = requests.post(f"{BASE_URL}/api/products", json=new_product)
print("\nCreated:", response.json())
created_id = response.json()["id"]

# Update product
update_data = {
    "price": 69.99,
    "stock": 30
}
response = requests.put(f"{BASE_URL}/api/products/{created_id}", json=update_data)
print("\nUpdated:", response.json())

# Delete product
response = requests.delete(f"{BASE_URL}/api/products/{created_id}")
print(f"\nDelete status: {response.status_code}")
```

---

## Validation Test Examples

Test that validation is working correctly:

### PowerShell - Test Invalid Product (Empty Name)
```powershell
$invalidProduct = @{
    name = ""
    description = "This should fail"
    price = -10
    stock = -5
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Post -Body $invalidProduct -ContentType "application/json"
} catch {
    Write-Host "Validation Error (Expected):" -ForegroundColor Yellow
    $_.ErrorDetails.Message | ConvertFrom-Json | ConvertTo-Json
}
```

### cURL - Test Invalid Product
```cmd
curl -X POST http://localhost:5145/api/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"\",\"price\":-10,\"stock\":-5}"
```

Expected response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Product name is required"],
    "Price": ["Price must be greater than 0"],
    "Stock": ["Stock cannot be negative"]
  }
}
```

---

## Pro Tips

1. **Save PowerShell script**: Save the PowerShell commands to a `.ps1` file and run them:
   ```powershell
   .\test-api.ps1
   ```

2. **Format JSON output**: Add `| jq` to curl commands for pretty JSON (requires jq installation):
   ```bash
   curl http://localhost:5145/api/products | jq
   ```

3. **Test in VS Code**: Install "REST Client" extension and use `.http` files

4. **Use Postman Collections**: Import these examples into Postman for easier testing

5. **Monitor in real-time**: Keep the console window visible to see logs as requests come in

---

**All examples assume the application is running on http://localhost:5145**

If using HTTPS, replace `http://localhost:5145` with `https://localhost:7016`
