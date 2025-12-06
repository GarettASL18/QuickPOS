using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data
{
    public interface IFacturaRepository
    {
        void Create(Factura factura);
        List<Factura> GetAll();
        List<FacturaDetalle> GetDetalles(long facturaId);

        // --- NUEVO MÉTODO PARA EL DASHBOARD ---
        decimal GetTotalVentasHoy();
    }
}