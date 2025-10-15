using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Payment.Domain.Common;
using Payment.Infrastructure.Persistence.EntityConfigurations;

namespace Payment.Infrastructure.Persistence;

public class PaymentContext : DbContext
{
    public DbSet<Domain.Aggregates.Payment> Payments { get; set; } = null!;
    
    public PaymentContext(DbContextOptions<PaymentContext> options) : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "dz"; //Todo set this
                        break;
                //.Modified if needed
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PaymentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentItemEntityTypeConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}