using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Services.Pdf
{
    public interface IContractPdfService
    {
        string GenerateContractHtml(RentalContract contract);
        byte[] GenerateContractPdfBytes(RentalContract contract);
    }
}
