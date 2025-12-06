using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuickPOS.Models;

namespace QuickPOS.Data
{
    public class ItemRepository : IItemRepository
    {
        private readonly SqlConnectionFactory _factory;

        public ItemRepository(SqlConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public Item? GetById(int id)
        {
            using var cn = _factory.Create();
            cn.Open();
            var cmd = new SqlCommand("SELECT ItemId, Nombre, Precio, Activo FROM Item WHERE ItemId = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);

            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Map(r);
        }

        public List<Item> GetAll()
        {
            var list = new List<Item>();
            using var cn = _factory.Create();
            cn.Open();
            var cmd = new SqlCommand("SELECT ItemId, Nombre, Precio, Activo FROM Item ORDER BY Nombre", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(Map(r));
            return list;
        }

        public int Create(Item item)
        {
            using var cn = _factory.Create();
            cn.Open();
            var cmd = new SqlCommand(@"
                INSERT INTO Item (Nombre, Precio, Activo) 
                VALUES (@n, @p, @a);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", cn);

            cmd.Parameters.AddWithValue("@n", item.Nombre);
            cmd.Parameters.AddWithValue("@p", item.Precio);
            cmd.Parameters.AddWithValue("@a", item.Activo);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Update(Item item)
        {
            using var cn = _factory.Create();
            cn.Open();
            var cmd = new SqlCommand(@"
                UPDATE Item 
                SET Nombre=@n, Precio=@p, Activo=@a 
                WHERE ItemId=@id", cn);

            cmd.Parameters.AddWithValue("@n", item.Nombre);
            cmd.Parameters.AddWithValue("@p", item.Precio);
            cmd.Parameters.AddWithValue("@a", item.Activo);
            cmd.Parameters.AddWithValue("@id", item.ItemId);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var cn = _factory.Create();
            cn.Open();
            var cmd = new SqlCommand("DELETE FROM Item WHERE ItemId=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private Item Map(SqlDataReader r)
        {
            return new Item
            {
                ItemId = r.GetInt32(0),
                Nombre = r.GetString(1),
                Precio = r.GetDecimal(2),
                Activo = r.GetBoolean(3)
            };
        }
    }
}