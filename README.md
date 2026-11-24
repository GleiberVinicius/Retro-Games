# RetroGames

Implementação inicial do escopo de Catálogo e Administração descrito em **RetroGames — Arquitetura e Stack**. A solução segue a separação Controller → Service → Repository, usa consultas ADO.NET com parâmetros e compõe dados relacionados no Service.

## Estrutura

- `RetroGames.Model`: entidades e tipos compartilhados.
- `RetroGames.Repository`: repositórios MySQL com MySqlConnector.
- `RetroGames.Services`: regras de negócio, autenticação e composição de dados.
- `RetroGames.WebApi.ApiModels`: contratos JSON e envelope de resposta.
- `RetroGames.WebApi`: API ASP.NET Core, controllers, validação, sessão e proteção CSRF.
- `RetroGames.WebApi.UnitTests`: testes unitários com xUnit v3 e Moq.
- `retrogames.web`: SPA React com Vite, Bootstrap, React Router e Axios.
- `database`: scripts numerados de estrutura e dados fixos de domínio.

## Requisitos locais

.NET SDK 10, Node.js 22 LTS, npm e MySQL 8.

## Banco de dados

Execute `database/001_criar_estrutura_inicial.sql` em uma instância MySQL 8. O script cria o schema `retrogames`, as tabelas do módulo e registros iniciais de perfis, condições e gêneros. A especificação técnica identifica essas tabelas de domínio, mas não enumera seus valores; os rótulos de condição e gênero do script são dados de exemplo e podem ser trocados pelos valores definidos na aula.

## Configuração privada da API

As configurações abaixo devem ficar no armazenamento local do User Secrets do projeto `RetroGames.WebApi`. Substitua os valores de conexão no seu terminal; não os salve neste repositório.

```powershell
dotnet user-secrets set "ConnectionStrings:Default" "<sua-connection-string-local>" --project RetroGames.WebApi
dotnet user-secrets set "AdminInicial:Nome" "<nome-do-administrador>" --project RetroGames.WebApi
dotnet user-secrets set "AdminInicial:Email" "<email-do-administrador>" --project RetroGames.WebApi
dotnet user-secrets set "AdminInicial:Senha" "<senha-local-segura>" --project RetroGames.WebApi
```

Na inicialização, a API cria a conta administradora quando os três valores `AdminInicial` estão definidos e o e-mail ainda não existe. A senha é gravada somente como hash pelo `PasswordHasher`. O script SQL cria o perfil de administrador com identificador `1`.

## Executar

No terminal, na raiz do repositório:

```powershell
dotnet run --project RetroGames.WebApi
```

A API atende em `http://localhost:5080`; Swagger fica habilitado em desenvolvimento em `/swagger`.

Em outro terminal:

```powershell
Set-Location retrogames.web
npm install
npm run dev
```

O Vite atende em `http://localhost:5173` e encaminha `/api` para a API local.

## Sessão e rotas administrativas

As rotas `/admin/jogos`, `/admin/jogos/novo`, `/admin/jogos/:id/editar` e `/admin/plataformas` ficam dentro de `RotaProtegida` e exigem o perfil Administrador. Se um visitante abrir uma delas, o front-end envia para `/login?voltarPara=...`; depois do login, retorna ao endereço solicitado. Ao recarregar uma rota protegida, o provider consulta `/api/Autenticacao/eu` usando o cookie de sessão.

O botão **Sair** encerra a sessão na API e retorna ao catálogo. Sem sessão, as rotas administrativas não aparecem no menu e os endpoints protegidos respondem HTTP 401.

Para chamadas de escrita no Swagger, execute primeiro `GET /api/Autenticacao/csrf`. Copie `data.requestToken` para o header `X-CSRF-TOKEN` da chamada `POST`, `PUT` ou `DELETE`. A validação CSRF continua ativa também no login.

Erros de validação usam `responseCode: 3`; `obterErrosDosCampos` extrai as mensagens por campo para os formulários. Os demais erros são exibidos como mensagem geral.

## Verificação

Com o SDK .NET 10 instalado, execute `dotnet test` na raiz. Para validar o front-end, execute `npm run build` dentro de `retrogames.web`.

O catálogo público lista jogos ativos. As telas de manutenção e os endpoints correspondentes exigem sessão Administrador. A API mantém a sessão em cookie HttpOnly e recebe o token CSRF no header `X-CSRF-TOKEN`.
