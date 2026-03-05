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

    public Quotation? GetQuotationById(int id)
    {
        return _context.Quotations.Find(id);
    }

    public List<Quotation> GetAllQuotations()
    {
        return _context.Quotations.ToList();
    }

    public List<Quotation> GetQuotationsByLead(int leadId)
    {
        return _context.Quotations.Where(q => q.LeadId == leadId).ToList();
    }

    public void UpdateQuotation(Quotation quotation)
    {
        _context.Quotations.Update(quotation);
        _context.SaveChanges();
    }

    public void DeleteQuotation(int id)
    {
        var quotation = _context.Quotations.Find(id);
        if (quotation == null)
        {
            return;
        }

        _context.Quotations.Remove(quotation);
        _context.SaveChanges();
    }
}
