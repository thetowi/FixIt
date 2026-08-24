using System.Security.Claims;
using FixIt.Application.DTOs.Usuarios;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize] // cualquier usuario logueado, sin importar el rol
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPut("ubicacion")]
    public async Task<IActionResult> ActualizarUbicacion([FromBody] ActualizarUbicacionRequest request)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var usuarioId = Guid.Parse(idClaim!);

        try
        {
            await _usuarioService.ActualizarUbicacionAsync(usuarioId, request);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}