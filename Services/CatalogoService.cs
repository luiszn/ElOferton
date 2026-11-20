using ElOferton.Data;
using ElOferton.Models.Entities;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class CatalogoService(ElOfertonDbContext db) : ICatalogoService
{
    public async Task<CatalogoListViewModel> ObtenerCategoriasAsync(string? buscar)
    {
        var query = from c in db.Categorias.AsNoTracking()
                    select new CatalogoItemViewModel
                    {
                        Id = c.Id,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        Activo = c.Activo,
                        CantidadRelacionada = c.Productos.Count()
                    };

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(c => c.Nombre.Contains(texto));
        }

        return new CatalogoListViewModel
        {
            Titulo = "Categorías",
            Controlador = "Categorias",
            Buscar = buscar,
            Items = await query.OrderBy(c => c.Nombre).ToListAsync()
        };
    }

    public async Task<CatalogoFormViewModel?> ObtenerCategoriaFormularioAsync(int? id)
    {
        var modelo = new CatalogoFormViewModel { Titulo = "Categoría", Controlador = "Categorias" };
        if (!id.HasValue)
        {
            return modelo;
        }

        var categoria = await db.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id.Value);
        if (categoria is null)
        {
            return null;
        }

        modelo.Id = categoria.Id;
        modelo.Nombre = categoria.Nombre;
        modelo.Descripcion = categoria.Descripcion;
        modelo.Activo = categoria.Activo;
        return modelo;
    }

    public Task<ResultadoOperacion> GuardarCategoriaAsync(CatalogoFormViewModel model) =>
        GuardarAsync(
            model,
            () => db.Categorias,
            id => db.Categorias.FirstOrDefaultAsync(c => c.Id == id),
            nombre => new Categoria { Nombre = nombre },
            "categoría");

    public Task<ResultadoOperacion> AlternarCategoriaAsync(int id) =>
        AlternarAsync(
            id,
            () => db.Categorias,
            "categoría");

    public async Task<CatalogoListViewModel> ObtenerMarcasAsync(string? buscar)
    {
        var query = from m in db.Marcas.AsNoTracking()
                    select new CatalogoItemViewModel
                    {
                        Id = m.Id,
                        Nombre = m.Nombre,
                        Descripcion = m.Descripcion,
                        Activo = m.Activo,
                        CantidadRelacionada = m.Productos.Count()
                    };

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var texto = buscar.Trim();
            query = query.Where(m => m.Nombre.Contains(texto));
        }

        return new CatalogoListViewModel
        {
            Titulo = "Marcas",
            Controlador = "Marcas",
            Buscar = buscar,
            Items = await query.OrderBy(m => m.Nombre).ToListAsync()
        };
    }

    public async Task<CatalogoFormViewModel?> ObtenerMarcaFormularioAsync(int? id)
    {
        var modelo = new CatalogoFormViewModel { Titulo = "Marca", Controlador = "Marcas" };
        if (!id.HasValue)
        {
            return modelo;
        }

        var marca = await db.Marcas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id.Value);
        if (marca is null)
        {
            return null;
        }

        modelo.Id = marca.Id;
        modelo.Nombre = marca.Nombre;
        modelo.Descripcion = marca.Descripcion;
        modelo.Activo = marca.Activo;
        return modelo;
    }

    public Task<ResultadoOperacion> GuardarMarcaAsync(CatalogoFormViewModel model) =>
        GuardarAsync(
            model,
            () => db.Marcas,
            id => db.Marcas.FirstOrDefaultAsync(m => m.Id == id),
            nombre => new Marca { Nombre = nombre },
            "marca");

    public Task<ResultadoOperacion> AlternarMarcaAsync(int id) =>
        AlternarAsync(
            id,
            () => db.Marcas,
            "marca");

    private async Task<ResultadoOperacion> GuardarAsync<T>(
        CatalogoFormViewModel model,
        Func<DbSet<T>> conjunto,
        Func<int, Task<T?>> buscar,
        Func<string, T> crear,
        string nombreEntidad)
        where T : class
    {
        var nombre = model.Nombre.Trim();
        var idActual = model.Id ?? 0;
        var set = conjunto();
        var duplicado = await set.AsNoTracking().AnyAsync(x => EF.Property<string>(x, "Nombre") == nombre && EF.Property<int>(x, "Id") != idActual);
        if (duplicado)
        {
            return ResultadoOperacion.Error($"Ya existe una {nombreEntidad} con ese nombre.");
        }

        T entidad;
        if (model.Id.HasValue)
        {
            var encontrada = await buscar(model.Id.Value);
            if (encontrada is null)
            {
                return ResultadoOperacion.Error($"La {nombreEntidad} no existe.");
            }

            entidad = encontrada;
        }
        else
        {
            entidad = crear(nombre);
            set.Add(entidad);
        }

        set.Entry(entidad).Property("Nombre").CurrentValue = nombre;
        set.Entry(entidad).Property("Descripcion").CurrentValue = model.Descripcion?.Trim();
        set.Entry(entidad).Property("Activo").CurrentValue = model.Activo;

        await db.SaveChangesAsync();
        var id = (int)set.Entry(entidad).Property("Id").CurrentValue!;
        return ResultadoOperacion.Ok($"{char.ToUpper(nombreEntidad[0]) + nombreEntidad[1..]} guardada correctamente.", id);
    }

    private async Task<ResultadoOperacion> AlternarAsync<T>(int id, Func<DbSet<T>> conjunto, string nombreEntidad)
        where T : class
    {
        var set = conjunto();
        var entidad = await set.FindAsync(id);
        if (entidad is null)
        {
            return ResultadoOperacion.Error($"La {nombreEntidad} no existe.");
        }

        var entrada = set.Entry(entidad);
        var activo = (bool)entrada.Property("Activo").CurrentValue!;
        entrada.Property("Activo").CurrentValue = !activo;
        await db.SaveChangesAsync();
        return ResultadoOperacion.Ok($"{(activo ? $"La {nombreEntidad} se desactivó" : $"La {nombreEntidad} se activó")} correctamente.");
    }
}
