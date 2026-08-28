namespace FixIt.Application.DTOs.Auth;

public class UsuarioResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool TutorialVisto { get; set; }
}