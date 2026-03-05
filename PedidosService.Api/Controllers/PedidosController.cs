using Microsoft.AspNetCore.Mvc;
using PedidosService.Api.DTOs;
using IntegracaoPedidos.Core.Interfaces;
using IntegracaoPedidos.Core.Enums;
using IntegracaoPedidos.Core.Models;




namespace PedidosService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoRepository _pedidoRepository;
        public PedidosController(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CreatePedidoDto dto)
        {
            var pedido = new Pedido
            {
                Numero = dto.Numero,
                ValorTotal = dto.ValorTotal,
            };

            await _pedidoRepository.AddPedidoAsync(pedido);

            return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var pedido = await _pedidoRepository.GetPedidoByIdAsync(id);
            
            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        [HttpGet("pendentes")]
        public async Task<IActionResult> GetPendentes()
        {
        var pedidos = await _pedidoRepository.ObterPendentesAsync();
        return Ok(pedidos);
        }

        [HttpPut("{id}/processar")]
        public async Task<IActionResult> Processar(int id)
        {
            var pedido = await _pedidoRepository.GetPedidoByIdAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == StatusPedido.Processado)
                return BadRequest("Pedido já foi processado.");    

            pedido.Status = StatusPedido.Processado;
            await _pedidoRepository.UpdatePedidoAsync(pedido);

            return Ok(pedido);
        }

    }
}