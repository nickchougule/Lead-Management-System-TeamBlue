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

    public Interaction? GetInteractionById(int id)
    {
        return _context.Interactions.Find(id);
    }

    public List<Interaction> GetAllInteractions()
    {
        return _context.Interactions.ToList();
    }

    public List<Interaction> GetInteractionsByLead(int leadId)
    {
        return _context.Interactions.Where(i => i.LeadId == leadId).ToList();
    }

    public void UpdateInteraction(Interaction interaction)
    {
        _context.Interactions.Update(interaction);
        _context.SaveChanges();
    }

    public void DeleteInteraction(int id)
    {
        var interaction = _context.Interactions.Find(id);
        if (interaction == null)
        {
            return;
        }

        _context.Interactions.Remove(interaction);
        _context.SaveChanges();
    }
}
