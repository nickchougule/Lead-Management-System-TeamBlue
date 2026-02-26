// Models/Lead.cs
namespace LeadManagementSystem.Models;

public class Lead 
{
    public int LeadId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public string? Position { get; set; } // Added required field
    public string Status { get; set; } = "New"; 
    public string Source { get; set; } = "Website";
    public string Priority { get; set; } = "Medium";
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public int? AssignedToRepId { get; set; }
    public virtual SalesRep? AssignedRep { get; set; }
    public virtual ICollection<Interaction> Interactions { get; set; } = new List<Interaction>();
}