using LeadManagementSystem.Data;
using LeadManagementSystem.Logic;
using LeadManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeadManagementTests;

public class UnitTest1
{
    private static LeadDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<LeadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LeadDbContext(options);
    }

    [Fact]
    public void LeadRepository_FullCrud_Works()
    {
        using var context = CreateContext();
        var leadRepo = new LeadRepository(context);

        var lead = new Lead
        {
            Name = "Alice",
            Email = "alice@example.com",
            Status = "New",
            Priority = "Medium"
        };

        leadRepo.AddLead(lead);
        Assert.True(lead.LeadId > 0);

        var loaded = leadRepo.GetLeadById(lead.LeadId);
        Assert.NotNull(loaded);
        Assert.Equal("Alice", loaded!.Name);

        loaded.Priority = "High";
        leadRepo.UpdateLead(loaded);

        var updated = leadRepo.GetLeadById(lead.LeadId);
        Assert.Equal("High", updated!.Priority);

        leadRepo.DeleteLead(lead.LeadId);
        Assert.Null(leadRepo.GetLeadById(lead.LeadId));
    }

    [Fact]
    public void InteractionService_FullCrud_Works()
    {
        using var context = CreateContext();
        var leadRepo = new LeadRepository(context);
        var interactionRepo = new InteractionRepository(context);
        var interactionService = new InteractionService(interactionRepo);

        var lead = new Lead { Name = "Bob", Status = "New" };
        leadRepo.AddLead(lead);

        var interaction = new Interaction
        {
            LeadId = lead.LeadId,
            InteractionType = "Call",
            Details = "Initial call"
        };

        interactionService.CreateInteraction(interaction);
        Assert.True(interaction.InteractionId > 0);

        var loaded = interactionService.GetInteractionById(interaction.InteractionId);
        Assert.NotNull(loaded);

        var updateMessage = interactionService.UpdateInteraction(interaction.InteractionId, "Email", "Follow-up email", null);
        Assert.Contains("Success", updateMessage);
        Assert.Equal("Email", interactionService.GetInteractionById(interaction.InteractionId)!.InteractionType);

        var deleteMessage = interactionService.DeleteInteraction(interaction.InteractionId);
        Assert.Contains("Success", deleteMessage);
        Assert.Null(interactionService.GetInteractionById(interaction.InteractionId));
    }

    [Fact]
    public void QuotationService_FullCrud_Works()
    {
        using var context = CreateContext();
        var leadRepo = new LeadRepository(context);
        var quotationRepo = new QuotationRepository(context);
        var quotationService = new QuotationService(quotationRepo);

        var lead = new Lead { Name = "Carol", Status = "New" };
        leadRepo.AddLead(lead);

        var quote = new Quotation
        {
            LeadId = lead.LeadId,
            QuoteNumber = "QT-100",
            Status = "Draft",
            TotalAmount = 1200m
        };

        quotationService.CreateQuotation(quote);
        Assert.True(quote.QuoteId > 0);

        var loaded = quotationService.GetQuotationById(quote.QuoteId);
        Assert.NotNull(loaded);

        var updateMessage = quotationService.UpdateQuotation(quote.QuoteId, "Sent", 1500m, DateTime.UtcNow.AddDays(10));
        Assert.Contains("Success", updateMessage);
        Assert.Equal("Sent", quotationService.GetQuotationById(quote.QuoteId)!.Status);

        var deleteMessage = quotationService.DeleteQuotation(quote.QuoteId);
        Assert.Contains("Success", deleteMessage);
        Assert.Null(quotationService.GetQuotationById(quote.QuoteId));
    }

    [Fact]
    public void LeadService_QualifiedLead_ConvertsToCustomer()
    {
        using var context = CreateContext();
        var leadRepo = new LeadRepository(context);
        var customerRepo = new CustomerRepository(context);
        var leadService = new LeadService(leadRepo, customerRepo, context);

        var lead = new Lead
        {
            Name = "Diana",
            Email = "diana@example.com",
            Company = "Blue Corp",
            Status = "Qualified"
        };

        leadRepo.AddLead(lead);

        var result = leadService.ConvertToCustomer(lead.LeadId);

        Assert.Contains("Success", result);
        Assert.Equal("Converted", leadService.GetLeadById(lead.LeadId)!.Status);
        Assert.NotNull(customerRepo.GetCustomerByLeadId(lead.LeadId));
    }

    [Fact]
    public void LeadService_NonQualifiedLead_DoesNotConvert()
    {
        using var context = CreateContext();
        var leadRepo = new LeadRepository(context);
        var customerRepo = new CustomerRepository(context);
        var leadService = new LeadService(leadRepo, customerRepo, context);

        var lead = new Lead
        {
            Name = "Ethan",
            Status = "Contacted"
        };

        leadRepo.AddLead(lead);

        var result = leadService.ConvertToCustomer(lead.LeadId);

        Assert.Contains("Error", result);
        Assert.Null(customerRepo.GetCustomerByLeadId(lead.LeadId));
    }

    [Fact]
    public void ReportService_ReturnsExpectedAnalytics()
    {
        using var context = CreateContext();
        var leadRepo = new LeadRepository(context);
        var customerRepo = new CustomerRepository(context);
        var quotationRepo = new QuotationRepository(context);
        var leadService = new LeadService(leadRepo, customerRepo, context);
        var reportService = new ReportService(leadRepo, customerRepo, quotationRepo);

        var lead1 = new Lead { Name = "L1", Status = "Qualified" };
        var lead2 = new Lead { Name = "L2", Status = "New" };
        leadRepo.AddLead(lead1);
        leadRepo.AddLead(lead2);

        quotationRepo.AddQuotation(new Quotation { LeadId = lead1.LeadId, QuoteNumber = "Q1", Status = "Draft", TotalAmount = 100m });
        quotationRepo.AddQuotation(new Quotation { LeadId = lead2.LeadId, QuoteNumber = "Q2", Status = "Sent", TotalAmount = 300m });

        leadService.ConvertToCustomer(lead1.LeadId);

        var statusDistribution = reportService.GetLeadStatusDistribution();
        var conversionSummary = reportService.GetConversionSummary();
        var totalsByStatus = reportService.GetQuotationTotalsByStatus();

        Assert.True(statusDistribution.Count >= 1);
        Assert.Equal(2, conversionSummary.TotalLeads);
        Assert.Equal(1, conversionSummary.ConvertedLeads);
        Assert.Equal(50m, conversionSummary.ConversionRatePercent);
        Assert.Equal(100m, totalsByStatus["Draft"]);
        Assert.Equal(300m, totalsByStatus["Sent"]);
    }
}
