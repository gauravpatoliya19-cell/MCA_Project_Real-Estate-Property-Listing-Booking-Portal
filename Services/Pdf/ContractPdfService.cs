using System;
using System.Text;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Services.Pdf
{
    public class ContractPdfService : IContractPdfService
    {
        public string GenerateContractHtml(RentalContract contract)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='en'>");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset='utf-8' />");
            sb.AppendLine($"  <title>Rental Lease Agreement - {contract.ContractNumber}</title>");
            sb.AppendLine("  <style>");
            sb.AppendLine("    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; color: #1e293b; line-height: 1.6; }");
            sb.AppendLine("    .header { text-align: center; border-bottom: 3px solid #0f172a; padding-bottom: 20px; margin-bottom: 30px; }");
            sb.AppendLine("    .header h1 { margin: 0; color: #0f172a; font-size: 26px; text-transform: uppercase; letter-spacing: 1px; }");
            sb.AppendLine("    .header p { margin: 5px 0 0; color: #64748b; font-size: 14px; }");
            sb.AppendLine("    .contract-badge { display: inline-block; background: #e0f2fe; color: #0369a1; padding: 4px 12px; border-radius: 999px; font-weight: 600; font-size: 13px; margin-top: 10px; }");
            sb.AppendLine("    .section-title { font-size: 16px; font-weight: 700; color: #0f172a; border-bottom: 1px solid #e2e8f0; padding-bottom: 6px; margin-top: 25px; margin-bottom: 12px; }");
            sb.AppendLine("    .grid { display: flex; justify-content: space-between; margin-bottom: 15px; }");
            sb.AppendLine("    .col { width: 48%; }");
            sb.AppendLine("    .info-card { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 15px; }");
            sb.AppendLine("    .info-row { display: flex; justify-content: space-between; margin-bottom: 8px; font-size: 14px; }");
            sb.AppendLine("    .info-label { color: #64748b; font-weight: 500; }");
            sb.AppendLine("    .info-val { font-weight: 600; color: #0f172a; }");
            sb.AppendLine("    .terms-box { background: #ffffff; border: 1px solid #cbd5e1; border-radius: 6px; padding: 15px; font-size: 13px; color: #334155; line-height: 1.7; white-space: pre-line; }");
            sb.AppendLine("    .sign-section { margin-top: 40px; display: flex; justify-content: space-between; }");
            sb.AppendLine("    .sign-block { width: 45%; border-top: 2px solid #0f172a; padding-top: 10px; }");
            sb.AppendLine("    .sig-hash { font-family: monospace; font-size: 11px; color: #0284c7; word-break: break-all; margin-top: 5px; }");
            sb.AppendLine("    .footer { text-align: center; margin-top: 40px; font-size: 12px; color: #94a3b8; border-top: 1px solid #f1f5f9; padding-top: 15px; }");
            sb.AppendLine("    @media print { body { margin: 15mm; } .no-print { display: none; } }");
            sb.AppendLine("  </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("  <div class='header'>");
            sb.AppendLine("    <h1>Residential Lease Agreement</h1>");
            sb.AppendLine("   <p>Real Estate Property &bull; Official Digital Contract</p>");
            sb.AppendLine($"   <div class='contract-badge'>Contract No: {contract.ContractNumber}</div>");
            sb.AppendLine("  </div>");

            sb.AppendLine("  <div class='grid'>");
            sb.AppendLine("    <div class='col info-card'>");
            sb.AppendLine("      <div class='section-title' style='margin-top:0'>Landlord / Lessor</div>");
            sb.AppendLine($"     <div class='info-row'><span class='info-label'>Company / Name:</span><span class='info-val'>{contract.LandlordName}</span></div>");
            sb.AppendLine($"     <div class='info-row'><span class='info-label'>Email:</span><span class='info-val'>{contract.LandlordEmail}</span></div>");
            sb.AppendLine("      <div class='info-row'><span class='info-label'>Authorized Status:</span><span class='info-val'>Managing Broker</span></div>");
            sb.AppendLine("    </div>");

            sb.AppendLine("    <div class='col info-card'>");
            sb.AppendLine("      <div class='section-title' style='margin-top:0'>Tenant / Lessee</div>");
            sb.AppendLine($"     <div class='info-row'><span class='info-label'>Full Name:</span><span class='info-val'>{contract.TenantName}</span></div>");
            sb.AppendLine($"     <div class='info-row'><span class='info-label'>Email:</span><span class='info-val'>{contract.TenantEmail}</span></div>");
            sb.AppendLine($"     <div class='info-row'><span class='info-label'>Phone:</span><span class='info-val'>{contract.TenantPhone}</span></div>");
            sb.AppendLine($"     <div class='info-row'><span class='info-label'>ID Proof:</span><span class='info-val'>{contract.TenantIdProof}</span></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");

            if (contract.Property != null)
            {
                sb.AppendLine("  <div class='section-title'>Leased Property Details</div>");
                sb.AppendLine("  <div class='info-card'>");
                sb.AppendLine($"   <div class='info-row'><span class='info-label'>Property:</span><span class='info-val'>{contract.Property.Title}</span></div>");
                sb.AppendLine($"   <div class='info-row'><span class='info-label'>Address:</span><span class='info-val'>{contract.Property.Address}, {contract.Property.City}, {contract.Property.State} {contract.Property.ZipCode}</span></div>");
                sb.AppendLine($"   <div class='info-row'><span class='info-label'>Type / Specs:</span><span class='info-val'>{contract.Property.PropertyType} &bull; {contract.Property.Bedrooms} Beds &bull; {contract.Property.Bathrooms} Baths &bull; {contract.Property.AreaSqFt} Sq Ft</span></div>");
                sb.AppendLine("  </div>");
            }

            sb.AppendLine("  <div class='section-title'>Financial Terms & Schedule</div>");
            sb.AppendLine("  <div class='info-card'>");
            sb.AppendLine($"   <div class='info-row'><span class='info-label'>Lease Term:</span><span class='info-val'>{contract.StartDate:MMM dd, yyyy} to {contract.EndDate:MMM dd, yyyy}</span></div>");
            sb.AppendLine($"   <div class='info-row'><span class='info-label'>Payment Frequency:</span><span class='info-val'>{contract.PaymentFrequency}</span></div>");
            sb.AppendLine($"   <div class='info-row'><span class='info-label'>Monthly Rent:</span><span class='info-val'>₹{contract.MonthlyRent:N2}</span></div>");
            sb.AppendLine($"   <div class='info-row'><span class='info-label'>Security Deposit:</span><span class='info-val'>₹{contract.SecurityDeposit:N2}</span></div>");
            sb.AppendLine($"   <div class='info-row'><span class='info-label'>Token Reservation Credited:</span><span class='info-val' style='color:#16a34a;'>-₹{contract.TokenPaid:N2}</span></div>");
            sb.AppendLine($"   <div class='info-row' style='font-size:16px; border-top:1px dashed #cbd5e1; padding-top:8px; margin-top:8px;'><span class='info-label' style='font-weight:700;'>Net Balance Due at Key Handover:</span><span class='info-val' style='color:#0f172a; font-weight:800;'>₹{contract.BalanceDue:N2}</span></div>");
            sb.AppendLine("  </div>");

            sb.AppendLine("  <div class='section-title'>Terms, Covenants & Conditions</div>");
            sb.AppendLine($" <div class='terms-box'>{contract.TermsAndConditions}</div>");

            sb.AppendLine("  <div class='sign-section'>");
            sb.AppendLine("    <div class='sign-block'>");
            sb.AppendLine($"     <div><strong>Landlord Representative:</strong> {contract.LandlordName}</div>");
            sb.AppendLine($"     <div style='font-size:12px; color:#64748b;'>Date: {contract.GeneratedAt:yyyy-MM-dd}</div>");
            sb.AppendLine("      <div style='font-family:cursive; font-size:18px; margin-top:5px; color:#0f172a;'>Real Estate Property Management</div>");
            sb.AppendLine("    </div>");

            sb.AppendLine("    <div class='sign-block'>");
            sb.AppendLine($"     <div><strong>Tenant:</strong> {contract.TenantName}</div>");
            sb.AppendLine($"     <div style='font-size:12px; color:#64748b;'>Date: {contract.GeneratedAt:yyyy-MM-dd}</div>");
            sb.AppendLine($"     <div class='sig-hash'>Digital Signature Hash:<br/>{contract.DigitalSignature}</div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");

            sb.AppendLine("  <div class='footer'>");
            sb.AppendLine($"   This document is an electronically signed legal binding agreement generated on {contract.GeneratedAt:MMMM dd, yyyy hh:mm tt UTC}. Verified by RealEstatePortal Cryptographic Verification Engine.");
            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public byte[] GenerateContractPdfBytes(RentalContract contract)
        {
            var html = GenerateContractHtml(contract);
            return Encoding.UTF8.GetBytes(html);
        }
    }
}
