using BLL.DTOs.Production;
using BLL.DTOs.Order;

namespace BLL.Interfaces;

public interface IProductionService
{
    // Step 1: Xem danh sách ??n t? các c?a hàng
    Task<List<KitchenOrderDetailDto>> GetKitchenOrdersAsync(string kitchenId, KitchenOrderFilterDto? filter = null);
    
    // Step 2: Ki?m tra kh? n?ng s?n xu?t (s? l??ng yêu c?u vs t?n kho)
    Task<ProductionCapacityResponseDto> CheckProductionCapacityAsync(CheckProductionCapacityDto dto);
    
    // Step 3: Ch?p nh?n ??n (confirmed) ho?c t? ch?i
    Task<bool> AcceptOrderAsync(string orderId);
    Task<bool> RejectOrderAsync(string orderId, string reason);
    
    // Step 4: B?t ??u s?n xu?t (preparing)
    Task<bool> StartProductionAsync(StartProductionDto dto);
    
    // Step 5: Hoàn thành s?n xu?t (ready)
    Task<bool> CompleteProductionAsync(CompleteProductionDto dto);
}
