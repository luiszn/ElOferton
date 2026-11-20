using ElOferton.Constants;
using ElOferton.Data;
using ElOferton.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElOferton.Services;

public static class DatosIniciales
{
    private static readonly string[] CategoriasAdicionales =
    [
        "Arreglos florales",
        "Uñas y arreglos",
        "Fragancias",
        "Cuidado facial",
        "Cuerpo e higiene"
    ];

    private static readonly string[] MarcasAdicionales =
    [
        "Revlon",
        "Avon",
        "OPI",
        "Babyliss",
        "The Ordinary",
        "Eucerin",
        "MAC"
    ];

    private static readonly ProductoSeed[] Productos =
    [
        new("MAQ-001", "Base líquida Fit Me 30ml", "Maquillaje", "Maybelline", 42m, 69m, 6, "Base de cobertura media para todo tipo de piel."),
        new("MAQ-002", "Corrector Instant Age Rewind", "Maquillaje", "Maybelline", 32m, 55m, 6, "Corrector de ojeras y manchas."),
        new("MAQ-003", "Polvo compacto Clean Matte", "Maquillaje", "Maybelline", 28m, 48m, 8, "Polvo compacto de acabado mate."),
        new("MAQ-004", "Labial Super Lustrous", "Maquillaje", "Revlon", 25m, 45m, 8, "Labial cremoso de larga duración."),
        new("MAQ-005", "Labial mate SuperStay", "Maquillaje", "Maybelline", 34m, 59m, 6, "Labial mate de alta fijación."),
        new("MAQ-006", "Máscara Great Lash", "Maquillaje", "Maybelline", 29m, 49m, 8, "Máscara de pestañas volumen y longitud."),
        new("MAQ-007", "Delineador líquido ColorStay", "Maquillaje", "Revlon", 26m, 44m, 6, "Delineador de precisión larga duración."),
        new("MAQ-008", "Sombra ColorStay 12 tonos", "Maquillaje", "Revlon", 38m, 65m, 4, "Paleta de sombras de ojos 12 tonos."),
        new("MAQ-009", "Rubor compacto", "Maquillaje", "Avon", 24m, 42m, 6, "Rubor compacto para mejillas."),
        new("MAQ-010", "Paleta de sombras L'Oréal", "Maquillaje", "L'Oréal Paris", 55m, 89m, 4, "Paleta profesional de sombras."),
        new("MAQ-011", "BB Cream UV Defence", "Maquillaje", "L'Oréal Paris", 39m, 65m, 6, "Crema con color y protección solar."),
        new("MAQ-012", "Primer facial", "Maquillaje", "Revlon", 36m, 59m, 5, "Prebase para prolongar el maquillaje."),
        new("MAQ-013", "Brillo labial", "Maquillaje", "Avon", 18m, 32m, 10, "Brillo labial con efecto gloss."),
        new("MAQ-014", "Base Studio Fix", "Maquillaje", "MAC", 95m, 149m, 3, "Base de alta cobertura profesional."),
        new("MAQ-015", "Spray fijador de maquillaje", "Maquillaje", "L'Oréal Paris", 30m, 52m, 6, "Fijador en spray para todo el día."),

        new("CUI-001", "Crema hidratante facial", "Cuidado facial", "Nivea", 18m, 32m, 10, "Hidratación diaria para rostro."),
        new("CUI-002", "Crema corporal Nutriss", "Cuerpo e higiene", "Dove", 16m, 29m, 12, "Crema corporal de hidratación intensa."),
        new("CUI-003", "Jabón líquido manos", "Cuerpo e higiene", "Dove", 12m, 22m, 12, "Jabón líquido para manos y cuerpo."),
        new("CUI-004", "Shampoo anticaída", "Peluquería", "Schwarzkopf", 24m, 42m, 8, "Shampoo fortificante para cabello."),
        new("CUI-005", "Acondicionador hidratante", "Peluquería", "Schwarzkopf", 23m, 40m, 8, "Acondicionador suavizante."),
        new("CUI-006", "Mascarilla capilar repair", "Peluquería", "Schwarzkopf", 28m, 49m, 6, "Tratamiento intensivo para cabello dañado."),
        new("CUI-007", "Gel de ducha", "Cuerpo e higiene", "Nivea", 14m, 26m, 10, "Gel de ducha hidratante."),
        new("CUI-008", "Desodorante Fresh", "Cuidado personal", "Nivea", 13m, 24m, 10, "Protección diaria contra el olor."),
        new("CUI-009", "Crema de manos", "Cuidado personal", "Nivea", 11m, 20m, 12, "Crema de manos de rápida absorción."),
        new("CUI-010", "Protector solar FPS 50", "Cuidado facial", "Eucerin", 55m, 89m, 5, "Protección solar alta para rostro."),
        new("CUI-011", "Sérum Niacinamida 10%", "Cuidado facial", "The Ordinary", 32m, 59m, 5, "Sérum para unificar el tono."),
        new("CUI-012", "Sérum Ácido Hialurónico", "Cuidado facial", "The Ordinary", 34m, 62m, 5, "Sérum de hidratación profunda."),
        new("CUI-013", "Limpiador facial suave", "Cuidado facial", "The Ordinary", 22m, 39m, 8, "Limpiador facial para uso diario."),
        new("CUI-014", "Crema corporal Eucerin", "Cuerpo e higiene", "Eucerin", 42m, 69m, 6, "Crema corporal para piel sensible."),
        new("CUI-015", "Loción corporal floral", "Cuerpo e higiene", "Avon", 20m, 36m, 8, "Loción corporal con fragancia floral."),
        new("CUI-016", "Shampoo anticaspa", "Peluquería", "Schwarzkopf", 26m, 45m, 7, "Shampoo para control de caspa."),
        new("CUI-017", "Jabón Dove barra", "Cuerpo e higiene", "Dove", 8m, 15m, 15, "Jabón en barra humectante."),
        new("CUI-018", "Espuma de afeitar", "Barbería", "Gillette", 18m, 32m, 8, "Espuma de afeitar para piel sensible."),
        new("CUI-019", "Toallitas desmaquillantes", "Cuidado facial", "Nivea", 12m, 22m, 10, "Toallitas para limpieza facial."),
        new("CUI-020", "Mascarilla facial hydrating", "Cuidado facial", "Dove", 15m, 28m, 8, "Mascarilla facial de hidratación."),

        new("ACC-001", "Brocha para polvo", "Accesorios de belleza", "Babyliss", 32m, 55m, 5, "Brocha suave para aplicación de polvo."),
        new("ACC-002", "Brocha para rubor", "Accesorios de belleza", "Babyliss", 28m, 48m, 5, "Brocha para rubor y contour."),
        new("ACC-003", "Esponja beauty blender", "Accesorios de belleza", "Avon", 12m, 22m, 12, "Esponja para difuminar base."),
        new("ACC-004", "Set de brochas profesionales", "Accesorios de belleza", "Babyliss", 65m, 109m, 3, "Set completo de brochas para maquillaje."),
        new("ACC-005", "Pinza para cejas", "Accesorios de belleza", "Revlon", 14m, 26m, 10, "Pinza de precisión para cejas."),
        new("ACC-006", "Tijera para cejas", "Accesorios de belleza", "Babyliss", 18m, 32m, 8, "Tijera pequeña para grooming."),
        new("ACC-007", "Rizador de pestañas", "Accesorios de belleza", "Revlon", 20m, 36m, 8, "Rizador manual de pestañas."),
        new("ACC-008", "Espejo de mano", "Accesorios de belleza", "Avon", 15m, 28m, 10, "Espejo portátil de mano."),
        new("ACC-009", "Estuche organizador", "Accesorios de belleza", "Avon", 45m, 75m, 5, "Organizador de maquillaje con compartimientos."),
        new("ACC-010", "Cepillo limpiador de brochas", "Accesorios de belleza", "Babyliss", 16m, 29m, 8, "Cepillo para limpieza de brochas."),
        new("ACC-011", "Aplicador de sombras", "Accesorios de belleza", "Revlon", 8m, 15m, 15, "Aplicador de espuma para sombras."),
        new("ACC-012", "Bolsa cosmética", "Accesorios de belleza", "Dove", 22m, 38m, 8, "Bolsa práctica para cosméticos."),

        new("ARR-001", "Arreglo floral de rosas", "Arreglos florales", "Avon", 45m, 89m, 4, "Arreglo decorativo de rosas frescas."),
        new("ARR-002", "Ramo de tulipanes", "Arreglos florales", "Avon", 38m, 75m, 4, "Ramo de tulipanes de temporada."),
        new("ARR-003", "Arreglo floral mixto", "Arreglos florales", "Avon", 52m, 99m, 3, "Arreglo con flores mixtas y follaje."),
        new("ARR-004", "Caja de flores preservadas", "Arreglos florales", "Avon", 68m, 129m, 3, "Caja premium de flores preservadas."),
        new("ARR-005", "Kit de uñas acrílicas", "Uñas y arreglos", "OPI", 48m, 85m, 4, "Kit completo para uñas acrílicas."),
        new("ARR-006", "Esmalte para uñas rojo", "Uñas y arreglos", "OPI", 22m, 39m, 8, "Esmalte de uñas tono rojo clásico."),
        new("ARR-007", "Esmalte para uñas nude", "Uñas y arreglos", "OPI", 22m, 39m, 8, "Esmalte de uñas tono nude."),
        new("ARR-008", "Aceite para cutículas", "Uñas y arreglos", "OPI", 18m, 32m, 8, "Aceite hidratante para cutículas."),
        new("ARR-009", "Lima profesional de uñas", "Uñas y arreglos", "OPI", 12m, 22m, 10, "Lima profesional para uñas."),
        new("ARR-010", "Mini secador de uñas", "Uñas y arreglos", "Babyliss", 55m, 95m, 3, "Secador compacto para esmaltado."),

        new("FRG-001", "Perfume floral", "Fragancias", "Avon", 45m, 79m, 5, "Fragancia floral para uso diario."),
        new("FRG-002", "Eau de parfum", "Fragancias", "L'Oréal Paris", 68m, 119m, 3, "Fragancia femenina de larga duración."),
        new("FRG-003", "Body mist floral", "Fragancias", "Avon", 22m, 39m, 8, "Bruma corporal ligera."),
        new("FRG-004", "Perfume Revlon", "Fragancias", "Revlon", 52m, 89m, 4, "Perfume de fragancia equilibrada."),
        new("FRG-005", "Fragancia cítrica", "Fragancias", "Avon", 40m, 72m, 5, "Fragancia fresca con notas cítricas."),

        new("BAR-001", "Navaja de afeitar", "Barbería", "Gillette", 28m, 49m, 6, "Navaja de afeitar de precisión."),
        new("BAR-002", "Hojas de afeitar x5", "Barbería", "Gillette", 18m, 32m, 10, "Paquete de 5 hojas de recambio."),
        new("BAR-003", "Crema de afeitar", "Barbería", "Gillette", 16m, 29m, 8, "Crema de afeitar tradicional."),
        new("BAR-004", "Cepillo para barba", "Barbería", "Babyliss", 24m, 42m, 6, "Cepillo para peinar y cuidar la barba."),
        new("BAR-005", "Aceite para barba", "Barbería", "Gillette", 30m, 55m, 6, "Aceite suavizante para barba."),
        new("BAR-006", "After shave", "Barbería", "Gillette", 20m, 36m, 8, "Loción calmante posterior al afeitado."),
        new("BAR-007", "Recortadora de barba", "Barbería", "Babyliss", 85m, 149m, 3, "Recortadora eléctrica para barba.")
    ];

