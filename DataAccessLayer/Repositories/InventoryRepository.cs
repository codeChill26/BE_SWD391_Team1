using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly AppDbContext _context;

    public InventoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<inventory>> GetByLocationAsync(string locationId, string locationType)
    {
        return await _context.inventories
            .Include(i => i.product)
                .ThenInclude(p => p.category)
            .Where(i => i.location_id == locationId && i.location_type == locationType)
            .ToListAsync();
    }

    public async Task<inventory?> GetByProductAndLocationAsync(string productId, string locationId, string locationType)
    {
        return await _context.inventories
            .FirstOrDefaultAsync(i => 
                i.product_id == productId && 
                i.location_id == locationId && 
                i.location_type == locationType);
    }

    public async Task<bool> CheckAvailabilityAsync(string productId, string locationId, string locationType, decimal requiredQuantity)
    {
        var inventory = await GetByProductAndLocationAsync(productId, locationId, locationType);
        return inventory != null && inventory.quantity >= requiredQuantity;
    }
}
