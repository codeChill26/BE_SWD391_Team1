namespace BLL.DTOs.Production;

/// <summary>
/// DTO ?? c?p nh?t tr?ng thái s?n xu?t
/// </summary>
public class UpdateProductionStatusDto
{
    public string OrderId { get; set; } = null!;
    public string Status { get; set; } = null!; // confirmed, preparing, ready
    public string? Note { get; set; }
}

/// <summary>
/// DTO ?? b?t ??u s?n xu?t
/// </summary>
public class StartProductionDto
{
    public string OrderId { get; set; } = null!;
    public string KitchenId { get; set; } = null!;
}

/// <summary>
/// DTO ?? hoàn thành s?n xu?t
/// </summary>
public class CompleteProductionDto
{
    public string OrderId { get; set; } = null!;
    public List<ProducedItemDto> ProducedItems { get; set; } = new();
}

public class ProducedItemDto
{
    public string ProductId { get; set; } = null!;
    public decimal ActualQuantity { get; set; }
    public string? QualityNote { get; set; }
}
