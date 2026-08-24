using FixIt.Application.DTOs.Busqueda;

namespace FixIt.Application.Interfaces;

public interface IBusquedaService
{
    Task<List<PrestadorEncontradoResponse>> BuscarPrestadoresAsync(BuscarPrestadoresRequest request);
}