using BLL.DTOs.Order;

namespace BLL.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderResponseDto> GetOrderByIdAsync(string orderId);
    Task<List<OrderResponseDto>> GetOrdersByStoreAsync(string storeId);
    Task<bool> UpdateOrderStatusAsync(string orderId, string status);
}
