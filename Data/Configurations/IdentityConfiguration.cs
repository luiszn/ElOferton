using ElOferton.Constants;
using ElOferton.Models.Entities;
using ElOferton.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElOferton.Data.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Usuarios");
        builder.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Activo).HasDefaultValue(true);
        builder.Property(u => u.FechaCreacion).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.HasIndex(u => u.PuntoVentaId);
        builder.HasOne(u => u.PuntoVenta)
            .WithMany(p => p.Usuarios)
            .HasForeignKey(u => u.PuntoVentaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<int>> builder)
    {
        builder.ToTable("Roles");

        builder.HasData(
            new IdentityRole<int>
            {
                Id = 1,
                Name = Roles.Administrador,
                NormalizedName = Roles.Administrador.ToUpperInvariant(),
                ConcurrencyStamp = "2d1f1c4e-0f2e-4a2e-9a4e-000000000001"
            },
            new IdentityRole<int>
            {
                Id = 2,
                Name = Roles.Vendedor,
                NormalizedName = Roles.Vendedor.ToUpperInvariant(),
                ConcurrencyStamp = "2d1f1c4e-0f2e-4a2e-9a4e-000000000002"
            },
            new IdentityRole<int>
            {
                Id = 3,
                Name = Roles.Almacenero,
                NormalizedName = Roles.Almacenero.ToUpperInvariant(),
                ConcurrencyStamp = "2d1f1c4e-0f2e-4a2e-9a4e-000000000003"
            });
    }
}

public sealed class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<int>> builder) => builder.ToTable("UsuarioRoles");
}

public sealed class IdentityUserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<int>> builder) => builder.ToTable("UsuarioClaims");
}

public sealed class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<int>> builder) => builder.ToTable("UsuarioLogins");
}

public sealed class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<int>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<int>> builder) => builder.ToTable("RolClaims");
}

public sealed class IdentityUserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<int>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<int>> builder) => builder.ToTable("UsuarioTokens");
}
