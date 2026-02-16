using DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IOrderItemRepository
{
    Task CreateAsync(order_item orderItem);
    Task CreateRangeAsync(IEnumerable<order_item> orderItems);
    Task<List<order_item>> GetByOrderIdAsync(string orderId);
}
