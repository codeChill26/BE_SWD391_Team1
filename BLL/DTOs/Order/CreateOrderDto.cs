namespace BLL.DTOs.Order;

public class CreateOrderDto
{
    public string StoreId { get; set; } = null!;
    public string KitchenId { get; set; } = null!;
    public DateTime ExpectedDeliveryAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductId { get; set; } = null!;
    public decimal Quantity { get; set; }
}
