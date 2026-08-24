using FixIt.Application.DTOs.Busqueda;
using FixIt.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FixIt.Api.Controllers;

[ApiController]
[Route("api/prestadores")]
public class PrestadoresController : ControllerBase
{
    private readonly IBusquedaService _busquedaService;

    public PrestadoresController(IBusquedaService busquedaService)
    {
        _busquedaService = busquedaService;
    }

    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar(
        [FromQuery] int categoriaId,
        [FromQuery] double latitud,
        [FromQuery] double longitud,
        [FromQuery] double radioKm = 10)
    {
        var request = new BuscarPrestadoresRequest
        {
            CategoriaId = categoriaId,
            Latitud = latitud,
            Longitud = longitud,
            RadioKm = radioKm
        };

        var resultado = await _busquedaService.BuscarPrestadoresAsync(request);
        return Ok(resultado);
    }
}