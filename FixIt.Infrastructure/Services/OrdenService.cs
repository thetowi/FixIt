using FixIt.Application.DTOs.Ordenes;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FixIt.Infrastructure.Services;

public class OrdenService : IOrdenService
{
    private readonly FixItDbContext _db;
    private readonly IConfiguration _config;

    public OrdenService(FixItDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<OrdenResponse> CrearAsync(Guid clienteId, CrearOrdenRequest request)
    {
        if (clienteId == request.PrestadorId)
        {
            throw new InvalidOperationException("No podés contratarte a vos mismo.");
        }

        if (request.MontoTotal <= 0)
        {
            throw new InvalidOperationException("El monto debe ser mayor a cero.");
        }

        var prestador = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Id == request.PrestadorId && u.Rol == RolUsuario.Prestador);
        if (prestador is null)
        {
            throw new InvalidOperationException("El prestador no existe.");
        }

        var ofreceCategoria = await _db.PrestadorCategorias
            .Include(pc => pc.Categoria)
            .FirstOrDefaultAsync(pc => pc.PrestadorId == request.PrestadorId && pc.CategoriaId == request.CategoriaId);
        if (ofreceCategoria is null)
        {
            throw new InvalidOperationException("Este prestador no ofrece esa categoría.");
        }

        var porcentajeComision = _config.GetValue<decimal>("Comision:PorcentajeDefault");
        var comision = Math.Round(request.MontoTotal * porcentajeComision, 2);

        var orden = new Orden
        {
            Id = Guid.NewGuid(),
            ClienteId = clienteId,
            PrestadorId = request.PrestadorId,
            CategoriaId = request.CategoriaId,
            Estado = EstadoOrden.PendientePago,
            MontoTotal = request.MontoTotal,
            ComisionPlataforma = comision
        };

        _db.Ordenes.Add(orden);
        await _db.SaveChangesAsync();

        return new OrdenResponse
        {
            Id = orden.Id,
            PrestadorId = prestador.Id,
            PrestadorNombreCompleto = $"{prestador.Nombre} {prestador.Apellido}",
            CategoriaId = ofreceCategoria.CategoriaId,
            CategoriaNombre = ofreceCategoria.Categoria.Nombre,
            Estado = orden.Estado.ToString(),
            MontoTotal = orden.MontoTotal,
            ComisionPlataforma = orden.ComisionPlataforma,
            CreadoEn = orden.CreadoEn
        };
    }

    public async Task<List<OrdenResponse>> ListarMisOrdenesAsync(Guid usuarioId)
    {
        return await _db.Ordenes
            .Where(o => o.ClienteId == usuarioId || o.PrestadorId == usuarioId)
            .Include(o => o.Prestador)
            .Include(o => o.Categoria)
            .OrderByDescending(o => o.CreadoEn)
            .Select(o => new OrdenResponse
            {
                Id = o.Id,
                PrestadorId = o.PrestadorId,
                PrestadorNombreCompleto = o.Prestador.Nombre + " " + o.Prestador.Apellido,
                CategoriaId = o.CategoriaId,
                CategoriaNombre = o.Categoria.Nombre,
                Estado = o.Estado.ToString(),
                MontoTotal = o.MontoTotal,
                ComisionPlataforma = o.ComisionPlataforma,
                CreadoEn = o.CreadoEn
            })
            .ToListAsync();
    }
}