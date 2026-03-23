using System.Text;
using System.Text.Json;
using RabbitMQ.Client;


namespace PedidosService.Api.Mensageria;

public class RabbitMqProducer : IRabbitMqProducer
{
    
    private readonly string _hostname = "rabbitmq"; // Nome do container no Docker Compose
    
    public async Task PublicarMensagem<T>(T mensagem, string nomeFila)
    {
        var factory = new ConnectionFactory { HostName = _hostname };
        
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        // Garante que a fila existe no RabbitMQ antes de tentar mandar a mensagem
        await channel.QueueDeclareAsync(queue: nomeFila, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var json = JsonSerializer.Serialize(mensagem);
        var body = Encoding.UTF8.GetBytes(json);

        // Publica a mensagem na fila
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: nomeFila, body: body);;
    }
}