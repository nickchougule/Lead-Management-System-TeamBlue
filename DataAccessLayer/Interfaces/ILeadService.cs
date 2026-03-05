using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces;

public interface ILeadService
{
    IReadOnlyCollection<LeadDto> GetAll();
    LeadDto? GetById(int id);
    LeadDto Create(LeadDto lead);
    bool Update(int id, LeadDto lead);
    bool Delete(int id);
}
