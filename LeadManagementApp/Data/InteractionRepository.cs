using LeadManagementSystem.Models;

namespace LeadManagementSystem.Data;

public class InteractionRepository
{
    public void AddInteraction(Interaction interaction)
    {
        using LeadDbContext context = new LeadDbContext();
        context.Interactions.Add(interaction); // Using EF Core for automated relationship handling
        context.SaveChanges();
    }

    public List<Interaction> GetInteractionsByLead(int leadId)
    {
        using LeadDbContext context = new LeadDbContext();
        return context.Interactions.Where(i => i.LeadId == leadId).ToList();
    }
}