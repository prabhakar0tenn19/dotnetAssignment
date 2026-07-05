using Catalog.Api.Models;

namespace Catalog.Api.Repositories;

// IProductRepository: Product data access operations ko abstraction define karta hai.
// All operations strictly return a Task (asynchronous).
public interface IProductRepository
{
    // Sare products retrieve karne ke liye asynchronous operation.
    Task<IEnumerable<Product>> GetAllAsync();

    // Specific product Id ke basis pe filter karne ke liye operation. Nullable return hai agar matching product na mile.
    Task<Product?> GetByIdAsync(int id);

    // Naya product list me add karne ke liye. Repository auto-incrementing id calculate karke safe copy return karegi.
    Task<Product> CreateAsync(Product product);

    // Existing product update karne ke liye. Returns true agar update successful raha, false agar product exist nahi karta.
    Task<bool> UpdateAsync(Product product);

    // Product delete karne ke liye matching Id se. Returns true agar item deleted, false agar code match nahi hua.
    Task<bool> DeleteAsync(int id);
}
