using Microsoft.EntityFrameworkCore;
using LeadManagementSystem.Models;

namespace LeadManagementSystem.Data;

public class LeadDbContext : DbContext
{
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Interaction> Interactions { get; set; }
    public DbSet<SalesRep> SalesRepresentatives { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer("Server=localhost;Database=CRM_LeadManagement;User Id=sa;Password=Nikhil@1234;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lead>().HasKey(l => l.LeadId);
        modelBuilder.Entity<Interaction>().HasKey(i => i.InteractionId);
        modelBuilder.Entity<SalesRep>().HasKey(r => r.RepId);

        modelBuilder.Entity<Lead>()
            .HasOne(l => l.AssignedRep)
            .WithMany(r => r.AssignedLeads)
            .HasForeignKey(l => l.AssignedToRepId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Interaction>()
            .HasOne(i => i.Lead)
            .WithMany(l => l.Interactions)
            .HasForeignKey(i => i.LeadId)
            .OnDelete(DeleteBehavior.Cascade);
            
    }
}