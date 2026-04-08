using PedidosService.Api.Auth;

namespace PedidosService.Tests;

public class TokenServiceTests
{
    // A tag [Fact] avisa o xUnit: "Ei, isso aqui é um robô que você deve executar!"
    [Fact] 
    public void GerarToken_DeveRetornarStringValida_QuandoDadosForemCorretos()
    {
        // ==========================================
        // 1. ARRANGE (Preparar)
        // ==========================================
        var tokenService = new TokenService();
        var nomeUsuario = "rafael";
        var regraUsuario = "Admin";

        // ==========================================
        // 2. ACT (Agir)
        // ==========================================
        var tokenGerado = tokenService.GerarToken(nomeUsuario, regraUsuario);

        // ==========================================
        // 3. ASSERT (Validar)
        // ==========================================
        // Validamos se o robô não recebeu um texto vazio
        Assert.False(string.IsNullOrWhiteSpace(tokenGerado)); 
        
        // Todo JWT válido possui 3 partes separadas por um ponto (Header.Payload.Signature). 
        // Vamos validar se o ponto existe!
        Assert.Contains(".", tokenGerado); 
    }
}
