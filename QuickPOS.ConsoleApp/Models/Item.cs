using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickPOS.Models;

public class Item
{
    public int ItemId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
}
