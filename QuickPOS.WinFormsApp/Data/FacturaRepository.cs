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

        // --- 1. CREAR FACTURA (Con Transacción) ---
        public void Create(Factura factura)
        {
            using var cn = _factory.Create();
            cn.Open();

            // Usamos transacción: O se guarda todo, o no se guarda nada.
            using var transaction = cn.BeginTransaction();

            try
            {
                // A. Insertar Cabecera
                const string sqlHeader = @"
                    INSERT INTO dbo.Factura (ClienteId, Fecha, Subtotal, Impuesto, Total) 
                    VALUES (@clienteId, @fecha, @sub, @imp, @tot);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                long newId;
                using (var cmdHeader = new SqlCommand(sqlHeader, cn, transaction))
                {
                    cmdHeader.Parameters.AddWithValue("@clienteId", factura.ClienteId > 0 ? (object)factura.ClienteId : DBNull.Value);
                    cmdHeader.Parameters.AddWithValue("@fecha", factura.Fecha);
                    cmdHeader.Parameters.AddWithValue("@sub", factura.Subtotal);
                    cmdHeader.Parameters.AddWithValue("@imp", factura.Impuesto);
                    cmdHeader.Parameters.AddWithValue("@tot", factura.Total);

                    // Obtenemos el ID de la nueva factura
                    newId = Convert.ToInt64(cmdHeader.ExecuteScalar());
                }

                // B. Insertar Detalles (Productos)
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

                // C. Confirmar transacción
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback(); // Si falla algo, deshacemos todo
                throw;
            }
        }

        // --- 2. OBTENER HISTORIAL ---
        public List<Factura> GetAll()
        {
            var list = new List<Factura>();
            using var cn = _factory.Create();
            cn.Open();

            // Traemos el nombre del cliente con un JOIN
            const string sql = @"
                SELECT f.FacturaId, f.Fecha, f.Total, c.Nombre 
                FROM dbo.Factura f
                LEFT JOIN dbo.Cliente c ON f.ClienteId = c.ClienteId
                ORDER BY f.Fecha DESC;";

            using var cmd = new SqlCommand(sql, cn);
            using var r = cmd.ExecuteReader();

            while (r.Read())
            {
                list.Add(new Factura
                {
                    FacturaId = r.GetInt64(0),
                    Fecha = r.GetDateTime(1),
                    Total = r.GetDecimal(2),
                    NombreCliente = r.IsDBNull(3) ? "Consumidor Final" : r.GetString(3)
                });
            }
            return list;
        }

        // --- 3. OBTENER DETALLES (Productos de una venta) ---
        public List<FacturaDetalle> GetDetalles(long facturaId)
        {
            var list = new List<FacturaDetalle>();
            using var cn = _factory.Create();
            cn.Open();

            const string sql = @"
                SELECT d.ItemId, i.Nombre, d.Cantidad, d.PrecioUnitario
                FROM FacturaDetalle d
                INNER JOIN Item i ON d.ItemId = i.ItemId
                WHERE d.FacturaId = @id";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", facturaId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new FacturaDetalle
                {
                    ItemId = r.GetInt32(0),
                    NombreProducto = r.GetString(1),
                    Cantidad = r.GetInt32(2),
                    PrecioUnitario = r.GetDecimal(3)
                });
            }
            return list;
        }

        // --- 4. OBTENER TOTAL VENDIDO HOY (CORREGIDO) ---
        public decimal GetTotalVentasHoy()
        {
            using var cn = _factory.Create();
            cn.Open();

            // Usamos un rango de fechas exacto generado desde C# para evitar problemas de zona horaria del servidor
            const string sql = @"
                SELECT ISNULL(SUM(Total), 0) 
                FROM dbo.Factura 
                WHERE Fecha >= @inicio AND Fecha < @fin";

            using var cmd = new SqlCommand(sql, cn);

            DateTime hoy = DateTime.Today; // Hoy a las 00:00:00

            cmd.Parameters.AddWithValue("@inicio", hoy);
            cmd.Parameters.AddWithValue("@fin", hoy.AddDays(1)); // Mañana a las 00:00:00

            object result = cmd.ExecuteScalar();

            return Convert.ToDecimal(result);
        }
    }
}