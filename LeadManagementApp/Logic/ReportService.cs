// Logic/ReportService.cs
using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Logic;

public class ReportService : IReportService // Implementing ISP
{
    private readonly ILeadRepository _repo;

    public ReportService(ILeadRepository repo) 
    {
        _repo = repo;
    }

    public Dictionary<string, int> GetLeadStatusDistribution()
    {
        var leads = _repo.GetAllLeads();
        return leads.GroupBy(l => l.Status)
                    .ToDictionary(g => g.Key, g => g.Count());
    }
}