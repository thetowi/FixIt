namespace FixIt.Application.DTOs.Conversaciones;

public class IniciarConversacionRequest
{
    public Guid PrestadorId { get; set; }
    public int CategoriaId { get; set; }
}