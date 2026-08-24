using FixIt.Application.DTOs.Prestadores;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Services;

public class PrestadorPerfilService : IPrestadorPerfilService
{
    private readonly FixItDbContext _db;

    public PrestadorPerfilService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<PerfilPrestadorResponse?> ObtenerPerfilAsync(Guid prestadorId)
    {
        var usuario = await _db.Usuarios
            .Where(u => u.Id == prestadorId && u.Rol == RolUsuario.Prestador)
            .Include(u => u.PrestadorCategorias)
                .ThenInclude(pc => pc.Categoria)
            .FirstOrDefaultAsync();

        if (usuario is null) return null;

        return new PerfilPrestadorResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Verificado = usuario.Verificado,
            FotoPerfilUrl = usuario.FotoPerfilUrl,
            MiembroDesde = usuario.CreadoEn,
            Servicios = usuario.PrestadorCategorias.Select(pc => new ServicioOfrecidoResponse
            {
                CategoriaId = pc.CategoriaId,
                CategoriaNombre = pc.Categoria.Nombre,
                Descripcion = pc.Descripcion,
                PrecioReferencia = pc.PrecioReferencia
            }).ToList()
        };
    }
}