# Quick Start Guide - Testing the API

## ? Your API is Currently Running!

The build system indicates your application is running in debug mode.

## How to Test the HTTP Requests

### Method 1: Using Visual Studio HTTP Client (Recommended)

1. **Keep the application running** (don't stop the debugger)
2. Open the file: `Minimal_API_Project_Sample.http`
3. You'll see green "Send Request" links above each request
4. Click **"Send Request"** on any request you want to test
5. The response will appear in a new panel on the right

**Example:**
```http
### Get All Products (HTTP)
GET {{Minimal_API_Project_Sample_HostAddress}}/api/products
Accept: application/json
```
Click "Send Request" above this line ??

### Method 2: Using Your Browser

Open any of these URLs in your browser:

**HTTP Endpoints:**
- API Info: http://localhost:5145/
- Health Check: http://localhost:5145/health
- All Products: http://localhost:5145/api/products
- Specific Product: http://localhost:5145/api/products/1

**HTTPS Endpoints:**
- API Info: https://localhost:7016/
- Health Check: https://localhost:7016/health
- All Products: https://localhost:7016/api/products
- Specific Product: https://localhost:7016/api/products/1

### Method 3: Using curl in PowerShell/Command Prompt

```powershell
# Get API Info
curl http://localhost:5145/

# Get All Products
curl http://localhost:5145/api/products

# Get Product by ID
curl http://localhost:5145/api/products/1

# Create a Product
curl -X POST http://localhost:5145/api/products `
  -H "Content-Type: application/json" `
  -d '{\"name\":\"Test Product\",\"description\":\"Testing\",\"price\":99.99,\"stock\":10}'

# Update a Product
curl -X PUT http://localhost:5145/api/products/1 `
  -H "Content-Type: application/json" `
  -d '{\"price\":1099.99,\"stock\":20}'

# Delete a Product
curl -X DELETE http://localhost:5145/api/products/10
```

### Method 4: Using PowerShell Invoke-RestMethod

```powershell
# Get All Products
Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Get

# Get Product by ID
Invoke-RestMethod -Uri "http://localhost:5145/api/products/1" -Method Get

# Create a Product
$body = @{
    name = "Test Product"
    description = "Testing the API"
    price = 99.99
    stock = 10
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5145/api/products" -Method Post -Body $body -ContentType "application/json"

# Update a Product
$updateBody = @{
    price = 89.99
    stock = 15
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5145/api/products/1" -Method Put -Body $updateBody -ContentType "application/json"

# Delete a Product
Invoke-RestMethod -Uri "http://localhost:5145/api/products/10" -Method Delete
```

## ?? Testing Recommendations

### Start with Simple GET Requests:

1. **Test API Info** (simplest test):
   - HTTP: http://localhost:5145/
   - Should return JSON with API information

2. **Test Health Check**:
   - HTTP: http://localhost:5145/health
   - Should return "Healthy"

3. **Test Get All Products**:
   - HTTP: http://localhost:5145/api/products
   - Should return array of 10 products

4. **Test Get Single Product**:
   - HTTP: http://localhost:5145/api/products/1
   - Should return the Laptop product

### Then Try POST/PUT/DELETE:

5. **Create a Product** (POST):
   - Use the HTTP file example for "Create Product"
   - Check the returned ID

6. **Update a Product** (PUT):
   - Use the returned ID from step 5
   - Update price or stock

7. **Delete a Product** (DELETE):
   - Delete the product created in step 5

## ?? Expected Responses

### GET /api/products
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "description": "High-performance laptop for developers",
    "price": 1299.99,
    "stock": 15
  },
  {
    "id": 2,
    "name": "Wireless Mouse",
    "description": "Ergonomic wireless mouse",
    "price": 29.99,
    "stock": 50
  }
  // ... 8 more products
]
```

### GET /api/products/1
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop for developers",
  "price": 1299.99,
  "stock": 15
}
```

### POST /api/products (Success)
```json
{
  "id": 11,
  "name": "Gaming Monitor",
  "description": "27-inch 144Hz gaming monitor with HDR",
  "price": 349.99,
  "stock": 20
}
```

### POST /api/products (Validation Error)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "Product name is required"
    ],
    "Price": [
      "Price must be greater than 0"
    ]
  }
}
```

### GET /api/products/999 (Not Found)
```
Status: 404 Not Found
(No content)
```

## ?? Quick Tests to Verify Everything Works

### Test 1: Basic Connectivity
```bash
curl http://localhost:5145/
```
? Should return API info with version and timestamp

### Test 2: Database Connection
```bash
curl http://localhost:5145/api/products
```
? Should return array of 10 products from database

### Test 3: Get Single Item
```bash
curl http://localhost:5145/api/products/1
```
? Should return the Laptop product

### Test 4: Validation
```bash
curl -X POST http://localhost:5145/api/products ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"\",\"price\":-10,\"stock\":-5}"
```
? Should return 400 Bad Request with validation errors

## ?? If Requests Still Don't Work

### Check These:

1. **Application is running:**
   - Look for console window showing "Now listening on..."
   - Or check Debug ? Windows ? Output in Visual Studio

2. **Correct ports:**
   - HTTP: port 5145
   - HTTPS: port 7016

3. **No typos in URLs:**
   - Must include `/api/products` (not `/products`)
   - Case-sensitive: use lowercase

4. **Content-Type header for POST/PUT:**
   - Must be: `Content-Type: application/json`

5. **Valid JSON in request body:**
   - Use double quotes for property names
   - No trailing commas

### Common Mistakes:

? **Wrong:** `GET http://localhost:5145/products` (missing `/api`)  
? **Correct:** `GET http://localhost:5145/api/products`

? **Wrong:** `POST` request without `Content-Type` header  
? **Correct:** Include `Content-Type: application/json` header

? **Wrong:** `{"name": 'Test'}` (single quotes)  
? **Correct:** `{"name": "Test"}` (double quotes)

## ?? Additional Resources

- **TROUBLESHOOTING.md** - Detailed troubleshooting guide
- **README.md** - Full project documentation
- **DATABASE_SETUP.md** - Database configuration help

## ?? Success Indicators

You'll know it's working when:
- ? Browser shows JSON responses
- ? HTTP file "Send Request" shows responses
- ? curl commands return JSON data
- ? No 404 or 500 errors
- ? You can create, read, update, and delete products

---

**Happy Testing! ??**

If you encounter any issues, check **TROUBLESHOOTING.md** for detailed solutions.
