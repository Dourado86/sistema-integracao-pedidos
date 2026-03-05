using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;

namespace ProcessamentoService.Api.Services;

public class PedidoWorker : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PedidoWorker> _logger;

    public PedidoWorker(IHttpClientFactory httpClientFactory, ILogger<PedidoWorker> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("=== LOOP DE PROCESSAMENTO EXECUTADO ===");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("PedidosService");

                var pedidos = await client.GetFromJsonAsync<List<PedidoDto>>("api/Pedidos/pendentes", stoppingToken);

                if (pedidos != null && pedidos.Any())
                {
                    foreach (var pedido in pedidos)
                    {
                        await client.PutAsync($"api/Pedidos/{pedido.Id}/processar", null, stoppingToken);
                        _logger.LogInformation("Pedido {Id} processado automaticamente.", pedido.Id);
                    }
                }
                else
                {
                    _logger.LogInformation("Nenhum pedido pendente encontrado.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante processamento automático.");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    public class PedidoDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
    }
}