using FixIt.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FixIt.Infrastructure.Services;

public class SupabaseStorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;
    private readonly string _serviceRoleKey;

    public SupabaseStorageService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _supabaseUrl = config["Supabase:Url"]!;
        _serviceRoleKey = config["Supabase:ServiceRoleKey"]!;
    }

    public async Task<string> SubirArchivoAsync(string bucket, string nombreArchivo, Stream contenido, string contentType)
    {
        var url = $"{_supabaseUrl}/storage/v1/object/{bucket}/{nombreArchivo}";

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {_serviceRoleKey}");
        request.Headers.Add("x-upsert", "true"); // permite sobreescribir si ya existe un archivo con ese nombre

        using var content = new StreamContent(contenido);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Error al subir el archivo: {error}");
        }

        return $"{_supabaseUrl}/storage/v1/object/public/{bucket}/{nombreArchivo}";
    }
}