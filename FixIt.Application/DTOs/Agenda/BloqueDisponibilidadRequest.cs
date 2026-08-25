namespace FixIt.Application.DTOs.Agenda;

public class BloqueDisponibilidadRequest
{
    public DayOfWeek DiaSemana { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
}