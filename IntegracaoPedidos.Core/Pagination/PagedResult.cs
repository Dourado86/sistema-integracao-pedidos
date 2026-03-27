namespace IntegracaoPedidos.Core.Pagination;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new(); 
    public int TotalCount { get; set; } 
    public int CurrentPage { get; set; } 
    public int PageSize { get; set; } 
    
    // Calcula automaticamente o total de páginas
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}