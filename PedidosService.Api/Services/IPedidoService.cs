using PedidosService.Api.DTOs;
using IntegracaoPedidos.Core.Pagination;

namespace PedidosService.Api.Services;
public interface IPedidoService
{
    Task<PedidoResponseDto> CriarPedido(CreatePedidoDto dto);
    Task<PedidoResponseDto?> ObterPedido(int id);

    Task<PagedResult<PedidoResponseDto>> ListarPedidosAsync(int pagina, int quantidade);

    Task<List<PedidoResponseDto>> ObterPendentes();
    Task<PedidoResponseDto?> ProcessarPedido(int id);
}