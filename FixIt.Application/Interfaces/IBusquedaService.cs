using FixIt.Application.DTOs.Busqueda;

namespace FixIt.Application.Interfaces;

public interface IBusquedaService
{
    Task<List<PrestadorEncontradoResponse>> BuscarPrestadoresAsync(BuscarPrestadoresRequest request);
    Task<List<PrestadorDestacadoResponse>> ObtenerDestacadosAsync(double? latitud, double? longitud, int limite = 6);
}