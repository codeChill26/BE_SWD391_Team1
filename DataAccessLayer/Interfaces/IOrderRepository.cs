using DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IOrderRepository
{
    Task<order> CreateAsync(order order);
    Task<order?> GetByIdAsync(string orderId);
    Task<List<order>> GetByStoreIdAsync(string storeId);
    Task<List<order>> GetByKitchenIdAsync(string kitchenId);
    Task<bool> UpdateStatusAsync(string orderId, string status);
    Task<bool> ExistsAsync(string orderId);
}
