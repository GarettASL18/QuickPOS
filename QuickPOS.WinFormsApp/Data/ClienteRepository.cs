using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuickPOS.Models;

namespace QuickPOS.Data
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SqlConnectionFactory _factory;

        public ClienteRepository(SqlConnectionFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        // --- LEER POR ID ---
        public Cliente? GetById(int id)
        {
            using var cn = _factory.Create();
            cn.Open();

            string sql = "SELECT ClienteId, Nombre, NIT FROM Cliente WHERE ClienteId = @id;";

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

        // --- LEER TODOS ---
        public List<Cliente> GetAll()
        {
            var list = new List<Cliente>();

            using var cn = _factory.Create();
            cn.Open();

            string sql = "SELECT ClienteId, Nombre, NIT FROM Cliente ORDER BY Nombre;";

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

        // --- CREAR (Implementado correctamente) ---
        public int Create(Cliente c)
        {
            using var cn = _factory.Create();
            cn.Open();

            // Insertar y devolver ID
            string sql = @"
                INSERT INTO dbo.Cliente (Nombre, NIT) 
                VALUES (@nombre, @nit);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@nit", (object?)c.NIT ?? DBNull.Value);

            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt32(idObj);
        }

        // --- ACTUALIZAR (Implementado correctamente) ---
        public void Update(Cliente c)
        {
            using var cn = _factory.Create();
            cn.Open();

            string sql = @"
                UPDATE dbo.Cliente 
                SET Nombre = @nombre, NIT = @nit 
                WHERE ClienteId = @id;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@nit", (object?)c.NIT ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", c.ClienteId);

            cmd.ExecuteNonQuery();
        }

        // --- ELIMINAR (Implementado correctamente) ---
        public void Delete(int id)
        {
            using var cn = _factory.Create();
            cn.Open();

            string sql = "DELETE FROM dbo.Cliente WHERE ClienteId = @id;";

            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
    }
}