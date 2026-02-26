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
        // 1. Setup Database Context (One context for the session)
        using var dbContext = new LeadDbContext();
        
        // Ensure Database exists and tables are created (great for testing)
        dbContext.Database.EnsureCreated(); 
        
        // 2. Setup Dependencies (SOLID)
        ILeadRepository leadRepo = new LeadRepository(dbContext);
        IInteractionRepository interactionRepo = new InteractionRepository(dbContext);
        var leadService = new LeadService(leadRepo);
        var reportService = new ReportService(leadRepo);

        // 3. Seed initial data if tables are empty
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
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1": AddNewLead(leadRepo); break;
                case "2": RecordInteraction(leadRepo, interactionRepo); break;
                case "3": UpdateLeadAttributes(leadService); break;
                case "4": ConvertLead(leadService); break;
                case "5": 
                    ShowReports(reportService); 
                    Console.ReadKey();
                    break;
                case "6": exit = true; break;
                default:
                    Console.WriteLine("Invalid selection. Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void SeedData(LeadDbContext context)
    {
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
        
        Console.Write("Enter Position: ");
        string? position = Console.ReadLine();

        var lead = new Lead
        {
            Name = name,
            Email = email,
            Position = string.IsNullOrWhiteSpace(position) ? "N/A" : position,
            Status = "New",
            Priority = "Medium",
            Source = "Manual Entry"
        };

        repo.AddLead(lead);
        Console.WriteLine("\nLead created successfully! Press any key...");
        Console.ReadKey();
    }

    static void UpdateLeadAttributes(LeadService service)
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

    static void ConvertLead(LeadService service)
    {
        Console.WriteLine("\n--- Convert Lead ---");
        int id = InputValidator.GetValidInt("Enter Lead ID to Convert: ");
        Console.WriteLine(service.ConvertToCustomer(id));
        Console.ReadKey();
    }

    static void ShowReports(ReportService service)
    {
        Console.WriteLine("\n--- Lead Status Distribution ---");
        var stats = service.GetLeadStatusDistribution();
        foreach (var stat in stats)
        {
            Console.WriteLine($"{stat.Key}: {stat.Value}");
        }
        Console.WriteLine("\nPress any key to return...");
    }
}