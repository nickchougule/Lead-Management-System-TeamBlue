using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApiDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeadsController : ControllerBase
{
    private readonly ILeadService _leadService;

    public LeadsController(ILeadService leadService)
    {
        _leadService = leadService;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<LeadDto>> Get()
    {
        return Ok(_leadService.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<LeadDto> GetById(int id)
    {
        var lead = _leadService.GetById(id);
        if (lead is null)
        {
            return NotFound();
        }

        return Ok(lead);
    }

    [HttpPost]
    public ActionResult<LeadDto> Create(LeadDto lead)
    {
        var created = _leadService.Create(lead);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, LeadDto lead)
    {
        if (id != lead.Id)
        {
            return BadRequest("Route id and body id must match.");
        }

        if (!_leadService.Update(id, lead))
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (!_leadService.Delete(id))
        {
            return NotFound();
        }

        return NoContent();
    }
}
