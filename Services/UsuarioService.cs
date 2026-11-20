using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Identity;
using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public sealed class UsuarioService(
    ElOfertonDbContext db,
    UserManager<ApplicationUser> userManager) : IUsuarioService
{
    public async Task<UsuarioListViewModel> ObtenerUsuariosAsync()
    {
        var usuarios = await (from u in db.Users.AsNoTracking()
                              join ur in db.UserRoles.AsNoTracking() on u.Id equals ur.UserId into userRoles
                              from ur in userRoles.DefaultIfEmpty()
                              join r in db.Roles.AsNoTracking() on ur.RoleId equals r.Id into roles
                              from r in roles.DefaultIfEmpty()
                              join pv in db.PuntosVenta.AsNoTracking() on u.PuntoVentaId equals pv.Id into puntos
                              from pv in puntos.DefaultIfEmpty()
                              select new UsuarioItemViewModel
                              {
                                  Id = u.Id,
                                  NombreCompleto = u.NombreCompleto,
                                  Email = u.Email ?? string.Empty,
                                  UserName = u.UserName ?? string.Empty,
                                  Rol = r.Name ?? "Sin rol",
                                  PuntoVenta = pv.Nombre ?? "Todos los ambientes",
                                  Activo = u.Activo,
                                  FechaCreacion = u.FechaCreacion
                              })
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();

        return new UsuarioListViewModel { Usuarios = usuarios };
    }

    public async Task<CrearUsuarioViewModel> ObtenerFormularioCrearAsync()
    {
        return new CrearUsuarioViewModel
        {
            Roles = Roles.Todos.Select(r => new SelectListItem(r, r)).ToArray(),
            PuntosVenta = await ServicioAyuda.PuntosVentaAsync(db)
        };
    }

    public async Task<ResultadoOperacion> CrearAsync(CrearUsuarioViewModel model)
    {
        if (!Roles.Todos.Contains(model.Rol))
        {
            return ResultadoOperacion.Error("El rol seleccionado no es válido.");
        }

        if (model.PuntoVentaId.HasValue && !await db.PuntosVenta.AnyAsync(p => p.Id == model.PuntoVentaId.Value && p.Activo))
        {
            return ResultadoOperacion.Error("El punto de venta seleccionado no es válido.");
        }

        var usuario = new ApplicationUser
        {
            UserName = model.UserName.Trim(),
            Email = model.Email.Trim(),
            NombreCompleto = model.NombreCompleto.Trim(),
            PuntoVentaId = model.PuntoVentaId,
            Activo = true,
            FechaCreacion = Reloj.Ahora
        };

        var creacion = await userManager.CreateAsync(usuario, model.Password);
        if (!creacion.Succeeded)
        {
            return ResultadoOperacion.Error(TraducirErrores(creacion));
        }

        var asignacion = await userManager.AddToRoleAsync(usuario, model.Rol);
        if (!asignacion.Succeeded)
        {
            await userManager.DeleteAsync(usuario);
            return ResultadoOperacion.Error(TraducirErrores(asignacion));
        }

        return ResultadoOperacion.Ok("Usuario creado correctamente.", usuario.Id);
    }

    public async Task<ResultadoOperacion> AlternarEstadoAsync(int id)
    {
        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return ResultadoOperacion.Error("El usuario no existe.");
        }

        usuario.Activo = !usuario.Activo;
        var resultado = await userManager.UpdateAsync(usuario);
        return resultado.Succeeded
            ? ResultadoOperacion.Ok(usuario.Activo ? "Usuario activado." : "Usuario desactivado.")
            : ResultadoOperacion.Error(TraducirErrores(resultado));
    }

    public async Task<ResultadoOperacion> CambiarRolAsync(int id, string rol)
    {
        if (!Roles.Todos.Contains(rol))
        {
            return ResultadoOperacion.Error("El rol seleccionado no es válido.");
        }

        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return ResultadoOperacion.Error("El usuario no existe.");
        }

        var actuales = await userManager.GetRolesAsync(usuario);
        if (actuales.Count > 0)
        {
            var eliminados = await userManager.RemoveFromRolesAsync(usuario, actuales);
            if (!eliminados.Succeeded)
            {
                return ResultadoOperacion.Error(TraducirErrores(eliminados));
            }
        }

        var asignado = await userManager.AddToRoleAsync(usuario, rol);
        return asignado.Succeeded
            ? ResultadoOperacion.Ok("Rol actualizado correctamente.")
            : ResultadoOperacion.Error(TraducirErrores(asignado));
    }

    public async Task<PerfilViewModel?> ObtenerPerfilAsync(int id)
    {
        var usuario = await (from u in db.Users.AsNoTracking()
                             join ur in db.UserRoles.AsNoTracking() on u.Id equals ur.UserId into userRoles
                             from ur in userRoles.DefaultIfEmpty()
                             join r in db.Roles.AsNoTracking() on ur.RoleId equals r.Id into roles
                             from r in roles.DefaultIfEmpty()
                             join pv in db.PuntosVenta.AsNoTracking() on u.PuntoVentaId equals pv.Id into puntos
                             from pv in puntos.DefaultIfEmpty()
                             where u.Id == id
                             select new PerfilViewModel
                             {
                                 NombreCompleto = u.NombreCompleto,
                                 Email = u.Email ?? string.Empty,
                                 Rol = r.Name ?? "Sin rol",
                                 PuntoVenta = pv.Nombre ?? "Todos los ambientes"
                             })
            .FirstOrDefaultAsync();

        return usuario;
    }

    private static string TraducirErrores(IdentityResult resultado)
    {
        var traducciones = new Dictionary<string, string>
        {
            ["DuplicateEmail"] = "Ya existe un usuario con ese correo.",
            ["DuplicateUserName"] = "Ya existe un usuario con ese nombre de usuario.",
            ["InvalidEmail"] = "El correo electrónico no es válido.",
            ["PasswordTooShort"] = "La contraseña debe tener al menos 8 caracteres.",
            ["PasswordRequiresDigit"] = "La contraseña debe incluir al menos un número.",
            ["PasswordRequiresLower"] = "La contraseña debe incluir al menos una letra minúscula.",
            ["PasswordRequiresUpper"] = "La contraseña debe incluir al menos una letra mayúscula.",
            ["PasswordRequiresNonAlphanumeric"] = "La contraseña debe incluir al menos un carácter especial."
        };

        var mensajes = resultado.Errors
            .Select(e => traducciones.TryGetValue(e.Code, out var texto) ? texto : e.Description)
            .Distinct();

        return string.Join(" ", mensajes);
    }
}
