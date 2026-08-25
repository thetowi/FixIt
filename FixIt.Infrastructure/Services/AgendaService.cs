using FixIt.Application.DTOs.Agenda;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Services;

public class AgendaService : IAgendaService
{
    private readonly FixItDbContext _db;

    public AgendaService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<List<BloqueDisponibilidadResponse>> ObtenerDisponibilidadAsync(Guid prestadorId)
    {
        return await _db.Disponibilidad
            .Where(d => d.PrestadorId == prestadorId)
            .OrderBy(d => d.DiaSemana).ThenBy(d => d.HoraInicio)
            .Select(d => new BloqueDisponibilidadResponse
            {
                Id = d.Id,
                DiaSemana = d.DiaSemana,
                HoraInicio = d.HoraInicio,
                HoraFin = d.HoraFin
            })
            .ToListAsync();
    }

    public async Task<BloqueDisponibilidadResponse> AgregarBloqueAsync(Guid prestadorId, BloqueDisponibilidadRequest request)
    {
        if (request.HoraFin <= request.HoraInicio)
        {
            throw new InvalidOperationException("La hora de fin debe ser posterior a la hora de inicio.");
        }

        var bloque = new DisponibilidadPrestador
        {
            PrestadorId = prestadorId,
            DiaSemana = request.DiaSemana,
            HoraInicio = request.HoraInicio,
            HoraFin = request.HoraFin
        };

        _db.Disponibilidad.Add(bloque);
        await _db.SaveChangesAsync();

        return new BloqueDisponibilidadResponse
        {
            Id = bloque.Id,
            DiaSemana = bloque.DiaSemana,
            HoraInicio = bloque.HoraInicio,
            HoraFin = bloque.HoraFin
        };
    }

    public async Task EliminarBloqueAsync(Guid prestadorId, int bloqueId)
    {
        var bloque = await _db.Disponibilidad
            .FirstOrDefaultAsync(d => d.Id == bloqueId && d.PrestadorId == prestadorId);

        if (bloque is null)
        {
            throw new InvalidOperationException("Bloque de disponibilidad no encontrado.");
        }

        _db.Disponibilidad.Remove(bloque);
        await _db.SaveChangesAsync();
    }

    public async Task ProgramarTurnoAsync(Guid prestadorId, Guid ordenId, ProgramarTurnoRequest request)
    {
        var orden = await _db.Ordenes.FirstOrDefaultAsync(o => o.Id == ordenId && o.PrestadorId == prestadorId);

        if (orden is null)
        {
            throw new InvalidOperationException("Orden no encontrada.");
        }

        if (orden.Estado != EstadoOrden.Pagado && orden.Estado != EstadoOrden.EnCurso)
        {
            throw new InvalidOperationException("Solo se pueden programar órdenes pagadas o en curso.");
        }

        if (request.FechaHora < DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("No se puede programar un turno en el pasado.");
        }

        orden.FechaHoraProgramada = request.FechaHora;
        await _db.SaveChangesAsync();
    }

    public async Task<List<OrdenAgendaResponse>> ObtenerAgendaAsync(Guid prestadorId, DateTimeOffset desde, DateTimeOffset hasta)
    {
        return await _db.Ordenes
            .Where(o => o.PrestadorId == prestadorId &&
                        o.FechaHoraProgramada != null &&
                        o.FechaHoraProgramada >= desde &&
                        o.FechaHoraProgramada <= hasta)
            .Include(o => o.Cliente)
            .Include(o => o.Categoria)
            .OrderBy(o => o.FechaHoraProgramada)
            .Select(o => new OrdenAgendaResponse
            {
                Id = o.Id,
                CategoriaNombre = o.Categoria.Nombre,
                ClienteNombreCompleto = o.Cliente.Nombre + " " + o.Cliente.Apellido,
                Estado = o.Estado.ToString(),
                FechaHoraProgramada = o.FechaHoraProgramada
            })
            .ToListAsync();
    }

    public async Task<List<OrdenAgendaResponse>> ObtenerSinProgramarAsync(Guid prestadorId)
    {
        return await _db.Ordenes
            .Where(o => o.PrestadorId == prestadorId &&
                        o.FechaHoraProgramada == null &&
                        (o.Estado == EstadoOrden.Pagado || o.Estado == EstadoOrden.EnCurso))
            .Include(o => o.Cliente)
            .Include(o => o.Categoria)
            .OrderBy(o => o.CreadoEn)
            .Select(o => new OrdenAgendaResponse
            {
                Id = o.Id,
                CategoriaNombre = o.Categoria.Nombre,
                ClienteNombreCompleto = o.Cliente.Nombre + " " + o.Cliente.Apellido,
                Estado = o.Estado.ToString(),
                FechaHoraProgramada = null
            })
            .ToListAsync();
    }
}