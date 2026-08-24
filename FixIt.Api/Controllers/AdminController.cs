using FixIt.Application.DTOs.Admin;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("categorias")]
    public async Task<IActionResult> ListarCategorias()
    {
        var resultado = await _adminService.ListarTodasLasCategoriasAsync();
        return Ok(resultado);
    }

    [HttpPost("categorias")]
    public async Task<IActionResult> CrearCategoria([FromBody] CrearCategoriaRequest request)
    {
        try
        {
            var resultado = await _adminService.CrearCategoriaAsync(request);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("categorias/{id}/estado")]
    public async Task<IActionResult> CambiarEstadoCategoria(int id, [FromBody] bool activa)
    {
        try
        {
            await _adminService.CambiarEstadoCategoriaAsync(id, activa);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> ListarUsuarios()
    {
        var resultado = await _adminService.ListarUsuariosAsync();
        return Ok(resultado);
    }
}