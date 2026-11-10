namespace InventoryHub.Web.Dtos;

public record ProductResponse(
    int Id,
    string Name,
    string Sku,
    decimal Price,
    int Stock,
    int CategoryId,
    string CategoryName,
    bool IsActive,
    DateTime UpdatedUtc);