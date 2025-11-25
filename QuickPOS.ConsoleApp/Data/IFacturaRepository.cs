using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data;

public interface IFacturaRepository
{
    long CreateFactura(Factura factura);
    Factura? GetById(long id);
    List<Factura> GetAll();
}
