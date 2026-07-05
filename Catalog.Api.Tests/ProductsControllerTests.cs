using Catalog.Api.Controllers;
using Catalog.Api.Models;
using Catalog.Api.Options;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace Catalog.Api.Tests;

// FakeProductRepository: Tests ko self-contained aur simple rakhne ke liye mock repository helper.
// Isme dynamic listing arrays modify kiye ja sakte hain bina kisi Moq package dependency ke.
public class FakeProductRepository : IProductRepository
{
    public List<Product> Products { get; set; } = new();

    public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult<IEnumerable<Product>>(Products);

    public Task<Product?> GetByIdAsync(int id) => Task.FromResult(Products.FirstOrDefault(p => p.Id == id));

    public Task<Product> CreateAsync(Product product)
    {
        var id = Products.Count > 0 ? Products.Max(p => p.Id) + 1 : 1;
        var newProduct = product with { Id = id };
        Products.Add(newProduct);
        return Task.FromResult(newProduct);
    }

    public Task<bool> UpdateAsync(Product product)
    {
        var existing = Products.FirstOrDefault(p => p.Id == product.Id);
        if (existing == null) return Task.FromResult(false);
        Products.Remove(existing);
        Products.Add(product);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var existing = Products.FirstOrDefault(p => p.Id == id);
        if (existing == null) return Task.FromResult(false);
        Products.Remove(existing);
        return Task.FromResult(true);
    }
}

public class ProductsControllerTests
{
    // Test: GET by id returns 404 Not Found when product is missing
    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductIsMissing()
    {
        // Arrange (Sare dependency fake and options build kar rahe hain)
        var fakeRepository = new FakeProductRepository(); // Empty repository list by default
        var options = Microsoft.Extensions.Options.Options.Create(new CatalogOptions { DefaultPageSize = 5 });
        var controller = new ProductsController(fakeRepository, options);

        // Act (Missing product ID 999 ke details load karne ka action execution)
        var result = await controller.GetById(999);

        // Assert (Result verify kiya ja raha hai ki code returns 404 (NotFoundObjectResult) with proper error message)
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal("Product with ID 999 was not found.", notFoundResult.Value);
    }
}
