using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Identity;
using ElOferton.Services;
using ElOferton.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IInventarioService, InventarioService>();
builder.Services.AddScoped<IVentaService, VentaService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddDbContext<ElOfertonDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ElOfertonDb"),
        sql => sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ElOfertonDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Politicas.Administrador, policy => policy.RequireRole(Roles.Administrador));
    options.AddPolicy(Politicas.Ventas, policy => policy.RequireRole(Roles.Administrador, Roles.Vendedor));
    options.AddPolicy(Politicas.Inventario, policy => policy.RequireRole(Roles.Administrador, Roles.Almacenero));
    options.AddPolicy(Politicas.Reportes, policy => policy.RequireRole(Roles.Administrador, Roles.Vendedor, Roles.Almacenero));
    options.AddPolicy(Politicas.Usuarios, policy => policy.RequireRole(Roles.Administrador));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var correo = builder.Configuration["BootstrapAdmin:Email"];
    var clave = builder.Configuration["BootstrapAdmin:Password"];

    if (!string.IsNullOrWhiteSpace(correo) &&
        !string.IsNullOrWhiteSpace(clave) &&
        !await userManager.Users.AnyAsync())
    {
        var puntoVentaId = int.TryParse(builder.Configuration["BootstrapAdmin:PuntoVentaId"], out var punto)
            ? punto
            : (int?)null;

        var administrador = new ApplicationUser
        {
            UserName = correo,
            Email = correo,
            NombreCompleto = builder.Configuration["BootstrapAdmin:NombreCompleto"] ?? "Administrador",
            PuntoVentaId = puntoVentaId,
            Activo = true
        };

        var creado = await userManager.CreateAsync(administrador, clave);
        if (creado.Succeeded)
        {
            await userManager.AddToRoleAsync(administrador, Roles.Administrador);
        }
    }

    await DatosIniciales.SeedAsync(scope.ServiceProvider.GetRequiredService<ElOfertonDbContext>());
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
