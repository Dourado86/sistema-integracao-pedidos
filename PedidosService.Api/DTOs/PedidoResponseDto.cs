using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosService.Api.DTOs
{
    public class PedidoResponseDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;  
        public decimal ValorTotal { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}