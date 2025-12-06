namespace QuickPOS.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        // --- NUEVA PROPIEDAD ---
        public string? Email { get; set; } // Puede ser nulo (null)
    }
}