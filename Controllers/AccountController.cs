using ElOferton.Services.Interfaces;
using ElOferton.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElOferton.Controllers;

public sealed class AccountController(
    SignInManager<Models.Identity.ApplicationUser> signInManager,
    UserManager<Models.Identity.ApplicationUser> userManager,
    IUsuarioService usuarioService) : Controller
{
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var usuario = await userManager.FindByEmailAsync(model.Email);
        if (usuario is null || !usuario.Activo)
        {
            ModelState.AddModelError(string.Empty, "Credenciales inválidas o usuario desactivado.");
            return View(model);
        }

        var resultado = await signInManager.PasswordSignInAsync(
            usuario,
            model.Password,
            model.Recordarme,
            lockoutOnFailure: true);

        if (resultado.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        if (resultado.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente. Intente en unos minutos.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Perfil()
    {
        var id = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var valor) ? valor : 0;
        var perfil = await usuarioService.ObtenerPerfilAsync(id);
        return perfil is null ? NotFound() : View(perfil);
    }

    [Authorize]
    public IActionResult CambiarPassword()
    {
        return View(new CambiarPasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var id = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var valor) ? valor : 0;
        var usuario = await userManager.FindByIdAsync(id.ToString());
        if (usuario is null)
        {
            return NotFound();
        }

        var resultado = await userManager.ChangePasswordAsync(usuario, model.Actual, model.Nueva);
        if (!resultado.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta o la nueva no cumple los requisitos.");
            return View(model);
        }

        await signInManager.RefreshSignInAsync(usuario);
        TempData["Mensaje"] = "Contraseña actualizada correctamente.";
        return RedirectToAction(nameof(Perfil));
    }
}
