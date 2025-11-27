using Microsoft.Data.SqlClient;
using QuickPOS.Models;
using System;
using System.Collections.Generic;

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

            const string sql = @"
                SELECT ItemId, Nombre, Precio, Activo 
                FROM dbo.Item 
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

            const string sql = @"SELECT ItemId, Nombre, Precio, Activo 
                                 FROM dbo.Item
                                 ORDER BY ItemId DESC;";

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

        public int Create(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            using var cn = _factory.Create();
            cn.Open();

            const string sql = @"
                INSERT INTO dbo.Item (Nombre, Precio, Activo)
                VALUES (@nombre, @precio, @activo);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", item.Nombre);
            cmd.Parameters.AddWithValue("@precio", item.Precio);
            cmd.Parameters.AddWithValue("@activo", item.Activo);

            var idObj = cmd.ExecuteScalar();
            if (idObj == null || idObj == DBNull.Value)
                throw new InvalidOperationException("No se pudo obtener el ID del item.");

            return Convert.ToInt32(idObj);
        }

        public void Update(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            using var cn = _factory.Create();
            cn.Open();

            const string sql = @"
                UPDATE dbo.Item
                SET Nombre = @nombre,
                    Precio = @precio,
                    Activo = @activo
                WHERE ItemId = @id;
            ";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", item.Nombre);
            cmd.Parameters.AddWithValue("@precio", item.Precio);
            cmd.Parameters.AddWithValue("@activo", item.Activo);
            cmd.Parameters.AddWithValue("@id", item.ItemId);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException("No se encontró el item a actualizar.");
        }

        public void Delete(int id)
        {
            using var cn = _factory.Create();
            cn.Open();

            const string sql = @"DELETE FROM dbo.Item WHERE ItemId = @id;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", id);

            var rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new InvalidOperationException("No se encontró el item a eliminar.");
        }
    }
}
