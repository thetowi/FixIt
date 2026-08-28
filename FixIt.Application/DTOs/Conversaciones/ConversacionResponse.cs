namespace FixIt.Application.DTOs.Conversaciones;

public class ConversacionResponse
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public Guid PrestadorId { get; set; }
    public string PrestadorNombreCompleto { get; set; } = string.Empty;
    public string ClienteNombreCompleto { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
}