using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface IInteractionService
{
    Interaction CreateInteraction(Interaction interaction);
    Interaction? GetInteractionById(int interactionId);
    List<Interaction> GetAllInteractions();
    List<Interaction> GetInteractionsByLead(int leadId);
    string UpdateInteraction(int interactionId, string interactionType, string details, DateTime? followUpDate);
    string DeleteInteraction(int interactionId);
}
