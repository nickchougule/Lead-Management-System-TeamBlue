using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface IInteractionRepository
{
    void AddInteraction(Interaction interaction);
    Interaction? GetInteractionById(int id);
    List<Interaction> GetAllInteractions();
    List<Interaction> GetInteractionsByLead(int leadId);
    void UpdateInteraction(Interaction interaction);
    void DeleteInteraction(int id);
}
