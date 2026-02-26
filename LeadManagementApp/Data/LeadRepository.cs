using LeadManagementSystem.Models;
using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Data;

public class LeadRepository : ILeadRepository
{
    private readonly LeadDbContext _context;

    // Passed in via Dependency Injection
    public LeadRepository(LeadDbContext context)
    {
        _context = context;
    }

    public void AddLead(Lead lead)
    {
        _context.Leads.Add(lead);
        _context.SaveChanges(); // ACID compliant transaction
    }

    public Lead? GetLeadById(int id) => _context.Leads.Find(id);
    
    public List<Lead> GetAllLeads() => _context.Leads.ToList();
    
    public void UpdateLead(Lead lead)
    {
        _context.Leads.Update(lead);
        _context.SaveChanges();
    }
    
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