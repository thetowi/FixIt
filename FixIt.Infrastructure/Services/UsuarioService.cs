using FixIt.Application.DTOs.Usuarios;
using FixIt.Application.Interfaces;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using FixIt.Domain.Entities;

namespace FixIt.Infrastructure.Services;

public class UsuarioService : IUsuarioService
{
    private readonly FixItDbContext _db;
    private readonly IStorageService _storageService;
    private static readonly GeometryFactory _geometryFactory =
        NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

    public UsuarioService(FixItDbContext db, IStorageService storageService)
    {
        _db = db;
        _storageService = storageService;
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
        usuario.UbicacionGeo = _geometryFactory.CreatePoint(new Coordinate(request.Longitud, request.Latitud));

        await _db.SaveChangesAsync();
    }

    public async Task<string> ActualizarFotoPerfilAsync(Guid usuarioId, Stream archivo, string contentType)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        var extension = contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => throw new InvalidOperationException("Formato de imagen no soportado. Usá JPG, PNG o WEBP.")
        };

        var nombreArchivo = $"{usuarioId}.{extension}";
        var url = await _storageService.SubirArchivoAsync("avatars", nombreArchivo, archivo, contentType);

        // Le agregamos un parámetro de fecha/hora para "romper" el caché del navegador
        // cuando el usuario actualiza su foto (si no, el navegador podría seguir mostrando
        // la imagen vieja aunque la URL en sí siga siendo la misma)
        usuario.FotoPerfilUrl = $"{url}?t={DateTimeOffset.UtcNow.Ticks}";
        await _db.SaveChangesAsync();

        return usuario.FotoPerfilUrl;
    }
        public async Task<PerfilPropioResponse> ObtenerPerfilPropioAsync(Guid usuarioId)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        return MapearAPerfilPropio(usuario);
    }

    public async Task<PerfilPropioResponse> ActualizarPerfilAsync(Guid usuarioId, ActualizarPerfilRequest request)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Apellido))
        {
            throw new InvalidOperationException("Nombre y apellido son obligatorios.");
        }

        usuario.Nombre = request.Nombre;
        usuario.Apellido = request.Apellido;
        usuario.Telefono = request.Telefono;

        await _db.SaveChangesAsync();

        return MapearAPerfilPropio(usuario);
    }
    public async Task MarcarTutorialVistoAsync(Guid usuarioId)
    {
        var usuario = await _db.Usuarios.FindAsync(usuarioId);
        if (usuario is null)
        {
            throw new InvalidOperationException("Usuario no encontrado.");
        }

        usuario.TutorialVisto = true;
        await _db.SaveChangesAsync();
    }

    private static PerfilPropioResponse MapearAPerfilPropio(Usuario usuario)
    {
        return new PerfilPropioResponse
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Telefono = usuario.Telefono,
            Rol = usuario.Rol.ToString(),
            FotoPerfilUrl = usuario.FotoPerfilUrl,
            Verificado = usuario.Verificado
        };
    }
}