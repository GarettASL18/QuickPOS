using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuickPOS.Models;

namespace QuickPOS.Data
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly SqlConnectionFactory _factory;
        public UsuarioRepository(SqlConnectionFactory factory) => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        public Usuario? GetById(int id)
        {
            const string sql = "SELECT UsuarioId, Username, PasswordHash, Role FROM dbo.Usuario WHERE UsuarioId=@id;";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Usuario { UsuarioId = r.GetInt32(0), Username = r.GetString(1), PasswordHash = r.GetString(2), Role = r.GetString(3) };
        }

        public Usuario? GetByUsername(string username)
        {
            const string sql = "SELECT UsuarioId, Username, PasswordHash, Role FROM dbo.Usuario WHERE Username=@u;";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u", username);
            cn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Usuario { UsuarioId = r.GetInt32(0), Username = r.GetString(1), PasswordHash = r.GetString(2), Role = r.GetString(3) };
        }

        public List<Usuario> GetAll()
        {
            var list = new List<Usuario>();
            const string sql = "SELECT UsuarioId, Username, PasswordHash, Role FROM dbo.Usuario ORDER BY UsuarioId DESC;";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Usuario { UsuarioId = r.GetInt32(0), Username = r.GetString(1), PasswordHash = r.GetString(2), Role = r.GetString(3) });
            }
            return list;
        }

        public int Create(Usuario u)
        {
            if (u == null) throw new ArgumentNullException(nameof(u));
            const string sql = @"
                INSERT INTO dbo.Usuario (Username, PasswordHash, Role)
                VALUES (@u, @p, @r);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u", u.Username);
            cmd.Parameters.AddWithValue("@p", u.PasswordHash);
            cmd.Parameters.AddWithValue("@r", u.Role);
            cn.Open();
            var obj = cmd.ExecuteScalar();
            return Convert.ToInt32(obj);
        }

        public void Update(Usuario u)
        {
            if (u == null) throw new ArgumentNullException(nameof(u));
            const string sql = "UPDATE dbo.Usuario SET Username=@u, PasswordHash=@p, Role=@r WHERE UsuarioId=@id;";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@u", u.Username);
            cmd.Parameters.AddWithValue("@p", u.PasswordHash);
            cmd.Parameters.AddWithValue("@r", u.Role);
            cmd.Parameters.AddWithValue("@id", u.UsuarioId);
            cn.Open();
            var rows = cmd.ExecuteNonQuery();
            if (rows == 0) throw new InvalidOperationException("Usuario no encontrado.");
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM dbo.Usuario WHERE UsuarioId=@id;";
            using var cn = _factory.Create();
            using var cmd = new SqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            var rows = cmd.ExecuteNonQuery();
            if (rows == 0) throw new InvalidOperationException("Usuario no encontrado.");
        }
    }
}
