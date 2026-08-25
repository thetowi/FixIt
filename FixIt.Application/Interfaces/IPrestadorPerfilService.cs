using FixIt.Application.DTOs.Prestadores;

namespace FixIt.Application.Interfaces;

public interface IPrestadorPerfilService
{
    Task<PerfilPrestadorResponse?> ObtenerPerfilAsync(Guid prestadorId);
    Task ActualizarAcercaDeMiAsync(Guid prestadorId, ActualizarAcercaDeMiRequest request);
    Task<FotoTrabajoResponse> AgregarFotoTrabajoAsync(Guid prestadorId, Stream archivo, string contentType, string? descripcion);
    Task EliminarFotoTrabajoAsync(Guid prestadorId, Guid fotoId);
}