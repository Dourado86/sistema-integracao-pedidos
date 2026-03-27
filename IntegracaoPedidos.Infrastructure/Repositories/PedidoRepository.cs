using IntegracaoPedidos.Core.Enums;
using IntegracaoPedidos.Core.Interfaces;
using IntegracaoPedidos.Core.Models;
using IntegracaoPedidos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using IntegracaoPedidos.Core.Pagination;    



namespace IntegracaoPedidos.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    // ========================================================================
    // NOVO MÉTODO PAGINADO (A EXPLICAÇÃO TÉCNICA)
    // ========================================================================
    public async Task<PagedResult<Pedido>> GetPedidosPaginadosAsync(int numeroPagina, int tamanhoPagina)
    {
        // 1. COUNT: Primeiro, perguntamos ao SQL Server quantos pedidos existem no total.
        // O Entity Framework traduz isso para um "SELECT COUNT(*) FROM Pedidos".
        // Isso é super leve e rápido para o banco de dados.
        var totalItens = await _context.Pedidos.CountAsync();

        // 2. A CONSULTA (Skip e Take)
        var pedidos = await _context.Pedidos
            // AsNoTracking(): Diz ao Entity Framework "apenas leia, não fique monitorando essas classes para salvar depois".
            // Isso economiza MUITA memória RAM e processamento no servidor da empresa.
            .AsNoTracking() 
            // Skip(): A matemática da paginação. Se estou na página 3, e o tamanho é 10: (3 - 1) * 10 = Pula os primeiros 20.
            .Skip((numeroPagina - 1) * tamanhoPagina) 
            // Take(): Depois de pular, pegue apenas a quantidade exata solicitada (ex: 10 registros).
            .Take(tamanhoPagina) 
            .ToListAsync();

        // 3. RETORNO: Empacotamos os dados com os metadados para o Frontend conseguir montar os botões de paginação
        return new PagedResult<Pedido>
        {
            Items = pedidos,
            TotalCount = totalItens,
            CurrentPage = numeroPagina,
            PageSize = tamanhoPagina
        };
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