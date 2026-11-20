using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Entities;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class VentaService(ElOfertonDbContext db, IProductoService productoService) : IVentaService
{
    public async Task<PosViewModel> PrepararPosAsync(int puntoVentaId, bool esAdministrador)
    {
        var puntos = await ServicioAyuda.PuntosVentaAsync(db);
        var metodos = await ServicioAyuda.MetodosPagoAsync(db);
        var puntoSeleccionado = puntos.Any(p => p.Value == puntoVentaId.ToString())
            ? puntoVentaId
            : puntos.Length > 0 ? int.Parse(puntos[0].Value!) : 0;
        var metodoSeleccionado = metodos.Length > 0 ? int.Parse(metodos[0].Value!) : 0;

        return new PosViewModel
        {
            PuntoVentaId = puntoSeleccionado,
            MetodoPagoId = metodoSeleccionado,
            Clientes = await ServicioAyuda.ClientesAsync(db),
            MetodosPago = metodos,
            PuntosVenta = puntos,
            EsAdministrador = esAdministrador
        };
    }

    public Task<List<PosProductoResultado>> BuscarProductosAsync(string? termino, int puntoVentaId) =>
        productoService.BuscarParaPosAsync(termino, puntoVentaId);

    public async Task<ResultadoVenta> RegistrarAsync(PosViewModel model, int usuarioId, bool esAdministrador)
    {
        if (model.Items is null || model.Items.Count == 0)
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "Agregue al menos un producto al carrito." };
        }

        if (model.Items.Any(i => i.Cantidad <= 0))
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "Las cantidades deben ser mayores a cero." };
        }

        if (!await db.PuntosVenta.AnyAsync(p => p.Id == model.PuntoVentaId && p.Activo))
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "El punto de venta seleccionado no es válido." };
        }

        var metodo = await db.MetodosPago.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == model.MetodoPagoId && m.Activo);
        if (metodo is null)
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "El método de pago seleccionado no es válido." };
        }

        if (model.PorcentajeDescuento < 0 || model.PorcentajeDescuento > 10)
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "El descuento debe estar entre 0% y 10%." };
        }

        if (!esAdministrador && model.PorcentajeDescuento > 0)
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "Solo el Administrador puede aplicar descuentos." };
        }

        int? clienteId = null;
        string? clienteOcasional = null;
        if (model.ClienteId.HasValue)
        {
            if (!await db.Clientes.AnyAsync(c => c.Id == model.ClienteId.Value && c.Activo))
            {
                return new ResultadoVenta { Exitoso = false, Mensaje = "El cliente seleccionado no está activo." };
            }

            clienteId = model.ClienteId.Value;
        }
        else if (!string.IsNullOrWhiteSpace(model.ClienteOcasional))
        {
            clienteOcasional = model.ClienteOcasional.Trim();
        }
        else
        {
            return new ResultadoVenta { Exitoso = false, Mensaje = "Seleccione un cliente o indique el nombre del cliente ocasional." };
        }

        var cantidades = model.Items
            .GroupBy(i => i.ProductoId)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
            .ToList();

        var productoIds = cantidades.Select(c => c.ProductoId).ToList();
        var productos = await db.Productos.AsNoTracking()
            .Where(p => productoIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var lineas = new List<LineaVenta>();
        foreach (var item in cantidades)
        {
            if (!productos.TryGetValue(item.ProductoId, out var producto))
            {
                return new ResultadoVenta { Exitoso = false, Mensaje = "Uno de los productos no existe." };
            }

            if (!producto.Activo)
            {
                return new ResultadoVenta { Exitoso = false, Mensaje = $"El producto {producto.Nombre} está desactivado." };
            }

            lineas.Add(new LineaVenta(
                producto,
                item.Cantidad,
                ServicioAyuda.Redondear(producto.PrecioVenta * item.Cantidad)));
        }

        var subtotal = lineas.Sum(l => l.Subtotal);
        var descuento = ServicioAyuda.Redondear(subtotal * model.PorcentajeDescuento / 100m);
        var total = subtotal - descuento;
        var montoRecibido = metodo.RequiereMontoRecibido ? model.MontoRecibido : null;
        decimal? vuelto = null;

        if (metodo.RequiereMontoRecibido)
        {
            if (!montoRecibido.HasValue || montoRecibido.Value < total)
            {
                return new ResultadoVenta { Exitoso = false, Mensaje = "El monto recibido debe ser mayor o igual al total." };
            }

            vuelto = ServicioAyuda.Redondear(montoRecibido.Value - total);
        }

        var estrategia = db.Database.CreateExecutionStrategy();
        return await estrategia.ExecuteAsync(async () =>
        {
            await using var transaccion = await db.Database.BeginTransactionAsync();

            try
            {
                foreach (var linea in lineas)
                {
                    var productoId = linea.Producto.Id;
                    var cantidad = linea.Cantidad;
                    var puntoVentaId = model.PuntoVentaId;
                    var afectados = await db.Inventarios
                        .Where(i => i.ProductoId == productoId
                                    && i.PuntoVentaId == puntoVentaId
                                    && i.Cantidad >= cantidad)
                        .ExecuteUpdateAsync(s => s.SetProperty(i => i.Cantidad, i => i.Cantidad - cantidad));

                    if (afectados == 0)
                    {
                        return new ResultadoVenta
                        {
                            Exitoso = false,
                            Mensaje = $"Stock insuficiente para {linea.Producto.Nombre} en el punto de venta seleccionado."
                        };
                    }
                }

                var stocks = await db.Inventarios.AsNoTracking()
                    .Where(i => i.PuntoVentaId == model.PuntoVentaId && productoIds.Contains(i.ProductoId))
                    .ToDictionaryAsync(i => i.ProductoId, i => i.Cantidad);

                var numeroComprobante = $"V-{Reloj.Ahora:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
                var venta = new Venta
                {
                    NumeroComprobante = numeroComprobante,
                    Fecha = Reloj.Ahora,
                    ClienteId = clienteId,
                    ClienteOcasional = clienteOcasional,
                    PuntoVentaId = model.PuntoVentaId,
                    UsuarioId = usuarioId,
                    MetodoPagoId = metodo.Id,
                    Subtotal = subtotal,
                    PorcentajeDescuento = model.PorcentajeDescuento,
                    Descuento = descuento,
                    Total = total,
                    MontoRecibido = montoRecibido,
                    Vuelto = vuelto,
                    Estado = EstadoVenta.Completada,
                    Observaciones = model.Observaciones?.Trim()
                };

                db.Ventas.Add(venta);
                await db.SaveChangesAsync();

                foreach (var linea in lineas)
                {
                    db.DetallesVenta.Add(new DetalleVenta
                    {
                        VentaId = venta.Id,
                        ProductoId = linea.Producto.Id,
                        CodigoProducto = linea.Producto.Codigo,
                        NombreProducto = linea.Producto.Nombre,
                        Cantidad = linea.Cantidad,
                        PrecioUnitario = linea.Producto.PrecioVenta,
                        Subtotal = linea.Subtotal
                    });

                    var posterior = stocks.GetValueOrDefault(linea.Producto.Id);
                    var anterior = posterior + linea.Cantidad;
                    db.MovimientosInventario.Add(new MovimientoInventario
                    {
                        Fecha = Reloj.Ahora,
                        ProductoId = linea.Producto.Id,
                        Tipo = TipoMovimiento.Venta,
                        Cantidad = -linea.Cantidad,
                        PuntoVentaId = model.PuntoVentaId,
                        VentaId = venta.Id,
                        UsuarioId = usuarioId,
                        Motivo = $"Venta {numeroComprobante}",
                        StockAnterior = anterior,
                        StockPosterior = posterior
                    });
                }

                await db.SaveChangesAsync();
                await transaccion.CommitAsync();

                var guardada = await db.Ventas.AsNoTracking()
                    .Include(v => v.PuntoVenta)
                    .Include(v => v.Usuario)
                    .Include(v => v.MetodoPago)
                    .Include(v => v.Detalles)
                    .FirstAsync(v => v.Id == venta.Id);

                return new ResultadoVenta
                {
                    Exitoso = true,
                    Mensaje = "Venta registrada correctamente.",
                    Comprobante = MapComprobante(guardada)
                };
            }
            catch (DbUpdateException)
            {
                return new ResultadoVenta { Exitoso = false, Mensaje = "No se pudo registrar la venta. Intente nuevamente." };
            }
        });
    }

    public async Task<VentaFiltroViewModel> ObtenerVentasAsync(VentaFiltroViewModel filtro)
    {
        var query = from v in db.Ventas.AsNoTracking()
                    join c in db.Clientes.AsNoTracking() on v.ClienteId equals c.Id into clientes
                    from c in clientes.DefaultIfEmpty()
                    join pv in db.PuntosVenta.AsNoTracking() on v.PuntoVentaId equals pv.Id
                    join u in db.Users.AsNoTracking() on v.UsuarioId equals u.Id
                    join mp in db.MetodosPago.AsNoTracking() on v.MetodoPagoId equals mp.Id
                    select new VentaItemViewModel
                    {
                        Id = v.Id,
                        NumeroComprobante = v.NumeroComprobante,
                        Fecha = v.Fecha,
                        Cliente = c.Nombre ?? v.ClienteOcasional ?? "Cliente ocasional",
                        PuntoVenta = pv.Nombre,
                        Vendedor = u.NombreCompleto,
                        MetodoPago = mp.Nombre,
                        PuntoVentaId = v.PuntoVentaId,
                        UsuarioId = v.UsuarioId,
                        Total = v.Total,
                        Estado = v.Estado.ToString()
                    };

        if (filtro.FechaInicio.HasValue)
        {
            query = query.Where(v => v.Fecha >= filtro.FechaInicio.Value.Date);
        }

        if (filtro.FechaFin.HasValue)
        {
            var fin = filtro.FechaFin.Value.Date.AddDays(1);
            query = query.Where(v => v.Fecha < fin);
        }

        if (filtro.PuntoVentaId.HasValue)
        {
            query = query.Where(v => v.PuntoVentaId == filtro.PuntoVentaId.Value);
        }

        if (filtro.UsuarioId.HasValue)
        {
            query = query.Where(v => v.UsuarioId == filtro.UsuarioId.Value);
        }

        filtro.Ventas = await query.OrderByDescending(v => v.Fecha).Take(500).ToListAsync();
        filtro.PuntosVenta = await ServicioAyuda.PuntosVentaAsync(db);
        filtro.Vendedores = await ServicioAyuda.UsuariosAsync(db);
        return filtro;
    }

    public async Task<VentaDetalleViewModel?> ObtenerDetalleAsync(int id)
    {
        var venta = await db.Ventas.AsNoTracking()
            .Include(v => v.PuntoVenta)
            .Include(v => v.Usuario)
            .Include(v => v.MetodoPago)
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venta is null)
        {
            return null;
        }

        var cliente = venta.ClienteId.HasValue
            ? await db.Clientes.AsNoTracking().Where(c => c.Id == venta.ClienteId.Value).Select(c => c.Nombre).FirstOrDefaultAsync()
            : null;

        return new VentaDetalleViewModel
        {
            Venta = MapComprobante(venta, cliente ?? venta.ClienteOcasional ?? "Cliente ocasional"),
            Observaciones = venta.Observaciones
        };
    }

    private static VentaConfirmacionViewModel MapComprobante(Venta venta, string? cliente = null)
    {
        return new VentaConfirmacionViewModel
        {
            VentaId = venta.Id,
            NumeroComprobante = venta.NumeroComprobante,
            Fecha = venta.Fecha,
            Cliente = cliente ?? venta.Cliente?.Nombre ?? venta.ClienteOcasional ?? "Cliente ocasional",
            PuntoVenta = venta.PuntoVenta.Nombre,
            Vendedor = venta.Usuario.NombreCompleto,
            MetodoPago = venta.MetodoPago.Nombre,
            Subtotal = venta.Subtotal,
            Descuento = venta.Descuento,
            Total = venta.Total,
            MontoRecibido = venta.MontoRecibido,
            Vuelto = venta.Vuelto,
            Items = venta.Detalles.Select(d => new PosItemViewModel
            {
                ProductoId = d.ProductoId,
                Codigo = d.CodigoProducto,
                Nombre = d.NombreProducto,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.Subtotal
            }).ToList()
        };
    }

    private sealed record LineaVenta(Producto Producto, int Cantidad, decimal Subtotal);
}
