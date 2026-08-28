using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IPagoService _pagoService;

    public WebhooksController(IPagoService pagoService)
    {
        _pagoService = pagoService;
    }

    [HttpPost("mercadopago")]
    public async Task<IActionResult> RecibirNotificacion([FromQuery] string? topic, [FromQuery] string? id, [FromQuery(Name = "data.id")] string? dataId)
    {
        // Mercado Pago manda el aviso en distintos formatos según el tipo de integración;
        // cubrimos las dos variantes más comunes de nombre de parámetro
        var paymentId = dataId ?? id;

        if (topic == "payment" && !string.IsNullOrEmpty(paymentId))
        {
            await _pagoService.ProcesarWebhookAsync(paymentId);
        }

        // Siempre respondemos 200, incluso si ignoramos la notificación —
        // si devolvemos error, Mercado Pago reintenta indefinidamente
        return Ok();
    }
}