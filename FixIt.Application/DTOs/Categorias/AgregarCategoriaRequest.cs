namespace FixIt.Application.DTOs.Categorias;

public class AgregarCategoriaRequest
{
    public int CategoriaId { get; set; }
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
}