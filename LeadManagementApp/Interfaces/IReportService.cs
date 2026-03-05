namespace LeadManagementSystem.Interfaces;

public interface IReportService
{
    Dictionary<string, int> GetLeadStatusDistribution();
    Dictionary<string, int> GetLeadsBySource();
    (int TotalLeads, int ConvertedLeads, decimal ConversionRatePercent) GetConversionSummary();
    Dictionary<string, (int AssignedLeads, int ConvertedLeads)> GetSalesRepPerformance();
    Dictionary<string, decimal> GetQuotationTotalsByStatus();
}
