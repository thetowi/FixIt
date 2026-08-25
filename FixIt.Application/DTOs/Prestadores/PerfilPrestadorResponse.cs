namespace FixIt.Application.DTOs.Prestadores;

public class PerfilPrestadorResponse
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public bool Verificado { get; set; }
    public string? FotoPerfilUrl { get; set; }
    public DateTimeOffset MiembroDesde { get; set; }
    public double? PromedioCalificacion { get; set; }
    public int CantidadCalificaciones { get; set; }
    public List<ServicioOfrecidoResponse> Servicios { get; set; } = new();
}

public class ServicioOfrecidoResponse
{
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
}