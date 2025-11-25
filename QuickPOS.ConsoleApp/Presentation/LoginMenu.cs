using System;
using QuickPOS.Services;
using QuickPOS.Models;
using QuickPOS.UI;

namespace QuickPOS.Presentation;

public class LoginMenu
{
    private readonly AuthService _auth;

    public LoginMenu(AuthService auth)
    {
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
    }

    /// <summary>
    /// Muestra login y retorna el user autenticado o null si canceló.
    /// </summary>
    public User? Show()
    {
        while (true)
        {
            ConsoleUI.Clear();
            ConsoleUI.Header("LOGIN - QuickPOS");

            Console.Write("Usuario (o ENTER para salir): ");
            var user = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(user)) return null;

            Console.Write("Contraseña: ");
            // leer contraseña sin mostrar (simple)
            var pass = ReadPassword();

            var u = _auth.Authenticate(user.Trim(), pass);
            if (u != null)
            {
                Console.WriteLine($"\nBienvenido {u.Username} (Rol: {u.Role}). ENTER para continuar.");
                Console.ReadLine();
                return u;
            }

            Console.WriteLine("\nCredenciales inválidas. Presiona ENTER para reintentar o escribe 'exit' para salir.");
            var r = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(r) && r.Trim().Equals("exit", System.StringComparison.OrdinalIgnoreCase))
                return null;
        }
    }

    private static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo info;
        while ((info = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (info.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password = password.Substring(0, password.Length - 1);
                    Console.Write("\b \b");
                }
            }
            else if (!char.IsControl(info.KeyChar))
            {
                password += info.KeyChar;
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return password;
    }
}
