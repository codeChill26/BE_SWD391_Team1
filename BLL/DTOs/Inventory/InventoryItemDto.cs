namespace BLL.DTOs.Inventory;

public class InventoryItemDto
{
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal AvailableQuantity { get; set; }
}
