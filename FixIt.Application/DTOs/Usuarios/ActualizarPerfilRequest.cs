namespace FixIt.Application.DTOs.Usuarios;

public class ActualizarPerfilRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}