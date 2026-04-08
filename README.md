# 📦 Sistema de Integração de Pedidos (Base & Qualidade)

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![xUnit](https://img.shields.io/badge/xUnit-Testing-2CA5E0?style=for-the-badge&logo=dotnet&logoColor=white)
![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)

> **🚀 Nota de Evolução Arquitetural:** > Este repositório consolida a fundação do sistema, com forte foco em **Qualidade de Código, Testes Unitários (TDD)** e integração base com filas. Se você deseja ver como este ecossistema evoluiu para uma arquitetura avançada de **SaaS Multi-tenant** com propagação de contexto, confira a versão 2.0 no meu projeto: **[SaaS Hub de Pedidos](Aguardem...)**.

---

## 📖 Sobre o Projeto

Arquitetura de microsserviços com **.NET 8** utilizando Arquitetura Orientada a Eventos (Event-Driven), mensageria com **RabbitMQ** e processamento assíncrono com Background Worker.

Este projeto demonstra uma **arquitetura de microsserviços utilizando ASP.NET Core**, simulando um fluxo real de sistemas distribuídos onde uma API recebe pedidos e delega o processamento de forma assíncrona, garantindo alta disponibilidade e tolerância a falhas.

O objetivo é demonstrar **boas práticas utilizadas no desenvolvimento backend moderno**, incluindo testes de software, mensageria, observabilidade, tratamento de erros e containerização.

---

## 🏗️ Arquitetura do Projeto

O sistema é composto por dois serviços principais totalmente desacoplados:

### API de Pedidos (Producer)
Responsável por:
* Receber novos pedidos via HTTP (REST).
* Persistir os pedidos no banco de dados.
* **Publicar eventos** de `PedidoCriado` na fila do RabbitMQ.
* Registrar logs estruturados das operações.

### API de Processamento (Consumer / Worker)
Responsável por:
* **Consumir mensagens** da fila do RabbitMQ instantaneamente.
* Processar a regra de negócio do pedido em segundo plano (BackgroundService).
* Atualizar o status do pedido no banco com segurança.


---

## 💻 Tecnologias Utilizadas

* ASP.NET Core (.NET 8)
* Entity Framework Core & SQL Server (Otimizado com `.AsNoTracking()` e Paginação)
* **Segurança State-less com JWT (JSON Web Tokens) e RBAC (Role-Based Access Control)**
* **Mensageria com RabbitMQ (Message Broker)**
* BackgroundService (Worker Service) para consumo de filas em background
* Arquitetura Orientada a Eventos (Event-Driven)
* Tratamento centralizado com Middleware Global de Exceções
* Containerização com Docker e Docker Compose
* **Testes unitários automatizados com xUnit e Moq**

---

## 🧪 Qualidade de Código e Testes Unitários (TDD)

A resiliência de um software começa na garantia de que suas regras de negócio funcionam isoladamente. Para isso, o projeto conta com uma suíte de testes robusta:

* **Ferramentas Utilizadas:** Implementação com `xUnit` como framework principal e `Moq` para a criação de dependências falsas (Mocks), garantindo que os testes não precisem de banco de dados ou RabbitMQ rodando para serem validados.
* **Padrão AAA (Arrange, Act, Assert):** Todo o código de teste segue uma estrutura limpa e semântica:
  * **Arrange (Preparar):** Configuração de cenários, injeção de Mocks e definição dos dados esperados.
  * **Act (Agir):** Execução do método isolado no Serviço.
  * **Assert (Verificar):** Confirmação rigorosa dos resultados, exceções lançadas e verificação de chamadas corretas aos repositórios.
* **Foco no Domínio:** Os testes garantem a integridade das validações de negócio, como impedimento de mudanças inválidas de status ou cálculos incorretos.

---

## ⚙️ Recursos Implementados

### Performance e Otimização de Banco de Dados
Para garantir a escalabilidade do sistema e evitar vazamento de memória (Memory Leaks) ao lidar com milhares de pedidos:
* **Paginação no Banco de Dados:** Uso de `IQueryable` com `.Skip()` e `.Take()` no Entity Framework Core para trafegar apenas os dados necessários.
* **Consultas de Leitura Otimizadas:** Aplicação de `.AsNoTracking()` nas listagens para liberar o Change Tracker.
* **Respostas Padronizadas (Wrappers):** Criação de uma classe genérica `PagedResult<T>` para devolver dados envelopados.

### 🔒 Segurança (Autenticação e Autorização)
Implementação de segurança State-less:
* **JSON Web Tokens (JWT):** Geração e validação de tokens criptografados (HmacSha256).
* **Role-Based Access Control (RBAC):** Controle de acesso granular (ex: Apenas `Admin` cria novos pedidos).

### Mensageria e Processamento Assíncrono (RabbitMQ)
O sistema abandonou o acoplamento síncrono para garantir resiliência:
* **Desacoplamento:** A API principal publica o evento e libera o usuário instantaneamente.
* **Confirmação de Entrega (Ack/Nack):** A mensagem só sai da fila se o processamento for 100% concluído. Em caso de erro, ela volta para a fila de retentativa.

### Logging Estruturado e Middleware
* Uso de logging nativo para rastreabilidade de eventos e falhas.
* Middleware Global para capturar erros inesperados e retornar respostas limpas sem expor a infraestrutura interna.

---

## 🐳 Containerização e Como Executar

A aplicação foi desenhada para ser executada utilizando **containers Docker**, permitindo subir toda a infraestrutura com facilidade, sem instalar bancos ou message brokers na máquina hospedeira.

O ambiente orquestra simultaneamente: API de Pedidos, API de Processamento, Banco SQL Server e Servidor RabbitMQ.

### Passos para rodar localmente:
1. Clone o repositório:
```bash
git clone [https://github.com/dourado86/sistema-integracao-pedidos.git](https://github.com/dourado86/sistema-integracao-pedidos.git)