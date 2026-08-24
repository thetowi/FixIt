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
    private readonly IJwtService _jwtService;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public AuthService(FixItDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistroRequest request)
    {
        var existe = await _db.Usuarios.AnyAsync(u => u.Email == request.Email);
        if (existe)
        {
            throw new InvalidOperationException("Ya existe una cuenta con ese email.");
        }

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

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (usuario is null)
        {
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            // Mensaje idéntico al de "usuario no existe" a propósito:
            // así no le damos pistas a un atacante de si el email existe o no
            throw new InvalidOperationException("Email o contraseña incorrectos.");
        }

        var token = _jwtService.GenerarToken(usuario);

        return new LoginResponse
        {
            Token = token,
            Usuario = new UsuarioResponse
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Rol = usuario.Rol.ToString()
            }
        };
    }
}