using System.Security.Claims;
using FixIt.Application.DTOs.Agenda;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/prestador")]
[Authorize(Roles = "Prestador")]
public class AgendaController : ControllerBase
{
    private readonly IAgendaService _agendaService;

    public AgendaController(IAgendaService agendaService)
    {
        _agendaService = agendaService;
    }

    private Guid ObtenerPrestadorId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(idClaim!);
    }

    [HttpGet("disponibilidad")]
    public async Task<IActionResult> ObtenerDisponibilidad()
    {
        var resultado = await _agendaService.ObtenerDisponibilidadAsync(ObtenerPrestadorId());
        return Ok(resultado);
    }

    [HttpPost("disponibilidad")]
    public async Task<IActionResult> AgregarBloque([FromBody] BloqueDisponibilidadRequest request)
    {
        try
        {
            var resultado = await _agendaService.AgregarBloqueAsync(ObtenerPrestadorId(), request);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("disponibilidad/{id}")]
    public async Task<IActionResult> EliminarBloque(int id)
    {
        try
        {
            await _agendaService.EliminarBloqueAsync(ObtenerPrestadorId(), id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("agenda")]
    public async Task<IActionResult> ObtenerAgenda([FromQuery] DateTimeOffset desde, [FromQuery] DateTimeOffset hasta)
    {
        var resultado = await _agendaService.ObtenerAgendaAsync(ObtenerPrestadorId(), desde, hasta);
        return Ok(resultado);
    }

    [HttpGet("agenda/sin-programar")]
    public async Task<IActionResult> ObtenerSinProgramar()
    {
        var resultado = await _agendaService.ObtenerSinProgramarAsync(ObtenerPrestadorId());
        return Ok(resultado);
    }
}