using Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Moze i vaka
        // builder.Property("Price").HasColumnType("decimal(18,2)");
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}
