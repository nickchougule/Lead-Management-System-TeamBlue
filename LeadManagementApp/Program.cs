using LeadManagementSystem.Models;
using LeadManagementSystem.Data;
using LeadManagementSystem.Logic;
using LeadManagementSystem.Utilities;

namespace LeadManagementSystem;

class Program
{
    static void Main(string[] args)
    {
        // Setup Dependencies (SOLID: Manual Dependency Injection)
        var leadRepo = new LeadRepository();
        var leadService = new LeadService(leadRepo);
        var reportService = new ReportService();

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== CRM: LEAD MANAGEMENT SYSTEM ===");
            Console.WriteLine("1. Create New Lead");
            Console.WriteLine("2. Record Interaction (Call/Email)");
            Console.WriteLine("3. Qualify & Convert Lead to Customer");
            Console.WriteLine("4. View Lead Analytics Report");
            Console.WriteLine("5. Exit");
            Console.Write("\nSelect an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddNewLead(leadRepo);
                    break;
                case "2":
                    RecordInteraction(leadRepo);
                    break;
                case "3":
                    ConvertLead(leadService);
                    break;
                case "4":
                    reportService.ShowLeadStatusDistribution();
                    Console.WriteLine("\nPress any key to return...");
                    Console.ReadKey();
                    break;
                case "5":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid selection. Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void AddNewLead(LeadRepository repo)
    {
        Console.WriteLine("\n--- Create New Lead ---");
        var lead = new Lead
        {
            Name = InputValidator.GetRequiredString("Enter Name: "),
            Email = InputValidator.GetRequiredString("Enter Email: "),
            Phone = Console.ReadLine(),
            Company = Console.ReadLine(),
            Status = "New",
            Source = "Manual Entry"
        };

        repo.AddLead(lead); // Member 2's secure parameterized method
        Console.WriteLine("Lead created successfully! Press any key...");
        Console.ReadKey();
    }

    static void ConvertLead(LeadService service)
    {
        Console.WriteLine("\n--- Convert Lead ---");
        int id = InputValidator.GetValidInt("Enter Lead ID to Convert: ");
        string result = service.ConvertToCustomer(id); // Member 3's logic
        Console.WriteLine(result);
        Console.ReadKey();
    }

    static void RecordInteraction(LeadRepository repo)
    {
        // Logic to link an InteractionId to a LeadId
        Console.WriteLine("Feature for recording interaction... Press any key.");
        Console.ReadKey();
    }
}
