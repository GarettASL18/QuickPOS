using Microsoft.Data.SqlClient;
using QuickPOS.Models;
using System.Collections.Generic;

namespace QuickPOS.Data;

public class ItemRepository : IItemRepository
{
    private readonly SqlConnectionFactory _factory;

    public ItemRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public Item? GetById(int id)
    {
        using var cn = _factory.Create();
        cn.Open();

        string sql = @"
            SELECT ItemId, Nombre, Precio, Activo 
            FROM Item 
            WHERE ItemId = @id;
        ";

        using var cmd = new SqlCommand(sql, cn);
        cmd.Parameters.AddWithValue("@id", id);

        using var dr = cmd.ExecuteReader();
        if (!dr.Read()) return null;

        return new Item
        {
            ItemId = dr.GetInt32(0),
            Nombre = dr.GetString(1),
            Precio = dr.GetDecimal(2),
            Activo = dr.GetBoolean(3)
        };
    }

    public List<Item> GetAll()
    {
        var list = new List<Item>();

        using var cn = _factory.Create();
        cn.Open();

        string sql = "SELECT ItemId, Nombre, Precio, Activo FROM Item;";

        using var cmd = new SqlCommand(sql, cn);
        using var dr = cmd.ExecuteReader();

        while (dr.Read())
        {
            list.Add(new Item
            {
                ItemId = dr.GetInt32(0),
                Nombre = dr.GetString(1),
                Precio = dr.GetDecimal(2),
                Activo = dr.GetBoolean(3)
            });
        }

        return list;
    }
}
