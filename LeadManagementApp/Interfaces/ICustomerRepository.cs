using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface ICustomerRepository
{
    void AddCustomer(Customer customer);
    Customer? GetCustomerById(int id);
    Customer? GetCustomerByLeadId(int leadId);
    List<Customer> GetAllCustomers();
}
