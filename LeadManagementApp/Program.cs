using LeadManagementSystem.Data;
using LeadManagementSystem.Interfaces;
using LeadManagementSystem.Logic;
using LeadManagementSystem.Models;
using LeadManagementSystem.Utilities;
using System.Globalization;

namespace LeadManagementSystem;

class Program
{
    static void Main(string[] args)
    {
        using var db = new LeadDbContext();
        ILeadService leadService = new LeadService(new LeadRepository(db), new CustomerRepository(db), db);
        IInteractionService interactionService = new InteractionService(new InteractionRepository(db));
        IQuotationService quotationService = new QuotationService(new QuotationRepository(db));
        IReportService reportService = new ReportService(new LeadRepository(db), new CustomerRepository(db), new QuotationRepository(db));
        int defaultRepId = SeedDefaultSalesRep(db);

        while (true)
        {
            Console.Clear();
            ShowDueFollowUpReminders(interactionService);
            ShowMenu();
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1": CreateLead(leadService, db, defaultRepId); break;
                case "2": ViewLeads(leadService); break;
                case "3": UpdateLead(leadService); break;
                case "4": DeleteLead(leadService); break;
                case "5": CreateInteraction(leadService, interactionService); break;
                case "6": ViewInteractionsByLead(interactionService); break;
                case "7": UpdateInteraction(interactionService); break;
                case "8": DeleteInteraction(interactionService); break;
                case "9": CreateQuotation(leadService, quotationService); break;
                case "10": ViewQuotationsByLead(quotationService); break;
                case "11": UpdateQuotation(quotationService); break;
                case "12": DeleteQuotation(quotationService); break;
                case "13": ConvertLeadToCustomer(leadService); break;
                case "14": ShowReports(reportService); break;
                case "15": return;
                default: Pause("Invalid selection. Press any key..."); break;
            }
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("""
=== CRM: LEAD MANAGEMENT SYSTEM ===
1. Create Lead
2. View Leads
3. Update Lead Status/Priority
4. Delete Lead
5. Create Interaction
6. View Interactions by Lead
7. Update Interaction
8. Delete Interaction
9. Create Quotation
10. View Quotations by Lead
11. Update Quotation
12. Delete Quotation
13. Convert Qualified Lead to Customer
14. View Analytics Report
15. Exit
""");
    }

    static int SeedDefaultSalesRep(LeadDbContext db)
    {
        var existingRep = db.SalesRepresentatives.FirstOrDefault();
        if (existingRep != null) return existingRep.RepId;

        var rep = new SalesRep { Name = "Default Rep", Email = "rep@company.com", Department = "Sales" };
        db.SalesRepresentatives.Add(rep);
        db.SaveChanges();
        return rep.RepId;
    }

    static void CreateLead(ILeadService service, LeadDbContext db, int defaultRepId)
    {
        Console.WriteLine("\n--- Create Lead ---");
        string name = InputValidator.GetRequiredString("Enter Name: ");
        string email = InputValidator.GetRequiredString("Enter Email: ");
        Console.Write("Enter Phone: "); var phone = Console.ReadLine();
        Console.Write("Enter Company: "); var company = Console.ReadLine();
        Console.Write("Enter Position: "); var position = Console.ReadLine();

        var lead = new Lead
        {
            Name = name,
            Email = email,
            Phone = string.IsNullOrWhiteSpace(phone) ? "N/A" : phone,
            Company = string.IsNullOrWhiteSpace(company) ? "N/A" : company,
            Position = string.IsNullOrWhiteSpace(position) ? "N/A" : position,
            Status = "New",
            Priority = "Medium",
            Source = SelectLeadSource(),
            AssignedToRepId = SelectRepId(db, defaultRepId)
        };

        Console.WriteLine($"Lead created. Lead ID: {service.CreateLead(lead).LeadId}");
        Pause();
    }

    static string SelectLeadSource()
    {
        Console.WriteLine("Select Source: 1.Website 2.Referral 3.Cold Call 4.Event 5.Partner");
        while (true)
        {
            switch (Console.ReadLine())
            {
                case "1": return "Website";
                case "2": return "Referral";
                case "3": return "Cold Call";
                case "4": return "Event";
                case "5": return "Partner";
                default:
                    Console.Write("Invalid source. Choose 1-5: ");
                    break;
            }
        }
    }

