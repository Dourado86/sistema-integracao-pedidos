

using IntegracaoPedidos.Core.Enums;


namespace IntegracaoPedidos.Core.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public string Numero { get; set; } = string.Empty;

        public decimal ValorTotal { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

      public StatusPedido Status { get; set; } = StatusPedido.Pendente;

        
    }

}