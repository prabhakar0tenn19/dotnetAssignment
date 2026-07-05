namespace Catalog.Api.DTOs;

// CreateProductDto: Product create karne wali requests ke body data payload ko receive karne ke liye use hota hai.
// Isme client product ka Id send nahi karega, hum Id automatically generate karenge repository me.
public record CreateProductDto(
    string Name,
    decimal Price,
    string Category
);
