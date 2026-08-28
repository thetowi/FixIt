using FixIt.Application.DTOs.Pagos;
using FixIt.Application.Interfaces;
using FixIt.Domain.Entities;
using FixIt.Infrastructure.Data;
using MercadoPago.Client.Common;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FixIt.Infrastructure.Services;

public class PagoService : IPagoService
{
    private readonly FixItDbContext _db;
    private readonly IConfiguration _config;

    public PagoService(FixItDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;

        // El SDK de Mercado Pago necesita el Access Token configurado globalmente
        // antes de crear cualquier cliente de sus APIs
        MercadoPagoConfig.AccessToken = _config["MercadoPago:AccessToken"];
    }

    public async Task<CrearPreferenciaResponse> CrearPreferenciaAsync(Guid ordenId, Guid clienteId)
    {
        var orden = await _db.Ordenes
            .Include(o => o.Categoria)
            .FirstOrDefaultAsync(o => o.Id == ordenId);

        if (orden is null || orden.ClienteId != clienteId)
        {
            throw new InvalidOperationException("Orden no encontrada.");
        }

        if (orden.Estado != EstadoOrden.PendientePago)
        {
            throw new InvalidOperationException("Esta orden ya no está pendiente de pago.");
        }

        var request = new PreferenceRequest
        {
            Items = new List<PreferenceItemRequest>
            {
                new PreferenceItemRequest
                {
                    Title = $"FixIt - {orden.Categoria.Nombre}",
                    Quantity = 1,
                    CurrencyId = "ARS",
                    UnitPrice = orden.MontoTotal
                }
            },
            // external_reference es CLAVE: es lo que nos permite, cuando llegue el webhook,
            // saber a qué Orden nuestra corresponde ese pago
            ExternalReference = orden.Id.ToString(),
                        NotificationUrl = EsUrlValida(_config["MercadoPago:WebhookUrl"]) ? _config["MercadoPago:WebhookUrl"] : null,
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = $"{_config["Frontend:Url"]}/ordenes?pago=exitoso",
                Failure = $"{_config["Frontend:Url"]}/ordenes?pago=fallido",
                Pending = $"{_config["Frontend:Url"]}/ordenes?pago=pendiente"
            }
        };

        var client = new PreferenceClient();
        var preference = await client.CreateAsync(request);

        return new CrearPreferenciaResponse
        {
            InitPoint = preference.InitPoint
        };
    }
        private static bool EsUrlValida(string? url)
    {
        return !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}