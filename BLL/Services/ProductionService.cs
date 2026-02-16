using DAL.Interfaces;
using BLL.DTOs.Production;
using BLL.DTOs.Order;
using BLL.Interfaces;

namespace BLL.Services;

public class ProductionService : IProductionService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IFranchiseStoreRepository _storeRepository;

    public ProductionService(
        IOrderRepository orderRepository,
        IInventoryRepository inventoryRepository,
        IFranchiseStoreRepository storeRepository)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _storeRepository = storeRepository;
    }

    /// <summary>
    /// Step 1: Xem danh sách ??n t? các c?a hàng
    /// </summary>
    public async Task<List<KitchenOrderDetailDto>> GetKitchenOrdersAsync(
        string kitchenId, 
        KitchenOrderFilterDto? filter = null)
    {
        var orders = await _orderRepository.GetByKitchenIdAsync(kitchenId);

        // Apply filters
        if (filter != null)
        {
            if (!string.IsNullOrEmpty(filter.Status))
                orders = orders.Where(o => o.status == filter.Status).ToList();

            if (filter.FromDate.HasValue)
                orders = orders.Where(o => o.created_at >= filter.FromDate.Value).ToList();

            if (filter.ToDate.HasValue)
                orders = orders.Where(o => o.created_at <= filter.ToDate.Value).ToList();
        }

        var result = new List<KitchenOrderDetailDto>();

        foreach (var order in orders)
        {
            var store = await _storeRepository.GetByIdAsync(order.store_id ?? "");
            
            var orderDto = new KitchenOrderDetailDto
            {
                Id = order.id,
                StoreId = order.store_id ?? "",
                StoreName = store?.name ?? "",
                StoreAddress = store?.address ?? "",
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

            // Check if can produce
            var capacityCheck = await CheckProductionCapacityAsync(new CheckProductionCapacityDto
            {
                OrderId = order.id,
                KitchenId = kitchenId
            });

            orderDto.CanProduce = capacityCheck.CanProduce;
            orderDto.ProductionNote = capacityCheck.Message;

            result.Add(orderDto);
        }

        return result;
    }

    /// <summary>
    /// Step 2: Ki?m tra kh? n?ng s?n xu?t (s? l??ng yêu c?u vs t?n kho)
    /// </summary>
    public async Task<ProductionCapacityResponseDto> CheckProductionCapacityAsync(
        CheckProductionCapacityDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
            throw new Exception("Order not found");

        var response = new ProductionCapacityResponseDto
        {
            OrderId = dto.OrderId,
            Items = new List<ProductionItemCapacityDto>()
        };

        bool allSufficient = true;

        foreach (var item in order.order_items)
        {
            var inventory = await _inventoryRepository.GetByProductAndLocationAsync(
                item.product_id,
                dto.KitchenId,
                "kitchen"
            );

            decimal availableQty = inventory?.quantity ?? 0;
            decimal requiredQty = item.quantity;
            bool isSufficient = availableQty >= requiredQty;

            if (!isSufficient)
                allSufficient = false;

            response.Items.Add(new ProductionItemCapacityDto
            {
                ProductId = item.product_id,
                ProductName = item.product.name,
                RequiredQuantity = requiredQty,
                AvailableQuantity = availableQty,
                IsSufficient = isSufficient,
                ShortageAmount = isSufficient ? 0 : (requiredQty - availableQty)
            });
        }

        response.CanProduce = allSufficient;
        response.Message = allSufficient 
            ? "?? nguyên li?u ?? s?n xu?t" 
            : "Thi?u nguyên li?u. Vui lòng ki?m tra l?i t?n kho";

        return response;
    }

    /// <summary>
    /// Step 3a: Ch?p nh?n ??n
    /// </summary>
    public async Task<bool> AcceptOrderAsync(string orderId)
    {
        return await _orderRepository.UpdateStatusAsync(orderId, "confirmed");
    }

    /// <summary>
    /// Step 3b: T? ch?i ??n
    /// </summary>
    public async Task<bool> RejectOrderAsync(string orderId, string reason)
    {
        // TODO: Save rejection reason to audit log or order notes
        return await _orderRepository.UpdateStatusAsync(orderId, "rejected");
    }

    /// <summary>
    /// Step 4: B?t ??u s?n xu?t
    /// </summary>
    public async Task<bool> StartProductionAsync(StartProductionDto dto)
    {
        // Verify order status
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
            throw new Exception("Order not found");

        if (order.status != "confirmed")
            throw new Exception("Order must be confirmed before starting production");

        // Check capacity again
        var capacity = await CheckProductionCapacityAsync(new CheckProductionCapacityDto
        {
            OrderId = dto.OrderId,
            KitchenId = dto.KitchenId
        });

        if (!capacity.CanProduce)
            throw new Exception("Insufficient inventory to start production");

        // Update order status to preparing
        return await _orderRepository.UpdateStatusAsync(dto.OrderId, "preparing");
    }

    /// <summary>
    /// Step 5: Hoàn thành s?n xu?t
    /// </summary>
    public async Task<bool> CompleteProductionAsync(CompleteProductionDto dto)
    {
        var order = await _orderRepository.GetByIdAsync(dto.OrderId);
        if (order == null)
            throw new Exception("Order not found");

        if (order.status != "preparing")
            throw new Exception("Order must be in preparing status");

        // TODO: Deduct inventory, create production records, etc.

        // Update order status to ready
        return await _orderRepository.UpdateStatusAsync(dto.OrderId, "ready");
    }
}
