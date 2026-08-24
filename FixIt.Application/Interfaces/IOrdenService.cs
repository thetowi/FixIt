using FixIt.Application.DTOs.Ordenes;

namespace FixIt.Application.Interfaces;

public interface IOrdenService
{
    Task<OrdenResponse> CrearAsync(Guid clienteId, CrearOrdenRequest request);
    Task<List<OrdenResponse>> ListarMisOrdenesAsync(Guid usuarioId);
}