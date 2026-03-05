using Microsoft.AspNetCore.Mvc;

namespace ProcessamentoService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessamentoController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProcessamentoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("processar-pendentes")]
        public async Task<IActionResult> ProcessarPendentes()
        {
            var client = _httpClientFactory.CreateClient("PedidosService");

            var response = await client.GetAsync("api/Pedidos/pendentes");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Erro ao buscar pendentes.");

            var pedidos = await response.Content.ReadFromJsonAsync<List<PedidoDto>>();

            int processados = 0;

            if (pedidos != null)
            {
                foreach (var pedido in pedidos)
                {
                    var processResponse = await client.PutAsync($"api/Pedidos/{pedido.Id}/processar", null);

                    if (processResponse.IsSuccessStatusCode)
                        processados++;
                }
            }

            return Ok(new { TotalProcessados = processados });
        }
    }

    public class PedidoDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}