using FixIt.Application.DTOs.Agenda;

namespace FixIt.Application.Interfaces;

public interface IAgendaService
{
    Task<List<BloqueDisponibilidadResponse>> ObtenerDisponibilidadAsync(Guid prestadorId);
    Task<BloqueDisponibilidadResponse> AgregarBloqueAsync(Guid prestadorId, BloqueDisponibilidadRequest request);
    Task EliminarBloqueAsync(Guid prestadorId, int bloqueId);
    Task ProgramarTurnoAsync(Guid prestadorId, Guid ordenId, ProgramarTurnoRequest request);
    Task<List<OrdenAgendaResponse>> ObtenerAgendaAsync(Guid prestadorId, DateTimeOffset desde, DateTimeOffset hasta);
    Task<List<OrdenAgendaResponse>> ObtenerSinProgramarAsync(Guid prestadorId);
}