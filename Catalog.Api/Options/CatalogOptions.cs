namespace Catalog.Api.Options;

// CatalogOptions: App settings ko represent karne wali Options configuration mapping class.
// appsettings.json file me define settings ko strongly-typed object se mapping ke liye use hoti hai.
public class CatalogOptions
{
    // Configuration section name constants taaki binding ke time hardcoding avoid ki ja sake
    public const string SectionName = "Catalog";

    // Products fetch karne ki default limit size (GET endpoint me use hoga)
    public int DefaultPageSize { get; set; } = 10;
}
