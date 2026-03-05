using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using WebApiDemo.Controllers;

namespace WebAPI.Tests.Controllers;

public class LeadsControllerTests
{
    private readonly ILeadService _fakeLeadService;
    private readonly LeadsController _controller;

    public LeadsControllerTests()
    {
        _fakeLeadService = A.Fake<ILeadService>();
        _controller = new LeadsController(_fakeLeadService);
    }

    [Fact]
    public void Get_ReturnsOk_WithLeads()
    {
        var leads = new List<LeadDto> { new() { Id = 1, Name = "Alice", Email = "a@x.com", Status = "New" } };
        A.CallTo(() => _fakeLeadService.GetAll()).Returns(leads);

        var result = _controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyCollection<LeadDto>>(ok.Value);
        Assert.Single(payload);
    }

    [Fact]
    public void GetById_WhenFound_ReturnsOk()
    {
        var lead = new LeadDto { Id = 1, Name = "Alice", Email = "a@x.com", Status = "New" };
        A.CallTo(() => _fakeLeadService.GetById(1)).Returns(lead);

        var result = _controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<LeadDto>(ok.Value);
        Assert.Equal(1, payload.Id);
    }

    [Fact]
    public void GetById_WhenMissing_ReturnsNotFound()
    {
        A.CallTo(() => _fakeLeadService.GetById(99)).Returns(null);

        var result = _controller.GetById(99);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void Create_ReturnsCreatedAtAction()
    {
        var input = new LeadDto { Name = "New", Email = "n@x.com", Status = "New" };
        var created = new LeadDto { Id = 10, Name = "New", Email = "n@x.com", Status = "New" };
        A.CallTo(() => _fakeLeadService.Create(input)).Returns(created);

        var result = _controller.Create(input);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(LeadsController.GetById), createdResult.ActionName);
        var payload = Assert.IsType<LeadDto>(createdResult.Value);
        Assert.Equal(10, payload.Id);
    }

    [Fact]
    public void Update_WhenIdMismatch_ReturnsBadRequest()
    {
        var input = new LeadDto { Id = 2, Name = "x", Email = "x@x.com", Status = "New" };

        var result = _controller.Update(1, input);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Update_WhenNotFound_ReturnsNotFound()
    {
        var input = new LeadDto { Id = 1, Name = "x", Email = "x@x.com", Status = "New" };
        A.CallTo(() => _fakeLeadService.Update(1, input)).Returns(false);

        var result = _controller.Update(1, input);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Update_WhenSuccess_ReturnsNoContent()
    {
        var input = new LeadDto { Id = 1, Name = "x", Email = "x@x.com", Status = "New" };
        A.CallTo(() => _fakeLeadService.Update(1, input)).Returns(true);

        var result = _controller.Update(1, input);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void Delete_WhenNotFound_ReturnsNotFound()
    {
        A.CallTo(() => _fakeLeadService.Delete(50)).Returns(false);

        var result = _controller.Delete(50);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_WhenSuccess_ReturnsNoContent()
    {
        A.CallTo(() => _fakeLeadService.Delete(1)).Returns(true);

        var result = _controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }
}
