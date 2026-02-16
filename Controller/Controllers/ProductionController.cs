using BLL.DTOs.Production;
using BLL.DTOs.Order;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly IProductionService _productionService;
    private readonly ILogger<ProductionController> _logger;

    public ProductionController(
        IProductionService productionService, 
        ILogger<ProductionController> logger)
    {
        _productionService = productionService;
        _logger = logger;
    }

    /// <summary>
    /// Step 1: Xem danh sách ??n t? các c?a hàng
    /// </summary>
    /// <param name="kitchenId">ID c?a b?p trung tâm</param>
    /// <param name="filter">B? l?c ??n hàng (status, date range)</param>
    /// <returns>Danh sách ??n hàng c?n x? lý</returns>
    [HttpGet("kitchen/{kitchenId}/orders")]
    public async Task<ActionResult<List<KitchenOrderDetailDto>>> GetKitchenOrders(
        string kitchenId, 
        [FromQuery] KitchenOrderFilterDto? filter)
    {
        try
        {
            var orders = await _productionService.GetKitchenOrdersAsync(kitchenId, filter);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kitchen orders");
            return StatusCode(500, new { message = "Có l?i x?y ra khi l?y danh sách ??n hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// Step 2: Ki?m tra kh? n?ng s?n xu?t (H? th?ng ki?m tra)
    /// </summary>
    /// <param name="dto">OrderId và KitchenId</param>
    /// <returns>Thông tin v? s? l??ng yêu c?u, t?n kho nguyên li?u</returns>
    [HttpPost("check-capacity")]
    public async Task<ActionResult<ProductionCapacityResponseDto>> CheckProductionCapacity(
        [FromBody] CheckProductionCapacityDto dto)
    {
        try
        {
            var result = await _productionService.CheckProductionCapacityAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking production capacity");
            return StatusCode(500, new { message = "Có l?i x?y ra khi ki?m tra kh? n?ng s?n xu?t", error = ex.Message });
        }
    }

    /// <summary>
    /// Step 3a: Ch?p nh?n ??n ? ??a vào k? ho?ch s?n xu?t
    /// </summary>
    /// <param name="orderId">ID ??n hàng</param>
    /// <returns>Success message</returns>
    [HttpPatch("{orderId}/accept")]
    public async Task<ActionResult> AcceptOrder(string orderId)
    {
        try
        {
            var success = await _productionService.AcceptOrderAsync(orderId);
            
            if (!success)
                return NotFound(new { message = "Không tìm th?y ??n hàng" });

            return Ok(new { 
                message = "?ã ch?p nh?n ??n hàng. Tr?ng thái: 'Confirmed'", 
                orderId,
                nextStep = "B?t ??u s?n xu?t khi s?n sàng"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting order");
            return StatusCode(500, new { message = "Có l?i x?y ra khi ch?p nh?n ??n hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// Step 3b: T? ch?i ??n (ho?c ch? nguyên li?u)
    /// </summary>
    /// <param name="orderId">ID ??n hàng</param>
    /// <param name="dto">Lý do t? ch?i</param>
    /// <returns>Success message</returns>
    [HttpPatch("{orderId}/reject")]
    public async Task<ActionResult> RejectOrder(string orderId, [FromBody] RejectOrderDto dto)
    {
        try
        {
            var success = await _productionService.RejectOrderAsync(orderId, dto.Reason);
            
            if (!success)
                return NotFound(new { message = "Không tìm th?y ??n hàng" });

            return Ok(new { 
                message = $"?ã t? ch?i ??n hàng. Lý do: {dto.Reason}", 
                orderId 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting order");
            return StatusCode(500, new { message = "Có l?i x?y ra khi t? ch?i ??n hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// Step 4: B?t ??u s?n xu?t (Preparing)
    /// </summary>
    /// <param name="dto">OrderId và KitchenId</param>
    /// <returns>Success message</returns>
    [HttpPost("start")]
    public async Task<ActionResult> StartProduction([FromBody] StartProductionDto dto)
    {
        try
        {
            var success = await _productionService.StartProductionAsync(dto);
            
            if (!success)
                return BadRequest(new { message = "Không th? b?t ??u s?n xu?t" });

            return Ok(new { 
                message = "?ã b?t ??u s?n xu?t. Tr?ng thái: 'Preparing'",
                orderId = dto.OrderId,
                nextStep = "Hoàn thành s?n xu?t khi xong"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting production");
            return StatusCode(500, new { message = ex.Message, error = ex.Message });
        }
    }

    /// <summary>
    /// Step 5: Hoàn thành s?n xu?t (Ready) ? chuy?n sang "S?n sàng giao"
    /// </summary>
    /// <param name="dto">OrderId và danh sách s?n ph?m ?ã s?n xu?t</param>
    /// <returns>Success message</returns>
    [HttpPost("complete")]
    public async Task<ActionResult> CompleteProduction([FromBody] CompleteProductionDto dto)
    {
        try
        {
            var success = await _productionService.CompleteProductionAsync(dto);
            
            if (!success)
                return BadRequest(new { message = "Không th? hoàn thành s?n xu?t" });

            return Ok(new { 
                message = "?ã hoàn thành s?n xu?t. Tr?ng thái: 'Ready' - S?n sàng giao hàng",
                orderId = dto.OrderId,
                nextStep = "Chuy?n sang Flow C: Giao hàng"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing production");
            return StatusCode(500, new { message = ex.Message, error = ex.Message });
        }
    }
}

public class RejectOrderDto
{
    public string Reason { get; set; } = null!;
}
