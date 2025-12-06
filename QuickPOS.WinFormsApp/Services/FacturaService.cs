using System;
using System.Collections.Generic;
using QuickPOS.Data;
using QuickPOS.Models;

namespace QuickPOS.Services
{
    public class FacturaService
    {
        private readonly IFacturaRepository _repo;
        private readonly IItemRepository _itemRepo; // Lo dejamos por si quieres validar stock luego

        public FacturaService(IFacturaRepository repo, IItemRepository itemRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _itemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
        }

        // --- MÉTODO CORREGIDO ---
        // Ahora acepta el objeto 'Factura' completo, tal como lo envía el formulario
        public void CrearFactura(Factura factura)
        {
            // 1. Validaciones de Negocio (Opcional)
            if (factura.Detalles == null || factura.Detalles.Count == 0)
            {
                throw new InvalidOperationException("No se puede crear una factura sin productos.");
            }

            if (factura.Total <= 0)
            {
                throw new InvalidOperationException("El total de la factura no es válido.");
            }

            // 2. Guardar en Base de Datos
            _repo.Create(factura);
        }
    }
}