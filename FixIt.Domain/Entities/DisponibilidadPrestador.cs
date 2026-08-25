namespace FixIt.Domain.Entities;

public class DisponibilidadPrestador
{
    public int Id { get; set; }

    public Guid PrestadorId { get; set; }
    public Usuario Prestador { get; set; } = null!;

    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
}