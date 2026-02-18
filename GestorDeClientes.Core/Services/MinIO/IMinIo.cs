namespace GestorDeClientes.Core.Services.MinIo
{
    public interface IMinio
    {
        Task UploadAsync(string bucket, string objectName, Stream fileStream, string contentType);
        Task<MemoryStream> DownloadAsync(string bucket, string objectName);
        Task<Stream> ObterArquivoAsync(string bucket, string nomeArquivo);
    }
}
