using System.Security.Claims;
using FixIt.Application.DTOs.Calificaciones;
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
    private readonly ICalificacionService _calificacionService;

    public OrdenesController(IOrdenService ordenService, ICalificacionService calificacionService)
    {
        _ordenService = ordenService;
        _calificacionService = calificacionService;
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

    [HttpPut("{id}/marcar-pagada")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MarcarComoPagada(Guid id)
    {
        try
        {
            await _ordenService.MarcarComoPagadaAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/iniciar")]
    [Authorize(Roles = "Prestador")]
    public async Task<IActionResult> Iniciar(Guid id)
    {
        try
        {
            await _ordenService.IniciarAsync(ObtenerUsuarioId(), id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/completar")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Completar(Guid id)
    {
        try
        {
            await _ordenService.CompletarAsync(ObtenerUsuarioId(), id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/calificacion")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Calificar(Guid id, [FromBody] CrearCalificacionRequest request)
    {
        try
        {
            var resultado = await _calificacionService.CrearAsync(ObtenerUsuarioId(), id, request);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}