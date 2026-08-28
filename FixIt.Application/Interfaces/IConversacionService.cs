using FixIt.Application.DTOs.Conversaciones;

namespace FixIt.Application.Interfaces;

public interface IConversacionService
{
    Task<ConversacionResponse> IniciarOEncontrarAsync(Guid clienteId, IniciarConversacionRequest request);
    Task<List<ConversacionResponse>> ListarMisConversacionesAsync(Guid usuarioId);
}