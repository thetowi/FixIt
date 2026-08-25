namespace FixIt.Application.DTOs.Prestadores;

public class FotoTrabajoResponse
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}