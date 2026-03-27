using Microsoft.AspNetCore.Mvc;
using PedidosService.Api.DTOs;
using PedidosService.Api.Services;
using PedidosService.Api.Mensageria;

namespace PedidosService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<PedidosController> _logger;
    private readonly IRabbitMqProducer _mensageria; // Nova dependência

    public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger, IRabbitMqProducer mensageria)// Injetando via construtor

        
    {
        _pedidoService = pedidoService;
        _logger = logger;
        _mensageria = mensageria;
    }


    [HttpPost]
    public async Task<IActionResult> Criar(CreatePedidoDto dto)

    {
        _logger.LogInformation("Recebido pedido número {numero}", dto.Numero);

        // 1. O Serviço faz a regra de negócio e salva no banco de dados (SQL)
        var pedido = await _pedidoService.CriarPedido(dto);


        // 2. apenas o ID do pedido criado. O nome da fila será "pedidos_criados"
       await _mensageria.PublicarMensagem(new { PedidoId = pedido.Id },"pedidos-criados"); // Publica a mensagem na fila "pedidos-criados"

        _logger.LogInformation("Pedido, {Id} criado com sucesso e enviado para a a fila", pedido.Id);

        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(int id)
    {
        _logger.LogInformation("Buscando pedido ID {Id}", id);

        var pedido = await _pedidoService.ObterPedido(id);

        if (pedido == null)
        {
            _logger.LogWarning("Pedido ID {Id} não encontrado", id);
            return NotFound();
        }


        return Ok(pedido);
    }
    

     [HttpGet]
     public async Task<IActionResult> Listar(
        // [FromQuery] diz para o C# procurar esses valores na URL (ex: ?pagina=2&quantidade=5)
        // "= 1" e "= 10" são os valores padrão. Se o usuário não digitar nada na URL, 
        // a API não quebra, ela simplesmente devolve a primeira página com 10 itens.
        [FromQuery] int pagina = 1, 
        [FromQuery] int quantidade = 10)
    {
        // Boa prática: logar o que está sendo buscado
        _logger.LogInformation("Listando pedidos: Página {pagina}, Quantidade {quantidade}", pagina, quantidade);

        // Chamamos o serviço passando exatamente os números que vieram da URL
        var resultadoPaginado = await _pedidoService.ListarPedidosAsync(pagina, quantidade);
        
        // Retornamos a nossa "caixinha" cheia de metadados com status 200 OK
        return Ok(resultadoPaginado);
    }
   

    [HttpGet("pendentes")]
    public async Task<IActionResult> Pendentes()
    {
        _logger.LogInformation("Listando pedidos pendentes");

        var pedidos = await _pedidoService.ObterPendentes();
        return Ok(pedidos);
    }


    [HttpPut("{id}/processar")]
    public async Task<IActionResult> Processar(int id)
    {
        _logger.LogInformation("Processando pedido ID {Id}", id);

        try
        {
            var pedido = await _pedidoService.ProcessarPedido(id);

            if (pedido == null)
            {

                _logger.LogWarning("Pedido ID {Id} não encontrado para processamento", id);
                return NotFound("Pedido não encontrado.");

            }

            
            _logger.LogInformation("Pedido ID {Id} processado com sucesso", id);
            return Ok(pedido);
        }

        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao processar pedido ID {Id}: {Message}", id, ex.Message);
            return BadRequest(ex.Message);
        }
    }
}