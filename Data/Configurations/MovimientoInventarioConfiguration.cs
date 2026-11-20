using ElOferton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElOferton.Data.Configurations;

public sealed class MovimientoInventarioConfiguration : IEntityTypeConfiguration<MovimientoInventario>
{
    public void Configure(EntityTypeBuilder<MovimientoInventario> builder)
    {
        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Movimientos_Cantidad", "[Cantidad] <> 0");
            table.HasCheckConstraint("CK_Movimientos_Stock", "[StockAnterior] >= 0 AND [StockPosterior] >= 0");
            table.HasCheckConstraint("CK_Movimientos_Tipo", "[Tipo] IN (1, 2, 3, 4, 5, 6)");
        });
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Fecha).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Property(m => m.Tipo).HasConversion<int>();
        builder.Property(m => m.Motivo).IsRequired().HasMaxLength(250);

        builder.HasIndex(m => new { m.ProductoId, m.Fecha });
        builder.HasIndex(m => new { m.PuntoVentaId, m.Fecha });
        builder.HasIndex(m => m.VentaId);
        builder.HasIndex(m => m.UsuarioId);

        builder.HasOne(m => m.Producto)
            .WithMany(p => p.Movimientos)
            .HasForeignKey(m => m.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.PuntoVenta)
            .WithMany(p => p.MovimientosOrigen)
            .HasForeignKey(m => m.PuntoVentaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.PuntoVentaDestino)
            .WithMany(p => p.MovimientosDestino)
            .HasForeignKey(m => m.PuntoVentaDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Venta)
            .WithMany()
            .HasForeignKey(m => m.VentaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
