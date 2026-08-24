using FixIt.Domain.Entities;

namespace FixIt.Application.Interfaces;

public interface IJwtService
{
    string GenerarToken(Usuario usuario);
}