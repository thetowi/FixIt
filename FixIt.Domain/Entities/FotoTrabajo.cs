namespace FixIt.Domain.Entities;

public class FotoTrabajo
{
    public Guid Id { get; set; }

    public Guid PrestadorId { get; set; }
    public Usuario Prestador { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
}