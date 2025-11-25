using Microsoft.Data.SqlClient;
using QuickPOS.Models;
using System.Collections.Generic;
using System.Data;

namespace QuickPOS.Data;

public class FacturaRepository : IFacturaRepository
{
    private readonly SqlConnectionFactory _factory;

    public FacturaRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public long CreateFactura(Factura factura)
    {
        using var cn = _factory.Create();
        cn.Open();

        using var tx = cn.BeginTransaction();

        try
        {
            // Insertar factura
            using var cmdFact = new SqlCommand(@"
                INSERT INTO Factura (ClienteId, Subtotal, Impuesto, Total)
                VALUES (@cli, @sub, @imp, @tot);

                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            ", cn, tx);

            cmdFact.Parameters.AddWithValue("@cli", (object?)factura.ClienteId ?? DBNull.Value);
            cmdFact.Parameters.AddWithValue("@sub", factura.Subtotal);
            cmdFact.Parameters.AddWithValue("@imp", factura.Impuesto);
            cmdFact.Parameters.AddWithValue("@tot", factura.Total);

            long facturaId = Convert.ToInt64(cmdFact.ExecuteScalar());

            // Insertar detalles
            using var cmdDet = new SqlCommand(@"
                INSERT INTO FacturaDetalle(FacturaId, ItemId, Cantidad, PrecioUnitario)
                VALUES (@fac, @item, @cant, @precio);
            ", cn, tx);

            cmdDet.Parameters.Add("@fac", SqlDbType.BigInt);
            cmdDet.Parameters.Add("@item", SqlDbType.Int);
            cmdDet.Parameters.Add("@cant", SqlDbType.Int);
            cmdDet.Parameters.Add("@precio", SqlDbType.Decimal).Precision = 18;
            cmdDet.Parameters["@precio"].Scale = 2;

            foreach (var d in factura.Detalles)
            {
                cmdDet.Parameters["@fac"].Value = facturaId;
                cmdDet.Parameters["@item"].Value = d.ItemId;
                cmdDet.Parameters["@cant"].Value = d.Cantidad;
                cmdDet.Parameters["@precio"].Value = d.PrecioUnitario;

                cmdDet.ExecuteNonQuery();
            }

            tx.Commit();
            return facturaId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public Factura? GetById(long id)
    {
        using var cn = _factory.Create();
        cn.Open();

        Factura? f = null;

        // Factura
        using (var cmd = new SqlCommand(@"
            SELECT FacturaId, ClienteId, Fecha, Subtotal, Impuesto, Total
            FROM Factura
            WHERE FacturaId = @id;
        ", cn))
        {
            cmd.Parameters.AddWithValue("@id", id);

            using var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                f = new Factura
                {
                    FacturaId = dr.GetInt64(0),
                    ClienteId = dr.IsDBNull(1) ? null : dr.GetInt32(1),
                    Fecha = dr.GetDateTime(2),
                    Subtotal = dr.GetDecimal(3),
                    Impuesto = dr.GetDecimal(4),
                    Total = dr.GetDecimal(5)
                };
            }
        }

        if (f == null) return null;

        // Detalles
        using (var cmd2 = new SqlCommand(@"
            SELECT DetalleId, FacturaId, ItemId, Cantidad, PrecioUnitario
            FROM FacturaDetalle
            WHERE FacturaId = @id;
        ", cn))
        {
            cmd2.Parameters.AddWithValue("@id", id);

            using var dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                f.Detalles.Add(new FacturaDetalle
                {
                    DetalleId = dr2.GetInt64(0),
                    FacturaId = dr2.GetInt64(1),
                    ItemId = dr2.GetInt32(2),
                    Cantidad = dr2.GetInt32(3),
                    PrecioUnitario = dr2.GetDecimal(4)
                });
            }
        }

        return f;
    }

    public List<Factura> GetAll()
    {
        var list = new List<Factura>();

        using var cn = _factory.Create();
        cn.Open();

        using var cmd = new SqlCommand(@"
            SELECT FacturaId, ClienteId, Fecha, Subtotal, Impuesto, Total
            FROM Factura;
        ", cn);

        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            list.Add(new Factura
            {
                FacturaId = dr.GetInt64(0),
                ClienteId = dr.IsDBNull(1) ? null : dr.GetInt32(1),
                Fecha = dr.GetDateTime(2),
                Subtotal = dr.GetDecimal(3),
                Impuesto = dr.GetDecimal(4),
                Total = dr.GetDecimal(5)
            });
        }

        return list;
    }
}
