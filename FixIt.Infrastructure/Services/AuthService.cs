using FixIt.Application.DTOs.Auth;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FixIt.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly FixItDbContext _db;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public AuthService(FixItDbContext db)
    {
        _db = db;
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistroRequest request)
    {
        // Validar que el email no exista ya
        var existe = await _db.Usuarios.AnyAsync(u => u.Email == request.Email);
        if (existe)
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese email.");
        }

        // Validar que el rol sea válido (Admin no se crea por registro público)
        if (!Enum.TryParse<RolUsuario>(request.Rol, ignoreCase: true, out var rol) || rol == RolUsuario.Admin)
        {
            throw new InvalidOperationException("Rol inválido. Debe ser 'cliente' o 'prestador'.");
        }

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Telefono = request.Telefono,
            Rol = rol
        };

        // Hasheamos la contraseña ANTES de guardarla
        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return new UsuarioResponse
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Rol = usuario.Rol.ToString()
        };
    }
}