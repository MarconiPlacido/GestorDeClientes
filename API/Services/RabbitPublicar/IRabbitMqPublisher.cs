
namespace GestorDeClientes.API.Services.RabbitPublicar
{
    public interface IRabbitMqPublisher
    {
        void Publicar<T>(string fila, T mensagem);

    }
}