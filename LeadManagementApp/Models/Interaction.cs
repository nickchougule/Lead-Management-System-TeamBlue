namespace LeadManagementSystem.Models;

public class Interaction
{
    public int InteractionId { get; set; }
    public string InteractionType { get; set; } = string.Empty; // Call, Email, Meeting
    public string Details { get; set; } = string.Empty;
    public DateTime InteractionDate { get; set; } = DateTime.Now;
    public DateTime? FollowUpDate { get; set; }

    // Foreign Key to Lead
    public int LeadId { get; set; }
    public virtual Lead? Lead { get; set; }
}