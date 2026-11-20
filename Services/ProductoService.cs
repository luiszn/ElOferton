using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Entities;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class ProductoService(ElOfertonDbContext db) : IProductoService
{
    public async Task<ProductoListViewModel> ObtenerListaAsync(string? buscar, int? categoriaId, int? marcaId, bool? soloActivos)
    {
        var inventario = db.Inventarios
            .GroupBy(i => i.ProductoId)
            .Select(g => new { ProductoId = g.Key, Total = g.Sum(i => i.Cantidad) });

        var query = from p in db.Productos.AsNoTracking()
                    join c in db.Categorias on p.CategoriaId equals c.Id
                    join m in db.Marcas on p.MarcaId equals m.Id
                    join inv in inventario on p.Id equals inv.ProductoId into existencias
                    from inv in existencias.DefaultIfEmpty()
                    select new ProductoItemViewModel
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        CategoriaId = p.CategoriaId,
                        Categoria = c.Nombre,
                        MarcaId = p.MarcaId,
                        Marca = m.Nombre,
                        PrecioCompra = p.PrecioCompra,
                        PrecioVenta = p.PrecioVenta,
                        StockTotal = inv == null ? 0 : inv.Total,
                        StockMinimo = p.StockMinimo,
                        Activo = p.Activo,
                        ImagenUrl = p.ImagenUrl
                    };

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();
            query = query.Where(p => p.Codigo.Contains(termino) || p.Nombre.Contains(termino));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (marcaId.HasValue)
        {
            query = query.Where(p => p.MarcaId == marcaId.Value);
        }

        if (soloActivos.HasValue)
        {
            query = query.Where(p => p.Activo == soloActivos.Value);
        }

        var productos = await query.OrderBy(p => p.Nombre).ToListAsync();

        return new ProductoListViewModel
        {
            Productos = productos,
            Buscar = buscar,
            CategoriaId = categoriaId,
            MarcaId = marcaId,
            SoloActivos = soloActivos,
            Categorias = await ServicioAyuda.CategoriasAsync(db, false),
            Marcas = await ServicioAyuda.MarcasAsync(db, false)
        };
    }

    public async Task<ProductoFormViewModel?> ObtenerFormularioAsync(int? id)
    {
        var modelo = new ProductoFormViewModel
        {
            Categorias = await ServicioAyuda.CategoriasAsync(db),
            Marcas = await ServicioAyuda.MarcasAsync(db)
        };

        if (!id.HasValue)
        {
            return modelo;
        }

        var producto = await db.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id.Value);
        if (producto is null)
        {
            return null;
        }

        modelo.Id = producto.Id;
        modelo.Codigo = producto.Codigo;
        modelo.Nombre = producto.Nombre;
        modelo.Descripcion = producto.Descripcion;
        modelo.CategoriaId = producto.CategoriaId;
        modelo.MarcaId = producto.MarcaId;
        modelo.PrecioCompra = producto.PrecioCompra;
        modelo.PrecioVenta = producto.PrecioVenta;
        modelo.StockMinimo = producto.StockMinimo;
        modelo.Activo = producto.Activo;
        modelo.ImagenUrl = producto.ImagenUrl;
        return modelo;
    }

    public async Task<ProductoDetalleViewModel?> ObtenerDetalleAsync(int id)
    {
        var producto = await db.Productos.AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductoItemViewModel
            {
                Id = p.Id,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Categoria = p.Categoria.Nombre,
                Marca = p.Marca.Nombre,
                PrecioCompra = p.PrecioCompra,
                PrecioVenta = p.PrecioVenta,
                StockMinimo = p.StockMinimo,
                Activo = p.Activo,
                ImagenUrl = p.ImagenUrl,
                StockTotal = p.Inventarios.Sum(i => i.Cantidad)
            })
            .FirstOrDefaultAsync();

        if (producto is null)
        {
            return null;
        }

        var stocks = await (from pv in db.PuntosVenta.AsNoTracking()
                            join i in db.Inventarios on pv.Id equals i.PuntoVentaId into existencias
                            from i in existencias.DefaultIfEmpty()
                            where i == null || i.ProductoId == id
                            select new StockAmbienteViewModel
                            {
                                PuntoVentaId = pv.Id,
                                Ambiente = pv.Nombre,
                                Tipo = pv.Tipo.ToString(),
                                Cantidad = i == null ? 0 : i.Cantidad
                            })
            .OrderBy(s => s.PuntoVentaId)
            .ToListAsync();

        return new ProductoDetalleViewModel { Producto = producto, Stocks = stocks };
    }

    public async Task<ResultadoOperacion> GuardarAsync(ProductoFormViewModel model)
    {
        var codigo = model.Codigo.Trim().ToUpperInvariant();
        if (await db.Productos.AnyAsync(p => p.Codigo == codigo && p.Id != model.Id))
        {
            return ResultadoOperacion.Error("Ya existe un producto con ese código.");
        }

        if (!await db.Categorias.AnyAsync(c => c.Id == model.CategoriaId && c.Activo))
        {
            return ResultadoOperacion.Error("La categoría seleccionada no está activa.");
        }

        if (!await db.Marcas.AnyAsync(m => m.Id == model.MarcaId && m.Activo))
        {
            return ResultadoOperacion.Error("La marca seleccionada no está activa.");
        }

        if (model.PrecioVenta < 0 || model.PrecioCompra < 0)
        {
            return ResultadoOperacion.Error("Los precios no pueden ser negativos.");
        }

        await using var transaccion = await db.Database.BeginTransactionAsync();

        try
        {
            Producto producto;
            if (model.Id.HasValue)
            {
                var existente = await db.Productos.FirstOrDefaultAsync(p => p.Id == model.Id.Value);
                if (existente is null)
                {
                    return ResultadoOperacion.Error("El producto no existe.");
                }

                producto = existente;
            }
            else
            {
                producto = new Producto();
                db.Productos.Add(producto);
            }

            producto.Codigo = codigo;
            producto.Nombre = model.Nombre.Trim();
            producto.Descripcion = model.Descripcion?.Trim();
            producto.CategoriaId = model.CategoriaId;
            producto.MarcaId = model.MarcaId;
            producto.PrecioCompra = ServicioAyuda.Redondear(model.PrecioCompra);
            producto.PrecioVenta = ServicioAyuda.Redondear(model.PrecioVenta);
            producto.StockMinimo = model.StockMinimo;
            producto.Activo = model.Activo;
            producto.ImagenUrl = model.ImagenUrl?.Trim();

            await db.SaveChangesAsync();

            if (!model.Id.HasValue)
            {
                var puntos = await db.PuntosVenta.Where(p => p.Activo).Select(p => p.Id).ToListAsync();
                db.Inventarios.AddRange(puntos.Select(p => new Inventario
                {
                    ProductoId = producto.Id,
                    PuntoVentaId = p,
                    Cantidad = 0
                }));
                await db.SaveChangesAsync();
            }

            await transaccion.CommitAsync();
            return ResultadoOperacion.Ok("Producto guardado correctamente.", producto.Id);
        }
        catch (DbUpdateConcurrencyException)
        {
            return ResultadoOperacion.Error("El producto fue modificado por otro usuario. Recargue la página.");
        }
    }

    public async Task<ResultadoOperacion> AlternarEstadoAsync(int id)
    {
        var producto = await db.Productos.FirstOrDefaultAsync(p => p.Id == id);
        if (producto is null)
        {
            return ResultadoOperacion.Error("El producto no existe.");
        }

        producto.Activo = !producto.Activo;
        await db.SaveChangesAsync();
        return ResultadoOperacion.Ok(producto.Activo ? "Producto activado." : "Producto desactivado.");
    }

    public async Task<List<PosProductoResultado>> BuscarParaPosAsync(string? termino, int puntoVentaId)
    {
        var query = from p in db.Productos.AsNoTracking()
                    join i in db.Inventarios.AsNoTracking() on p.Id equals i.ProductoId
                    join c in db.Categorias.AsNoTracking() on p.CategoriaId equals c.Id
                    join m in db.Marcas.AsNoTracking() on p.MarcaId equals m.Id
                    where p.Activo && i.PuntoVentaId == puntoVentaId && i.Cantidad > 0
                    select new PosProductoResultado
                    {
                        ProductoId = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Precio = p.PrecioVenta,
                        Stock = i.Cantidad,
                        Categoria = c.Nombre,
                        Marca = m.Nombre
                    };

        if (!string.IsNullOrWhiteSpace(termino))
        {
            var texto = termino.Trim();
            query = query.Where(p => p.Codigo.Contains(texto)
                                     || p.Nombre.Contains(texto)
                                     || p.Categoria.Contains(texto)
                                     || p.Marca.Contains(texto));
        }

        return await query.OrderBy(p => p.Nombre).Take(20).ToListAsync();
    }
}
