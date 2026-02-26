using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Logic;

public class LeadService
{
    private readonly ILeadRepository _repo;

    public LeadService(ILeadRepository repo)
    {
        _repo = repo;
    }

    public string UpdateStatus(int leadId, string newStatus)
    {
        var lead = _repo.GetLeadById(leadId);
        if (lead == null) return "Error: Lead not found.";

        lead.Status = newStatus;
        _repo.UpdateLead(lead);
        return $"Success: Status updated to {newStatus}.";
    }

    public string UpdatePriority(int leadId, string newPriority)
    {
        var lead = _repo.GetLeadById(leadId);
        if (lead == null) return "Error: Lead not found.";

        lead.Priority = newPriority;
        _repo.UpdateLead(lead);
        return $"Success: Priority updated to {newPriority}.";
    }

    public string ConvertToCustomer(int leadId)
    {
        var lead = _repo.GetLeadById(leadId);
        if (lead != null && lead.Status == "Qualified")
        {
            lead.Status = "Converted";
            _repo.UpdateLead(lead);
            return "Success: Lead has been converted to a Customer.";
        }
        return "Error: Only 'Qualified' leads can be converted.";
    }
}