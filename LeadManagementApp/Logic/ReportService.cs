using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Logic;

public class ReportService : IReportService
{
    private readonly ILeadRepository _leadRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IQuotationRepository _quotationRepo;

    public ReportService(ILeadRepository leadRepo, ICustomerRepository customerRepo, IQuotationRepository quotationRepo)
    {
        _leadRepo = leadRepo;
        _customerRepo = customerRepo;
        _quotationRepo = quotationRepo;
    }

    public Dictionary<string, int> GetLeadStatusDistribution()
    {
        var leads = _leadRepo.GetAllLeads();
        return leads.GroupBy(l => l.Status)
                    .ToDictionary(g => g.Key, g => g.Count());
    }

    public Dictionary<string, int> GetLeadsBySource()
    {
        return _leadRepo.GetAllLeads()
            .GroupBy(l => l.Source)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public (int TotalLeads, int ConvertedLeads, decimal ConversionRatePercent) GetConversionSummary()
    {
        var totalLeads = _leadRepo.GetAllLeads().Count;
        var convertedLeads = _customerRepo.GetAllCustomers().Count;
        var conversionRate = totalLeads == 0 ? 0 : decimal.Round((decimal)convertedLeads / totalLeads * 100, 2);

        return (totalLeads, convertedLeads, conversionRate);
    }

    public Dictionary<string, (int AssignedLeads, int ConvertedLeads)> GetSalesRepPerformance()
    {
        var assignedByRep = _leadRepo.GetAllLeads()
            .Where(l => l.AssignedToRepId.HasValue)
            .GroupBy(l => l.AssignedToRepId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var convertedByRep = _customerRepo.GetAllCustomers()
            .Where(c => c.ConvertedByRepId.HasValue)
            .GroupBy(c => c.ConvertedByRepId!.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        return assignedByRep.Keys.Union(convertedByRep.Keys)
            .ToDictionary(
                repId => $"Rep {repId}",
                repId => (
                    AssignedLeads: assignedByRep.GetValueOrDefault(repId, 0),
                    ConvertedLeads: convertedByRep.GetValueOrDefault(repId, 0)
                ));
    }

    public Dictionary<string, decimal> GetQuotationTotalsByStatus()
    {
        return _quotationRepo.GetAllQuotations()
            .GroupBy(q => q.Status)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.TotalAmount));
    }
}
