using BLL.DTOs.Inventory;

namespace BLL.Interfaces;

public interface IInventoryService
{
    Task<List<InventoryItemDto>> GetKitchenInventoryAsync(string kitchenId);
    Task<Dictionary<string, bool>> CheckInventoryAvailabilityAsync(string kitchenId, Dictionary<string, decimal> requestedItems);
}
