using PedidosService.Api.DTOs;

public interface IPedidoService
{
    Task<PedidoResponseDto> CriarPedido(CreatePedidoDto dto);
    Task<PedidoResponseDto?> ObterPedido(int id);
    Task<List<PedidoResponseDto>> ListarPedidos();
    Task<List<PedidoResponseDto>> ObterPendentes();
    Task<PedidoResponseDto?> ProcessarPedido(int id);
}