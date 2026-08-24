using FixIt.Application.DTOs.Admin;

namespace FixIt.Application.Interfaces;

public interface IAdminService
{
    Task<List<CategoriaAdminResponse>> ListarTodasLasCategoriasAsync();
    Task<CategoriaAdminResponse> CrearCategoriaAsync(CrearCategoriaRequest request);
    Task CambiarEstadoCategoriaAsync(int categoriaId, bool activa);
    Task<List<UsuarioAdminResponse>> ListarUsuariosAsync();
}