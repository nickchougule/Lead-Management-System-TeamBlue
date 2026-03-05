using LeadManagementSystem.Interfaces;
using LeadManagementSystem.Models;

namespace LeadManagementSystem.Logic;

public class InteractionService : IInteractionService
{
    private readonly IInteractionRepository _repo;

    public InteractionService(IInteractionRepository repo)
    {
        _repo = repo;
    }

    public Interaction CreateInteraction(Interaction interaction)
    {
        _repo.AddInteraction(interaction);
        return interaction;
    }

    public Interaction? GetInteractionById(int interactionId)
    {
        return _repo.GetInteractionById(interactionId);
    }

    public List<Interaction> GetAllInteractions()
    {
        return _repo.GetAllInteractions();
    }

    public List<Interaction> GetInteractionsByLead(int leadId)
    {
        return _repo.GetInteractionsByLead(leadId);
    }

    public string UpdateInteraction(int interactionId, string interactionType, string details, DateTime? followUpDate)
    {
        var interaction = _repo.GetInteractionById(interactionId);
        if (interaction == null)
        {
            return "Error: Interaction not found.";
        }

        interaction.InteractionType = interactionType;
        interaction.Details = details;
        interaction.FollowUpDate = followUpDate;
        _repo.UpdateInteraction(interaction);
        return "Success: Interaction updated.";
    }

    public string DeleteInteraction(int interactionId)
    {
        var interaction = _repo.GetInteractionById(interactionId);
        if (interaction == null)
        {
            return "Error: Interaction not found.";
        }

        _repo.DeleteInteraction(interactionId);
        return "Success: Interaction deleted.";
    }
}
