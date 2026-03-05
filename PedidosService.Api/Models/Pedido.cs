namespace PedidosService.Api.Models;

public class Pedido
{
    public int Id { get; set; }
    public string Numero { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string Status { get; set; } = "Criado";
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
