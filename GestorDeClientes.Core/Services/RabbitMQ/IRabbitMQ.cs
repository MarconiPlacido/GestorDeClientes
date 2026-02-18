namespace GestorDeClientes.Core.Services
{
    public interface IRabbitService
    {
        void Publicar<T>(T mensagem);
        void Consumir<T>(Action<T> onMessage);
    }
}
