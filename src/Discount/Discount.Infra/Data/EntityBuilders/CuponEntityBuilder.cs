using Discount.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discount.Infra.Data.EntityBuilders;

public class CuponEntityBuilder : IEntityTypeConfiguration<Cupon>
{
    public void Configure(EntityTypeBuilder<Cupon> builder)
    {
        builder.ToTable("discounts");
        builder.HasKey(p => new { p.Id, p.UserId });
        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.UserId).IsRequired();
    }
}