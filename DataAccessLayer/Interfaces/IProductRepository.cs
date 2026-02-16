using DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IProductRepository
{
    Task<product?> GetByIdAsync(string productId);
    Task<List<product>> GetByIdsAsync(List<string> productIds);
    Task<bool> ExistsAsync(string productId);
}
