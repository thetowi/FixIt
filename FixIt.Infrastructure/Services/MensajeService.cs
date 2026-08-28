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

    public async Task<bool> UsuarioPerteneceALaConversacionAsync(Guid conversacionId, Guid usuarioId)
    {
        return await _db.Conversaciones.AnyAsync(c =>
            c.Id == conversacionId && (c.ClienteId == usuarioId || c.PrestadorId == usuarioId));
    }

    public async Task<List<MensajeResponse>> ListarHistorialAsync(Guid conversacionId)
    {
        return await _db.Mensajes
            .Where(m => m.ConversacionId == conversacionId)
            .Include(m => m.Emisor)
            .OrderBy(m => m.EnviadoEn)
            .Select(m => new MensajeResponse
            {
                Id = m.Id,
                ConversacionId = m.ConversacionId,
                EmisorId = m.EmisorId,
                EmisorNombre = m.Emisor.Nombre,
                Tipo = m.Tipo.ToString(),
                Contenido = m.Contenido,
                ImagenUrl = m.ImagenUrl,
                MontoOferta = m.MontoOferta,
                OfertaVigente = m.OfertaVigente,
                EnviadoEn = m.EnviadoEn
            })
            .ToListAsync();
    }

    public async Task<MensajeResponse> GuardarMensajeTextoAsync(Guid conversacionId, Guid emisorId, string contenido)
    {
        var emisor = await _db.Usuarios.FindAsync(emisorId);

        var mensaje = new Mensaje
        {
            Id = Guid.NewGuid(),
            ConversacionId = conversacionId,
            EmisorId = emisorId,
            Tipo = TipoMensaje.Texto,
            Contenido = contenido
        };

        _db.Mensajes.Add(mensaje);
        await _db.SaveChangesAsync();

        return new MensajeResponse
        {
            Id = mensaje.Id,
            ConversacionId = mensaje.ConversacionId,
            EmisorId = mensaje.EmisorId,
            EmisorNombre = emisor!.Nombre,
            Tipo = mensaje.Tipo.ToString(),
            Contenido = mensaje.Contenido,
            EnviadoEn = mensaje.EnviadoEn
        };
    }
}