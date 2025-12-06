using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data
{
    public interface IFacturaRepository
    {
        // El contrato debe coincidir con la implementación
        void Create(Factura factura);

        // (Opcional por ahora)
        List<Factura> GetAll();
    }
}