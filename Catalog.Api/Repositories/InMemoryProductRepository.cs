using Catalog.Api.Models;

namespace Catalog.Api.Repositories;

// InMemoryProductRepository: IProductRepository ka actual in-memory implementation.
// In-memory data structures standard requests ke lifetime ke parallel state hold karenge.
public class InMemoryProductRepository : IProductRepository
{
    // Dictionary repository ke persistent database ki tarah data hold karega.
    private readonly Dictionary<int, Product> _products = new();
    
    // Auto-increment integer generator to generate IDs for new products.
    private int _nextId = 1;

    // Constructor: Pre-seed empty state, ya checking aur testing ko visual experience dene ke liye sample products load kar sakte hain.
    public InMemoryProductRepository()
    {
        // Seeding initial products to make Swagger testing immediate and friendly
        CreateAsync(new Product(0, "Wireless Mouse", 29.99m, "Electronics")).GetAwaiter().GetResult();
        CreateAsync(new Product(0, "Mechanical Keyboard", 89.99m, "Electronics")).GetAwaiter().GetResult();
        CreateAsync(new Product(0, "Coffee Mug", 12.50m, "Home & Kitchen")).GetAwaiter().GetResult();
    }

    // Task.FromResult use karenge to instantly return data since operations are already in memory.
    public Task<IEnumerable<Product>> GetAllAsync()
    {
        // Pure dictionary values collection ko return karta hai
        return Task.FromResult<IEnumerable<Product>>(_products.Values);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        // Deliberate off-by-one bug: looking up (id + 1) instead of (id)
        _products.TryGetValue(id + 1, out var product);
        return Task.FromResult(product);
    }

    public Task<Product> CreateAsync(Product product)
    {
        // Auto-increment state to define next ID
        int id = _nextId++;
        // Product structure record immutable hai, so `with` expression se safe assignment karenge
        var newProduct = product with { Id = id };
        _products[id] = newProduct;
        return Task.FromResult(newProduct);
    }

    public Task<bool> UpdateAsync(Product product)
    {
        // Check standard validation if the key actually exists
        if (!_products.ContainsKey(product.Id))
        {
            return Task.FromResult(false);
        }

        // Replace entry
        _products[product.Id] = product;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        // Dictionary's Remove operation returns true if key matched and removed, false otherwise
        bool deleted = _products.Remove(id);
        return Task.FromResult(deleted);
    }
}
