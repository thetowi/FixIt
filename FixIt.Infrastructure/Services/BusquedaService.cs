using FixIt.Application.DTOs.Busqueda;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
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
        public async Task<List<PrestadorDestacadoResponse>> ObtenerDestacadosAsync(double? latitud, double? longitud, int limite = 6)
    {
        IQueryable<Usuario> query = _db.Usuarios.Where(u => u.Rol == RolUsuario.Prestador);

        Point? punto = null;
        if (latitud.HasValue && longitud.HasValue)
        {
            punto = _geometryFactory.CreatePoint(new Coordinate(longitud.Value, latitud.Value));
            const double radioMetrosAmplio = 50000; // 50km, red amplia para "destacados cerca tuyo"
            query = query.Where(u => u.UbicacionGeo != null && u.UbicacionGeo.IsWithinDistance(punto, radioMetrosAmplio));
        }

        var baseData = punto is null
            ? await query.Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Apellido,
                    u.FotoPerfilUrl,
                    u.Verificado,
                    Distancia = (double?)null,
                    Categorias = u.PrestadorCategorias.Select(pc => pc.Categoria.Nombre).ToList()
                }).ToListAsync()
            : await query.Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Apellido,
                    u.FotoPerfilUrl,
                    u.Verificado,
                    Distancia = (double?)(u.UbicacionGeo!.Distance(punto) / 1000),
                    Categorias = u.PrestadorCategorias.Select(pc => pc.Categoria.Nombre).ToList()
                }).ToListAsync();

        var ids = baseData.Select(p => p.Id).ToList();

        var calificaciones = await _db.Calificaciones
            .Where(c => ids.Contains(c.Orden.PrestadorId))
            .Select(c => new { PrestadorId = c.Orden.PrestadorId, c.Puntuacion })
            .ToListAsync();

        var promediosPorPrestador = calificaciones
            .GroupBy(c => c.PrestadorId)
            .ToDictionary(g => g.Key, g => (Promedio: g.Average(x => (double)x.Puntuacion), Cantidad: g.Count()));

        var resultado = baseData.Select(p =>
        {
            promediosPorPrestador.TryGetValue(p.Id, out var stats);
            return new PrestadorDestacadoResponse
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Verificado = p.Verificado,
                FotoPerfilUrl = p.FotoPerfilUrl,
                PromedioCalificacion = stats.Cantidad > 0 ? stats.Promedio : null,
                CantidadCalificaciones = stats.Cantidad,
                DistanciaKm = p.Distancia,
                Categorias = p.Categorias
            };
        })
        .OrderByDescending(p => p.PromedioCalificacion ?? -1)
        .ThenByDescending(p => p.CantidadCalificaciones)
        .ThenBy(p => p.DistanciaKm ?? double.MaxValue)
        .Take(limite)
        .ToList();

        return resultado;
    }
}