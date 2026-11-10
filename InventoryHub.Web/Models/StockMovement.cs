namespace InventoryHub.Web.Models;

public enum MovementType { In, Out, Adjustment }

public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public MovementType Type { get; set; }
    public int Quantity { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}