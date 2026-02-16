using BLL.DTOs.Order;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Step 2: T?o ??n ??t hàng g?i b?p trung tâm
    /// </summary>
    /// <param name="dto">Thông tin ??n hàng (ch?n s?n ph?m, s? l??ng, th?i gian)</param>
    /// <returns>??n hàng ???c t?o v?i tr?ng thái "Ch? b?p ti?p nh?n"</returns>
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                return BadRequest(new { message = "??n hàng ph?i có ít nh?t 1 s?n ph?m" });
            }

            var order = await _orderService.CreateOrderAsync(dto);
            
            return CreatedAtAction(
                nameof(GetOrderById), 
                new { id = order.Id }, 
                new 
                { 
                    message = "??n hàng ???c t?o thành công v?i tr?ng thái 'Ch? b?p ti?p nh?n'",
                    order 
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { message = "Có l?i x?y ra khi t?o ??n hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// L?y chi ti?t ??n hàng theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto>> GetOrderById(string id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order");
            return NotFound(new { message = "Không tìm th?y ??n hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// L?y danh sách ??n hàng c?a c?a hàng
    /// </summary>
    [HttpGet("store/{storeId}")]
    public async Task<ActionResult<List<OrderResponseDto>>> GetOrdersByStore(string storeId)
    {
        try
        {
            var orders = await _orderService.GetOrdersByStoreAsync(storeId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting store orders");
            return StatusCode(500, new { message = "Có l?i x?y ra khi l?y danh sách ??n hàng", error = ex.Message });
        }
    }

    /// <summary>
    /// C?p nh?t tr?ng thái ??n hàng
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult> UpdateOrderStatus(string id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
            
            if (!success)
                return NotFound(new { message = "Không tìm th?y ??n hàng" });

            return Ok(new { message = $"C?p nh?t tr?ng thái ??n hàng thành '{dto.Status}' thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            return StatusCode(500, new { message = "Có l?i x?y ra khi c?p nh?t tr?ng thái", error = ex.Message });
        }
    }
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; } = null!;
}
