using DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IInventoryRepository
{
    Task<List<inventory>> GetByLocationAsync(string locationId, string locationType);
    Task<inventory?> GetByProductAndLocationAsync(string productId, string locationId, string locationType);
    Task<bool> CheckAvailabilityAsync(string productId, string locationId, string locationType, decimal requiredQuantity);
}
