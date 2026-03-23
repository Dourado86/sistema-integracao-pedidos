using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ProcessamentoService.Api.Services;

public class PedidoWorker : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PedidoWorker> _logger;
    private readonly string _hostname = "rabbitmq"; // Nome do container do RabbitMQ no Docker
    private readonly string _nomeFila = "pedidos_criados";

    private IConnection? _connection;
    private IChannel? _channel;


    public PedidoWorker(IHttpClientFactory httpClientFactory, ILogger<PedidoWorker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("=== WORKER CONECTADO NO RABBITMQ AGUARDANDO PEDIDOS ===");

        // 1. Configura a conexão
        var factory = new ConnectionFactory { HostName = _hostname };
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        // 2. Garante que a fila existe
        await _channel.QueueDeclareAsync(queue: _nomeFila, durable: false, exclusive: false, autoDelete: false, arguments: null, cancellationToken: stoppingToken);

        // 3. Cria o "Ouvinte" (Consumer)
        var consumer = new AsyncEventingBasicConsumer(_channel);

        // 4. Define o que acontece QUANDO a mensagem chega
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                // Extrai o JSON da mensagem
                var body = ea.Body.ToArray();
                var mensagemJson = Encoding.UTF8.GetString(body);
                var evento = JsonSerializer.Deserialize<PedidoEvento>(mensagemJson);

                if (evento != null)
                {
                    _logger.LogInformation("*** Mensagem Recebida! Processando Pedido ID: {Id} ***", evento.PedidoId);

                    // Mantemos a sua lógica de avisar a API de Pedidos via HTTP
                    var client = _httpClientFactory.CreateClient("PedidosService");
                    var response = await client.PutAsync($"api/Pedidos/{evento.PedidoId}/processar", null, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("*** Pedido {Id} processado e atualizado com sucesso! ***", evento.PedidoId);
                        
                        // O "Ack" (Acknowledge) avisa o RabbitMQ: "Deu certo, pode apagar a mensagem da fila!"
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning("A API de Pedidos recusou a atualização do Pedido {Id}.", evento.PedidoId);
                        // O "Nack" devolve a mensagem pra fila para tentar de novo depois
                        await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar a mensagem do RabbitMQ.");
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
            }
        };

        // 5. Inicia o consumo da fila. autoAck: false significa que NÓS controlamos quando a mensagem é apagada
        await _channel.BasicConsumeAsync(queue: _nomeFila, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        // Mantém o Worker rodando infinitamente sem consumir CPU
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    // Fecha as conexões de forma limpa quando o Docker for desligado
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    // Classe auxiliar para mapear o JSON que enviamos da API de Pedidos
    private class PedidoEvento
    {
        public int PedidoId { get; set; }
    }
}