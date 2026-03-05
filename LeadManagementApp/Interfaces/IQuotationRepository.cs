using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface IQuotationRepository
{
    void AddQuotation(Quotation quotation);
    Quotation? GetQuotationById(int id);
    List<Quotation> GetAllQuotations();
    List<Quotation> GetQuotationsByLead(int leadId);
    void UpdateQuotation(Quotation quotation);
    void DeleteQuotation(int id);
}
