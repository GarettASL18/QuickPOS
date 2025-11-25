using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPOS.Models;

public class FacturaDetalle
{
    public long DetalleId { get; set; }
    public long FacturaId { get; set; }
    public int ItemId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}

