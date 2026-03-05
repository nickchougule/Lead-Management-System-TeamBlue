using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface ILeadService
{
    Lead CreateLead(Lead lead);
    Lead? GetLeadById(int leadId);
    List<Lead> GetAllLeads();
    string UpdateStatus(int leadId, string newStatus);
    string UpdatePriority(int leadId, string newPriority);
    string ConvertToCustomer(int leadId);
    string DeleteLead(int leadId);
}
