using FixIt.Application.DTOs.Categorias;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Services;

public class CategoriaService : ICategoriaService
{
    private readonly FixItDbContext _db;

    public CategoriaService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoriaResponse>> ListarActivasAsync()
    {
        return await _db.Categorias
            .Where(c => c.Activa)
            .Select(c => new CategoriaResponse
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Icono = c.Icono
            })
            .ToListAsync();
    }

    public async Task<List<PrestadorCategoriaResponse>> ListarMisCategoriasAsync(Guid prestadorId)
    {
        return await _db.PrestadorCategorias
            .Where(pc => pc.PrestadorId == prestadorId)
            .Include(pc => pc.Categoria)
            .Select(pc => new PrestadorCategoriaResponse
            {
                Id = pc.Id,
                CategoriaId = pc.CategoriaId,
                CategoriaNombre = pc.Categoria.Nombre,
                Descripcion = pc.Descripcion,
                PrecioReferencia = pc.PrecioReferencia
            })
            .ToListAsync();
    }

    public async Task<PrestadorCategoriaResponse> AgregarAsync(Guid prestadorId, AgregarCategoriaRequest request)
    {
        var categoria = await _db.Categorias.FindAsync(request.CategoriaId);
        if (categoria is null || !categoria.Activa)
        {
            throw new InvalidOperationException("La categoría no existe o no está activa.");
        }

        var yaExiste = await _db.PrestadorCategorias
            .AnyAsync(pc => pc.PrestadorId == prestadorId && pc.CategoriaId == request.CategoriaId);
        if (yaExiste)
        {
            throw new InvalidOperationException("Ya ofrecés esta categoría.");
        }

        var prestadorCategoria = new PrestadorCategoria
        {
            PrestadorId = prestadorId,
            CategoriaId = request.CategoriaId,
            Descripcion = request.Descripcion,
            PrecioReferencia = request.PrecioReferencia
        };

        _db.PrestadorCategorias.Add(prestadorCategoria);
        await _db.SaveChangesAsync();

        return new PrestadorCategoriaResponse
        {
            Id = prestadorCategoria.Id,
            CategoriaId = categoria.Id,
            CategoriaNombre = categoria.Nombre,
            Descripcion = prestadorCategoria.Descripcion,
            PrecioReferencia = prestadorCategoria.PrecioReferencia
        };
    }

    public async Task QuitarAsync(Guid prestadorId, int prestadorCategoriaId)
    {
        var registro = await _db.PrestadorCategorias
            .FirstOrDefaultAsync(pc => pc.Id == prestadorCategoriaId && pc.PrestadorId == prestadorId);

        if (registro is null)
        {
            throw new InvalidOperationException("No se encontró esa categoría para tu cuenta.");
        }

        _db.PrestadorCategorias.Remove(registro);
        await _db.SaveChangesAsync();
    }
}