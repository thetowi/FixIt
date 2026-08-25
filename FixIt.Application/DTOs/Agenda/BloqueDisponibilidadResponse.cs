namespace FixIt.Application.DTOs.Agenda;

public class BloqueDisponibilidadResponse
{
    public int Id { get; set; }
    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
}