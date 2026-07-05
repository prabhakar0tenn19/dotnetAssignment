namespace Catalog.Api.Models;

// Product record: Catalog application ka core domain model hai.
// Isme product ki details jaise Id, Name, Price, aur Category store hoti hain.
public record Product(
    int Id,
    string Name,
    decimal Price,
    string Category
);
