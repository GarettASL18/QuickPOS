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

        // --- PROPIEDAD QUE FALTABA ---
        // Esta no se guarda en la tabla Factura, pero la llenamos al leer el historial
        public string NombreCliente { get; set; } = "Cliente Casual";

        public List<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
    }
}