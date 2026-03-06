# sistema-integracao-pedidos
Arquitetura de micro serviços com .NET 8 - Comunicação HTTP + Background Worker

# Sistema de Integração de Pedidos

Este projeto demonstra uma arquitetura de microsserviços utilizando ASP.NET Core para integração e processamento de pedidos.

O objetivo do projeto é simular um fluxo real de sistemas distribuídos, onde uma API responsável por receber pedidos se comunica com outra API responsável pelo processamento desses pedidos.

---

## Arquitetura do Projeto

O sistema é composto por dois serviços principais:

### API de Pedidos
Responsável por:

- Receber novos pedidos via HTTP
- Persistir os pedidos no banco de dados
- Disponibilizar os pedidos para processamento

### API de Processamento

Responsável por:

- Buscar pedidos pendentes
- Processar os pedidos automaticamente
- Atualizar o status do pedido

O processamento é feito utilizando **BackgroundService**, simulando um worker que executa tarefas em segundo plano.

---

## Tecnologias Utilizadas

- ASP.NET Core
- Entity Framework Core
- SQL Server
- BackgroundService
- REST API
- Arquitetura em camadas

---

## Fluxo do Sistema

1. Um pedido é criado na **API de Pedidos**
2. O pedido é salvo no banco com status **Pendente**
3. A **API de Processamento** executa um worker em segundo plano
4. A cada intervalo de tempo o worker busca novos pedidos
5. O pedido é processado e atualizado para **Processado**

---

## Estrutura do Projeto

IntegracaoPedidos
│
├── PedidosAPI
│ ├── Controllers
│ ├── Data
│ ├── Models
│
├── ProcessamentoAPI
│ ├── Services
│ ├── Workers
│
└── Shared

---

## Como Executar o Projeto

### 1 - Clonar o repositório


git clone https://github.com/dourado86/sistema-integracao-pedidos.git


### 2 - Executar as APIs

Abra dois terminais e execute:

API de Pedidos


dotnet run


API de Processamento


dotnet run


---

## Próximos Passos do Projeto

Este projeto terá novas versões demonstrando diferentes formas de comunicação entre microsserviços:

Versão atual

Comunicação entre APIs utilizando **HTTP**

Próxima versão

Comunicação utilizando **RabbitMQ (Mensageria)**

Objetivo: demonstrar diferentes estratégias de integração em sistemas distribuídos.

---

## Autor

Desenvolvido por  
[Rafael Dourado]