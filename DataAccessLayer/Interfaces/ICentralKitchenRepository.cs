using DAL.Entities;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface ICentralKitchenRepository
{
    Task<central_kitchen?> GetByIdAsync(string kitchenId);
    Task<bool> ExistsAsync(string kitchenId);
}
