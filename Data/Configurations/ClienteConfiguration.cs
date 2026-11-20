using ElOferton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElOferton.Data.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable(table => table.HasCheckConstraint("CK_Clientes_TipoDocumento", "[TipoDocumento] IN (1, 2, 3)"));
        builder.HasKey(c => c.Id);
        builder.Property(c => c.TipoDocumento).HasConversion<int>();
        builder.Property(c => c.NumeroDocumento).HasMaxLength(20).IsUnicode(false);
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Telefono).HasMaxLength(20).IsUnicode(false);
        builder.Property(c => c.Email).HasMaxLength(120);
        builder.Property(c => c.Direccion).HasMaxLength(250);
        builder.Property(c => c.FechaRegistro).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(c => c.Activo).HasDefaultValue(true);

        builder.HasIndex(c => c.NumeroDocumento).IsUnique().HasFilter("[NumeroDocumento] IS NOT NULL");
        builder.HasIndex(c => c.Nombre);
        builder.HasIndex(c => c.Activo);
    }
}
