namespace FixIt.Application.DTOs.Categorias;

public class CategoriaResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Icono { get; set; }
}