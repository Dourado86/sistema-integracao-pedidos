using IntegracaoPedidos.Core.Interfaces;
using IntegracaoPedidos.Core.Models;
using IntegracaoPedidos.Core.Enums;
using PedidosService.Api.DTOs;

namespace PedidosService.Api.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoService(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    public async Task<PedidoResponseDto> CriarPedido(CreatePedidoDto dto)
    {
        var pedido = new Pedido
        {
            Numero = dto.Numero,
            ValorTotal = dto.ValorTotal,
            Status = StatusPedido.Pendente,
            CriadoEm = DateTime.UtcNow
        };

        await _pedidoRepository.AddPedidoAsync(pedido);

        return MapToResponseDto(pedido);
    }

    public async Task<PedidoResponseDto?> ObterPedido(int id)
    {
        var pedido = await _pedidoRepository.GetPedidoByIdAsync(id);
        if (pedido == null) return null;

        return MapToResponseDto(pedido);
    }

    public async Task<List<PedidoResponseDto>> ListarPedidos()
    {
        var pedidos = await _pedidoRepository.GetPedidosAsync();

        return pedidos
            .Select(MapToResponseDto)
            .ToList();
    }

    public async Task<List<PedidoResponseDto>> ObterPendentes()
    {
        var pedidos = await _pedidoRepository.ObterPendentesAsync();

        return pedidos
            .Select(MapToResponseDto)
            .ToList();
    }

    public async Task<PedidoResponseDto?> ProcessarPedido(int id)
    {
        var pedido = await _pedidoRepository.GetPedidoByIdAsync(id);

        if (pedido == null)
            return null;

        if (pedido.Status == StatusPedido.Processado)
            throw new InvalidOperationException("Pedido já foi processado.");

        pedido.Status = StatusPedido.Processado;

        await _pedidoRepository.UpdatePedidoAsync(pedido);

        return MapToResponseDto(pedido);
    }

    private static PedidoResponseDto MapToResponseDto(Pedido pedido)
    {
        return new PedidoResponseDto
        {
            Id = pedido.Id,
            Numero = pedido.Numero,
            ValorTotal = pedido.ValorTotal,
            Status = pedido.Status.ToString(),
            CriadoEm = pedido.CriadoEm
        };
    }
}