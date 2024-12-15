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
        builder.Property(o => o.Id).UseHiLo("paymentitemseq");

        builder.Property<string>("MovieId")
            .HasColumnType("VARCHAR(24)")
            .HasColumnName("MovieId")
            .IsRequired();
    }
}