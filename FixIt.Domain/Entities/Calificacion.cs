namespace FixIt.Domain.Entities;

public class Calificacion
{
    public Guid Id { get; set; }

    public Guid OrdenId { get; set; }
    public Orden Orden { get; set; } = null!;

    public short Puntuacion { get; set; } // 1 a 5
    public string? Comentario { get; set; }
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
}