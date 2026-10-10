using System;
using System.Collections.Generic;

namespace RealEstatePortal.Web.Areas.Admin.ViewModels
{
    public class MonthlyCommissionRecordVM
    {
        public string MonthName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int DealsClosed { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal CommissionEarned { get; set; }
    }

    public class AgentCommissionSummaryVM
    {
        public string AgentName { get; set; } = string.Empty;
        public string AgentEmail { get; set; } = string.Empty;
        public int ActiveListings { get; set; }
        public int ClosedDeals { get; set; }
        public decimal TotalCommissionEarned { get; set; }
        public decimal AverageCommissionRate { get; set; }
    }

    public class CommissionTransactionVM
    {
        public string TransactionId { get; set; } = string.Empty;
        public string PropertyTitle { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public decimal DealAmount { get; set; }
        public decimal CommissionRate { get; set; }
        public decimal CommissionPayout { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Type { get; set; } = "Sale";
    }

    public class CommissionSummaryVM
    {
        public int SelectedYear { get; set; } = DateTime.UtcNow.Year;
        public string? SelectedAgent { get; set; }

        public decimal TotalCommissionYearToDate { get; set; }
        public decimal TotalDealVolume { get; set; }
        public int TotalDealsClosed { get; set; }
        public decimal AverageCommissionPerDeal => TotalDealsClosed > 0 ? TotalCommissionYearToDate / TotalDealsClosed : 0;

        public List<MonthlyCommissionRecordVM> MonthlyBreakdown { get; set; } = new List<MonthlyCommissionRecordVM>();
        public List<AgentCommissionSummaryVM> AgentSummaries { get; set; } = new List<AgentCommissionSummaryVM>();
        public List<CommissionTransactionVM> RecentCommissionTransactions { get; set; } = new List<CommissionTransactionVM>();
        public List<string> AgentNames { get; set; } = new List<string>();
    }
}
