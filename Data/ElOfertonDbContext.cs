using ElOferton.Models.Entities;
using ElOferton.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Data;

public class ElOfertonDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ElOfertonDbContext(DbContextOptions<ElOfertonDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Marca> Marcas => Set<Marca>();

    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<Inventario> Inventarios => Set<Inventario>();

    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();

    public DbSet<PuntoVenta> PuntosVenta => Set<PuntoVenta>();

    public DbSet<Venta> Ventas => Set<Venta>();

    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElOfertonDbContext).Assembly);
    }
}
