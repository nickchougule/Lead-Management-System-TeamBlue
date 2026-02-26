using LeadManagementSystem.Interfaces;
using LeadManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementSystem.Data;

public class SalesRepository:ISalesRepository
{
    // Connection string used by the team
    private readonly string _connectionString = "Server=localhost;Database=CRM_LeadManagement;User Id=sa;Password=Nikhil@1234;TrustServerCertificate=True";
    private readonly LeadDbContext _context;
    public SalesRepository(LeadDbContext context)
    {
        _context = context;
    }
    // 1. CREATE: Add a new Sales Representative
    public void AddSalesRep(SalesRep rep)
    {
        _context.SalesRepresentatives.Add(rep);
        _context.SaveChanges();
    }

    // 2. READ: Get all reps to populate an assignment dropdown/list
    public List<SalesRep> GetAllReps()
    {
        return _context.SalesRepresentatives.ToList();
    }

    // 3. READ: Get a specific rep by ID
    public SalesRep? GetRepById(int id)
    {
        return _context.SalesRepresentatives.Find(id);
    }

    // 4. UPDATE: Change rep details (e.g., Department)
    public void UpdateRep(SalesRep rep)
    {
        _context.SalesRepresentatives.Update(rep);
        _context.SaveChanges();
    }

    // 5. DELETE: Remove a rep from the system
    public void DeleteRep(int id)
    {
        var rep = _context.SalesRepresentatives.Find(id);
        if (rep != null)
        {
            _context.SalesRepresentatives.Remove(rep);
            _context.SaveChanges();
        }
    }
}