namespace FixIt.Application.DTOs.Categorias;

public class PrestadorCategoriaResponse
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
}