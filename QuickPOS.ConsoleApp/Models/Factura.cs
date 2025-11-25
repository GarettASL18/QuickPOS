namespace QuickPOS.Models;

public class Factura
{
    public long FacturaId { get; set; }
    public int? ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }

    public List<FacturaDetalle> Detalles { get; set; } = new();
}
