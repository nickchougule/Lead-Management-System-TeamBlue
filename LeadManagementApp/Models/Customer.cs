namespace LeadManagementSystem.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Company { get; set; }
    public DateTime ConvertedOn { get; set; } = DateTime.UtcNow;
    public int? ConvertedByRepId { get; set; }

    public int LeadId { get; set; }
    public virtual Lead? Lead { get; set; }
}
