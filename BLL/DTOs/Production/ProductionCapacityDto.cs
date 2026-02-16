namespace BLL.DTOs.Production;

/// <summary>
/// DTO ?? ki?m tra kh? n?ng s?n xu?t
/// </summary>
public class CheckProductionCapacityDto
{
    public string OrderId { get; set; } = null!;
    public string KitchenId { get; set; } = null!;
}

/// <summary>
/// Response v? kh? n?ng s?n xu?t
/// </summary>
public class ProductionCapacityResponseDto
{
    public string OrderId { get; set; } = null!;
    public bool CanProduce { get; set; }
    public List<ProductionItemCapacityDto> Items { get; set; } = new();
    public string? Message { get; set; }
}

public class ProductionItemCapacityDto
{
    public string ProductId { get; set; } = null!;
    public string ProductName { get; set; } = null!;
    public decimal RequiredQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public bool IsSufficient { get; set; }
    public decimal ShortageAmount { get; set; }
}
