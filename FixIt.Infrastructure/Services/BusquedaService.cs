using FixIt.Application.DTOs.Busqueda;
using FixIt.Application.Interfaces;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FixIt.Infrastructure.Services;

public class BusquedaService : IBusquedaService
{
    private readonly FixItDbContext _db;
    private static readonly GeometryFactory _geometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public BusquedaService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<List<PrestadorEncontradoResponse>> BuscarPrestadoresAsync(BuscarPrestadoresRequest request)
    {
        var puntoBusqueda = _geometryFactory.CreatePoint(
            new Coordinate(request.Longitud, request.Latitud));

        var radioMetros = request.RadioKm * 1000;

        var resultado = await _db.PrestadorCategorias
            .Where(pc => pc.CategoriaId == request.CategoriaId)
            .Where(pc => pc.Prestador.UbicacionGeo != null &&
                         pc.Prestador.UbicacionGeo.IsWithinDistance(puntoBusqueda, radioMetros))
            .Select(pc => new PrestadorEncontradoResponse
            {
                Id = pc.Prestador.Id,
                Nombre = pc.Prestador.Nombre,
                Apellido = pc.Prestador.Apellido,
                Verificado = pc.Prestador.Verificado,
                FotoPerfilUrl = pc.Prestador.FotoPerfilUrl,
                Descripcion = pc.Descripcion,
                PrecioReferencia = pc.PrecioReferencia,
                DistanciaKm = pc.Prestador.UbicacionGeo!.Distance(puntoBusqueda) / 1000
            })
            .OrderBy(r => r.DistanciaKm)
            .ToListAsync();

        return resultado;
    }
}