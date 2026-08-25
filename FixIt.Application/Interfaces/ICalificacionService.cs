using FixIt.Application.DTOs.Calificaciones;

namespace FixIt.Application.Interfaces;

public interface ICalificacionService
{
    Task<CalificacionResponse> CrearAsync(Guid clienteId, Guid ordenId, CrearCalificacionRequest request);
    Task<List<CalificacionResponse>> ListarPorPrestadorAsync(Guid prestadorId);
}