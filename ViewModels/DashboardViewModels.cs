namespace ElOferton.ViewModels;

public class DashboardViewModel
{
    public decimal VentasHoy { get; set; }

    public decimal VentasMes { get; set; }

    public int ProductosRegistrados { get; set; }

    public int ProductosStockBajo { get; set; }

    public int ClientesRegistrados { get; set; }

    public int AmbientesActivos { get; set; }

    public int VentasHoyCantidad { get; set; }

    public List<VentaResumenViewModel> UltimasVentas { get; set; } = [];

    public List<ProductoMasVendidoViewModel> ProductosMasVendidos { get; set; } = [];

    public List<IngresoMetodoPagoViewModel> IngresosPorMetodoPago { get; set; } = [];

    public List<PuntoSerieViewModel> SerieVentas { get; set; } = [];
}

public class VentaResumenViewModel
{
    public string NumeroComprobante { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public string Cliente { get; set; } = string.Empty;

    public string PuntoVenta { get; set; } = string.Empty;

    public decimal Total { get; set; }
}

public class ProductoMasVendidoViewModel
{
    public string Nombre { get; set; } = string.Empty;

    public int Cantidad { get; set; }

    public decimal Total { get; set; }
}

public class IngresoMetodoPagoViewModel
{
    public string Metodo { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public int Cantidad { get; set; }
}

public class PuntoSerieViewModel
{
    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }
}
