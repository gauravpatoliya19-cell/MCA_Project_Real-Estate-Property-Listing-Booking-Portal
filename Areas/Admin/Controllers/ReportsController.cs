using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePortal.Web.Areas.Admin.ViewModels;
using RealEstatePortal.Web.Models;
using RealEstatePortal.Web.Repositories.Interfaces;

namespace RealEstatePortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Agent")]
    public class ReportsController : Controller
    {
        private readonly IPropertyRepository _propertyRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IContractRepository _contractRepo;
        private readonly IInquiryRepository _inquiryRepo;

        public ReportsController(
            IPropertyRepository propertyRepo,
            IBookingRepository bookingRepo,
            IPaymentRepository paymentRepo,
            IContractRepository contractRepo,
            IInquiryRepository inquiryRepo)
        {
            _propertyRepo = propertyRepo;
            _bookingRepo = bookingRepo;
            _paymentRepo = paymentRepo;
            _contractRepo = contractRepo;
            _inquiryRepo = inquiryRepo;
        }

        // 1. PROPERTY STATUS LEDGER REPORT
        public async Task<IActionResult> Ledger(PropertyStatus? status, ListingType? listingType, string? city, string? search)
        {
            var properties = (await _propertyRepo.GetAllAsync()).ToList();
            var payments = (await _paymentRepo.GetAllAsync()).ToList();
            var bookings = (await _bookingRepo.GetAllAsync()).ToList();
            var inquiries = (await _inquiryRepo.GetAllAsync()).ToList();

            var query = properties.AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (listingType.HasValue)
            {
                query = query.Where(p => p.ListingType == listingType.Value);
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.Equals(city, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(s) || p.AgentName.ToLower().Contains(s));
            }

            var filteredList = query.ToList();

            var ledgerItems = filteredList.Select(p => new PropertyStatusLedgerItemVM
            {
                PropertyId = p.Id,
                Title = p.Title,
                City = p.City,
                PropertyType = p.PropertyType,
                ListingType = p.ListingType,
                Status = p.Status,
                Price = p.Price,
                TokenAmount = p.TokenAmount,
                AgentName = p.AgentName,
                TotalSiteVisits = bookings.Count(b => b.PropertyId == p.Id),
                TotalTokensCollected = payments.Where(pay => pay.PropertyId == p.Id && pay.Status == PaymentStatus.Success).Sum(pay => pay.Amount),
                InquiriesCount = inquiries.Count(i => i.PropertyId == p.Id),
                CreatedAt = p.CreatedAt
            }).ToList();

            var vm = new PropertyStatusLedgerVM
            {
                FilterStatus = status,
                FilterListingType = listingType,
                FilterCity = city,
                SearchTerm = search,
                TotalProperties = properties.Count,
                TotalAvailable = properties.Count(p => p.Status == PropertyStatus.Available),
                TotalBooked = properties.Count(p => p.Status == PropertyStatus.Booked),
                TotalRented = properties.Count(p => p.Status == PropertyStatus.Rented),
                TotalSold = properties.Count(p => p.Status == PropertyStatus.Sold),
                TotalPortfolioValue = properties.Where(p => p.ListingType == ListingType.Sale).Sum(p => p.Price),
                TotalMonthlyRentalYield = properties.Where(p => p.ListingType == ListingType.Rent).Sum(p => p.Price),
                TotalTokensEscrowed = payments.Where(p => p.Status == PaymentStatus.Success).Sum(p => p.Amount),
                LedgerItems = ledgerItems,
                AvailableCities = properties.Select(p => p.City).Distinct().OrderBy(c => c).ToList()
            };

            return View(vm);
        }

        // 2. MONTHLY COMMISSION SUMMARY REPORT
        public async Task<IActionResult> Commissions(int? year, string? agent)
        {
            var selectedYear = year ?? DateTime.UtcNow.Year;
            var properties = (await _propertyRepo.GetAllAsync()).ToList();
            var payments = (await _paymentRepo.GetAllAsync()).ToList();
            var contracts = (await _contractRepo.GetAllAsync()).ToList();

            var allAgents = properties.Select(p => p.AgentName).Distinct().OrderBy(a => a).ToList();

            var transactions = new List<CommissionTransactionVM>();

            foreach (var prop in properties.Where(p => p.Status == PropertyStatus.Sold || p.Status == PropertyStatus.Rented || p.Status == PropertyStatus.Booked))
            {
                if (!string.IsNullOrWhiteSpace(agent) && !prop.AgentName.Equals(agent, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                decimal dealAmount = prop.Price;
                decimal payout = 0;
                string dealType = prop.ListingType == ListingType.Sale ? "Sale" : "Rental";

                if (prop.Status == PropertyStatus.Sold)
                {
                    payout = prop.Price * (prop.CommissionRate / 100m);
                }
                else if (prop.Status == PropertyStatus.Rented)
                {
                    payout = prop.Price * (prop.CommissionRate > 0 ? (prop.CommissionRate / 100m * 12) : 1.0m);
                    if (payout == 0) payout = prop.Price;
                }
                else if (prop.Status == PropertyStatus.Booked)
                {
                    payout = prop.TokenAmount * 0.5m;
                    dealType = "Reservation / Advance";
                }

                transactions.Add(new CommissionTransactionVM
                {
                    TransactionId = "COM-" + prop.Id + "-" + prop.CreatedAt.ToString("MMdd"),
                    PropertyTitle = prop.Title,
                    AgentName = prop.AgentName,
                    DealAmount = dealAmount,
                    CommissionRate = prop.CommissionRate,
                    CommissionPayout = payout,
                    TransactionDate = prop.CreatedAt,
                    Type = dealType
                });
            }

            var monthlyBreakdown = new List<MonthlyCommissionRecordVM>();
            var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int m = 1; m <= 12; m++)
            {
                var monthTxns = transactions.Where(t => t.TransactionDate.Month == m && t.TransactionDate.Year == selectedYear).ToList();
                monthlyBreakdown.Add(new MonthlyCommissionRecordVM
                {
                    MonthName = monthNames[m - 1],
                    Year = selectedYear,
                    DealsClosed = monthTxns.Count,
                    TotalVolume = monthTxns.Sum(t => t.DealAmount),
                    CommissionEarned = monthTxns.Sum(t => t.CommissionPayout)
                });
            }

            var agentSummaries = allAgents.Select(agentName =>
            {
                var agentProps = properties.Where(p => p.AgentName == agentName).ToList();
                var agentTxns = transactions.Where(t => t.AgentName == agentName).ToList();
                return new AgentCommissionSummaryVM
                {
                    AgentName = agentName,
                    AgentEmail = agentProps.FirstOrDefault()?.AgentEmail ?? "",
                    ActiveListings = agentProps.Count(p => p.Status == PropertyStatus.Available),
                    ClosedDeals = agentTxns.Count,
                    TotalCommissionEarned = agentTxns.Sum(t => t.CommissionPayout),
                    AverageCommissionRate = agentProps.Count > 0 ? agentProps.Average(p => p.CommissionRate) : 3.0m
                };
            }).OrderByDescending(a => a.TotalCommissionEarned).ToList();

            var vm = new CommissionSummaryVM
            {
                SelectedYear = selectedYear,
                SelectedAgent = agent,
                TotalCommissionYearToDate = transactions.Sum(t => t.CommissionPayout),
                TotalDealVolume = transactions.Sum(t => t.DealAmount),
                TotalDealsClosed = transactions.Count,
                MonthlyBreakdown = monthlyBreakdown,
                AgentSummaries = agentSummaries,
                RecentCommissionTransactions = transactions.OrderByDescending(t => t.TransactionDate).ToList(),
                AgentNames = allAgents
            };

            return View(vm);
        }

        // 3. INQUIRY CONVERSION RATE REPORT
        public async Task<IActionResult> Conversions()
        {
            var inquiries = (await _inquiryRepo.GetAllAsync()).ToList();
            var bookings = (await _bookingRepo.GetAllAsync()).ToList();
            var payments = (await _paymentRepo.GetAllAsync()).ToList();
            var contracts = (await _contractRepo.GetAllAsync()).ToList();
            var properties = (await _propertyRepo.GetAllAsync()).ToList();

            int totalInquiries = Math.Max(inquiries.Count, 1);
            int totalVisits = bookings.Count;
            int totalPayments = payments.Count;
            int totalContracts = contracts.Count;

            var funnelSteps = new List<ConversionFunnelStepVM>
            {
                new ConversionFunnelStepVM
                {
                    StepName = "1. Customer Inquiries Received",
                    Count = totalInquiries,
                    PercentageOfTotal = 100.0,
                    StepConversionRate = 100.0,
                    ColorClass = "primary"
                },
                new ConversionFunnelStepVM
                {
                    StepName = "2. Scheduled Site Visits",
                    Count = totalVisits,
                    PercentageOfTotal = Math.Round(((double)totalVisits / totalInquiries) * 100, 1),
                    StepConversionRate = Math.Round(((double)totalVisits / totalInquiries) * 100, 1),
                    ColorClass = "info"
                },
                new ConversionFunnelStepVM
                {
                    StepName = "3. Token Reservation Payments",
                    Count = totalPayments,
                    PercentageOfTotal = Math.Round(((double)totalPayments / totalInquiries) * 100, 1),
                    StepConversionRate = totalVisits > 0 ? Math.Round(((double)totalPayments / totalVisits) * 100, 1) : 0,
                    ColorClass = "warning"
                },
                new ConversionFunnelStepVM
                {
                    StepName = "4. Executed Contracts (Closed Deals)",
                    Count = totalContracts,
                    PercentageOfTotal = Math.Round(((double)totalContracts / totalInquiries) * 100, 1),
                    StepConversionRate = totalPayments > 0 ? Math.Round(((double)totalContracts / totalPayments) * 100, 1) : 0,
                    ColorClass = "success"
                }
            };

            var monthlyTrends = new List<MonthlyConversionTrendVM>();
            var now = DateTime.UtcNow;
            for (int i = 5; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var monthInq = inquiries.Count(x => x.InquiryDate.Month == monthDate.Month && x.InquiryDate.Year == monthDate.Year);
                var monthVis = bookings.Count(x => x.CreatedAt.Month == monthDate.Month && x.CreatedAt.Year == monthDate.Year);
                var monthPay = payments.Count(x => x.PaymentDate.Month == monthDate.Month && x.PaymentDate.Year == monthDate.Year);
                var monthCon = contracts.Count(x => x.GeneratedAt.Month == monthDate.Month && x.GeneratedAt.Year == monthDate.Year);

                double rate = monthInq > 0 ? ((double)monthCon / monthInq) * 100 : (monthPay > 0 ? 50 : 0);

                monthlyTrends.Add(new MonthlyConversionTrendVM
                {
                    MonthLabel = monthDate.ToString("MMM yyyy"),
                    Inquiries = monthInq,
                    SiteVisits = monthVis,
                    TokenPayments = monthPay,
                    Contracts = monthCon,
                    OverallRate = Math.Round(rate, 1)
                });
            }

            var propertyPerformances = properties.Select(p =>
            {
                int inqCount = inquiries.Count(i => i.PropertyId == p.Id);
                int visCount = bookings.Count(b => b.PropertyId == p.Id);
                int payCount = payments.Count(t => t.PropertyId == p.Id);
                int conCount = contracts.Count(c => c.PropertyId == p.Id);
                double conv = inqCount > 0 ? ((double)conCount / inqCount) * 100 : (payCount > 0 ? 100 : 0);

                return new PropertyConversionDetailVM
                {
                    PropertyId = p.Id,
                    Title = p.Title,
                    City = p.City,
                    Status = p.Status.ToString(),
                    Inquiries = inqCount,
                    Visits = visCount,
                    Payments = payCount,
                    Contracts = conCount,
                    ConversionRate = Math.Round(conv, 1)
                };
            }).OrderByDescending(p => p.ConversionRate).ToList();

            var vm = new ConversionRateVM
            {
                TotalInquiries = totalInquiries,
                TotalSiteVisits = totalVisits,
                TotalTokenPayments = totalPayments,
                TotalContractsGenerated = totalContracts,
                FunnelSteps = funnelSteps,
                MonthlyTrends = monthlyTrends,
                PropertyPerformances = propertyPerformances
            };

            return View(vm);
        }
    }
}
