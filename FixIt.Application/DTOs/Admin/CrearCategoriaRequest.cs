namespace FixIt.Application.DTOs.Admin;

public class CrearCategoriaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Icono { get; set; }
}