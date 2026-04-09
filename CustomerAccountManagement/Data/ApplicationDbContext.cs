using CustomerAccountManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerAccountManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(c => c.Email)
                  .IsRequired()
                  .HasMaxLength(320);

            entity.HasIndex(c => c.Email)
                  .IsUnique()
                  .HasDatabaseName("IX_Clients_Email_Unique");

            entity.Property(c => c.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Amount)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(i => i.Currency)
                  .IsRequired()
                  .HasConversion<string>()
                  .HasMaxLength(10);

            entity.Property(i => i.OriginalFileName)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(i => i.StoredFileName)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(i => i.Status)
                  .IsRequired()
                  .HasConversion<string>()
                  .HasMaxLength(20)
                  .HasDefaultValue(CustomerAccountManagement.Enums.InvoiceStatus.Pending);

            entity.Property(i => i.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(i => i.Client)
                  .WithMany(c => c.Invoices)
                  .HasForeignKey(i => i.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(i => i.ClientId)
                  .HasDatabaseName("IX_Invoices_ClientId");

            entity.HasIndex(i => i.Currency)
                  .HasDatabaseName("IX_Invoices_Currency");
        });
    }
}
