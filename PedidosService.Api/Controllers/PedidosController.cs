using Microsoft.AspNetCore.Mvc;
using PedidosService.Api.DTOs;
using PedidosService.Api.Services;

namespace PedidosService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly ILogger<PedidosController> _logger;

    public PedidosController(IPedidoService pedidoService, ILogger<PedidosController> logger)
    {
        _pedidoService = pedidoService;
        _logger = logger;
    }


    [HttpPost]
    public async Task<IActionResult> Criar(CreatePedidoDto dto)

    {
        _logger.LogInformation("Recebido pedido número {numero}", dto.Numero);

        var pedido = await _pedidoService.CriarPedido(dto);

        _logger.LogInformation("Pedido {Id} criado com sucesso", pedido.Id);

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
    public async Task<IActionResult> Listar()
    {
        _logger.LogInformation("Listando todos os pedidos");
        //throw new Exception("Erro teste"); // testando o tratamento global de exceções

        var pedidos = await _pedidoService.ListarPedidos();
        return Ok(pedidos);
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