using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PedidosService.Api.Auth;

public class TokenService
{
    // A "Chave Mestra" do nosso prédio. 
    // É com ela que o Token é assinado matematicamente. Se um hacker tentar 
    // falsificar um token, a matemática não vai bater com essa chave e a API bloqueia.
    // (Em produção, isso fica escondido em variáveis de ambiente, não no código!)
    private const string ChaveSecreta = "MinhaChaveSuperSecretaMuitoLongaParaOJwtFuncionar123!";

    public string GerarToken(string nomeUsuario, string regra)
    {
        // 1. PREPARAÇÃO DA CHAVE E CREDENCIAIS
        // Transformamos a nossa string secreta em um array de bytes, pois a criptografia 
        // trabalha com bytes, não com texto puro.
        var chave = Encoding.ASCII.GetBytes(ChaveSecreta);
        
        // Criamos a credencial de assinatura usando o algoritmo HmacSha256 (o padrão ouro atual).
        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(chave), 
            SecurityAlgorithms.HmacSha256Signature);

        // 2. CRIAÇÃO DOS "DADOS DO CRACHÁ" (CLAIMS)
        // Claims são as informações que vão dentro do Token. 
        // Aqui dizemos o nome da pessoa e qual a regra (role) dela (ex: "Admin", "Cliente").
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, nomeUsuario),
            new Claim(ClaimTypes.Role, regra)
        });

        // 3. O DESENHO DO TOKEN (DESCRIPTOR)
        // Aqui nós juntamos tudo: As informações (Subject), a validade (Expires) e a assinatura (SigningCredentials).
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddHours(2), // Este crachá só vale por 2 horas!
            SigningCredentials = credenciais
        };

        // 4. O FABRICANTE DE TOKENS (HANDLER)
        // O JwtSecurityTokenHandler é a classe do .NET responsável por pegar esse "desenho"
        // e efetivamente fabricar a string criptografada final.
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Retornamos a string criptografada (O crachá pronto)
        return tokenHandler.WriteToken(token);
    }
}