    static int SelectRepId(LeadDbContext db, int defaultRepId)
    {
        Console.WriteLine("Available Sales Reps:");
        foreach (var rep in db.SalesRepresentatives.OrderBy(r => r.RepId))
            Console.WriteLine($"{rep.RepId}. {rep.Name} ({rep.Email})");

        while (true)
        {
            Console.Write($"Assign Rep ID (press Enter for default {defaultRepId}): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return defaultRepId;
            if (int.TryParse(input, out var selected) && db.SalesRepresentatives.Any(r => r.RepId == selected)) return selected;
            Console.WriteLine("Invalid rep id.");
        }
    }

    static void ViewLeads(ILeadService service)
    {
        Console.WriteLine("\n--- Leads ---");
        var leads = service.GetAllLeads();
        if (leads.Count == 0) { Pause("No leads found."); return; }

        foreach (var l in leads)
            Console.WriteLine($"ID: {l.LeadId} | Name: {l.Name} | Status: {l.Status} | Priority: {l.Priority} | Source: {l.Source} | RepId: {l.AssignedToRepId}");
        Pause();
    }

    static void UpdateLead(ILeadService service)
    {
        Console.WriteLine("\n--- Update Lead ---");
        int id = InputValidator.GetValidInt("Enter Lead ID: ");
        Console.WriteLine("1. Update Status (New, Contacted, Qualified, Unqualified)");
        Console.WriteLine("2. Update Priority (Low, Medium, High)");

        string result = Console.ReadLine() switch
        {
            "1" => service.UpdateStatus(id, GetAllowedValue("Enter new status", "New", "Contacted", "Qualified", "Unqualified")),
            "2" => service.UpdatePriority(id, GetAllowedValue("Enter new priority", "Low", "Medium", "High")),
            _ => "Invalid update option."
        };

        Console.WriteLine(result);
        Pause();
    }

    static void DeleteLead(ILeadService service) { Console.WriteLine("\n--- Delete Lead ---\nWARNING: This will delete lead and related records."); Console.WriteLine(service.DeleteLead(InputValidator.GetValidInt("Enter Lead ID to delete: "))); Pause(); }

    static void CreateInteraction(ILeadService leadService, IInteractionService interactionService)
    {
        Console.WriteLine("\n--- Create Interaction ---");
        int leadId = InputValidator.GetValidInt("Enter Lead ID: ");
        if (leadService.GetLeadById(leadId) == null) { Pause("Error: Lead not found."); return; }

        var interaction = new Interaction
        {
            LeadId = leadId,
            InteractionType = GetAllowedValue("Type", "Call", "Email", "Meeting"),
            Details = InputValidator.GetRequiredString("Details: "),
            InteractionDate = DateTime.UtcNow,
            FollowUpDate = GetOptionalDate("Follow-up date (yyyy-MM-dd, optional): ")
        };

        Console.WriteLine($"Interaction created. Interaction ID: {interactionService.CreateInteraction(interaction).InteractionId}");
        Pause();
    }

    static void ViewInteractionsByLead(IInteractionService service) { Console.WriteLine("\n--- View Interactions by Lead ---"); var list = service.GetInteractionsByLead(InputValidator.GetValidInt("Enter Lead ID: ")); if (list.Count == 0) { Pause("No interactions found for this lead."); return; } foreach (var i in list) Console.WriteLine($"ID: {i.InteractionId} | Type: {i.InteractionType} | Date: {i.InteractionDate:u} | Follow-up: {i.FollowUpDate:yyyy-MM-dd} | Details: {i.Details}"); Pause(); }
    static void UpdateInteraction(IInteractionService service) { Console.WriteLine("\n--- Update Interaction ---"); Console.WriteLine(service.UpdateInteraction(InputValidator.GetValidInt("Enter Interaction ID: "), GetAllowedValue("Enter new type", "Call", "Email", "Meeting"), InputValidator.GetRequiredString("Enter new details: "), GetOptionalDate("Enter follow-up date (yyyy-MM-dd) or leave empty: "))); Pause(); }
    static void DeleteInteraction(IInteractionService service) { Console.WriteLine("\n--- Delete Interaction ---"); Console.WriteLine(service.DeleteInteraction(InputValidator.GetValidInt("Enter Interaction ID: "))); Pause(); }

    static void CreateQuotation(ILeadService leadService, IQuotationService quotationService)
    {
        Console.WriteLine("\n--- Create Quotation ---");
        int leadId = InputValidator.GetValidInt("Enter Lead ID: ");
        if (leadService.GetLeadById(leadId) == null) { Pause("Error: Lead not found."); return; }
        var amount = GetPositiveDecimal("Enter total amount: ");

        var q = new Quotation { LeadId = leadId, QuoteNumber = InputValidator.GetRequiredString("Enter Quote Number: "), TotalAmount = amount, Status = "Draft", QuoteDate = DateTime.UtcNow };
        Console.WriteLine($"Quotation created. Quotation ID: {quotationService.CreateQuotation(q).QuoteId}");
        Pause();
    }

    static void ViewQuotationsByLead(IQuotationService service) { Console.WriteLine("\n--- View Quotations by Lead ---"); var list = service.GetQuotationsByLead(InputValidator.GetValidInt("Enter Lead ID: ")); if (list.Count == 0) { Pause("No quotations found for this lead."); return; } foreach (var q in list) Console.WriteLine($"ID: {q.QuoteId} | Number: {q.QuoteNumber} | Status: {q.Status} | Amount: {q.TotalAmount}"); Pause(); }
    static void UpdateQuotation(IQuotationService service) { Console.WriteLine("\n--- Update Quotation ---"); var amount = GetPositiveDecimal("Enter new total amount: "); Console.WriteLine(service.UpdateQuotation(InputValidator.GetValidInt("Enter Quotation ID: "), GetAllowedValue("Enter new status", "Draft", "Sent", "Accepted", "Rejected"), amount, GetOptionalDate("Enter expiry date (yyyy-MM-dd) or leave empty: "))); Pause(); }
    static void DeleteQuotation(IQuotationService service) { Console.WriteLine("\n--- Delete Quotation ---"); Console.WriteLine(service.DeleteQuotation(InputValidator.GetValidInt("Enter Quotation ID: "))); Pause(); }
    static void ConvertLeadToCustomer(ILeadService service) { Console.WriteLine("\n--- Convert Lead to Customer ---"); Console.WriteLine(service.ConvertToCustomer(InputValidator.GetValidInt("Enter Qualified Lead ID: "))); Pause(); }

    static void ShowReports(IReportService reportService)
    {
        Console.WriteLine("\n--- Lead Status Distribution ---");
        foreach (var s in reportService.GetLeadStatusDistribution()) Console.WriteLine($"{s.Key}: {s.Value}");

        Console.WriteLine("\n--- Leads by Source ---");
        foreach (var s in reportService.GetLeadsBySource()) Console.WriteLine($"{s.Key}: {s.Value}");

        var c = reportService.GetConversionSummary();
        Console.WriteLine($"\n--- Conversion Summary ---\nTotal Leads: {c.TotalLeads}\nConverted Customers: {c.ConvertedLeads}\nConversion Rate: {c.ConversionRatePercent}%");

        Console.WriteLine("\n--- Sales Rep Performance ---");
        foreach (var r in reportService.GetSalesRepPerformance()) Console.WriteLine($"{r.Key}: Assigned={r.Value.AssignedLeads}, Converted={r.Value.ConvertedLeads}");

        Console.WriteLine("\n--- Quotation Totals by Status ---");
        foreach (var q in reportService.GetQuotationTotalsByStatus()) Console.WriteLine($"{q.Key}: {q.Value}");

        Pause("\nPress any key to return...");
    }

    static void ShowDueFollowUpReminders(IInteractionService interactionService)
    {
        var today = DateTime.UtcNow.Date;
        var due = interactionService.GetAllInteractions().Where(i => i.FollowUpDate.HasValue && i.FollowUpDate.Value.Date <= today).ToList();
        if (due.Count == 0) return;

        Console.WriteLine($"REMINDER: {due.Count} interaction(s) have due follow-ups.");
        foreach (var i in due.Take(3)) Console.WriteLine($"  Lead {i.LeadId}, Interaction {i.InteractionId}, Due {i.FollowUpDate:yyyy-MM-dd}");
        Console.WriteLine();
    }

    static DateTime? GetOptionalDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return null;
            if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)) return d;
            Console.WriteLine("Invalid date. Use yyyy-MM-dd or leave empty.");
        }
    }

    static string GetAllowedValue(string label, params string[] allowed)
    {
        while (true)
        {
            Console.Write($"{label} ({string.Join("/", allowed)}): ");
            var input = Console.ReadLine();
            var selected = allowed.FirstOrDefault(v => string.Equals(v, input?.Trim(), StringComparison.OrdinalIgnoreCase));
            if (selected != null) return selected;
            Console.WriteLine("Invalid option.");
        }
    }

    static decimal GetPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out var amount) && amount >= 0) return amount;
            Console.WriteLine("Invalid amount.");
        }
    }
    static void Pause(string? message = null) { if (!string.IsNullOrWhiteSpace(message)) Console.WriteLine(message); Console.ReadKey(); }
}
