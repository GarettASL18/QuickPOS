using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuickPOS.Models;

namespace QuickPOS.Data
{
    public class FacturaRepository : IFacturaRepository
    {
        private readonly SqlConnectionFactory _factory;

        public FacturaRepository(SqlConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        // --- ESTE ES EL MÉTODO QUE FALTABA O ESTABA DESACTUALIZADO ---
        public void Create(Factura factura)
        {
            using var cn = _factory.Create();
            cn.Open();

            // Usamos transacción para asegurar que se guarde todo o nada
            using var transaction = cn.BeginTransaction();

            try
            {
                // 1. Insertar Cabecera (Tabla Factura)
                const string sqlHeader = @"
                    INSERT INTO dbo.Factura (ClienteId, Fecha, Subtotal, Impuesto, Total) 
                    VALUES (@clienteId, @fecha, @sub, @imp, @tot);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                long newId;

                using (var cmdHeader = new SqlCommand(sqlHeader, cn, transaction))
                {
                    // Manejo de Cliente Nulo
                    cmdHeader.Parameters.AddWithValue("@clienteId", factura.ClienteId > 0 ? (object)factura.ClienteId : DBNull.Value);
                    cmdHeader.Parameters.AddWithValue("@fecha", factura.Fecha);
                    cmdHeader.Parameters.AddWithValue("@sub", factura.Subtotal);
                    cmdHeader.Parameters.AddWithValue("@imp", factura.Impuesto);
                    cmdHeader.Parameters.AddWithValue("@tot", factura.Total);

                    // Obtenemos el ID generado
                    newId = Convert.ToInt64(cmdHeader.ExecuteScalar());
                }

                // 2. Insertar Detalles (Tabla FacturaDetalle)
                const string sqlDetail = @"
                    INSERT INTO dbo.FacturaDetalle (FacturaId, ItemId, Cantidad, PrecioUnitario) 
                    VALUES (@fid, @itemId, @cant, @precio);";

                foreach (var item in factura.Detalles)
                {
                    using (var cmdDetail = new SqlCommand(sqlDetail, cn, transaction))
                    {
                        cmdDetail.Parameters.AddWithValue("@fid", newId);
                        cmdDetail.Parameters.AddWithValue("@itemId", item.ItemId);
                        cmdDetail.Parameters.AddWithValue("@cant", item.Cantidad);
                        cmdDetail.Parameters.AddWithValue("@precio", item.PrecioUnitario);
                        cmdDetail.ExecuteNonQuery();
                    }
                }

                // Confirmar cambios
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback(); // Si falla, deshacer todo
                throw;
            }
        }

        // --- MÉTODO REQUERIDO POR LA INTERFAZ (Aunque esté vacío por ahora) ---
        public List<Factura> GetAll()
        {
            // Retornamos lista vacía para cumplir el contrato (implementaremos historial luego)
            return new List<Factura>();
        }
    }
}