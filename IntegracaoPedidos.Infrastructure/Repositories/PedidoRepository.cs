using IntegracaoPedidos.Core.Models;
using IntegracaoPedidos.Core.Interfaces;
using IntegracaoPedidos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using IntegracaoPedidos.Core.Enums;

namespace IntegracaoPedidos.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pedido>> GetPedidosAsync()
    {
        return await _context.Pedidos.ToListAsync();
    }

    public async Task<Pedido?> GetPedidoByIdAsync(int id)
    {
        return await _context.Pedidos.FindAsync(id);
    }

    public async Task AddPedidoAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePedidoAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Pedido>> ObterPendentesAsync()
    {
    return await _context.Pedidos
        .Where(p => p.Status == StatusPedido.Pendente)
        .ToListAsync();
    }
}
