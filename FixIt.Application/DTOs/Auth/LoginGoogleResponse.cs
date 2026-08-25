namespace FixIt.Application.DTOs.Auth;

public class LoginGoogleResponse
{
    // Si el usuario ya existía, viene esto completo:
    public string? Token { get; set; }
    public UsuarioResponse? Usuario { get; set; }

    // Si es un usuario NUEVO, viene esto en su lugar:
    public bool RequiereRol { get; set; }
    public string? EmailPendiente { get; set; }
    public string? NombrePendiente { get; set; }
    public string? IdTokenPendiente { get; set; } // se lo devolvemos al frontend para que nos lo reenvíe en el paso 2
}