namespace FixIt.Application.DTOs.Auth;

public class RegistroRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "cliente" o "prestador" (Admin no se crea por acá)
}