using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosService.Api.DTOs
{
    public class CreatePedidoDto
    {
        public string Numero { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
    }
}