using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Logic;

public class ReportService
{
    private readonly ILeadRepository _repo;

    // SOLID: Constructor Injection
    public ReportService(ILeadRepository repo) 
    {
        _repo = repo;
    }

    // SOLID: Return data, don't use Console.WriteLine here
    public Dictionary<string, int> GetLeadStatusDistribution()
    {
        var leads = _repo.GetAllLeads();
        return leads.GroupBy(l => l.Status)
                    .ToDictionary(g => g.Key, g => g.Count());
    }
}