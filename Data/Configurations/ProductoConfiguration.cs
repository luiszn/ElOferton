using ElOferton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElOferton.Data.Configurations;

public sealed class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Productos_Precios", "[PrecioCompra] >= 0 AND [PrecioVenta] >= 0");
            table.HasCheckConstraint("CK_Productos_StockMinimo", "[StockMinimo] >= 0");
        });
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Codigo).IsRequired().HasMaxLength(20).IsUnicode(false);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Descripcion).HasMaxLength(500);
        builder.Property(p => p.PrecioCompra).HasColumnType("decimal(18,2)");
        builder.Property(p => p.PrecioVenta).HasColumnType("decimal(18,2)");
        builder.Property(p => p.ImagenUrl).HasMaxLength(250);
        builder.Property(p => p.Activo).HasDefaultValue(true);
        builder.Property(p => p.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(p => p.Codigo).IsUnique();
        builder.HasIndex(p => p.Nombre);
        builder.HasIndex(p => new { p.CategoriaId, p.MarcaId });
        builder.HasIndex(p => p.Activo);

        builder.HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Marca)
            .WithMany(m => m.Productos)
            .HasForeignKey(p => p.MarcaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class InventarioConfiguration : IEntityTypeConfiguration<Inventario>
{
    public void Configure(EntityTypeBuilder<Inventario> builder)
    {
        builder.ToTable(table => table.HasCheckConstraint("CK_Inventarios_Cantidad", "[Cantidad] >= 0"));
        builder.HasKey(i => new { i.ProductoId, i.PuntoVentaId });
        builder.Property(i => i.Cantidad).HasDefaultValue(0);
        builder.Property(i => i.RowVersion).IsRowVersion().IsConcurrencyToken();

        builder.HasIndex(i => i.PuntoVentaId);

        builder.HasOne(i => i.Producto)
            .WithMany(p => p.Inventarios)
            .HasForeignKey(i => i.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.PuntoVenta)
            .WithMany(p => p.Inventarios)
            .HasForeignKey(i => i.PuntoVentaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
