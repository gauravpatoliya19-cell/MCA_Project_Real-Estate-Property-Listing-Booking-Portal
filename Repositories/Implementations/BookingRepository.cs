using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Repositories.Implementations
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SiteVisitBooking>> GetAllAsync()
        {
            return await _context.SiteVisitBookings
                .Include(b => b.Property)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<SiteVisitBooking?> GetByIdAsync(int id)
        {
            return await _context.SiteVisitBookings
                .Include(b => b.Property)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<SiteVisitBooking>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.SiteVisitBookings
                .Where(b => b.PropertyId == propertyId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(SiteVisitBooking booking)
        {
            await _context.SiteVisitBookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<SiteVisitBooking> bookings)
        {
            await _context.SiteVisitBookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, BookingStatus status)
        {
            var booking = await _context.SiteVisitBookings.FindAsync(id);
            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.SiteVisitBookings.CountAsync();
        }
    }
}
