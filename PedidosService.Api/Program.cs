using Microsoft.EntityFrameworkCore;
using IntegracaoPedidos.Infrastructure.Data;
using IntegracaoPedidos.Infrastructure.Repositories;
using IntegracaoPedidos.Core.Interfaces;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

