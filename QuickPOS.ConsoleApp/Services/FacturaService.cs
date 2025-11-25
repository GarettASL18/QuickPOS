using System;
using System.Collections.Generic;
using QuickPOS;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.Services;

public class FacturaService
{
    private readonly IFacturaRepository _factRepo;
    private readonly IItemRepository _itemRepo;

    public FacturaService(IFacturaRepository factRepo, IItemRepository itemRepo)
    {
        _factRepo = factRepo ?? throw new ArgumentNullException(nameof(factRepo));
        _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
    }

    /// <summary>
    /// Crea la factura: valida items, calcula totales usando Config.Impuesto,
    /// y delega al repositorio la persistencia (con transacción).
    /// Retorna el id de la factura.
    /// </summary>
    public long CrearFactura(int? clienteId, List<(int ItemId, int Cantidad, decimal PrecioUnitario)> lineas)
    {
        if (lineas == null || lineas.Count == 0)
            throw new ArgumentException("La factura debe tener al menos una línea.", nameof(lineas));

        var factura = new Factura
        {
            ClienteId = clienteId,
            Fecha = DateTime.UtcNow
        };

        decimal subtotal = 0m;

        foreach (var l in lineas)
        {
            if (l.Cantidad <= 0)
                throw new ArgumentException("Cantidad debe ser mayor que 0.");

            // Validar que el item existe y está activo
            var item = _itemRepo.GetById(l.ItemId);
            if (item == null)
                throw new InvalidOperationException($"Item {l.ItemId} no existe.");
            if (!item.Activo)
                throw new InvalidOperationException($"Item {item.Nombre} está inactivo.");

            var importe = l.PrecioUnitario * l.Cantidad;
            subtotal += importe;

            factura.Detalles.Add(new FacturaDetalle
            {
                ItemId = l.ItemId,
                Cantidad = l.Cantidad,
                PrecioUnitario = l.PrecioUnitario
            });
        }

        var impuesto = Math.Round(subtotal * Config.Impuesto, 2, MidpointRounding.AwayFromZero);
        var total = Math.Round(subtotal + impuesto, 2, MidpointRounding.AwayFromZero);

        factura.Subtotal = subtotal;
        factura.Impuesto = impuesto;
        factura.Total = total;

        // Persiste la factura (el repositorio debe manejar la transacción)
        try
        {
            var id = _factRepo.CreateFactura(factura);
            return id;
        }
        catch (Exception)
        {
            // Puedes loguear aquí o envolver la excepción si quieres
            throw;
        }
    }
}
