using ElOferton.Constants;
using ElOferton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElOferton.Data.Configurations;

public sealed class PuntoVentaConfiguration : IEntityTypeConfiguration<PuntoVenta>
{
    public void Configure(EntityTypeBuilder<PuntoVenta> builder)
    {
        builder.ToTable("PuntosVenta");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Codigo).IsRequired().HasMaxLength(10).IsUnicode(false);
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Direccion).HasMaxLength(200);
        builder.Property(p => p.Tipo).HasConversion<int>();
        builder.Property(p => p.Activo).HasDefaultValue(true);
        builder.HasIndex(p => p.Codigo).IsUnique();
        builder.HasIndex(p => p.Tipo);

        builder.HasData(
            new PuntoVenta { Id = 1, Codigo = "PV-01", Nombre = "Punto de venta 1", Tipo = TipoAmbiente.PuntoVenta, Direccion = "Cajamarca", Activo = true },
            new PuntoVenta { Id = 2, Codigo = "PV-02", Nombre = "Punto de venta 2", Tipo = TipoAmbiente.PuntoVenta, Direccion = "Cajamarca", Activo = true },
            new PuntoVenta { Id = 3, Codigo = "PV-03", Nombre = "Punto de venta 3", Tipo = TipoAmbiente.PuntoVenta, Direccion = "Cajamarca", Activo = true },
            new PuntoVenta { Id = 4, Codigo = "PV-04", Nombre = "Punto de venta 4", Tipo = TipoAmbiente.PuntoVenta, Direccion = "Cajamarca", Activo = true },
            new PuntoVenta { Id = 5, Codigo = "ADM-01", Nombre = "Administración", Tipo = TipoAmbiente.Administracion, Direccion = "Cajamarca", Activo = true },
            new PuntoVenta { Id = 6, Codigo = "ALM-01", Nombre = "Almacén central", Tipo = TipoAmbiente.AlmacenCentral, Direccion = "Cajamarca", Activo = true });
    }
}

public sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(80);
        builder.Property(c => c.Descripcion).HasMaxLength(300);
        builder.Property(c => c.Activo).HasDefaultValue(true);
        builder.HasIndex(c => c.Nombre).IsUnique();

        builder.HasData(
            new Categoria { Id = 1, Nombre = "Maquillaje", Descripcion = "Productos de maquillaje facial y labial", Activo = true },
            new Categoria { Id = 2, Nombre = "Cosmética", Descripcion = "Cuidado facial y corporal", Activo = true },
            new Categoria { Id = 3, Nombre = "Peluquería", Descripcion = "Productos para cabello", Activo = true },
            new Categoria { Id = 4, Nombre = "Barbería", Descripcion = "Productos para barba y afeitado", Activo = true },
            new Categoria { Id = 5, Nombre = "Cuidado personal", Descripcion = "Higiene y cuidado diario", Activo = true },
            new Categoria { Id = 6, Nombre = "Accesorios de belleza", Descripcion = "Brochas, esponjas y accesorios", Activo = true });
    }
}

public sealed class MarcaConfiguration : IEntityTypeConfiguration<Marca>
{
    public void Configure(EntityTypeBuilder<Marca> builder)
    {
        builder.ToTable("Marcas");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(80);
        builder.Property(m => m.Descripcion).HasMaxLength(300);
        builder.Property(m => m.Activo).HasDefaultValue(true);
        builder.HasIndex(m => m.Nombre).IsUnique();

        builder.HasData(
            new Marca { Id = 1, Nombre = "Maybelline", Descripcion = "Maquillaje", Activo = true },
            new Marca { Id = 2, Nombre = "L'Oréal Paris", Descripcion = "Cosmética y peluquería", Activo = true },
            new Marca { Id = 3, Nombre = "Nivea", Descripcion = "Cuidado personal", Activo = true },
            new Marca { Id = 4, Nombre = "Gillette", Descripcion = "Barbería", Activo = true },
            new Marca { Id = 5, Nombre = "Dove", Descripcion = "Cuidado personal", Activo = true },
            new Marca { Id = 6, Nombre = "Schwarzkopf", Descripcion = "Peluquería", Activo = true });
    }
}

public sealed class MetodoPagoConfiguration : IEntityTypeConfiguration<MetodoPago>
{
    public void Configure(EntityTypeBuilder<MetodoPago> builder)
    {
        builder.ToTable("MetodosPago");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Nombre).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Activo).HasDefaultValue(true);
        builder.HasIndex(m => m.Nombre).IsUnique();

        builder.HasData(
            new MetodoPago { Id = 1, Nombre = "Efectivo", RequiereMontoRecibido = true, Activo = true },
            new MetodoPago { Id = 2, Nombre = "Yape", RequiereMontoRecibido = false, Activo = true },
            new MetodoPago { Id = 3, Nombre = "Plin", RequiereMontoRecibido = false, Activo = true },
            new MetodoPago { Id = 4, Nombre = "Tarjeta", RequiereMontoRecibido = false, Activo = true },
            new MetodoPago { Id = 5, Nombre = "Otros", RequiereMontoRecibido = false, Activo = true });
    }
}
