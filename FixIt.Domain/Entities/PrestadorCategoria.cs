namespace FixIt.Domain.Entities;

public class PrestadorCategoria
{
    public int Id { get; set; }

    public Guid PrestadorId { get; set; }
    public Usuario Prestador { get; set; } = null!;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
}