using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Entities;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class ClienteService(ElOfertonDbContext db) : IClienteService
{
    public async Task<ClienteListViewModel> ObtenerListaAsync(string? buscar)
    {
        var query = from c in db.Clientes.AsNoTracking()
                    select new ClienteItemViewModel
                    {
                        Id = c.Id,
                        TipoDocumento = c.TipoDocumento.ToString(),
                        NumeroDocumento = c.NumeroDocumento,
                        Nombre = c.Nombre,
                        Telefono = c.Telefono,
                        Email = c.Email,
                        Activo = c.Activo,
                        CantidadCompras = db.Ventas.AsNoTracking()
                            .Count(v => v.ClienteId == c.Id && v.Estado == EstadoVenta.Completada),
                        TotalCompras = db.Ventas.AsNoTracking()
                            .Where(v => v.ClienteId == c.Id && v.Estado == EstadoVenta.Completada)
                            .Sum(v => (decimal?)v.Total) ?? 0
                    };

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(c => c.Nombre.Contains(texto) || (c.NumeroDocumento != null && c.NumeroDocumento.Contains(texto)));
        }

        return new ClienteListViewModel
        {
            Clientes = await query.OrderBy(c => c.Nombre).ToListAsync(),
            Buscar = buscar
        };
    }

    public async Task<ClienteFormViewModel?> ObtenerFormularioAsync(int? id)
    {
        if (!id.HasValue)
        {
            return new ClienteFormViewModel();
        }

        var cliente = await db.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id.Value);
        if (cliente is null)
        {
            return null;
        }

        return new ClienteFormViewModel
        {
            Id = cliente.Id,
            TipoDocumento = cliente.TipoDocumento,
            NumeroDocumento = cliente.NumeroDocumento,
            Nombre = cliente.Nombre,
            Telefono = cliente.Telefono,
            Email = cliente.Email,
            Direccion = cliente.Direccion,
            Activo = cliente.Activo
        };
    }

    public async Task<ClienteDetalleViewModel?> ObtenerDetalleAsync(int id)
    {
        var cliente = await db.Clientes.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ClienteDetalleViewModel
            {
                Cliente = new ClienteItemViewModel
                {
                    Id = c.Id,
                    TipoDocumento = c.TipoDocumento.ToString(),
                    NumeroDocumento = c.NumeroDocumento,
                    Nombre = c.Nombre,
                    Telefono = c.Telefono,
                    Email = c.Email,
                    Activo = c.Activo
                },
                Direccion = c.Direccion
            })
            .FirstOrDefaultAsync();

        if (cliente is null)
        {
            return null;
        }

        cliente.Compras = await db.Ventas.AsNoTracking()
            .Where(v => v.ClienteId == id)
            .OrderByDescending(v => v.Fecha)
            .Select(v => new CompraClienteViewModel
            {
                VentaId = v.Id,
                NumeroComprobante = v.NumeroComprobante,
                Fecha = v.Fecha,
                PuntoVenta = v.PuntoVenta.Nombre,
                Total = v.Total
            })
            .ToListAsync();

        return cliente;
    }

    public async Task<ResultadoOperacion> GuardarAsync(ClienteFormViewModel model)
    {
        var documento = string.IsNullOrWhiteSpace(model.NumeroDocumento) ? null : model.NumeroDocumento.Trim();
        if (documento is not null && await db.Clientes.AnyAsync(c => c.NumeroDocumento == documento && c.Id != model.Id))
        {
            return ResultadoOperacion.Error("Ya existe un cliente con ese número de documento.");
        }

        if (model.TipoDocumento == TipoDocumento.DNI && documento is not null && documento.Length != 8)
        {
            return ResultadoOperacion.Error("El DNI debe tener 8 dígitos.");
        }

        if (model.TipoDocumento == TipoDocumento.RUC && documento is not null && documento.Length != 11)
        {
            return ResultadoOperacion.Error("El RUC debe tener 11 dígitos.");
        }

        Cliente cliente;
        if (model.Id.HasValue)
        {
            var existente = await db.Clientes.FirstOrDefaultAsync(c => c.Id == model.Id.Value);
            if (existente is null)
            {
                return ResultadoOperacion.Error("El cliente no existe.");
            }

            cliente = existente;
        }
        else
        {
            cliente = new Cliente();
            db.Clientes.Add(cliente);
        }

        cliente.TipoDocumento = model.TipoDocumento;
        cliente.NumeroDocumento = documento;
        cliente.Nombre = model.Nombre.Trim();
        cliente.Telefono = model.Telefono?.Trim();
        cliente.Email = model.Email?.Trim();
        cliente.Direccion = model.Direccion?.Trim();
        cliente.Activo = model.Activo;

        await db.SaveChangesAsync();
        return ResultadoOperacion.Ok("Cliente guardado correctamente.", cliente.Id);
    }

    public async Task<ResultadoOperacion> AlternarEstadoAsync(int id)
    {
        var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null)
        {
            return ResultadoOperacion.Error("El cliente no existe.");
        }

        cliente.Activo = !cliente.Activo;
        await db.SaveChangesAsync();
        return ResultadoOperacion.Ok(cliente.Activo ? "Cliente activado." : "Cliente desactivado.");
    }
}
