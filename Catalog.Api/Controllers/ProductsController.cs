using Catalog.Api.DTOs;
using Catalog.Api.Models;
using Catalog.Api.Options;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Catalog.Api.Controllers;

// [ApiController] attribute enables automatic model state validation, HTTP response binding, and routing.
// Route: api/products ke path par controller exposed rahega.
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly CatalogOptions _options;

    // Dependency Injection ke through IProductRepository aur CatalogOptions inject kar rahe hain
    public ProductsController(IProductRepository productRepository, IOptions<CatalogOptions> options)
    {
        _productRepository = productRepository;
        _options = options.Value;
    }

    // GET: api/products/health
    // App aur repository connection status retrieve karne ke liye simple health check endpoint.
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            Status = "Healthy",
            Storage = "In-Memory Dictionary (Active)",
            Timestamp = DateTime.UtcNow
        });
    }

    // GET: api/products
    // Sabhi products fetch karne ka asynchronous endpoint. appsettings.json me configured DefaultPageSize key se records constraint kiye gaye hain.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _productRepository.GetAllAsync();
        // default settings se top N records return karenge
        var limitedProducts = products.Take(_options.DefaultPageSize);
        return Ok(limitedProducts);
    }

    // GET: api/products/{id}
    // Specific product fetching endpoint by ID.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            // Product nahi mila - 404 Not Found return karega
            return NotFound($"Product with ID {id} was not found.");
        }
        // Product mil gaya - 200 OK return karega
        return Ok(product);
    }

    // POST: api/products
    // Naya product create karne ka endpoint. Body me DTO object map hoga.
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] CreateProductDto dto)
    {
        // Simple manual validation checking for bad inputs (400 Bad Request)
        if (dto == null)
        {
            return BadRequest("Product data is required.");
        }
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Product name is required.");
        }
        if (dto.Price < 0)
        {
            return BadRequest("Product price cannot be negative.");
        }
        if (string.IsNullOrWhiteSpace(dto.Category))
        {
            return BadRequest("Product category is required.");
        }

        // DTO mapping to core Product domain record (Initial ID key will be 0)
        var productToCreate = new Product(0, dto.Name, dto.Price, dto.Category);
        var createdProduct = await _productRepository.CreateAsync(productToCreate);

        // 201 Created return ho raha hai using CreatedAtAction with headers pointing to the resource location
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    // PUT: api/products/{id}
    // Product data key updates submit karne ke liye endpoint.
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Product update data is required.");
        }
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Product name is required.");
        }
        if (dto.Price < 0)
        {
            return BadRequest("Product price cannot be negative.");
        }
        if (string.IsNullOrWhiteSpace(dto.Category))
        {
            return BadRequest("Product category is required.");
        }

        // Product copy generate karenge custom route ID ke basis pe
        var updatedProduct = new Product(id, dto.Name, dto.Price, dto.Category);
        bool isUpdated = await _productRepository.UpdateAsync(updatedProduct);

        if (!isUpdated)
        {
            // Target item dictionary me nahi tha
            return NotFound($"Product with ID {id} not found for update.");
        }

        // Successfully updated (204 No Content)
        return NoContent();
    }

    // DELETE: api/products/{id}
    // Matching id product removal ke liye endpoint.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool isDeleted = await _productRepository.DeleteAsync(id);
        if (!isDeleted)
        {
            // Product not found
            return NotFound($"Product with ID {id} not found for deletion.");
        }

        // Successfully deleted (204 No Content)
        return NoContent();
    }
}
