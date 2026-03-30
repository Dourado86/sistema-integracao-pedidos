using Microsoft.AspNetCore.Mvc;
using PedidosService.Api.Auth;
using PedidosService.Api.DTOs;

namespace PedidosService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // 1. INJEÇÃO DE DEPENDÊNCIA
    // Nós "chamamos" o nosso fabricante de crachás para trabalhar dentro desta portaria.
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    // 2. O MÉTODO DE LOGIN (A ÚNICA PORTA DE ENTRADA)
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        // ========================================================================
        // PASSO A: VALIDAÇÃO DO USUÁRIO (A checagem de identidade)
        // Em um sistema real, você iria no banco de dados fazer um SELECT 
        // para ver se a senha bate com o hash salvo. Para focarmos na arquitetura 
        // do Token agora, vamos "simular" (Hardcode) que temos dois usuários válidos.
        // ========================================================================

        if (dto.Username == "rafael" && dto.Password == "admin123")
        {
            // PASSO B: FABRICAÇÃO DO CRACHÁ PARA O ADMIN
            // Se ele acertou a senha, chamamos o serviço passando o nome e a regra "Admin".
            var token = _tokenService.GerarToken(dto.Username, "Admin");
            
            // Retornamos um 200 OK com o token formatado em JSON.
            return Ok(new { Token = token });
        }

        if (dto.Username == "vendedor" && dto.Password == "vend123")
        {
            // PASSO B: FABRICAÇÃO DO CRACHÁ PARA O FUNCIONÁRIO COMUM
            // Aqui a regra muda. Ele só ganha o crachá de "Funcionario".
            var token = _tokenService.GerarToken(dto.Username, "Funcionario");
            
            return Ok(new { Token = token });
        }

        // ========================================================================
        // PASSO C: ACESSO NEGADO
        // Se ele errar a senha ou o usuário não existir, devolvemos o erro HTTP 401.
        // O 401 Unauthorized significa exatamente "Você não provou quem você é".
        // ========================================================================
        return Unauthorized(new { Erro = "Usuário ou senha inválidos." });
    }
}
   
