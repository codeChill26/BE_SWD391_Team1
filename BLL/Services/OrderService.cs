using DAL.Entities;
using DAL.Interfaces;
using BLL.DTOs.Order;
using BLL.Interfaces;

namespace BLL.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IFranchiseStoreRepository _storeRepository;
    private readonly ICentralKitchenRepository _kitchenRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IFranchiseStoreRepository storeRepository,
        ICentralKitchenRepository kitchenRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _storeRepository = storeRepository;
        _kitchenRepository = kitchenRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// T?o ??n hàng m?i (Step 2-3: T?o ??n + G?i ??n)
    /// </summary>
    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // Validate store exists
        if (!await _storeRepository.ExistsAsync(dto.StoreId))
            throw new Exception("Franchise store not found");

        // Validate kitchen exists
        if (!await _kitchenRepository.ExistsAsync(dto.KitchenId))
            throw new Exception("Central kitchen not found");

        // Validate products exist
        var productIds = dto.Items.Select(i => i.ProductId).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds);

        if (products.Count != productIds.Count)
            throw new Exception("Some products not found");

        // Generate order ID
        var orderId = $"ORD_{DateTime.Now:yyyyMMdd}_{Guid.NewGuid().ToString().Substring(0, 4)}";

        // Create order
        var order = new order
        {
            id = orderId,
            store_id = dto.StoreId,
            kitchen_id = dto.KitchenId,
            expected_delivery_at = dto.ExpectedDeliveryAt,
            status = "pending"
        };

        await _orderRepository.CreateAsync(order);

        // Create order items
        var orderItems = dto.Items.Select(item => new order_item
        {
            order_id = orderId,
            product_id = item.ProductId,
            quantity = item.Quantity
        }).ToList();

        await _orderItemRepository.CreateRangeAsync(orderItems);

        // Return response
        return await GetOrderByIdAsync(orderId);
    }

    /// <summary>
    /// L?y chi ti?t ??n hàng
    /// </summary>
    public async Task<OrderResponseDto> GetOrderByIdAsync(string orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new Exception("Order not found");

        return MapToDto(order);
    }

    /// <summary>
    /// L?y danh sách ??n hàng c?a c?a hàng
    /// </summary>
    public async Task<List<OrderResponseDto>> GetOrdersByStoreAsync(string storeId)
    {
        var orders = await _orderRepository.GetByStoreIdAsync(storeId);
        return orders.Select(MapToDto).ToList();
    }

    /// <summary>
    /// C?p nh?t tr?ng thái ??n hàng
    /// </summary>
    public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
    {
        return await _orderRepository.UpdateStatusAsync(orderId, status);
    }

    private OrderResponseDto MapToDto(order order)
    {
        return new OrderResponseDto
        {
            Id = order.id,
            StoreId = order.store_id ?? "",
            StoreName = order.store?.name ?? "",
            KitchenId = order.kitchen_id ?? "",
            KitchenName = order.kitchen?.name ?? "",
            Status = order.status,
            CreatedAt = order.created_at ?? DateTime.Now,
            ExpectedDeliveryAt = order.expected_delivery_at ?? DateTime.Now,
            Items = order.order_items.Select(oi => new OrderItemResponseDto
            {
                ProductId = oi.product_id,
                ProductName = oi.product.name,
                Unit = oi.product.unit ?? "",
                Quantity = oi.quantity
            }).ToList()
        };
    }
}
