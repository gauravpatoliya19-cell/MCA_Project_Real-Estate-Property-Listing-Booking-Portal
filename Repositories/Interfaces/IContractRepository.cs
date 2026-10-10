using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Repositories.Interfaces
{
    public interface IContractRepository
    {
        Task<IEnumerable<RentalContract>> GetAllAsync();
        Task<RentalContract?> GetByIdAsync(int id);
        Task<RentalContract?> GetByPropertyIdAsync(int propertyId);
        Task AddAsync(RentalContract contract);
        Task UpdateAsync(RentalContract contract);
        Task<int> GetCountAsync();
    }
}
