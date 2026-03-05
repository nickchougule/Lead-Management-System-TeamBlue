using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface IQuotationService
{
    Quotation CreateQuotation(Quotation quotation);
    Quotation? GetQuotationById(int quotationId);
    List<Quotation> GetAllQuotations();
    List<Quotation> GetQuotationsByLead(int leadId);
    string UpdateQuotation(int quotationId, string status, decimal totalAmount, DateTime? expiryDate);
    string DeleteQuotation(int quotationId);
}
