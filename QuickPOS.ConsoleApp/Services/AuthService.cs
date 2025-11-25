using System.Collections.Generic;
using System.Linq;
using QuickPOS.Models;

namespace QuickPOS.Services;

public class AuthService
{
    private readonly List<User> _users;

    public AuthService()
    {
        // Usuarios de ejemplo. Cambiar luego por repositorio/BD.
        _users = new List<User>
        {
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "empleado", Password = "emp123", Role = "Employee" }
        };
    }

    /// <summary>
    /// Intenta autenticar; retorna el usuario si OK, null si falla.
    /// </summary>
    public User? Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return null;

        // comparación simple; en producción debe usar hash + salt
        return _users.FirstOrDefault(u =>
            u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase)
            && u.Password == password);
    }

    /// <summary>
    /// Agregar usuario en memoria (útil para pruebas)
    /// </summary>
    public void AddUser(User u)
    {
        if (u == null) return;
        _users.Add(u);
    }
}
