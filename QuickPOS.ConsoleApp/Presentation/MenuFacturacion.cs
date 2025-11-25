using System;
using System.Collections.Generic;
using System.Linq;
using QuickPOS;                 // para Config
using QuickPOS.Data;
using QuickPOS.Models;
using QuickPOS.Services;
using QuickPOS.UI;

namespace QuickPOS.Presentation;

public class MenuFacturacion
{
    private readonly FacturaService _facturaService;
    private readonly IItemRepository _itemRepo;   // usar interfaz en lugar de la clase concreta

    public MenuFacturacion(FacturaService facturaService, IItemRepository itemRepo)
    {
        _facturaService = facturaService ?? throw new ArgumentNullException(nameof(facturaService));
        _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
    }

    public void CrearFactura()
    {
        ConsoleUI.Clear();
        ConsoleUI.Header("CREAR FACTURA");

        Console.Write("ClienteId (ENTER si ninguno): ");
        var cliInput = Console.ReadLine();
        int? clienteId = null;
        if (!string.IsNullOrWhiteSpace(cliInput) && int.TryParse(cliInput, out var cid)) clienteId = cid;

        var lineas = new List<(int ItemId, int Cantidad, decimal PrecioUnitario)>();

        while (true)
        {
            Console.Write("ItemId (ENTER para terminar): ");
            var it = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(it)) break;

            if (!int.TryParse(it, out var itemId))
            {
                Console.WriteLine("ItemId inválido.");
                continue;
            }

            var item = _itemRepo.GetById(itemId);
            if (item == null) { Console.WriteLine("Item no existe."); continue; }
            if (!item.Activo) { Console.WriteLine("Item inactivo."); continue; }

            Console.Write($"Cantidad para '{item.Nombre}' (default 1): ");
            var cantStr = Console.ReadLine();
            int cant = 1;
            if (!string.IsNullOrWhiteSpace(cantStr) && !int.TryParse(cantStr, out cant))
            {
                Console.WriteLine("Cantidad inválida, se usará 1.");
                cant = 1;
            }

            lineas.Add((itemId, cant, item.Precio));

            // Mostrar subtotal local
            var subtotal = lineas.Sum(l => l.GetImporte());
            var impuesto = Math.Round(subtotal * Config.Impuesto, 2);
            var total = subtotal + impuesto;

            Console.WriteLine($"Líneas: {lineas.Count}  Subtotal: {subtotal:0.00}  Impuesto: {impuesto:0.00}  Total: {total:0.00}");
            Console.Write("Agregar otro item? (s/n): ");
            var r = Console.ReadLine();
            if (!string.Equals(r, "s", StringComparison.OrdinalIgnoreCase)) break;
        }

        if (lineas.Count == 0)
        {
            Console.WriteLine("No se agregaron líneas. Presiona ENTER para volver.");
            Console.ReadLine();
            return;
        }

        Console.Write("Confirmar factura? (s/n): ");
        var confirm = Console.ReadLine();
        if (!string.Equals(confirm, "s", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Factura cancelada. ENTER para volver.");
            Console.ReadLine();
            return;
        }

        try
        {
            var id = _facturaService.CrearFactura(clienteId, lineas);
            Console.WriteLine($"Factura creada con id: {id}. ENTER para continuar.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando factura: {ex.Message}");
        }

        Console.ReadLine();
    }
}

// extension method to compute line importe (sin acentos en el nombre)
static class MenuFacturacionExtensions
{
    public static decimal GetImporte(this (int ItemId, int Cantidad, decimal PrecioUnitario) l)
    {
        return l.Cantidad * l.PrecioUnitario;
    }
}
