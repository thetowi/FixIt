using System.Security.Claims;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FixIt.Api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IMensajeService _mensajeService;

    public ChatHub(IMensajeService mensajeService)
    {
        _mensajeService = mensajeService;
    }

    private Guid ObtenerUsuarioId()
    {
        var idClaim = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");
        return Guid.Parse(idClaim!);
    }

    public async Task UnirseAOrden(string ordenId)
    {
        var usuarioId = ObtenerUsuarioId();
        var ordenGuid = Guid.Parse(ordenId);

        var pertenece = await _mensajeService.UsuarioPerteneceALaOrdenAsync(ordenGuid, usuarioId);
        if (!pertenece)
        {
            throw new HubException("No tenés acceso a esta orden.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ordenId);
    }

    public async Task EnviarMensaje(string ordenId, string contenido)
    {
        var usuarioId = ObtenerUsuarioId();
        var ordenGuid = Guid.Parse(ordenId);

        var pertenece = await _mensajeService.UsuarioPerteneceALaOrdenAsync(ordenGuid, usuarioId);
        if (!pertenece)
        {
            throw new HubException("No tenés acceso a esta orden.");
        }

        if (string.IsNullOrWhiteSpace(contenido))
        {
            throw new HubException("El mensaje no puede estar vacío.");
        }

        var mensajeGuardado = await _mensajeService.GuardarMensajeAsync(ordenGuid, usuarioId, contenido);

        // Reenvía el mensaje a TODOS los conectados a este grupo (incluido quien lo mandó,
        // así el frontend no necesita lógica especial para "mostrar mi propio mensaje")
        await Clients.Group(ordenId).SendAsync("RecibirMensaje", mensajeGuardado);
    }
}