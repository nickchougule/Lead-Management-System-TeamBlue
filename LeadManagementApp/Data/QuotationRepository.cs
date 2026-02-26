// Data/QuotationRepository.cs
using LeadManagementSystem.Models;
using LeadManagementSystem.Interfaces;

namespace LeadManagementSystem.Data;

public class QuotationRepository : IQuotationRepository
{
    private readonly LeadDbContext _context;

    public QuotationRepository(LeadDbContext context)
    {
        _context = context;
    }

    public void AddQuotation(Quotation quotation)
    {
        _context.Quotations.Add(quotation);
        _context.SaveChanges();
    }
}