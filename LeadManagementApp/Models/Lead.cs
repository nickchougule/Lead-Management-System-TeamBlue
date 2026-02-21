namespace LeadManagementSystem.Models;

public class Lead 
{
    public int LeadId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string Status { get; set; } = "New"; // New, Contacted, Qualified, Unqualified, Converted
    public string Source { get; set; } = "Website";
    public string Priority { get; set; } = "Medium";
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Foreign Key for SalesRep
    public int? AssignedToRepId { get; set; }
    public virtual SalesRep? AssignedRep { get; set; }

    // Relationship: One Lead can have many Interactions
    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}