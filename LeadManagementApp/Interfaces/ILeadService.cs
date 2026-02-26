// Interfaces/ILeadService.cs
namespace LeadManagementSystem.Interfaces;

public interface ILeadService
{
    string UpdateStatus(int leadId, string newStatus);
    string UpdatePriority(int leadId, string newPriority);
    string ConvertToCustomer(int leadId);
    string DeleteLead(int leadId); // New Delete method
}