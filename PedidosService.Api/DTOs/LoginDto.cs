using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosService.Api.DTOs;

    public class LoginDto
    {
        // O usuário vai enviar o nome dele (ex: "Rafael")
    public string Username { get; set; } = string.Empty;
    
    // E a senha dele (ex: "123456")
    public string Password { get; set; } = string.Empty;

    }
