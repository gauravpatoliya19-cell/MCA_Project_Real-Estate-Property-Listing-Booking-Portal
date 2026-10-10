using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Data;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TokenPayment>> GetAllAsync()
        {
            return await _context.TokenPayments
                .Include(p => p.Property)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<TokenPayment?> GetByIdAsync(int id)
        {
            return await _context.TokenPayments
                .Include(p => p.Property)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<TokenPayment>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.TokenPayments
                .Where(p => p.PropertyId == propertyId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task AddAsync(TokenPayment payment)
        {
            await _context.TokenPayments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.TokenPayments
                .Where(p => p.Status == PaymentStatus.Success)
                .SumAsync(p => p.Amount);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.TokenPayments.CountAsync();
        }
    }
}
