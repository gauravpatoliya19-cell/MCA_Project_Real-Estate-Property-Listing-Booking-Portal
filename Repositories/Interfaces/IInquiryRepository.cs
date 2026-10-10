using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Repositories.Interfaces
{
    public interface IInquiryRepository
    {
        Task<IEnumerable<Inquiry>> GetAllAsync();
        Task<Inquiry?> GetByIdAsync(int id);
        Task<IEnumerable<Inquiry>> GetByPropertyIdAsync(int propertyId);
        Task AddAsync(Inquiry inquiry);
        Task UpdateStatusAsync(int id, InquiryStatus status);
        Task<int> GetCountAsync();
    }
}
