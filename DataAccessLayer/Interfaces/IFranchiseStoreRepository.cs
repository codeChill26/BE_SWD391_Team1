using DAL.Entities;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IFranchiseStoreRepository
{
    Task<franchise_store?> GetByIdAsync(string storeId);
    Task<bool> ExistsAsync(string storeId);
}
