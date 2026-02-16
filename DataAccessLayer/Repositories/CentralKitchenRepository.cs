using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class CentralKitchenRepository : ICentralKitchenRepository
{
    private readonly AppDbContext _context;

    public CentralKitchenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<central_kitchen?> GetByIdAsync(string kitchenId)
    {
        return await _context.central_kitchens.FindAsync(kitchenId);
    }

    public async Task<bool> ExistsAsync(string kitchenId)
    {
        return await _context.central_kitchens.AnyAsync(k => k.id == kitchenId);
    }
}
