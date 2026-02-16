using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _context;

    public OrderItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(order_item orderItem)
    {
        _context.order_items.Add(orderItem);
        await _context.SaveChangesAsync();
    }

    public async Task CreateRangeAsync(IEnumerable<order_item> orderItems)
    {
        _context.order_items.AddRange(orderItems);
        await _context.SaveChangesAsync();
    }

    public async Task<List<order_item>> GetByOrderIdAsync(string orderId)
    {
        return await _context.order_items
            .Include(oi => oi.product)
            .Where(oi => oi.order_id == orderId)
            .ToListAsync();
    }
}
