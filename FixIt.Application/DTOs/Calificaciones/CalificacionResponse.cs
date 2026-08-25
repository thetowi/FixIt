namespace FixIt.Application.DTOs.Calificaciones;

public class CalificacionResponse
{
    public Guid Id { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public short Puntuacion { get; set; }
    public string? Comentario { get; set; }
    public DateTimeOffset CreadoEn { get; set; }
}