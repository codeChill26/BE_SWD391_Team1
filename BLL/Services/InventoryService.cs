using DAL.Interfaces;
using BLL.DTOs.Inventory;
using BLL.Interfaces;

namespace BLL.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    /// <summary>
    /// L?y danh sách t?n kho t?i b?p trung tâm
    /// </summary>
    public async Task<List<InventoryItemDto>> GetKitchenInventoryAsync(string kitchenId)
    {
        var inventory = await _inventoryRepository.GetByLocationAsync(kitchenId, "kitchen");

        return inventory.Select(i => new InventoryItemDto
        {
            ProductId = i.product_id,
            ProductName = i.product.name,
            CategoryName = i.product.category.name,
            Unit = i.product.unit ?? "",
            AvailableQuantity = i.quantity
        }).ToList();
    }

    /// <summary>
    /// Ki?m tra t?n kho có ?? ?? ?áp ?ng ??n hàng không
    /// </summary>
    public async Task<Dictionary<string, bool>> CheckInventoryAvailabilityAsync(
        string kitchenId, 
        Dictionary<string, decimal> requestedItems)
    {
        var result = new Dictionary<string, bool>();

        foreach (var item in requestedItems)
        {
            var productId = item.Key;
            var requestedQty = item.Value;

            var isAvailable = await _inventoryRepository.CheckAvailabilityAsync(
                productId, kitchenId, "kitchen", requestedQty);

            result[productId] = isAvailable;
        }

        return result;
    }
}
