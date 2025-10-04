using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payment.Infrastructure.Persistence.EntityConfigurations;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Payment>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityColumn();

        builder.OwnsOne(p => p.Money, m =>
        {
            m.Property(money => money.Amount).HasColumnName("Amount");
            m.Property(money => money.Currency).HasColumnName("Currency");
            m.WithOwner();
        });
        
        var navigation = builder.Metadata.FindNavigation(nameof(Domain.Aggregates.Payment.PaymentItems)) 
                         ?? throw new NullReferenceException($"Navigation property not found on {{nameof(Order.OrderItems)}}");
        navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}