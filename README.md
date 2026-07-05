# Catalog.Api - ASP.NET Core Web API (.NET 10 & Swagger)

This is a minimal, clean, and controller-based **ASP.NET Core Web API** assignment project built using **.NET 10** and **Swagger (Swashbuckle)**. It performs CRUD operations on products using in-memory storage.

---

## Technical Stack & Configuration

1. **Target Framework**: `.NET 10.0`
2. **API Documentation**: `Swashbuckle.AspNetCore` (Version 10.2.3) - Swagger UI (`/swagger`)
3. **Data Storage**: In-memory `Dictionary<int, Product>`
4. **Dependency Injection (DI)**: Singleton lifetime for the repository to persist data across HTTP requests.
5. **Configuration**: Options Pattern (`CatalogOptions`) mapped with `DefaultPageSize` in `appsettings.json`.

---

## Project Structure

The project follows the following folder structure:

* **Catalog.Api**: Main API project directory.

  * **Controllers**: Handles HTTP requests.

    * `ProductsController.cs` - CRUD endpoints (GET, POST, PUT, DELETE).
  * **Models**: Domain entities.

    * `Product.cs` - Product record.
  * **DTOs**: Data Transfer Objects.

    * `CreateProductDto.cs` - Request body schema for creating and updating products.
  * **Repositories**: Data access abstraction and implementation.

    * `IProductRepository.cs` - Interface for asynchronous CRUD operations.
    * `InMemoryProductRepository.cs` - In-memory implementation using a plain `Dictionary`.
  * **Options**: Configuration settings.

    * `CatalogOptions.cs` - Application configuration class.

---

## Step-by-Step Implementation Details

### Step 1: Project Setup and Cleanup

* Created a controller-based Web API project using the .NET 10 CLI:

```bash
dotnet new webapi --use-controllers -f net10.0 -n Catalog.Api
```

* Removed the default template files:

  * `WeatherForecast.cs`
  * `WeatherForecastController.cs`

* Removed the default .NET 10 OpenAPI dependency and installed **Swashbuckle.AspNetCore** according to the assignment requirements:

  * Removed `Microsoft.AspNetCore.OpenApi`
  * Added `Swashbuckle.AspNetCore`

* Configured Swagger in `Program.cs` using:

  * `AddEndpointsApiExplorer()`
  * `AddSwaggerGen()`
  * `UseSwagger()`
  * `UseSwaggerUI()`

### Step 2: Models & DTOs

* **Product.cs**: Defined the `Product` record with:

  * `Id` (int)
  * `Name` (string)
  * `Price` (decimal)
  * `Category` (string)

* **CreateProductDto.cs**: Created a DTO for creating and updating products without the `Id` property.

### Step 3: In-Memory Data Storage

* **IProductRepository.cs**: Declared fully asynchronous CRUD operations.

* **InMemoryProductRepository.cs**:

  * Implemented storage using `Dictionary<int, Product>`.
  * Used an auto-incrementing `_nextId` field to generate unique IDs.
  * Pre-seeded three sample products to simplify testing and verification.

### Step 4: Dependency Injection & Configuration Using the Options Pattern

* Registered `InMemoryProductRepository` as a Singleton so that in-memory data persists across API requests:

```csharp
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
```

* Added the following configuration section to `appsettings.json`:

```json
"Catalog": {
  "DefaultPageSize": 2
}
```

* Created `CatalogOptions.cs` and bound it using the Options pattern:

```csharp
builder.Services.Configure<CatalogOptions>(
    builder.Configuration.GetSection(CatalogOptions.SectionName));
```

* Injected `IOptions<CatalogOptions>` into `ProductsController` and used:

```csharp
Take(_options.DefaultPageSize)
```

to limit the number of products returned by `GET /api/products`.

### Step 5: CRUD Endpoints

* **GET `/api/products`**

  * Returns a list of products based on `DefaultPageSize`.
  * Response: `200 OK`

* **GET `/api/products/{id}`**

  * Returns a product by ID.
  * Response: `200 OK` if found, otherwise `404 Not Found`

* **POST `/api/products`**

  * Creates a new product.
  * Returns `400 Bad Request` for invalid input.
  * Returns `201 Created` using `CreatedAtAction` on success.

* **PUT `/api/products/{id}`**

  * Updates an existing product using the DTO.
  * Returns `204 No Content` on success.
  * Returns `404 Not Found` if the product does not exist.

* **DELETE `/api/products/{id}`**

  * Removes a product.
  * Returns `204 No Content` on success.
  * Returns `404 Not Found` if the product does not exist.

---

## How to Run & Test

### 1. Build the Project

Navigate to the project directory and run:

```bash
dotnet build Catalog.Api
```

### 2. Run the Web API

Start the API server using:

```bash
dotnet run --project Catalog.Api
```

By default, the application will listen on:

* `http://localhost:5233`
* `https://localhost:7053`

### 3. Open Swagger UI

Open the following URL in your browser:

```text
http://localhost:5233/swagger
```

You can test all endpoints (GET, POST, PUT, DELETE) directly from the Swagger interface.

### 4. Verify the Options Pattern (DefaultPageSize)

* The in-memory repository is pre-seeded with three products.
* `GET /api/products` returns only **two products** because `DefaultPageSize` is set to `2` in `appsettings.json`.
* This confirms that the Options Pattern has been configured and injected correctly.
