using LeadManagementSystem.Data;
using LeadManagementSystem.Interfaces;
using LeadManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementSystem.Logic;

public class LeadService : ILeadService
{
    private readonly ILeadRepository _repo;
    private readonly ICustomerRepository _customerRepo;
    private readonly LeadDbContext _context;

    public LeadService(ILeadRepository repo, ICustomerRepository customerRepo, LeadDbContext context)
    {
        _repo = repo;
        _customerRepo = customerRepo;
        _context = context;
    }

    public Lead CreateLead(Lead lead)
    {
        _repo.AddLead(lead);
        return lead;
    }

    public Lead? GetLeadById(int leadId)
    {
        return _repo.GetLeadById(leadId);
    }

    public List<Lead> GetAllLeads()
    {
        return _repo.GetAllLeads();
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
        if (lead == null)
        {
            return "Error: Lead not found.";
        }

        if (!string.Equals(lead.Status.Trim(), "Qualified", StringComparison.OrdinalIgnoreCase))
        {
            return $"Error: Only 'Qualified' leads can be converted. Current status is: '{lead.Status}'";
        }

        if (_customerRepo.GetCustomerByLeadId(leadId) != null)
        {
            return "Error: This lead is already converted to a customer.";
        }

        using var transaction = _context.Database.IsRelational() ? _context.Database.BeginTransaction() : null;
        try
        {
            lead.Status = "Converted";
            _repo.UpdateLead(lead);

            var customer = new Customer
            {
                LeadId = lead.LeadId,
                Name = lead.Name,
                Email = lead.Email,
                Company = lead.Company,
                ConvertedOn = DateTime.UtcNow,
                ConvertedByRepId = lead.AssignedToRepId
            };

            _customerRepo.AddCustomer(customer);
            transaction?.Commit();
            return "Success: Lead has been converted to a Customer.";
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            return $"Error: Conversion failed. {ex.Message}";
        }
    }

    public string DeleteLead(int leadId)
    {
        var lead = _repo.GetLeadById(leadId);
        if (lead == null) return "Error: Lead not found.";

        _repo.DeleteLead(leadId);
        return $"Success: Lead ID {leadId} and all related data have been deleted.";
    }
}
