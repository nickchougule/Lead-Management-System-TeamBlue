using LeadManagementSystem.Interfaces;
using LeadManagementSystem.Models;

namespace LeadManagementSystem.Logic;

public class QuotationService : IQuotationService
{
    private readonly IQuotationRepository _repo;

    public QuotationService(IQuotationRepository repo)
    {
        _repo = repo;
    }

    public Quotation CreateQuotation(Quotation quotation)
    {
        _repo.AddQuotation(quotation);
        return quotation;
    }

    public Quotation? GetQuotationById(int quotationId)
    {
        return _repo.GetQuotationById(quotationId);
    }

    public List<Quotation> GetAllQuotations()
    {
        return _repo.GetAllQuotations();
    }

    public List<Quotation> GetQuotationsByLead(int leadId)
    {
        return _repo.GetQuotationsByLead(leadId);
    }

    public string UpdateQuotation(int quotationId, string status, decimal totalAmount, DateTime? expiryDate)
    {
        var quotation = _repo.GetQuotationById(quotationId);
        if (quotation == null)
        {
            return "Error: Quotation not found.";
        }

        quotation.Status = status;
        quotation.TotalAmount = totalAmount;
        quotation.ExpiryDate = expiryDate;
        _repo.UpdateQuotation(quotation);
        return "Success: Quotation updated.";
    }

    public string DeleteQuotation(int quotationId)
    {
        var quotation = _repo.GetQuotationById(quotationId);
        if (quotation == null)
        {
            return "Error: Quotation not found.";
        }

        _repo.DeleteQuotation(quotationId);
        return "Success: Quotation deleted.";
    }
}
