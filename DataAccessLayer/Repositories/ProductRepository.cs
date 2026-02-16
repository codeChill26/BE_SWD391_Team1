using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<product?> GetByIdAsync(string productId)
    {
        return await _context.products
            .Include(p => p.category)
            .FirstOrDefaultAsync(p => p.id == productId);
    }

    public async Task<List<product>> GetByIdsAsync(List<string> productIds)
    {
        return await _context.products
            .Include(p => p.category)
            .Where(p => productIds.Contains(p.id))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(string productId)
    {
        return await _context.products.AnyAsync(p => p.id == productId);
    }
}
