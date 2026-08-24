using FixIt.Application.DTOs.Usuarios;
using FixIt.Application.Interfaces;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace FixIt.Infrastructure.Services;

public class UsuarioService : IUsuarioService
{
    private readonly FixItDbContext _db;
    private static readonly GeometryFactory _geometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public UsuarioService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task ActualizarUbicacionAsync(Guid usuarioId, ActualizarUbicacionRequest request)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        usuario.Latitud = request.Latitud;
        usuario.Longitud = request.Longitud;

        // Ojo: Point(X, Y) = Point(longitud, latitud) — orden invertido respecto a como lo pensamos en español
        usuario.UbicacionGeo = _geometryFactory.CreatePoint(new Coordinate(request.Longitud, request.Latitud));

        await _db.SaveChangesAsync();
    }
}