using System;
using System.Collections.Generic;

namespace RealEstatePortal.Web.Areas.Admin.ViewModels
{
    public class ConversionFunnelStepVM
    {
        public string StepName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double PercentageOfTotal { get; set; }
        public double StepConversionRate { get; set; }
        public string ColorClass { get; set; } = "primary";
    }

    public class MonthlyConversionTrendVM
    {
        public string MonthLabel { get; set; } = string.Empty;
        public int Inquiries { get; set; }
        public int SiteVisits { get; set; }
        public int TokenPayments { get; set; }
        public int Contracts { get; set; }
        public double OverallRate { get; set; }
    }

    public class PropertyConversionDetailVM
    {
        public int PropertyId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Inquiries { get; set; }
        public int Visits { get; set; }
        public int Payments { get; set; }
        public int Contracts { get; set; }
        public double ConversionRate { get; set; }
    }

    public class ConversionRateVM
    {
        public int TotalInquiries { get; set; }
        public int TotalSiteVisits { get; set; }
        public int TotalTokenPayments { get; set; }
        public int TotalContractsGenerated { get; set; }

        public double InquiryToVisitRate => TotalInquiries > 0 ? ((double)TotalSiteVisits / TotalInquiries) * 100 : 0;
        public double VisitToPaymentRate => TotalSiteVisits > 0 ? ((double)TotalTokenPayments / TotalSiteVisits) * 100 : 0;
        public double PaymentToContractRate => TotalTokenPayments > 0 ? ((double)TotalContractsGenerated / TotalTokenPayments) * 100 : 0;
        public double OverallConversionRate => TotalInquiries > 0 ? ((double)TotalContractsGenerated / TotalInquiries) * 100 : 0;

        public List<ConversionFunnelStepVM> FunnelSteps { get; set; } = new List<ConversionFunnelStepVM>();
        public List<MonthlyConversionTrendVM> MonthlyTrends { get; set; } = new List<MonthlyConversionTrendVM>();
        public List<PropertyConversionDetailVM> PropertyPerformances { get; set; } = new List<PropertyConversionDetailVM>();
    }
}
