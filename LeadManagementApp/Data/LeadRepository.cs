using LeadManagementSystem.Models;
using LeadManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementSystem.Data;

public class LeadRepository : ILeadRepository
{
    private readonly LeadDbContext _context;

    public LeadRepository()
    {
        _context = new LeadDbContext();
    }

    // CREATE
    public void AddLead(Lead lead)
    {
        _context.Leads.Add(lead);
        _context.SaveChanges();
    }

    // READ BY ID
    public Lead? GetLeadById(int id)
    {
        return _context.Leads.FirstOrDefault(l => l.LeadId == id);
    }

    // READ ALL
    public List<Lead> GetAllLeads()
    {
        return _context.Leads.ToList();
    }

    // UPDATE
    public void UpdateLead(Lead lead)
    {
        var existingLead = _context.Leads.Find(lead.LeadId);

        if (existingLead != null)
        {
            existingLead.Name = lead.Name;
            existingLead.Email = lead.Email;
            existingLead.Phone = lead.Phone;
            existingLead.Company = lead.Company;
            existingLead.Status = lead.Status;
            existingLead.Source = lead.Source;
            existingLead.Priority = lead.Priority;

            _context.SaveChanges();
        }
    }

    // DELETE
    public void DeleteLead(int id)
    {
        var lead = _context.Leads.Find(id);

        if (lead != null)
        {
            _context.Leads.Remove(lead);
            _context.SaveChanges();
        }
    }
}