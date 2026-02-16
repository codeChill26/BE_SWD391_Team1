using BLL.DTOs.Inventory;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
    {
        _inventoryService = inventoryService;
        _logger = logger;
    }

    /// <summary>
    /// Step 1: Ki?m tra t?n kho c?a b?p trung tâm
    /// </summary>
    /// <param name="kitchenId">ID c?a b?p trung tâm</param>
    /// <returns>Danh sách s?n ph?m và s? l??ng t?n kho</returns>
    [HttpGet("kitchen/{kitchenId}")]
    public async Task<ActionResult<List<InventoryItemDto>>> GetKitchenInventory(string kitchenId)
    {
        try
        {
            var inventory = await _inventoryService.GetKitchenInventoryAsync(kitchenId);
            return Ok(inventory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting kitchen inventory");
            return StatusCode(500, new { message = "Có l?i x?y ra khi l?y danh sách t?n kho", error = ex.Message });
        }
    }
}
