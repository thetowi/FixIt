namespace FixIt.Application.DTOs.Busqueda;

public class PrestadorEncontradoResponse
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public bool Verificado { get; set; }
    public string? FotoPerfilUrl { get; set; }
    public string? Descripcion { get; set; }
    public decimal? PrecioReferencia { get; set; }
    public double? DistanciaKm { get; set; }
    public double? PromedioCalificacion { get; set; }
    public int CantidadCalificaciones { get; set; }
}