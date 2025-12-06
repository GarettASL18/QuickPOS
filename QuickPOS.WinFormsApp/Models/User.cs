namespace QuickPOS.Models
{
    public class User
    {
        // --- AGREGAR ESTA PROPIEDAD QUE FALTABA ---
        public int UsuarioId { get; set; }
        // ------------------------------------------

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}