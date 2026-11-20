using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ElOferton.Models.Entities;

public class Inventario
{
    public int ProductoId { get; set; }

    public int PuntoVentaId { get; set; }

    public int Cantidad { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    [ValidateNever]
    public Producto Producto { get; set; } = null!;

    [ValidateNever]
    public PuntoVenta PuntoVenta { get; set; } = null!;
}
