namespace QuickPOS.Models;

public class User
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = ""; // en producción NO guardar plaintext
    public string Role { get; set; } = ""; // "Admin" o "Employee"
}
