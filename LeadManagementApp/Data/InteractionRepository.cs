using LeadManagementSystem.Models;
using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Data;

public class InteractionRepository : IInteractionRepository
{
    private readonly LeadDbContext _context;

    public InteractionRepository(LeadDbContext context)
    {
        _context = context;
    }

    public void AddInteraction(Interaction interaction)
    {
        _context.Interactions.Add(interaction);
        _context.SaveChanges();
    }

    public List<Interaction> GetInteractionsByLead(int leadId)
    {
        return _context.Interactions.Where(i => i.LeadId == leadId).ToList();
    }
}