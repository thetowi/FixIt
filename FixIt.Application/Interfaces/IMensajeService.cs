using FixIt.Application.DTOs.Mensajes;

namespace FixIt.Application.Interfaces;

public interface IMensajeService
{
    Task<bool> UsuarioPerteneceALaOrdenAsync(Guid ordenId, Guid usuarioId);
    Task<List<MensajeResponse>> ListarHistorialAsync(Guid ordenId);
    Task<MensajeResponse> GuardarMensajeAsync(Guid ordenId, Guid emisorId, string contenido);
}