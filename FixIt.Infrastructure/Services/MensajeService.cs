using FixIt.Application.DTOs.Mensajes;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Services;

public class MensajeService : IMensajeService
{
    private readonly FixItDbContext _db;

    public MensajeService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<bool> UsuarioPerteneceALaOrdenAsync(Guid ordenId, Guid usuarioId)
    {
        return await _db.Ordenes.AnyAsync(o =>
            o.Id == ordenId && (o.ClienteId == usuarioId || o.PrestadorId == usuarioId));
    }

    public async Task<List<MensajeResponse>> ListarHistorialAsync(Guid ordenId)
    {
        return await _db.Mensajes
            .Where(m => m.OrdenId == ordenId)
            .Include(m => m.Emisor)
            .OrderBy(m => m.EnviadoEn)
            .Select(m => new MensajeResponse
            {
                Id = m.Id,
                OrdenId = m.OrdenId,
                EmisorId = m.EmisorId,
                EmisorNombre = m.Emisor.Nombre,
                Contenido = m.Contenido,
                EnviadoEn = m.EnviadoEn
            })
            .ToListAsync();
    }

    public async Task<MensajeResponse> GuardarMensajeAsync(Guid ordenId, Guid emisorId, string contenido)
    {
        var emisor = await _db.Usuarios.FindAsync(emisorId);

        var mensaje = new Mensaje
        {
            Id = Guid.NewGuid(),
            OrdenId = ordenId,
            EmisorId = emisorId,
            Contenido = contenido
        };

        _db.Mensajes.Add(mensaje);
        await _db.SaveChangesAsync();

        return new MensajeResponse
        {
            Id = mensaje.Id,
            OrdenId = mensaje.OrdenId,
            EmisorId = mensaje.EmisorId,
            EmisorNombre = emisor!.Nombre,
            Contenido = mensaje.Contenido,
            EnviadoEn = mensaje.EnviadoEn
        };
    }
}