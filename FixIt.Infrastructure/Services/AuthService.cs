using FixIt.Application.DTOs.Auth;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FixIt.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly FixItDbContext _db;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public AuthService(FixItDbContext db, IJwtService jwtService, IConfiguration config)
    {
        _db = db;
        _jwtService = jwtService;
        _config = config;
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

        if (string.IsNullOrEmpty(usuario.PasswordHash))
        {
            throw new InvalidOperationException("Esta cuenta fue creada con Google. Iniciá sesión con el botón de Google.");
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
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

    private async Task<GoogleJsonWebSignature.Payload> ValidarTokenDeGoogleAsync(string idToken)
    {
        var clientId = _config["Google:ClientId"];
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { clientId }
        };

        try
        {
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            throw new InvalidOperationException("El token de Google no es válido.");
        }
    }

    public async Task<LoginGoogleResponse> LoginConGoogleAsync(LoginGoogleRequest request)
    {
        var payload = await ValidarTokenDeGoogleAsync(request.IdToken);

        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (usuario is null)
        {
            // No existe todavía: le pedimos al frontend que nos diga el rol antes de crear la cuenta
            return new LoginGoogleResponse
            {
                RequiereRol = true,
                EmailPendiente = payload.Email,
                NombrePendiente = payload.GivenName,
                IdTokenPendiente = request.IdToken
            };
        }

        var token = _jwtService.GenerarToken(usuario);

        return new LoginGoogleResponse
        {
            RequiereRol = false,
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

    public async Task<LoginResponse> CompletarRegistroGoogleAsync(CompletarRegistroGoogleRequest request)
    {
        var payload = await ValidarTokenDeGoogleAsync(request.IdToken);

        var yaExiste = await _db.Usuarios.AnyAsync(u => u.Email == payload.Email);
        if (yaExiste)
        {
            throw new InvalidOperationException("Esta cuenta ya fue registrada.");
        }

        if (!Enum.TryParse<RolUsuario>(request.Rol, ignoreCase: true, out var rol) || rol == RolUsuario.Admin)
        {
            throw new InvalidOperationException("Rol inválido. Debe ser 'cliente' o 'prestador'.");
        }

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = payload.Email,
            Nombre = payload.GivenName ?? "Usuario",
            Apellido = payload.FamilyName ?? "",
            Telefono = "",
            Rol = rol,
            PasswordHash = "", // no tiene contraseña propia, entra siempre por Google
            FotoPerfilUrl = payload.Picture,
            Verificado = payload.EmailVerified
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

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