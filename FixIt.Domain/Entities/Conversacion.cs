namespace FixIt.Domain.Entities;

public class Conversacion
{
    public Guid Id { get; set; }

    public Guid ClienteId { get; set; }
    public Usuario Cliente { get; set; } = null!;

    public Guid PrestadorId { get; set; }
    public Usuario Prestador { get; set; } = null!;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
}