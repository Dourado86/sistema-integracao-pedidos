using Microsoft.EntityFrameworkCore;
using IntegracaoPedidos.Infrastructure.Data;
using IntegracaoPedidos.Infrastructure.Repositories;
using IntegracaoPedidos.Core.Interfaces;
using System.Text.Json.Serialization;
using PedidosService.Api.Services;
using PedidosService.Api.Middleware;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
     ));

builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();



builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter());
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

