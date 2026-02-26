using LeadManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementSystem.Data;

public class SalesRepository
{
    // Connection string used by the team
    private readonly string _connectionString = "Server=localhost;Database=CRM_LeadManagement;User Id=sa;Password=Nikhil@1234;TrustServerCertificate=True";

    // 1. CREATE: Add a new Sales Representative
    public void AddSalesRep(SalesRep rep)
    {
        using LeadDbContext context = new LeadDbContext();
        context.SalesRepresentatives.Add(rep);
        context.SaveChanges();
    }

    // 2. READ: Get all reps to populate an assignment dropdown/list
    public List<SalesRep> GetAllReps()
    {
        using LeadDbContext context = new LeadDbContext();
        return context.SalesRepresentatives.ToList();
    }

    // 3. READ: Get a specific rep by ID
    public SalesRep? GetRepById(int id)
    {
        using LeadDbContext context = new LeadDbContext();
        return context.SalesRepresentatives.Find(id);
    }

    // 4. UPDATE: Change rep details (e.g., Department)
    public void UpdateRep(SalesRep rep)
    {
        using LeadDbContext context = new LeadDbContext();
        context.SalesRepresentatives.Update(rep);
        context.SaveChanges();
    }

    // 5. DELETE: Remove a rep from the system
    public void DeleteRep(int id)
    {
        using LeadDbContext context = new LeadDbContext();
        var rep = context.SalesRepresentatives.Find(id);
        if (rep != null)
        {
            context.SalesRepresentatives.Remove(rep);
            context.SaveChanges();
        }
    }
}