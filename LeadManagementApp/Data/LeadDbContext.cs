// Data/LeadDbContext.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LeadManagementSystem.Models;

namespace LeadManagementSystem.Data;

public class LeadDbContext : DbContext
{
    public LeadDbContext()
    {
    }

    public LeadDbContext(DbContextOptions<LeadDbContext> options) : base(options)
    {
    }

    public DbSet<Lead> Leads { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<SalesRep> SalesRepresentatives { get; set; }
    public DbSet<Quotation> Quotations { get; set; }
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (options.IsConfigured)
        {
            return;
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection") ??
            Environment.GetEnvironmentVariable("LMS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Database connection string is missing. Set ConnectionStrings:DefaultConnection in appsettings.json or LMS_CONNECTION_STRING environment variable.");
        }

        options.UseSqlServer(connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lead>().HasKey(l => l.LeadId);
        modelBuilder.Entity<Interaction>().HasKey(i => i.InteractionId);
        modelBuilder.Entity<SalesRep>().HasKey(r => r.RepId);
        modelBuilder.Entity<Quotation>().HasKey(q => q.QuoteId);
        modelBuilder.Entity<Customer>().HasKey(c => c.CustomerId);
        modelBuilder.Entity<Quotation>()
            .Property(q => q.TotalAmount)
            .HasPrecision(18, 2);

        // If a rep is deleted, keep the lead but set RepId to null
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.AssignedRep)
            .WithMany(r => r.AssignedLeads)
            .HasForeignKey(l => l.AssignedToRepId)
            .OnDelete(DeleteBehavior.SetNull);

        // CASCADE DELETE: If a Lead is deleted, delete all their interactions
        modelBuilder.Entity<Interaction>()
            .HasOne(i => i.Lead)
            .WithMany(l => l.Interactions)
            .HasForeignKey(i => i.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // CASCADE DELETE: If a Lead is deleted, delete all their quotes
        modelBuilder.Entity<Quotation>()
            .HasOne(q => q.Lead)
            .WithMany(l => l.Quotations)
            .HasForeignKey(q => q.LeadId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .HasOne(c => c.Lead)
            .WithOne(l => l.Customer)
            .HasForeignKey<Customer>(c => c.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
