using Microsoft.Data.SqlClient;
using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data;

public class ClienteRepository : IClienteRepository
{
    private readonly SqlConnectionFactory _factory;

    public ClienteRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public Cliente? GetById(int id)
    {
        using var cn = _factory.Create();
        cn.Open();

        string sql = @"
            SELECT ClienteId, Nombre, NIT
            FROM Cliente
            WHERE ClienteId = @id;
        ";

        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@id", id);

        using var dr = cmd.ExecuteReader();
        if (!dr.Read()) return null;

        return new Cliente
        {
            ClienteId = dr.GetInt32(0),
            Nombre = dr.GetString(1),
            NIT = dr.IsDBNull(2) ? null : dr.GetString(2)
        };
    }

    public List<Cliente> GetAll()
    {
        var list = new List<Cliente>();

        using var cn = _factory.Create();
        cn.Open();

        string sql = "SELECT ClienteId, Nombre, NIT FROM Cliente;";

        using var cmd = new SqlCommand(sql, cn);
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            list.Add(new Cliente
            {
                ClienteId = dr.GetInt32(0),
                Nombre = dr.GetString(1),
                NIT = dr.IsDBNull(2) ? null : dr.GetString(2)
            });
        }

        return list;
    }

    public void Create(Cliente c)
    {
        using var cn = _factory.Create();
        cn.Open();

        string sql = @"
            INSERT INTO Cliente(Nombre, NIT)
            VALUES (@nom, @nit);
        ";

        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@nom", c.Nombre);
        cmd.Parameters.AddWithValue("@nit", (object?)c.NIT ?? DBNull.Value);

        cmd.ExecuteNonQuery();
    }
}
