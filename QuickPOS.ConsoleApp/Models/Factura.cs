using System;
using System.Collections.Generic;

namespace QuickPOS.Models
{
    public class Factura
    {
        public long FacturaId { get; set; }
        public int? ClienteId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }

        // Lista de productos dentro de la factura
        public List<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
    }
}