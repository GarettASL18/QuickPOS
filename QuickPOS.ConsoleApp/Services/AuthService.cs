using QuickPOS.Data;
using QuickPOS.Models;

public class AuthService
{
    private readonly IUsuarioRepository _repo;
    public AuthService(IUsuarioRepository repo) => _repo = repo;

    public User? Authenticate(string username, string password)
    {
        var u = _repo.GetByUsername(username);
        if (u == null) return null;
        // DEMO: comparación en texto plano
        if (u.PasswordHash != password) return null;
        return new User { Username = u.Username, Password = "", Role = u.Role };
    }
}
