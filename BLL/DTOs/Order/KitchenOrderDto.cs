namespace BLL.DTOs.Order;

/// <summary>
/// DTO ?? l?y danh sách ??n hàng c?a b?p
/// </summary>
public class KitchenOrderFilterDto
{
    public string? Status { get; set; } // pending, confirmed, preparing, ready
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Order detail cho Central Kitchen
/// </summary>
public class KitchenOrderDetailDto
{
    public string Id { get; set; } = null!;
    public string StoreId { get; set; } = null!;
    public string StoreName { get; set; } = null!;
    public string StoreAddress { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpectedDeliveryAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
    public bool CanProduce { get; set; }
    public string? ProductionNote { get; set; }
}
