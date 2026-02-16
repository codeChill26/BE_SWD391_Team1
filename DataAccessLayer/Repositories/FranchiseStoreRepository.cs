using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class FranchiseStoreRepository : IFranchiseStoreRepository
{
    private readonly AppDbContext _context;

    public FranchiseStoreRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<franchise_store?> GetByIdAsync(string storeId)
    {
        return await _context.franchise_stores.FindAsync(storeId);
    }

    public async Task<bool> ExistsAsync(string storeId)
    {
        return await _context.franchise_stores.AnyAsync(s => s.id == storeId);
    }
}
