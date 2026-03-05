using System.Text.Json.Serialization;
using ProcessamentoService.Api.Services;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });

// HttpClient para chamar o PedidosService
builder.Services.AddHttpClient("PedidosService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5293/"); // ⚠ coloque aqui a porta do seu PedidosService
});


builder.Services.AddHostedService<PedidoWorker>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();