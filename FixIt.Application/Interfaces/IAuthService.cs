using FixIt.Application.DTOs.Auth;

namespace FixIt.Application.Interfaces;

public interface IAuthService
{
    Task<UsuarioResponse> RegistrarAsync(RegistroRequest request);
}