using FixIt.Application.DTOs.Usuarios;

namespace FixIt.Application.Interfaces;

public interface IUsuarioService
{
    Task ActualizarUbicacionAsync(Guid usuarioId, ActualizarUbicacionRequest request);
}