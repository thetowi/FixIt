namespace FixIt.Application.Interfaces;

public interface IStorageService
{
    Task<string> SubirArchivoAsync(string bucket, string nombreArchivo, Stream contenido, string contentType);
}