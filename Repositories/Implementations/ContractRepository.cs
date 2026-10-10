using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Repositories.Implementations
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RentalContract>> GetAllAsync()
        {
            return await _context.RentalContracts
                .Include(c => c.Property)
                .OrderByDescending(c => c.GeneratedAt)
                .ToListAsync();
        }

        public async Task<RentalContract?> GetByIdAsync(int id)
        {
            return await _context.RentalContracts
                .Include(c => c.Property)
                    .ThenInclude(p => p!.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<RentalContract?> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.RentalContracts
                .Include(c => c.Property)
                .OrderByDescending(c => c.GeneratedAt)
                .FirstOrDefaultAsync(c => c.PropertyId == propertyId);
        }

        public async Task AddAsync(RentalContract contract)
        {
            await _context.RentalContracts.AddAsync(contract);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RentalContract contract)
        {
            _context.RentalContracts.Update(contract);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.RentalContracts.CountAsync();
        }
    }
}
