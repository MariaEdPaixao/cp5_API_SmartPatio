# Mottu API - Gestão de Motos e Patios

API RESTful desenvolvida em .NET 8 para gerenciamento de motos e patios, utilizando Oracle, Entity Framework Core, Clean Architecture e princípios de DDD.

*solução criada para o **Challenge Mottu***
---

## 📦 Estrutura da Solução

- **Dominio**: Entidades de domínio, enums e exceções.
- **Aplicacao**: DTOs, validações e mapeamentos.
- **Infraestrutura**: Contexto do EF Core, configurações de banco e mapeamentos Fluent API.
- **Apresentacao**: Controllers, endpoints e configuração de rotas.

---

## 🚀 Funcionalidades

- Cadastro, consulta, atualização e remoção de motos.
- Cadastro, consulta, atualização e remoção de patios.
- Relacionamento muitos-para-um entre Moto e Patio.
- Validações de domínio e unicidade de placa.
- Documentação automática via Swagger/OpenAPI.
- Respostas HTTP padronizadas (200, 201, 204, 400, 404, 409, 500, 503).
- Uso de DTOs para entrada e saída de dados.
- Injeção de dependência e separação por camadas.

---

## 🔗 Endpoints Principais

### Motos

- `GET /api/motos` — Lista todas as motos.
- `GET /api/motos/{id}` — Consulta uma moto pelo ID.
- `POST /api/motos` — Cadastra uma nova moto.
- `PATCH /api/motos/{id}` — Atualiza parcialmente uma moto.
- `DELETE /api/motos/{id}` — Remove uma moto.

### Patios

- `GET /api/patio` — Lista todos os patios (sem motos associadas).
- `GET /api/patio/{id}` — Consulta um patio pelo ID (inclui as motos associadas).
- `POST /api/patio` — Cadastra um novo patio.
- `PATCH /api/patio/{id}` — Atualiza parcialmente um patio.
- `DELETE /api/patio/{id}` — Remove um patio.

---

## ⚙️ Tecnologias Utilizadas

- .NET 8 / ASP.NET Core
- C# 12
- Entity Framework Core 9
- Oracle (Oracle.EntityFrameworkCore)
- Swagger (Swashbuckle)
- Clean Architecture
- Domain-Driven Design (DDD)

---

## 🏗️ Como Executar

1. **Configurar a string de conexão Oracle**
   - No `appsettings.json` ou via variável de ambiente:
     ```
     "ConnectionStrings": {
       "Oracle": "Data Source=...;User ID=...;Password=..."
     }
     ```
2. **Restaurar pacotes e aplicar migrations**
   (No CMD da aplicação)
   `dotnet restore dotnet ef database update`

3. **Executar a aplicação**
   (No CMD da aplicação)
   `dotnet run`

Acesse o Swagger em: `https://localhost:7018/swagger/index.html`
---

## 👥 Equipe - Prisma.Code
- Laura de Oliveira Cintra - RM 558843
- Maria Eduarda Alves da Paixão - RM 558832
- Vinícius Saes de Souza - RM 554456

> “Faça o teu melhor, na condição que você tem, enquanto você não tem condições melhores, para fazer melhor ainda.” — Mario Sergio Cortella