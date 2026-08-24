namespace FixIt.Application.DTOs.Admin;

public class CategoriaAdminResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Icono { get; set; }
    public bool Activa { get; set; }
}