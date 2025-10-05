using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Payment.Domain.Entities;

namespace Payment.Infrastructure.Persistence.EntityConfigurations;

public class PaymentItemEntityTypeConfiguration : IEntityTypeConfiguration<PaymentItem>
{
    public void Configure(EntityTypeBuilder<PaymentItem> builder)
    {
        builder.ToTable("PaymentItems");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).UseIdentityColumn();

        builder.Property(p => p.MovieName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.MovieId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Price)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(p => p.Quantity)
            .IsRequired();

        // Configure the relationship with Payment
        builder.HasOne<Domain.Aggregates.Payment>()
            .WithMany("PaymentItems")
            .HasForeignKey("PaymentId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}