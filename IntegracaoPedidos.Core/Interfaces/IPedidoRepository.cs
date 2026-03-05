using IntegracaoPedidos.Core.Models;



namespace IntegracaoPedidos.Core.Interfaces
{
    public interface IPedidoRepository
    {
        Task<List<Pedido>> GetPedidosAsync();
        Task<Pedido?> GetPedidoByIdAsync(int id); 
        Task AddPedidoAsync(Pedido pedido);
        Task UpdatePedidoAsync(Pedido pedido);
        Task<IEnumerable<Pedido>> ObterPendentesAsync();
    }
}