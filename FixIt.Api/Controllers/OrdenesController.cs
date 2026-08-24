using System.Security.Claims;
using FixIt.Application.DTOs.Ordenes;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/ordenes")]
[Authorize]
public class OrdenesController : ControllerBase
{
    private readonly IOrdenService _ordenService;

    public OrdenesController(IOrdenService ordenService)
    {
        _ordenService = ordenService;
    }

    private Guid ObtenerUsuarioId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(idClaim!);
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Crear([FromBody] CrearOrdenRequest request)
    {
        try
        {
            var resultado = await _ordenService.CrearAsync(ObtenerUsuarioId(), request);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("mias")]
    public async Task<IActionResult> MisOrdenes()
    {
        var resultado = await _ordenService.ListarMisOrdenesAsync(ObtenerUsuarioId());
        return Ok(resultado);
    }
}