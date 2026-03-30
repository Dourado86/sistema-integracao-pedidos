using Microsoft.EntityFrameworkCore;
using IntegracaoPedidos.Infrastructure.Data;
using IntegracaoPedidos.Infrastructure.Repositories;
using IntegracaoPedidos.Core.Interfaces;
using System.Text.Json.Serialization;
using PedidosService.Api.Services;
using PedidosService.Api.Middleware;
using PedidosService.Api.Mensageria;
using Microsoft.OpenApi.Models; 
using System.Text; 
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens; 
using PedidosService.Api.Auth; 

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1. REGISTRO DE SERVIÇOS (Injeção de Dependência)
// =======================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IRabbitMqProducer, RabbitMqProducer>();
builder.Services.AddScoped<TokenService>(); // Fabricante de Crachás

// Configuração do Banco de Dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// =======================================================
// 2. CONFIGURAÇÃO DO SWAGGER COM JWT
// =======================================================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PedidosService.Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =======================================================
// 3. CONFIGURAÇÃO DA AUTENTICAÇÃO (A CATRACA)
// =======================================================
var chaveSecreta = Encoding.ASCII.GetBytes("MinhaChaveSuperSecretaMuitoLongaParaOJwtFuncionar123!");

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false; 
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(chaveSecreta), 
        ValidateIssuer = false, 
        ValidateAudience = false 
    };
});

var app = builder.Build();

// =======================================================
// 4. PIPELINE DE EXECUÇÃO (A Ordem Importa!)
// =======================================================

// AQUI ESTAVA O PROBLEMA! Precisamos ligar a tela do Swagger.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();

// Ligando a segurança (Autenticação ANTES da Autorização)
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

// Garante que o banco seja criado ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();