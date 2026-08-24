using System.Security.Claims;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/ordenes/{ordenId}/mensajes")]
[Authorize]
public class MensajesController : ControllerBase
{
    private readonly IMensajeService _mensajeService;

    public MensajesController(IMensajeService mensajeService)
    {
        _mensajeService = mensajeService;
    }

    [HttpGet]
    public async Task<IActionResult> Historial(Guid ordenId)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        var usuarioId = Guid.Parse(idClaim!);

        var pertenece = await _mensajeService.UsuarioPerteneceALaOrdenAsync(ordenId, usuarioId);
        if (!pertenece)
        {
            return Forbid();
        }

        var historial = await _mensajeService.ListarHistorialAsync(ordenId);
        return Ok(historial);
    }
}