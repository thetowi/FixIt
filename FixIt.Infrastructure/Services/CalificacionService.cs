using FixIt.Application.DTOs.Calificaciones;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Services;

public class CalificacionService : ICalificacionService
{
    private readonly FixItDbContext _db;

    public CalificacionService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<CalificacionResponse> CrearAsync(Guid clienteId, Guid ordenId, CrearCalificacionRequest request)
    {
        if (request.Puntuacion < 1 || request.Puntuacion > 5)
        {
            throw new InvalidOperationException("La puntuación debe estar entre 1 y 5.");
        }

        var orden = await _db.Ordenes
            .Include(o => o.Cliente)
            .Include(o => o.Calificacion)
            .FirstOrDefaultAsync(o => o.Id == ordenId);

        if (orden is null || orden.ClienteId != clienteId)
        {
            throw new InvalidOperationException("Orden no encontrada.");
        }
        if (orden.Estado != EstadoOrden.Completado)
        {
            throw new InvalidOperationException("Solo podés calificar trabajos ya completados.");
        }
        if (orden.Calificacion is not null)
        {
            throw new InvalidOperationException("Esta orden ya fue calificada.");
        }

        var calificacion = new Calificacion
        {
            Id = Guid.NewGuid(),
            OrdenId = orden.Id,
            Puntuacion = request.Puntuacion,
            Comentario = request.Comentario
        };

        _db.Calificaciones.Add(calificacion);
        await _db.SaveChangesAsync();

        return new CalificacionResponse
        {
            Id = calificacion.Id,
            ClienteNombre = orden.Cliente.Nombre,
            Puntuacion = calificacion.Puntuacion,
            Comentario = calificacion.Comentario,
            CreadoEn = calificacion.CreadoEn
        };
    }

    public async Task<List<CalificacionResponse>> ListarPorPrestadorAsync(Guid prestadorId)
    {
        return await _db.Calificaciones
            .Where(c => c.Orden.PrestadorId == prestadorId)
            .Include(c => c.Orden)
                .ThenInclude(o => o.Cliente)
            .OrderByDescending(c => c.CreadoEn)
            .Select(c => new CalificacionResponse
            {
                Id = c.Id,
                ClienteNombre = c.Orden.Cliente.Nombre,
                Puntuacion = c.Puntuacion,
                Comentario = c.Comentario,
                CreadoEn = c.CreadoEn
            })
            .ToListAsync();
    }
}