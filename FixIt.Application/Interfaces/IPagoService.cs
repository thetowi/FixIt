using FixIt.Application.DTOs.Pagos;

namespace FixIt.Application.Interfaces;

public interface IPagoService
{
    Task<CrearPreferenciaResponse> CrearPreferenciaAsync(Guid ordenId, Guid clienteId);
}