    private static readonly (string Nombre, string Telefono, string Documento, TipoDocumento Tipo, string Direccion, string Email)[] Clientes =
    [
        ("Rosa Vargas", "987654321", "10456789", TipoDocumento.DNI, "Jr. Amazonas 123", "rosa.vargas@example.com"),
        ("Carlos Ruiz", "987123456", "20345678", TipoDocumento.DNI, "Av. Perú 456", "carlos.ruiz@example.com"),
        ("Ana Torres", "945678912", "30567890", TipoDocumento.DNI, "Jr. Cajamarca 78", "ana.torres@example.com"),
        ("Luis Medina", "932145678", "40123456", TipoDocumento.DNI, "Av. Atahualpa 210", "luis.medina@example.com"),
        ("María Chávez", "999876543", "50678901", TipoDocumento.DNI, "Jr. Sucre 345", "maria.chavez@example.com"),
        ("Jhon Quispe", "918273645", "60789012", TipoDocumento.DNI, "Av. Bolivia 512", "jhon.quispe@example.com"),
        ("Elizabeth Rojas", "976543218", "70890123", TipoDocumento.DNI, "Jr. Junín 64", "elizabeth.rojas@example.com"),
        ("Pedro Salazar", "963852741", "80901234", TipoDocumento.DNI, "Av. Independencia 890", "pedro.salazar@example.com"),
        ("Fanny Cáceres", "951753852", "90112345", TipoDocumento.DNI, "Jr. Puno 156", "fanny.caceres@example.com"),
        ("Miguel Santisteban", "924681357", "10234567", TipoDocumento.DNI, "Av. Grau 321", "miguel.santisteban@example.com"),
        ("Distribuidora Belleza EIRL", "988123654", "10456789012", TipoDocumento.RUC, "Av. Comercio 654", "ventas@belleza.example.com"),
        ("Salón Estilo Cajamarca SAC", "977654123", "20567890123", TipoDocumento.RUC, "Jr. Lima 987", "contacto@estilo.example.com")
    ];

