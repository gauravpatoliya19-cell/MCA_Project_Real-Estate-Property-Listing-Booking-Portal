using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Repositories.Interfaces
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllAsync();
        Task<IEnumerable<Property>> GetFeaturedAsync();
        Task<Property?> GetByIdAsync(int id);
        Task<Property?> GetDetailsByIdAsync(int id);
        Task AddAsync(Property property);
        Task UpdateAsync(Property property);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(int id, PropertyStatus status);
        Task<IEnumerable<string>> GetCitiesAsync();
        Task<int> GetCountByStatusAsync(PropertyStatus status);
    }
}
