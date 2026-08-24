using System.Security.Claims;
using FixIt.Application.DTOs.Categorias;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/prestador")]
[Authorize(Roles = "Prestador")]
public class PrestadorController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public PrestadorController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    private Guid ObtenerPrestadorId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(idClaim!);
    }

    [HttpGet("categorias")]
    public async Task<IActionResult> MisCategorias()
    {
        var resultado = await _categoriaService.ListarMisCategoriasAsync(ObtenerPrestadorId());
        return Ok(resultado);
    }

    [HttpPost("categorias")]
    public async Task<IActionResult> AgregarCategoria([FromBody] AgregarCategoriaRequest request)
    {
        try
        {
            var resultado = await _categoriaService.AgregarAsync(ObtenerPrestadorId(), request);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("categorias/{id}")]
    public async Task<IActionResult> QuitarCategoria(int id)
    {
        try
        {
            await _categoriaService.QuitarAsync(ObtenerPrestadorId(), id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}