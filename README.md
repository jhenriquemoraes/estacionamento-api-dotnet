# 🅿️ MinimalAPI - Sistema de Estacionamento em .NET

Projeto de API REST desenvolvida em .NET 8, utilizando autenticação JWT, Entity Framework Core e princípios de Clean Code e Onion Architecture. O sistema tem como objetivo o gerenciamento de usuários administradores e veículos dentro de um estacionamento.

> ⚠️ Projeto em desenvolvimento.

## Esse projeto é baseado em uma aula de instruções de um Desafio na DIO no modulo de APIs do curso Full Stack .NET

## 📦 Estrutura do Projeto

📁 Dominio
-┣ 📂 DTOs # Objetos de transferência de dados
-┣ 📂 Entidade # Entidades principais do domínio
-┣ 📂 Enuns # Tipos de perfis e enums gerais
-┣ 📂 Interfaces # Interfaces para injeção de dependência
-┣ 📂 ModelViews # Modelos de entrada e saída
-┣ 📂 Servicos # Lógica de negócio implementada

📁 Infraestrutura
-┣ 📂 Db # DbContext e Migrations

📄 Program.cs # Ponto de entrada
📄 MinimalAPI.csproj # Projeto .NET

---

## 🚀 Tecnologias Utilizadas

- ✅ .NET 8
- ✅ C#
- ✅ Entity Framework Core
- ✅ SQL Server
- ✅ JWT (Json Web Token)
- ✅ Onion Architecture
- ✅ Clean Code
- ✅ REST APIs
- ✅ Teste Unitarios

---

## 🔐 Funcionalidades atuais

- Cadastro e login de administradores
- Autenticação via JWT
- Controle de acesso por perfil
- CRUD de veículos
- Validações estruturadas
- Migrations organizadas por entidade
- Documentação automática via Swagger
- Testes Unitarios (Em andamento)

---

## 🧪 Testes

O projeto conta com **testes unitarios**, cobrindo:

- Regras de negócio
- Entidades
- Serviços
- Usando MSTest

Isso garante maior confiabilidade, facilidade de manutenção e qualidade do código.

Inicialmente os testes foram desenvolvimentos em MSTest, seguindo as instruções do desafio
Agora estou no processo de refatoração em XUnit, para deixar o projeto mais atual

---

## ▶️ Como executar

1. Clone o repositório:
   ```bash
   git clone https://github.com/seuusuario/minimalapi.git
   cd minimalapi
   ```
2. Atualize o appsettings.json com sua string de conexão do SQL Server.

3. Execute as migrations:

```bash
  dotnet ef database update
```

4. Execute o projeto:

```bash
  dotnet run
```

Acesse a documentação Swagger em:
http://localhost:5000/swagger

## 🧩 Próximos passos

    🔄 Implementar refresh token no JWT

    🔄 Refatorar os teste para Xunit

    ☁️ Deploy com Docker (futuro)

## 📄 Licença

Projeto aberto para fins de estudo e demonstração.

## 🤝 Contribuições

Sinta-se livre para abrir issues, relatar bugs ou sugerir melhorias!
