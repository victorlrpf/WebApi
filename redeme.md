🔐 Projeto C# Web API com JWT + Supabase

Este é um projeto em C# (.NET 6/7) que implementa autenticação via JWT e persistência de dados usando Supabase (PostgreSQL).
A ideia é demonstrar como criar uma API segura, com cadastro de usuários, login e rotas protegidas.

🚀 Funcionalidades

✅ Cadastro de usuários no banco Supabase

✅ Login com verificação de senha (hash com BCrypt)

✅ Geração de tokens JWT para autenticação

✅ Rotas protegidas que exigem token válido

✅ Estrutura modular (Controllers, Services, Models)

🏗️ Tecnologias Utilizadas

- C# .NET 6/7 (Web API)

- JWT (JSON Web Token)

- Supabase (PostgreSQL)

- Npgsql (conexão com Postgres)

- BCrypt.Net (hash de senhas)

📂 Estrutura do Projeto
```
/ProjetoJWT
  /Controllers
    AuthController.cs
    UsersController.cs
  /Models
    User.cs
    LoginRequest.cs
  /Services
    JwtService.cs
    SupabaseService.cs
  Program.cs
  appsettings.json
```

Appsettings.json

```
{
  "Jwt": {
    "Key": "sua_chave_super_secreta",
    "Issuer": "suaaplicacao",
    "Audience": "suaaplicacao"
  },
  "ConnectionStrings": {
    "Supabase": "Host=xxxxx.supabase.co;Database=postgres;Username=postgres;Password=senha"
  }
}
```

📌 Endpoints
🔹 Cadastro de Usuário

POST /auth/register

Body (JSON):

```
{
  "nome": "Victão",
  "email": "victao@email.com",
  "senha": "123456"
}
```

🔹 Login

POST /auth/login

Body (JSON):
```
{
  "email": "victao@email.com",
  "senha": "123456"
}
```

retorno

```
{
  "token": "eyJhbGciOiJIUzI1NiIsInR..."
}
```

🛡️ Segurança

- Senhas são armazenadas com hash BCrypt.

- JWT expira em 2 horas.

- Rotas críticas exigem Authorization: Bearer <token> no header.
