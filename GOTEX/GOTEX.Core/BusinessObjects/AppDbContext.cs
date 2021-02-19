using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GOTEX.Core.BusinessObjects
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationHistory> ApplicationHistories { get; set; }
        public DbSet<WorkFlow> WorkFlows { get; set; }
        public DbSet<ApplicationType> ApplicationTypes { get; set; }
        public DbSet<ApplicationTypeDocuments> ApplicationTypeDocuments { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<Permit> Permits { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Quarter> Quarters { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RemitaPayment> RemitaPayments { get; set; }
        public DbSet<PaymentApproval> PaymentApprovals { get; set; }
        public DbSet<ManualRemitaValue> ManualRemitaValues { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<MailTemplate> MailTemplates { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<PaymentEvidence> PaymentEvidences { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            
            builder.Entity<Application>()
                .Property(o => o.ProductAmount)
                .HasColumnType("decimal(18,2)");
            builder.Entity<Application>()
                .Property(o => o.Fee)
                .HasColumnType("decimal(18,2)");
            builder.Entity<Application>()
                .Property(o => o.ServiceCharge)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<ApplicationType>()
                .Property(o => o.Amount)
                .HasColumnType("decimal(18,2)");
            builder.Entity<ApplicationType>()
                .Property(o => o.ProcessingFee)
                .HasColumnType("decimal(18,2)");
            builder.Entity<ApplicationType>()
                .Property(o => o.ServiceCharge)
                .HasColumnType("decimal(18,2)");
            
            builder.Entity<PaymentApproval>()
                .Property(o => o.Amount)
                .HasColumnType("decimal(18,2)");
        }
    }
}
