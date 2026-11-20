using ElOferton.Data;
using ElOferton.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

internal static class ServicioAyuda
{
    public static async Task<SelectListItem[]> CategoriasAsync(ElOfertonDbContext db, bool soloActivos = true)
    {
        IQueryable<Categoria> query = db.Categorias.AsNoTracking().OrderBy(c => c.Nombre);
        if (soloActivos)
        {
            query = query.Where(c => c.Activo);
        }

        return await query.Select(c => new SelectListItem(c.Nombre, c.Id.ToString())).ToArrayAsync();
    }

    public static async Task<SelectListItem[]> MarcasAsync(ElOfertonDbContext db, bool soloActivos = true)
    {
        IQueryable<Marca> query = db.Marcas.AsNoTracking().OrderBy(m => m.Nombre);
        if (soloActivos)
        {
            query = query.Where(m => m.Activo);
        }

        return await query.Select(m => new SelectListItem(m.Nombre, m.Id.ToString())).ToArrayAsync();
    }

    public static async Task<SelectListItem[]> PuntosVentaAsync(ElOfertonDbContext db, bool soloActivos = true)
    {
        IQueryable<PuntoVenta> query = db.PuntosVenta.AsNoTracking().OrderBy(p => p.Id);
        if (soloActivos)
        {
            query = query.Where(p => p.Activo);
        }

        return await query.Select(p => new SelectListItem(p.Nombre, p.Id.ToString())).ToArrayAsync();
    }

    public static async Task<SelectListItem[]> ClientesAsync(ElOfertonDbContext db)
    {
        return await db.Clientes.AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
            .ToArrayAsync();
    }

    public static async Task<SelectListItem[]> MetodosPagoAsync(ElOfertonDbContext db)
    {
        return await db.MetodosPago.AsNoTracking()
            .Where(m => m.Activo)
            .OrderBy(m => m.Nombre)
            .Select(m => new SelectListItem(m.Nombre, m.Id.ToString()))
            .ToArrayAsync();
    }

    public static async Task<SelectListItem[]> UsuariosAsync(ElOfertonDbContext db)
    {
        return await db.Users.AsNoTracking()
            .OrderBy(u => u.NombreCompleto)
            .Select(u => new SelectListItem(u.NombreCompleto, u.Id.ToString()))
            .ToArrayAsync();
    }

    public static decimal Redondear(decimal valor) =>
        Math.Round(valor, 2, MidpointRounding.AwayFromZero);
}
