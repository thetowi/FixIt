namespace FixIt.Application.DTOs.Ordenes;

public class CrearOrdenRequest
{
    public Guid PrestadorId { get; set; }
    public int CategoriaId { get; set; }
    public decimal MontoTotal { get; set; }
}