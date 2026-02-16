using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<order> CreateAsync(order order)
    {
        _context.orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<order?> GetByIdAsync(string orderId)
    {
        return await _context.orders
            .Include(o => o.store)
            .Include(o => o.kitchen)
            .Include(o => o.order_items)
                .ThenInclude(oi => oi.product)
            .FirstOrDefaultAsync(o => o.id == orderId);
    }

    public async Task<List<order>> GetByStoreIdAsync(string storeId)
    {
        return await _context.orders
            .Include(o => o.store)
            .Include(o => o.kitchen)
            .Include(o => o.order_items)
                .ThenInclude(oi => oi.product)
            .Where(o => o.store_id == storeId)
            .OrderByDescending(o => o.created_at)
            .ToListAsync();
    }

    public async Task<List<order>> GetByKitchenIdAsync(string kitchenId)
    {
        return await _context.orders
            .Include(o => o.store)
            .Include(o => o.kitchen)
            .Include(o => o.order_items)
                .ThenInclude(oi => oi.product)
            .Where(o => o.kitchen_id == kitchenId)
            .OrderByDescending(o => o.created_at)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(string orderId, string status)
    {
        var order = await _context.orders.FindAsync(orderId);
        if (order == null)
            return false;

        order.status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string orderId)
    {
        return await _context.orders.AnyAsync(o => o.id == orderId);
    }
}
