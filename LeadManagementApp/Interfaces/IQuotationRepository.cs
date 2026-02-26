// Interfaces/IQuotationRepository.cs
using LeadManagementSystem.Models;

namespace LeadManagementSystem.Interfaces;

public interface IQuotationRepository
{
    void AddQuotation(Quotation quotation);
}