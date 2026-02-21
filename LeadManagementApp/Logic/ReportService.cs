using LeadManagementSystem.Data;

namespace LeadManagementSystem.Logic;

public class ReportService
{
    private readonly LeadRepository _repo = new();

    // Generate Lead Analytics 
    public void ShowLeadStatusDistribution()
    {
        var leads = _repo.GetAllLeads();
        var stats = leads.GroupBy(l => l.Status)
                         .Select(g => new { Status = g.Key, Count = g.Count() });

        Console.WriteLine("\n--- Lead Status Distribution ---");
        foreach (var item in stats)
        {
            Console.WriteLine($"{item.Status}: {item.Count}");
        }
    }
}