using FixIt.Application.DTOs.Prestadores;

namespace FixIt.Application.Interfaces;

public interface IPrestadorPerfilService
{
    Task<PerfilPrestadorResponse?> ObtenerPerfilAsync(Guid prestadorId);
}