    public static async Task SeedAsync(ElOfertonDbContext db)
    {
        await AsegurarCatalogosAsync(db);
        await AsegurarProductosAsync(db);
        await AsegurarClientesAsync(db);
    }

    private static async Task AsegurarCatalogosAsync(ElOfertonDbContext db)
    {
        var categoriasExistentes = await db.Categorias.Select(c => c.Nombre).ToListAsync();
        foreach (var nombre in CategoriasAdicionales.Where(n => !categoriasExistentes.Contains(n)))
        {
            db.Categorias.Add(new Categoria { Nombre = nombre, Descripcion = $"Productos de {nombre.ToLowerInvariant()}", Activo = true });
        }

        var marcasExistentes = await db.Marcas.Select(m => m.Nombre).ToListAsync();
        foreach (var nombre in MarcasAdicionales.Where(n => !marcasExistentes.Contains(n)))
        {
            db.Marcas.Add(new Marca { Nombre = nombre, Descripcion = $"Productos de la marca {nombre}", Activo = true });
        }

        await db.SaveChangesAsync();
    }

    private static async Task AsegurarProductosAsync(ElOfertonDbContext db)
    {
        if (await db.Productos.AnyAsync())
        {
            return;
        }

        var categorias = await db.Categorias.ToDictionaryAsync(c => c.Nombre, c => c.Id);
        var marcas = await db.Marcas.ToDictionaryAsync(m => m.Nombre, m => m.Id);
        var productos = new List<Producto>();

        foreach (var dato in Productos)
        {
            productos.Add(new Producto
            {
                Codigo = dato.Codigo,
                Nombre = dato.Nombre,
                Descripcion = dato.Descripcion,
                CategoriaId = categorias[dato.Categoria],
                MarcaId = marcas[dato.Marca],
                PrecioCompra = dato.Compra,
                PrecioVenta = dato.Venta,
                StockMinimo = dato.StockMinimo,
                Activo = true
            });
        }

        db.Productos.AddRange(productos);
        await db.SaveChangesAsync();

        var puntosVenta = await db.PuntosVenta.OrderBy(p => p.Id).Select(p => p.Id).ToListAsync();
        var inventarios = new List<Inventario>();

        for (var i = 0; i < productos.Count; i++)
        {
            var cantidades = new[]
            {
                8 + i % 7,
                6 + (i * 3) % 9,
                4 + (i * 5) % 6,
                10 + (i * 2) % 8,
                2 + i % 4,
                40 + (i * 7) % 60
            };

            for (var p = 0; p < puntosVenta.Count; p++)
            {
                inventarios.Add(new Inventario
                {
                    ProductoId = productos[i].Id,
                    PuntoVentaId = puntosVenta[p],
                    Cantidad = cantidades[p]
                });
            }
        }

        db.Inventarios.AddRange(inventarios);
        await db.SaveChangesAsync();
    }

    private static async Task AsegurarClientesAsync(ElOfertonDbContext db)
    {
        foreach (var dato in Clientes)
        {
            if (await db.Clientes.AnyAsync(c => c.NumeroDocumento == dato.Documento))
            {
                continue;
            }

            db.Clientes.Add(new Cliente
            {
                TipoDocumento = dato.Tipo,
                NumeroDocumento = dato.Documento,
                Nombre = dato.Nombre,
                Telefono = dato.Telefono,
                Email = dato.Email,
                Direccion = dato.Direccion,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }

    private sealed record ProductoSeed(
        string Codigo,
        string Nombre,
        string Categoria,
        string Marca,
        decimal Compra,
        decimal Venta,
        int StockMinimo,
        string Descripcion);
}
