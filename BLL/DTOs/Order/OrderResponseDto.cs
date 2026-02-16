namespace BLL.DTOs.Order;

public class OrderResponseDto
{
    public string Id { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public string StoreName { get; set; } = null!;
    public string KitchenId { get; set; } = null!;
    public string KitchenName { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpectedDeliveryAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal Quantity { get; set; }
}
