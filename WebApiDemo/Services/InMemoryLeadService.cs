using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;

namespace WebApiDemo.Services;

public class InMemoryLeadService : ILeadService
{
    private readonly List<LeadDto> _leads =
    [
        new LeadDto { Id = 1, Name = "Alice", Email = "alice@demo.com", Status = "New" },
        new LeadDto { Id = 2, Name = "Bob", Email = "bob@demo.com", Status = "Contacted" }
    ];

    private readonly object _lock = new();
    private int _nextId = 3;

    public IReadOnlyCollection<LeadDto> GetAll()
    {
        lock (_lock)
        {
            return _leads.Select(Clone).ToList();
        }
    }

    public LeadDto? GetById(int id)
    {
        lock (_lock)
        {
            var lead = _leads.FirstOrDefault(x => x.Id == id);
            return lead is null ? null : Clone(lead);
        }
    }

    public LeadDto Create(LeadDto lead)
    {
        lock (_lock)
        {
            var created = new LeadDto
            {
                Id = _nextId++,
                Name = lead.Name,
                Email = lead.Email,
                Status = lead.Status
            };
            _leads.Add(created);
            return Clone(created);
        }
    }

    public bool Update(int id, LeadDto lead)
    {
        lock (_lock)
        {
            var existing = _leads.FirstOrDefault(x => x.Id == id);
            if (existing is null)
            {
                return false;
            }

            existing.Name = lead.Name;
            existing.Email = lead.Email;
            existing.Status = lead.Status;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var existing = _leads.FirstOrDefault(x => x.Id == id);
            if (existing is null)
            {
                return false;
            }

            _leads.Remove(existing);
            return true;
        }
    }

    private static LeadDto Clone(LeadDto lead)
    {
        return new LeadDto
        {
            Id = lead.Id,
            Name = lead.Name,
            Email = lead.Email,
            Status = lead.Status
        };
    }
}
