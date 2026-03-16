using System.Text.Json.Serialization;
using ProcessamentoService.Api.Services;


var builder = WebApplication.CreateBuilder(args);

// Controllers com suporte para converter enums em strings no JSON, facilitando a leitura e a manutenção dos dados. Isso é especialmente útil para APIs que retornam ou recebem dados em formato JSON, garantindo que os valores dos enums sejam representados de forma clara e compreensível.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

// HttpClient para chamar o PedidosService dentro da variavel de ambiente "Services:PedidosService" do appsettings.json
var pedidosServiceUrl = builder.Configuration["Services:PedidosService"];

builder.Services.AddHttpClient("PedidosService", client =>
{
    client.BaseAddress = new Uri(pedidosServiceUrl!);
});


builder.Services.AddHostedService<PedidoWorker>(); // Adiciona o serviço de processamento em segundo plano (Hosted Service) para processar os pedidos.
builder.Services.AddEndpointsApiExplorer(); // Adiciona o serviço para explorar os endpoints da API, necessário para o Swagger.
builder.Services.AddSwaggerGen();// Adiciona o serviço para gerar a documentação da API usando o Swagger.

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection(); // Redireciona as requisições HTTP para HTTPS.

app.MapControllers(); // Mapeia os controladores para as rotas da API, permitindo que as requisições sejam direcionadas para os métodos corretos nos controladores.

app.Run();