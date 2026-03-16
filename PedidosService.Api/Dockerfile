# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY IntegracaoPedidos.sln .
COPY IntegracaoPedidos.Core/ IntegracaoPedidos.Core/
COPY IntegracaoPedidos.Infrastructure/ IntegracaoPedidos.Infrastructure/
COPY PedidosService.Api/ PedidosService.Api/

WORKDIR /src/PedidosService.Api

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "PedidosService.Api.dll"]