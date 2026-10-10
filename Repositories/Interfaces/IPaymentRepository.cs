using System.Collections.Generic;
using System.Threading.Tasks;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<IEnumerable<TokenPayment>> GetAllAsync();
        Task<TokenPayment?> GetByIdAsync(int id);
        Task<IEnumerable<TokenPayment>> GetByPropertyIdAsync(int propertyId);
        Task AddAsync(TokenPayment payment);
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetCountAsync();
    }
}
