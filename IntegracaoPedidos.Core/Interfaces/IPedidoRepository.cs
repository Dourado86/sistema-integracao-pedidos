using IntegracaoPedidos.Core.Models;
using IntegracaoPedidos.Core.Pagination;



namespace IntegracaoPedidos.Core.Interfaces
{
    public interface IPedidoRepository
    {
      
        Task<PagedResult<Pedido>> GetPedidosPaginadosAsync(int numeroPagina, int tamanhoPagina);
        Task<Pedido?> GetPedidoByIdAsync(int id); 
        Task AddPedidoAsync(Pedido pedido);
        Task UpdatePedidoAsync(Pedido pedido);
        Task<IEnumerable<Pedido>> ObterPendentesAsync();
       
    }
}