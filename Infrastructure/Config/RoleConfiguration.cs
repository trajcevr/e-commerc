using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "A1B2C3D4-E5F6-7890-1234-56789ABCDEF0", 
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole
            {
                Id = "B2C3D4E5-F678-9012-3456-7890ABCDEF12", 
                Name = "Customer",
                NormalizedName = "CUSTOMER"
            }
        );
    }
}
