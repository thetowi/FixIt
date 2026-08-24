using FixIt.Application.DTOs.Categorias;
using FixIt.Application.Interfaces;

namespace FixIt.Application.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaResponse>> ListarActivasAsync();
    Task<List<PrestadorCategoriaResponse>> ListarMisCategoriasAsync(Guid prestadorId);
    Task<PrestadorCategoriaResponse> AgregarAsync(Guid prestadorId, AgregarCategoriaRequest request);
    Task QuitarAsync(Guid prestadorId, int prestadorCategoriaId);
}