namespace FixIt.Domain.Entities;

public class Mensaje
{
    public Guid Id { get; set; }

    public Guid OrdenId { get; set; }
    public Orden Orden { get; set; } = null!;

    public Guid EmisorId { get; set; }
    public Usuario Emisor { get; set; } = null!;

    public string Contenido { get; set; } = string.Empty;
    public DateTimeOffset EnviadoEn { get; set; } = DateTimeOffset.UtcNow;
    public bool Leido { get; set; } = false;
}