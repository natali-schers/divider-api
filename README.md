# 🧮 Divider API (em desenvolvimento)

---

## 📌 Sobre o projeto
O Divider foi criado com o intuito de aprofundar meus conhecimentos em backend e colocar em prática o que venho estudando ao longo dos últimos anos. Inspirado em aplicativos conhecidos de divisão de gastos, como o Tricount, o Divider permite dividir despesas entre um grupo, com cálculo igualitário ou personalizado de acordo com a escolha do usuário.

Além da funcionalidade principal, o projeto foi uma oportunidade de aplicar decisões de arquitetura comuns em produtos reais: autenticação via JWT, modelagem de domínio (com participantes de grupo que não necessariamente possuem uma conta na plataforma) e cálculo de regras de negócio centralizado no servidor.
## ✨ Funcionalidades
- **Autenticação JWT** completa (registro, login, hash de senha com BCrypt)
- **Grupos** — criação, listagem e exclusão, restritos aos membros participantes
- **Despesas** — criação com divisão igualitária ou customizada, validada no servidor
- **Cálculo de saldos e simplificação de dívidas** — algoritmo que reduz o número de transferências necessárias para quitar um grupo
- **Modelo misto convidado/conta** — um participante pode existir no grupo sem ter conta na plataforma, e reivindicar seu lugar depois via convite por e-mail
- **Autorização por participação** — cada endpoint valida se o usuário autenticado realmente pertence ao grupo/despesa que está tentando acessar

## 🏗️ Arquitetura
```
Controllers/   → HTTP, validação de entrada, orquestração
Services/      → regras de negócio (BalanceCalculatorService, JwtService)
Data/          → DbContext (EF Core)
Models/        → entidades de banco
DTOs/          → contratos de entrada/saída (nunca expõem entidades EF diretamente)
```

## 🔌 Endpoints principais
| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/auth/register` | Cria uma conta |
| `POST` | `/api/auth/login` | Autentica e retorna um JWT |
| `GET` | `/api/groups` | Lista grupos do usuário logado |
| `POST` | `/api/groups` | Cria um grupo |
| `GET` | `/api/groups/{id}` | Detalhe de um grupo |
| `DELETE` | `/api/groups/{id}` | Exclui um grupo |
| `GET` | `/api/groups/{groupId}/expenses` | Lista despesas do grupo |
| `POST` | `/api/groups/{groupId}/expenses` | Cria uma despesa (com split) |
| `DELETE` | `/api/groups/{groupId}/expenses/{expenseId}` | Exclui uma despesa |
| `GET` | `/api/groups/{groupId}/expenses/settlements` | Calcula saldos e simplifica dívidas |
| `PATCH` | `/api/groups/{groupId}/members/{memberId}/invite-email` | Vincula e-mail de convite a um membro sem conta |
| `GET` | `/api/members/pending-invites` | Lista convites pendentes do usuário logado |
| `POST` | `/api/members/{memberId}/claim` | Reivindica um membro convidado como sua conta |

## 🛠️ Stack técnica
- **ASP.NET Core Web API**
- **Entity Framework Core** — ORM + migrations
- **SQL Server** (Azure SQL Database)
- **JWT Bearer** para autenticação + **BCrypt** para hash de senha
- **Swagger** para documentação OpenAPI

## ☁️ Deploy
A API está publicada na **Render**, com banco de dados em **Azure SQL Database**. Variáveis sensíveis (connection string, chaves JWT) foram configuradas via variáveis de ambiente da plataforma. Acesse via: **[Divider API](https://divider-api.onrender.com/swagger/)**

## 🔗 Projeto relacionado
Este backend é consumido por um app mobile/web em Flutter:
👉 **[divider-app](https://github.com/natali-schers/divider-app)**
