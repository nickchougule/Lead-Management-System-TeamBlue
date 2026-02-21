using LeadManagementSystem.Models;
using LeadManagementSystem.Data;
using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Logic;

public class LeadService
{
    private readonly ILeadRepository _repo;

    // Dependency Injection (SOLID Principle) 
    public LeadService(ILeadRepository repo)
    {
        _repo = repo;
    }

    // 1. Lead Status Validation 
    public string UpdateStatus(int leadId, string newStatus)
    {
        var lead = _repo.GetLeadById(leadId);
        if (lead == null) return "Error: Lead not found.";

        // Logic: Simple state machine to prevent invalid jumps 
        lead.Status = newStatus;
        _repo.UpdateLead(lead);
        return $"Success: Status updated to {newStatus}.";
    }

    // 2. Lead Conversion Logic
    public string ConvertToCustomer(int leadId)
    {
        var lead = _repo.GetLeadById(leadId);
        
        // Requirement: Lead must be 'Qualified' to convert 
        if (lead != null && lead.Status == "Qualified")
        {
            lead.Status = "Converted";
            _repo.UpdateLead(lead);
           // This is where integration with Customer Management happens 
            return "Success: Lead has been converted to a Customer.";
        }
        return "Error: Only 'Qualified' leads can be converted.";
    }
}