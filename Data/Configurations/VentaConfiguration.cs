using ElOferton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElOferton.Data.Configurations;

public sealed class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Ventas_Montos", "[Subtotal] >= 0 AND [Descuento] >= 0 AND [Total] >= 0");
            table.HasCheckConstraint("CK_Ventas_Descuento", "[PorcentajeDescuento] >= 0 AND [PorcentajeDescuento] <= 10");
            table.HasCheckConstraint("CK_Ventas_Cliente", "[ClienteId] IS NOT NULL OR [ClienteOcasional] IS NOT NULL");
            table.HasCheckConstraint("CK_Ventas_Pago", "[MontoRecibido] IS NULL OR [MontoRecibido] >= [Total]");
            table.HasCheckConstraint("CK_Ventas_Estado", "[Estado] IN (1, 2)");
        });
        builder.HasKey(v => v.Id);
        builder.Property(v => v.NumeroComprobante).IsRequired().HasMaxLength(20).IsUnicode(false);
        builder.Property(v => v.Fecha).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(v => v.ClienteOcasional).HasMaxLength(150);
        builder.Property(v => v.Subtotal).HasColumnType("decimal(18,2)");
        builder.Property(v => v.PorcentajeDescuento).HasColumnType("decimal(5,2)");
        builder.Property(v => v.Descuento).HasColumnType("decimal(18,2)");
        builder.Property(v => v.Total).HasColumnType("decimal(18,2)");
        builder.Property(v => v.MontoRecibido).HasColumnType("decimal(18,2)");
        builder.Property(v => v.Vuelto).HasColumnType("decimal(18,2)");
        builder.Property(v => v.Observaciones).HasMaxLength(300);
        builder.Property(v => v.Estado).HasConversion<int>();

        builder.HasIndex(v => v.NumeroComprobante).IsUnique();
        builder.HasIndex(v => v.Fecha);
        builder.HasIndex(v => new { v.PuntoVentaId, v.Fecha });
        builder.HasIndex(v => new { v.UsuarioId, v.Fecha });
        builder.HasIndex(v => v.ClienteId);
        builder.HasIndex(v => v.MetodoPagoId);

        builder.HasOne(v => v.Cliente)
            .WithMany(c => c.Ventas)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.PuntoVenta)
            .WithMany(p => p.Ventas)
            .HasForeignKey(v => v.PuntoVentaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Usuario)
            .WithMany()
            .HasForeignKey(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.MetodoPago)
            .WithMany(m => m.Ventas)
            .HasForeignKey(v => v.MetodoPagoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_DetallesVenta_Cantidad", "[Cantidad] > 0");
            table.HasCheckConstraint("CK_DetallesVenta_Montos", "[PrecioUnitario] >= 0 AND [Subtotal] >= 0");
        });
        builder.HasKey(d => d.Id);
        builder.Property(d => d.CodigoProducto).IsRequired().HasMaxLength(20).IsUnicode(false);
        builder.Property(d => d.NombreProducto).IsRequired().HasMaxLength(150);
        builder.Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
        builder.Property(d => d.Subtotal).HasColumnType("decimal(18,2)");

        builder.HasIndex(d => d.VentaId);
        builder.HasIndex(d => d.ProductoId);

        builder.HasOne(d => d.Venta)
            .WithMany(v => v.Detalles)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Producto)
            .WithMany(p => p.DetallesVenta)
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
