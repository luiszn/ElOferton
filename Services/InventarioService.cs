using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Entities;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class InventarioService(ElOfertonDbContext db) : IInventarioService
{
    public async Task<InventarioFiltroViewModel> ObtenerInventarioAsync(string? buscar, int? puntoVentaId, bool? soloStockBajo)
    {
        var query = from p in db.Productos.AsNoTracking().Where(p => p.Activo)
                    from pv in db.PuntosVenta.AsNoTracking().Where(pv => pv.Activo)
                    join i in db.Inventarios.AsNoTracking()
                        on new { ProductoId = p.Id, PuntoVentaId = pv.Id }
                        equals new { ProductoId = i.ProductoId, PuntoVentaId = i.PuntoVentaId } into existencias
                    from i in existencias.DefaultIfEmpty()
                    select new InventarioItemViewModel
                    {
                        ProductoId = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        PuntoVentaId = pv.Id,
                        Ambiente = pv.Nombre,
                        Cantidad = i == null ? 0 : i.Cantidad,
                        StockBajo = (i == null ? 0 : i.Cantidad) <= p.StockMinimo
                    };

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(x => x.Codigo.Contains(texto) || x.Nombre.Contains(texto));
        }

        if (puntoVentaId.HasValue)
        {
            query = query.Where(x => x.PuntoVentaId == puntoVentaId.Value);
        }

        if (soloStockBajo == true)
        {
            query = query.Where(x => x.StockBajo);
        }

        return new InventarioFiltroViewModel
        {
            Buscar = buscar,
            PuntoVentaId = puntoVentaId,
            SoloStockBajo = soloStockBajo,
            Items = await query.OrderBy(x => x.Ambiente).ThenBy(x => x.Nombre).ToListAsync(),
            PuntosVenta = await ServicioAyuda.PuntosVentaAsync(db)
        };
    }

    public async Task<MovimientoFormViewModel> ObtenerFormularioMovimientoAsync()
    {
        return new MovimientoFormViewModel
        {
            Productos = await db.Productos.AsNoTracking()
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem($"{p.Codigo} - {p.Nombre}", p.Id.ToString()))
                .ToArrayAsync(),
            PuntosVenta = await ServicioAyuda.PuntosVentaAsync(db)
        };
    }

    public async Task<MovimientoFiltroViewModel> ObtenerMovimientosAsync(MovimientoFiltroViewModel filtro)
    {
        var query = from m in db.MovimientosInventario.AsNoTracking()
                    join p in db.Productos.AsNoTracking() on m.ProductoId equals p.Id
                    join pv in db.PuntosVenta.AsNoTracking() on m.PuntoVentaId equals pv.Id
                    join u in db.Users.AsNoTracking() on m.UsuarioId equals u.Id
                    select new MovimientoItemViewModel
                    {
                        Id = m.Id,
                        Fecha = m.Fecha,
                        ProductoId = m.ProductoId,
                        Producto = p.Nombre,
                        TipoMovimiento = m.Tipo,
                        Tipo = m.Tipo.ToString(),
                        Cantidad = m.Cantidad,
                        PuntoVentaId = m.PuntoVentaId,
                        Ambiente = pv.Nombre,
                        AmbienteDestino = m.PuntoVentaDestinoId.HasValue
                            ? db.PuntosVenta.AsNoTracking().Where(x => x.Id == m.PuntoVentaDestinoId.Value).Select(x => x.Nombre).FirstOrDefault()
                            : null,
                        Usuario = u.NombreCompleto,
                        Motivo = m.Motivo,
                        StockAnterior = m.StockAnterior,
                        StockPosterior = m.StockPosterior
                    };

        if (filtro.FechaInicio.HasValue)
        {
            query = query.Where(m => m.Fecha >= filtro.FechaInicio.Value.Date);
        }

        if (filtro.FechaFin.HasValue)
        {
            var fin = filtro.FechaFin.Value.Date.AddDays(1);
            query = query.Where(m => m.Fecha < fin);
        }

        if (filtro.ProductoId.HasValue)
        {
            query = query.Where(m => m.ProductoId == filtro.ProductoId.Value);
        }

        if (filtro.PuntoVentaId.HasValue)
        {
            query = query.Where(m => m.PuntoVentaId == filtro.PuntoVentaId.Value);
        }

        if (filtro.Tipo.HasValue)
        {
            query = query.Where(m => m.TipoMovimiento == filtro.Tipo.Value);
        }

        filtro.Items = await query.OrderByDescending(m => m.Fecha).Take(500).ToListAsync();
        filtro.Productos = await db.Productos.AsNoTracking()
            .OrderBy(p => p.Nombre)
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(p.Nombre, p.Id.ToString()))
            .ToArrayAsync();
        filtro.PuntosVenta = await ServicioAyuda.PuntosVentaAsync(db);
        return filtro;
    }

    public async Task<int> ObtenerStockAsync(int productoId, int puntoVentaId)
    {
        return await db.Inventarios.AsNoTracking()
            .Where(i => i.ProductoId == productoId && i.PuntoVentaId == puntoVentaId)
            .Select(i => i.Cantidad)
            .FirstOrDefaultAsync();
    }

    public async Task<ResultadoOperacion> RegistrarMovimientoAsync(MovimientoFormViewModel model, int usuarioId)
    {
        var cantidad = model.Cantidad;
        if (cantidad == 0)
        {
            return ResultadoOperacion.Error("La cantidad debe ser diferente de cero.");
        }

        if (!await db.Productos.AnyAsync(p => p.Id == model.ProductoId && p.Activo))
        {
            return ResultadoOperacion.Error("El producto seleccionado no está activo.");
        }

        if (!await db.PuntosVenta.AnyAsync(p => p.Id == model.PuntoVentaId && p.Activo))
        {
            return ResultadoOperacion.Error("El ambiente seleccionado no está activo.");
        }

        var motivo = model.Motivo.Trim();
        if (motivo.Length == 0)
        {
            return ResultadoOperacion.Error("Debe indicar el motivo del movimiento.");
        }

        var esTraspaso = model.Tipo == TipoMovimiento.TraspasoSalida;
        if (esTraspaso && (!model.PuntoVentaDestinoId.HasValue || model.PuntoVentaDestinoId == model.PuntoVentaId))
        {
            return ResultadoOperacion.Error("Seleccione un ambiente destino diferente al origen.");
        }

        if (esTraspaso && cantidad < 0)
        {
            return ResultadoOperacion.Error("La cantidad del traspaso debe ser positiva.");
        }

        await using var transaccion = await db.Database.BeginTransactionAsync();

        try
        {
            await AsegurarInventarioAsync(model.ProductoId, model.PuntoVentaId);

            var anterior = await ObtenerStockAsync(model.ProductoId, model.PuntoVentaId);
            var delta = model.Tipo switch
            {
                TipoMovimiento.Entrada => Math.Abs(cantidad),
                TipoMovimiento.Salida => -Math.Abs(cantidad),
                TipoMovimiento.Ajuste => cantidad,
                TipoMovimiento.TraspasoSalida => -Math.Abs(cantidad),
                _ => 0
            };

            if (delta == 0)
            {
                return ResultadoOperacion.Error("El tipo de movimiento no es válido.");
            }

            var afectados = await db.Inventarios
                .Where(i => i.ProductoId == model.ProductoId
                            && i.PuntoVentaId == model.PuntoVentaId
                            && i.Cantidad + delta >= 0)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.Cantidad, i => i.Cantidad + delta));

            if (afectados == 0)
            {
                return ResultadoOperacion.Error("No hay stock suficiente para realizar el movimiento.");
            }

            var posterior = anterior + delta;
            db.MovimientosInventario.Add(new MovimientoInventario
            {
                Fecha = Reloj.Ahora,
                ProductoId = model.ProductoId,
                Tipo = model.Tipo == TipoMovimiento.TraspasoSalida ? TipoMovimiento.TraspasoSalida : model.Tipo,
                Cantidad = delta,
                PuntoVentaId = model.PuntoVentaId,
                PuntoVentaDestinoId = esTraspaso ? model.PuntoVentaDestinoId : null,
                UsuarioId = usuarioId,
                Motivo = motivo,
                StockAnterior = anterior,
                StockPosterior = posterior
            });

            if (esTraspaso)
            {
                var destinoId = model.PuntoVentaDestinoId!.Value;
                await AsegurarInventarioAsync(model.ProductoId, destinoId);
                var anteriorDestino = await ObtenerStockAsync(model.ProductoId, destinoId);

                await db.Inventarios
                    .Where(i => i.ProductoId == model.ProductoId && i.PuntoVentaId == destinoId)
                    .ExecuteUpdateAsync(s => s.SetProperty(i => i.Cantidad, i => i.Cantidad + Math.Abs(cantidad)));

                db.MovimientosInventario.Add(new MovimientoInventario
                {
                    Fecha = Reloj.Ahora,
                    ProductoId = model.ProductoId,
                    Tipo = TipoMovimiento.TraspasoEntrada,
                    Cantidad = Math.Abs(cantidad),
                    PuntoVentaId = destinoId,
                    UsuarioId = usuarioId,
                    Motivo = motivo,
                    StockAnterior = anteriorDestino,
                    StockPosterior = anteriorDestino + Math.Abs(cantidad)
                });
            }

            await db.SaveChangesAsync();
            await transaccion.CommitAsync();
            return ResultadoOperacion.Ok("Movimiento registrado correctamente.");
        }
        catch (DbUpdateException)
        {
            return ResultadoOperacion.Error("No se pudo registrar el movimiento. Intente nuevamente.");
        }
    }

    private async Task AsegurarInventarioAsync(int productoId, int puntoVentaId)
    {
        if (!await db.Inventarios.AnyAsync(i => i.ProductoId == productoId && i.PuntoVentaId == puntoVentaId))
        {
            db.Inventarios.Add(new Inventario
            {
                ProductoId = productoId,
                PuntoVentaId = puntoVentaId,
                Cantidad = 0
            });
            await db.SaveChangesAsync();
        }
    }
}
