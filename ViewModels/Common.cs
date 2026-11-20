namespace ElOferton.ViewModels;

public class ResultadoOperacion
{
    public bool Exitoso { get; set; }

    public string Mensaje { get; set; } = string.Empty;

    public int? Id { get; set; }

    public static ResultadoOperacion Ok(string mensaje, int? id = null) =>
        new() { Exitoso = true, Mensaje = mensaje, Id = id };

    public static ResultadoOperacion Error(string mensaje) =>
        new() { Exitoso = false, Mensaje = mensaje };
}

public sealed class ResultadoVenta : ResultadoOperacion
{
    public VentaConfirmacionViewModel? Comprobante { get; set; }
}

public sealed class PosProductoResultado
{
    public int ProductoId { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;
}
