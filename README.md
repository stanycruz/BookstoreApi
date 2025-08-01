# 📚 Bookstore API

API RESTful construída em .NET 9 com autenticação via [Keycloak](https://www.keycloak.org/), persistência em banco de dados PostgreSQL, e controle de acesso por níveis de permissão (roles). Esse projeto tem como objetivo demonstrar de forma simples o uso de Keycloak para segurança e controle de autorização em aplicações reais.

---

## 🚀 Tecnologias

- [.NET 9](https://dotnet.microsoft.com/)
- [Entity Framework Core 9](https://learn.microsoft.com/ef/)
- [PostgreSQL](https://www.postgresql.org/)
- [Keycloak](https://www.keycloak.org/)
- [JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt)
- RESTful API

---

## 🔐 Autenticação & Autorização

A API utiliza tokens JWT emitidos pelo Keycloak.

### 🔑 Roles disponíveis:
### 🛡️ Política personalizada por roles

A API possui validação detalhada de permissões por role através de policies customizadas. É possível definir que apenas determinados perfis (ex: `admin` ou `maintainer`) tenham acesso a uma rota, e a resposta 403 será personalizada conforme a role exigida.

Isso é feito através de `IAuthorizationHandler` e `AuthorizationRequirement` registrados via injeção de dependência.


| Role         | Permissões principais                                     |
|--------------|-----------------------------------------------------------|
| `admin`      | Pode criar, atualizar, deletar e acessar qualquer dado    |
| `owner`      | Pode criar, atualizar e deletar dados da própria loja     |
| `maintainer` | Pode criar ações específicas como registrar vendas        |
| `rookie`     | Somente leitura dos dados que ele mesmo criou            |

---

## 🧱 Estrutura da API

```
BookstoreApi/
├── Application/
│   └── Services/           # Serviços de aplicação (ex: UserService)
├── Domain/
│   └── Entities/           # Entidades (User, Book, Grocery, etc)
├── Infrastructure/
│   └── Data/               # DbContext e Migrations
├── Controllers/
│   └── V1/                 # Controllers versionados (Admin, Book, etc)
├── Helpers/                # Utilitários (UserHttpContextHelper, etc)
├── Program.cs              # Configuração da aplicação
```

---

## 📦 Endpoints

> Todos os endpoints `/v1/...` são protegidos por token JWT.

### ✅ Públicos
- `GET /` → Resposta simples ("API pública - sem login")

### 🔐 Protegidos com Role

#### `GET /v1/admin` → `admin`
- Retorna os dados do usuário autenticado

#### `GET /v1/claims` → Qualquer usuário logado
- Retorna todos os claims contidos no token JWT

#### `GET /v1/books` → `rookie`, `maintainer`, `owner`, `admin`
- Lista os livros visíveis ao usuário
- `rookie` vê apenas os próprios livros

#### `POST /v1/books` → `maintainer`, `owner`, `admin`
- Cria um novo livro

#### `PATCH /v1/books/{id}` → `owner`, `admin`
- Atualiza um livro

#### `DELETE /v1/books/{id}` → `owner`, `admin`
- Deleta um livro

---

## 🛠️ Como rodar o projeto localmente

### 1. Clonar o repositório
```bash
git clone https://github.com/seu-usuario/bookstore-api.git
cd bookstore-api
```

### 2. Configurar o banco de dados PostgreSQL
Crie um banco chamado `bookstore_db` e configure a string de conexão em `Program.cs`.

### 3. Aplicar as migrations
```bash
dotnet ef database update
```

### 4. Rodar a aplicação
```bash
dotnet run
```

### 5. (Opcional) Rodar via Docker (em breve)
### 6. Testar via Swagger

Acesse `http://localhost:5000` no navegador. Clique em "Authorize" e insira o token JWT no formato:

```
Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-QV30
```

O Swagger está configurado para listar todas as rotas, inclusive as protegidas, e permite testá-las com autenticação.


---

## 🔒 Geração de token (exemplo via curl)

```bash
curl --request POST \
  --url http://localhost:8080/realms/bookstore-app/protocol/openid-connect/token \
  --header 'Content-Type: application/x-www-form-urlencoded' \
  --data client_id=bookstore-client \
  --data grant_type=password \
  --data username=admin \
  --data password=Admin@123
```

---

## 👥 Sobre

Projeto de estudo criado para explorar o uso de Keycloak em um sistema real de controle de acesso baseado em permissões. Ideal para times que precisam de autenticação robusta e escalável.
