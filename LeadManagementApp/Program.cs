// Program.cs
using LeadManagementSystem.Models;
using LeadManagementSystem.Data;
using LeadManagementSystem.Logic;
using LeadManagementSystem.Utilities;
using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem;

class Program
{
    static void Main(string[] args)
    {
        using var dbContext = new LeadDbContext();
        
        // 1. Repositories
        ILeadRepository leadRepo = new LeadRepository(dbContext);
        IInteractionRepository interactionRepo = new InteractionRepository(dbContext);
        IQuotationRepository quoteRepo = new QuotationRepository(dbContext); // New Quote Repo
        
        // 2. Services
        ILeadService leadService = new LeadService(leadRepo);
        IReportService reportService = new ReportService(leadRepo);

        // Seed default Sales Rep so we can assign leads
        SeedData(dbContext);

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== CRM: LEAD MANAGEMENT SYSTEM ===");
            Console.WriteLine("1. Create New Lead");
            Console.WriteLine("2. Record Interaction (Call/Email)");
            Console.WriteLine("3. Update Lead Status or Priority");
            Console.WriteLine("4. Qualify & Convert Lead to Customer");
            Console.WriteLine("5. View Lead Analytics Report");
            Console.WriteLine("6. Create Quotation for a Lead"); // NEW OPTION
            Console.WriteLine("7. Delete a Lead");
            Console.WriteLine("8. Exit");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1": AddNewLead(leadRepo); break;
                case "2": RecordInteraction(leadRepo, interactionRepo); break;
                case "3": UpdateLeadAttributes(leadService); break;
                case "4": ConvertLead(leadService); break;
                case "5": ShowReports(reportService); break;
                case "6": CreateQuotation(leadRepo, quoteRepo); break; // NEW METHOD CALL
                case "7": DeleteLeadRecord(leadService); break;
                case "8": exit = true; break;
                default:
                    Console.WriteLine("Invalid selection. Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void SeedData(LeadDbContext context)
    {
        // We only seed the Sales Rep now. Leads and Quotes are user-driven!
        if (!context.SalesRepresentatives.Any())
        {
            context.SalesRepresentatives.Add(new SalesRep { Name = "Default Rep", Email = "rep@company.com", Department = "Sales" });
            context.SaveChanges();
        }
    }

    static void AddNewLead(ILeadRepository repo)
    {
        Console.WriteLine("\n--- Create New Lead ---");
        string name = InputValidator.GetRequiredString("Enter Name: ");
        string email = InputValidator.GetRequiredString("Enter Email: ");
        
        Console.Write("Enter Phone: ");
        string? phone = Console.ReadLine();
        
        Console.Write("Enter Company: ");
        string? company = Console.ReadLine();

        Console.Write("Enter Position: ");
        string? position = Console.ReadLine();

        var lead = new Lead
        {
            Name = name,
            Email = email,
            Phone = string.IsNullOrWhiteSpace(phone) ? "N/A" : phone,
            Company = string.IsNullOrWhiteSpace(company) ? "N/A" : company,
            Position = string.IsNullOrWhiteSpace(position) ? "N/A" : position,
            Status = "New",
            Priority = "Medium",
            Source = "Manual Entry",
            AssignedToRepId = 1 // Automatically assign to our seeded default rep
        };

        repo.AddLead(lead);
        Console.WriteLine("\nLead created successfully! Press any key...");
        Console.ReadKey();
    }

    static void UpdateLeadAttributes(ILeadService service)
    {
        Console.WriteLine("\n--- Update Lead ---");
        int id = InputValidator.GetValidInt("Enter Lead ID: ");
        
        Console.WriteLine("1. Update Status (New, Contacted, Qualified, Unqualified)");
        Console.WriteLine("2. Update Priority (Low, Medium, High)");
        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            string status = InputValidator.GetRequiredString("Enter new status: ");
            Console.WriteLine(service.UpdateStatus(id, status));
        }
        else if (choice == "2")
        {
            string priority = InputValidator.GetRequiredString("Enter new priority: ");
            Console.WriteLine(service.UpdatePriority(id, priority));
        }
        Console.ReadKey();
    }

    static void RecordInteraction(ILeadRepository leadRepo, IInteractionRepository interactionRepo)
    {
        Console.WriteLine("\n--- Record Interaction ---");
        int leadId = InputValidator.GetValidInt("Enter Lead ID: ");
        
        var lead = leadRepo.GetLeadById(leadId);
        if (lead == null)
        {
            Console.WriteLine("Lead not found.");
            Console.ReadKey();
            return;
        }

        string type = InputValidator.GetRequiredString("Type (Call/Email/Meeting): ");
        string details = InputValidator.GetRequiredString("Details: ");

        var interaction = new Interaction
        {
            LeadId = leadId,
            InteractionType = type,
            Details = details,
            InteractionDate = DateTime.Now
        };

        interactionRepo.AddInteraction(interaction);
        Console.WriteLine("Interaction recorded successfully.");
        Console.ReadKey();
    }

    static void ConvertLead(ILeadService service)
    {
        Console.WriteLine("\n--- Convert Lead ---");
        int id = InputValidator.GetValidInt("Enter Lead ID to Convert: ");
        Console.WriteLine(service.ConvertToCustomer(id));
        Console.ReadKey();
    }

    static void ShowReports(IReportService service)
    {
        Console.WriteLine("\n--- Lead Status Distribution ---");
        var stats = service.GetLeadStatusDistribution();
        foreach (var stat in stats)
        {
            Console.WriteLine($"{stat.Key}: {stat.Value}");
        }
        Console.WriteLine("\nPress any key to return...");
        Console.ReadKey();
    }

    // NEW METHOD: Creating a Quotation manually
    static void CreateQuotation(ILeadRepository leadRepo, IQuotationRepository quoteRepo)
    {
        Console.WriteLine("\n--- Create Quotation ---");
        int leadId = InputValidator.GetValidInt("Enter Lead ID to attach this Quote to: ");
        
        var lead = leadRepo.GetLeadById(leadId);
        if (lead == null)
        {
            Console.WriteLine("Error: Lead not found.");
            Console.ReadKey();
            return;
        }

        string quoteNumber = InputValidator.GetRequiredString("Enter Quote Number (e.g., QT-100): ");
        Console.Write("Enter Total Amount: ");
        
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            var quote = new Quotation
            {
                QuoteNumber = quoteNumber,
                TotalAmount = amount,
                Status = "Draft",
                LeadId = leadId // Here is where we make the connection!
            };

            quoteRepo.AddQuotation(quote);
            Console.WriteLine($"\nSuccess! Quotation {quoteNumber} attached to Lead ID {leadId}.");
        }
        else
        {
            Console.WriteLine("Error: Invalid amount entered.");
        }
        Console.ReadKey();
    }

    static void DeleteLeadRecord(ILeadService service)
    {
        Console.WriteLine("\n--- Delete Lead ---");
        Console.WriteLine("WARNING: This will permanently delete the Lead and Cascade Delete all their Interactions and Quotations.");
        int id = InputValidator.GetValidInt("Enter Lead ID to Delete: ");
        
        Console.WriteLine(service.DeleteLead(id));
        Console.ReadKey();
    }
}