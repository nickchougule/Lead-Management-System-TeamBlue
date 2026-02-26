// Models/Quotation.cs
namespace LeadManagementSystem.Models;

public class Quotation
{
    public int QuoteId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime QuoteDate { get; set; } = DateTime.Now;
    public DateTime? ExpiryDate { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Sent, Accepted, Rejected
    public decimal TotalAmount { get; set; }

    // Foreign Key connecting back to the Lead
    public int LeadId { get; set; }
    public virtual Lead? Lead { get; set; }
}