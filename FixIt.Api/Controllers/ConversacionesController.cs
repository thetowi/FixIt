using System.Security.Claims;
using FixIt.Application.DTOs.Conversaciones;
using FixIt.Application.DTOs.Mensajes;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/conversaciones")]
[Authorize]
public class ConversacionesController : ControllerBase
{
    private readonly IConversacionService _conversacionService;
    private readonly IMensajeService _mensajeService;
    private readonly IPagoService _pagoService;

    public ConversacionesController(IConversacionService conversacionService, IMensajeService mensajeService, IPagoService pagoService)
    {
        _conversacionService = conversacionService;
        _mensajeService = mensajeService;
        _pagoService = pagoService;
    }

    private Guid ObtenerUsuarioId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(idClaim!);
    }

    [HttpPost]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> IniciarOEncontrar([FromBody] IniciarConversacionRequest request)
    {
        try
        {
            var resultado = await _conversacionService.IniciarOEncontrarAsync(ObtenerUsuarioId(), request);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("mias")]
    public async Task<IActionResult> MisConversaciones()
    {
        var resultado = await _conversacionService.ListarMisConversacionesAsync(ObtenerUsuarioId());
        return Ok(resultado);
    }

    [HttpPost("{conversacionId}/ofertas")]
    [Authorize(Roles = "Prestador")]
    public async Task<IActionResult> EnviarOferta(Guid conversacionId, [FromBody] EnviarOfertaRequest request)
    {
        try
        {
            var resultado = await _mensajeService.EnviarOfertaAsync(conversacionId, ObtenerUsuarioId(), request.Monto);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("ofertas/{mensajeOfertaId}/pagar")]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> PagarOferta(Guid mensajeOfertaId)
    {
        try
        {
            var resultado = await _pagoService.CrearPreferenciaDesdeOfertaAsync(mensajeOfertaId, ObtenerUsuarioId());
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}