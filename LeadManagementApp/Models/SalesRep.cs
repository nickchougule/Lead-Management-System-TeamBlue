namespace LeadManagementSystem.Models;

public class SalesRep
{
    public int RepId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = "Sales";

    // Relationship: One Rep can be assigned to many Leads
    public virtual ICollection<Lead> AssignedLeads { get; set; } = new List<Lead>();
}