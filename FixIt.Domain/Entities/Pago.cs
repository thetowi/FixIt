namespace FixIt.Domain.Entities;

public enum EstadoPago
{
    Retenido,
    Liberado,
    Reembolsado
}

public class Pago
{
    public Guid Id { get; set; }

    public Guid OrdenId { get; set; }
    public Orden Orden { get; set; } = null!;

    public string? MercadoPagoPaymentId { get; set; }
    public EstadoPago Estado { get; set; } = EstadoPago.Retenido;
    public decimal Monto { get; set; }
    public DateTimeOffset? LiberadoEn { get; set; }
}