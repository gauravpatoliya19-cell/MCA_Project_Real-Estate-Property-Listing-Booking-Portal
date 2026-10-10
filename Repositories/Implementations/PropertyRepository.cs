using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Repositories.Implementations
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _context;

        public PropertyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.Features)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetFeaturedAsync()
        {
            return await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.Features)
                .Where(p => p.IsFeatured)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.Features)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Property?> GetDetailsByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Images)
                .Include(p => p.Features)
                .Include(p => p.SiteVisits)
                .Include(p => p.TokenPayments)
                .Include(p => p.RentalContracts)
                .Include(p => p.Inquiries)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Property property)
        {
            _context.Properties.Update(property);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var prop = await _context.Properties.FindAsync(id);
            if (prop != null)
            {
                _context.Properties.Remove(prop);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStatusAsync(int id, PropertyStatus status)
        {
            var prop = await _context.Properties.FindAsync(id);
            if (prop != null)
            {
                prop.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<string>> GetCitiesAsync()
        {
            return await _context.Properties
                .Select(p => p.City)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<int> GetCountByStatusAsync(PropertyStatus status)
        {
            return await _context.Properties.CountAsync(p => p.Status == status);
        }
    }
}
