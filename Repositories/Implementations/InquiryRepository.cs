using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Repositories.Implementations
{
    public class InquiryRepository : IInquiryRepository
    {
        private readonly ApplicationDbContext _context;

        public InquiryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inquiry>> GetAllAsync()
        {
            return await _context.Inquiries
                .Include(i => i.Property)
                .OrderByDescending(i => i.InquiryDate)
                .ToListAsync();
        }

        public async Task<Inquiry?> GetByIdAsync(int id)
        {
            return await _context.Inquiries
                .Include(i => i.Property)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Inquiry>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.Inquiries
                .Where(i => i.PropertyId == propertyId)
                .OrderByDescending(i => i.InquiryDate)
                .ToListAsync();
        }

        public async Task AddAsync(Inquiry inquiry)
        {
            await _context.Inquiries.AddAsync(inquiry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, InquiryStatus status)
        {
            var inquiry = await _context.Inquiries.FindAsync(id);
            if (inquiry != null)
            {
                inquiry.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Inquiries.CountAsync();
        }
    }
}
