namespace FixIt.Application.DTOs.Mensajes;

public class MensajeResponse
{
    public Guid Id { get; set; }
    public Guid OrdenId { get; set; }
    public Guid EmisorId { get; set; }
    public string EmisorNombre { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
    public DateTimeOffset EnviadoEn { get; set; }
}