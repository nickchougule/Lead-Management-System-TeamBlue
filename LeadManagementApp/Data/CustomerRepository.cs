using LeadManagementSystem.Interfaces;
using LeadManagementSystem.Models;

namespace LeadManagementSystem.Data;

public class CustomerRepository : ICustomerRepository
{
    private readonly LeadDbContext _context;

    public CustomerRepository(LeadDbContext context)
    {
        _context = context;
    }

    public void AddCustomer(Customer customer)
    {
        _context.Customers.Add(customer);
        //triggers EF Core to generate and execute the actual SQL (INSERT, UPDATE, DELETE) against your database
        _context.SaveChanges();
    }

    public Customer? GetCustomerById(int id)
    {
        return _context.Customers.Find(id);
    }

    public Customer? GetCustomerByLeadId(int leadId)
    {
        return _context.Customers.FirstOrDefault(c => c.LeadId == leadId);
    }

    public List<Customer> GetAllCustomers()
    {
        return _context.Customers.ToList();
    }
}
