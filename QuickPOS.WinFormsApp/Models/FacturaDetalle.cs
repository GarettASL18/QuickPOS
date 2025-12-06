namespace QuickPOS.Models
{
    public class FacturaDetalle
    {
        public long DetalleId { get; set; }
        public long FacturaId { get; set; }
        public int ItemId { get; set; }

        // Propiedades extras para mostrar en la grilla (Nombre del producto)
        public string NombreProducto { get; set; } = "";

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalLinea => Cantidad * PrecioUnitario;
    }
}