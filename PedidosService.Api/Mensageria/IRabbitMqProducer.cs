using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosService.Api.Mensageria
{
    public interface IRabbitMqProducer
    {
        Task PublicarMensagem<T>(T mensagem, string nomeFila);
    }
}