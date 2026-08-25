namespace FixIt.Application.DTOs.Auth;

public class CompletarRegistroGoogleRequest
{
    public string IdToken { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "cliente" o "prestador"
}