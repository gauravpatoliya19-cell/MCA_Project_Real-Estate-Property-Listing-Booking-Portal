using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<IEnumerable<SiteVisitBooking>> GetAllAsync();
        Task<SiteVisitBooking?> GetByIdAsync(int id);
        Task<IEnumerable<SiteVisitBooking>> GetByPropertyIdAsync(int propertyId);
        Task AddAsync(SiteVisitBooking booking);
        Task AddRangeAsync(IEnumerable<SiteVisitBooking> bookings);
        Task UpdateStatusAsync(int id, BookingStatus status);
        Task<int> GetCountAsync();
    }
}
