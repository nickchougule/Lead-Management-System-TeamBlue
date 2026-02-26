// Interfaces/IReportService.cs
namespace LeadManagementSystem.Interfaces;

public interface IReportService
{
    Dictionary<string, int> GetLeadStatusDistribution();
}