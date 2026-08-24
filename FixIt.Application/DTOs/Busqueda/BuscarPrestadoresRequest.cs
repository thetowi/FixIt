namespace FixIt.Application.DTOs.Busqueda;

public class BuscarPrestadoresRequest
{
    public int CategoriaId { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public double RadioKm { get; set; } = 10; // valor por defecto razonable
}