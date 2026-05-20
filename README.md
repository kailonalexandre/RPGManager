# RPG Manager

Aplicação web/PWA para gerenciar campanhas e fichas de RPG de mesa. O projeto prioriza controle manual, conteúdo criado pelo usuário e suporte a mesas privadas: campanhas, personagens, magias, talentos/características, inventário, notebook, rolagens, descansos, upload de imagens e exportação resumida da ficha em PDF.

## Visão Geral

O RPG Manager é dividido em API ASP.NET Core, frontend React e banco PostgreSQL. A API centraliza autenticação, regras de permissão e persistência. O frontend entrega a ficha digital e as bibliotecas em uma interface responsiva, com tema claro/escuro e preparação PWA.

O sistema não tenta automatizar todas as exceções de regras de classe ou sistema. O objetivo atual é oferecer uma ficha editável, previsível e segura para dados cadastrados pelo próprio usuário.

Documentos complementares:

- [Arquitetura](docs/ARCHITECTURE.md)
- [API](docs/API.md)
- [Roadmap](docs/ROADMAP.md)
- [Importação Open5e](docs/spell-import.md)
- [Avisos legais](LEGAL_NOTICES.md)

## Stack Usada

- Backend: C# 14, ASP.NET Core Web API, EF Core, PostgreSQL, JWT, Swagger, BCrypt.
- Frontend: React, TypeScript, Vite, TailwindCSS, React Router, lucide-react.
- Infra: Docker Compose, Nginx para servir o frontend, volumes Docker para banco e uploads.
- Testes: xUnit no backend e ESLint/build TypeScript no frontend.

## Arquitetura

Backend em camadas:

- `RpgManager.Api`: controllers HTTP, autenticação, CORS, Swagger e bootstrap.
- `RpgManager.Application`: contratos, DTOs e interfaces de serviços.
- `RpgManager.Domain`: entidades e enums de domínio.
- `RpgManager.Infrastructure`: EF Core, migrations, serviços, armazenamento local e integrações.

Frontend:

- SPA React em `frontend/src`.
- API consumida via `VITE_API_BASE_URL`.
- Estado local com React hooks.
- PDF gerado no navegador via página de impressão.
- PWA preparado com manifest, ícones e service worker em build de produção.

Persistência:

- PostgreSQL.
- EF Core migrations versionadas em `backend/src/RpgManager.Infrastructure/Data/Migrations`.
- Migrations são aplicadas automaticamente ao iniciar a API.

## Estrutura de Pastas

```text
backend/
  Dockerfile
  RpgManager.slnx
  src/
    RpgManager.Api/
    RpgManager.Application/
    RpgManager.Domain/
    RpgManager.Infrastructure/
  tests/
    RpgManager.Infrastructure.Tests/
frontend/
  Dockerfile
  nginx.conf
  public/
  src/
docs/
  API.md
  ARCHITECTURE.md
  ROADMAP.md
  spell-import.md
docker-compose.yml
.env.example
LEGAL_NOTICES.md
README.md
```

## Variáveis de Ambiente

Crie o `.env` a partir do exemplo:

```bash
cp .env.example .env
```

Principais variáveis:

- `DATABASE_URL`: URL PostgreSQL do Neon. Use o formato `postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require`.
- `NODE_ENV`: ambiente do frontend/build Node.
- `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_PORT`: banco local.
- `API_PORT`: porta externa da API no Docker.
- `FRONTEND_PORT`: porta externa do frontend no Docker.
- `ALLOWED_ORIGINS`: origens permitidas no CORS da API, separadas por vírgula.
- `VITE_API_BASE_URL`: URL base da API embutida no build do frontend, incluindo `/api`.
- `JWT_ISSUER`, `JWT_AUDIENCE`, `JWT_SECRET`, `JWT_EXPIRATION_MINUTES`: autenticação JWT.
- `LOCAL_FILE_STORAGE_*`: diretório, rota pública e tamanho máximo dos uploads.
- `OPEN5E_*`: configuração do importador de magias SRD do Open5e.

Em produção, troque `JWT_SECRET` por um segredo forte e não reutilize a senha padrão do PostgreSQL.

## Deploy Neon + Vercel

Este repositório é dividido em duas aplicações:

- Backend: API ASP.NET Core com Entity Framework Core e PostgreSQL via Npgsql.
- Frontend: React/Vite, pronto para deploy estático na Vercel.

A Vercel deve hospedar somente o frontend. A API ASP.NET Core tradicional deve rodar em um host compatível com .NET ou Docker, como Render, Railway, Azure, Fly.io ou VPS. Depois de publicar a API, configure `VITE_API_BASE_URL` na Vercel apontando para a URL pública da API com `/api` no final.

1. Crie o banco no Neon:

   - Crie um projeto no Neon.
   - Copie a connection string PostgreSQL.
   - Use uma URL com SSL obrigatório, neste formato fictício:

```bash
DATABASE_URL="postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require"
```

2. Configure as variáveis do backend:

- `DATABASE_URL`: connection string do Neon no formato acima.
- `ConnectionStrings__DefaultConnection`: alternativa compatível com Npgsql. Use uma das duas variáveis, não precisa das duas.
- `ASPNETCORE_URLS=http://+:8080`: porta esperada pelo Dockerfile de produção.
- `ASPNETCORE_ENVIRONMENT=Production`
- `Jwt__Issuer=RpgManager`
- `Jwt__Audience=RpgManager`
- `Jwt__Secret`: segredo forte com pelo menos 32 caracteres.
- `AllowedOrigins=https://seu-frontend.vercel.app,http://localhost:5173`: origens liberadas para CORS, separadas por vírgula.
- `LocalFileStorage__RootPath=/app/wwwroot/uploads`
- `LocalFileStorage__PublicBasePath=/uploads`
- `LocalFileStorage__MaxBytes=5242880`

Não versionar `.env`. Ele já está listado no `.gitignore`.

3. Aplique migrations no Neon:

```bash
export DATABASE_URL="postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require"
dotnet tool restore
dotnet ef database update \
  --project backend/src/RpgManager.Infrastructure/RpgManager.Infrastructure.csproj \
  --startup-project backend/src/RpgManager.Api/RpgManager.Api.csproj
```

A API também executa `Database.Migrate()` ao iniciar. Para produção, prefira rodar o comando de migration no pipeline ou manualmente antes de liberar tráfego para a nova versão.

4. Hospede o backend via Docker:

O Dockerfile do backend fica em `backend/Dockerfile`, publica em Release e expõe a porta interna `8080`.

Build local da imagem:

```bash
docker build -t rpgmanager-api ./backend
```

Execução local apontando para Neon:

```bash
docker run --rm -p 5000:8080 \
  -e ASPNETCORE_URLS="http://+:8080" \
  -e ASPNETCORE_ENVIRONMENT="Production" \
  -e DATABASE_URL="postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require" \
  -e Jwt__Issuer="RpgManager" \
  -e Jwt__Audience="RpgManager" \
  -e Jwt__Secret="change-this-development-secret-with-at-least-32-chars" \
  -e AllowedOrigins="http://localhost:5173" \
  rpgmanager-api
```

No Render, Railway, Azure ou Fly.io, aponte o serviço para `backend/Dockerfile`, configure as variáveis acima e use a porta interna `8080`.

5. Configure o frontend na Vercel:

- Root Directory: `.`
- Build Command: `cd frontend && npm run build`
- Output Directory: `frontend/dist`
- Install Command: `cd frontend && npm ci`
- Environment Variable: `VITE_API_BASE_URL=https://sua-api-publica.example.com/api`

O `vercel.json` na raiz já registra esses comandos para o monorepo.

6. Rode localmente usando Neon:

Backend:

```bash
export DATABASE_URL="postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require"
export Jwt__Issuer="RpgManager"
export Jwt__Audience="RpgManager"
export Jwt__Secret="change-this-development-secret-with-at-least-32-chars"
export AllowedOrigins="http://localhost:5173"
export LocalFileStorage__RootPath="$(pwd)/backend/uploads"
export LocalFileStorage__PublicBasePath="/uploads"
export LocalFileStorage__MaxBytes="5242880"
dotnet run --project backend/src/RpgManager.Api/RpgManager.Api.csproj
```

Frontend:

```bash
cd frontend
npm install
VITE_API_BASE_URL=http://localhost:5000/api npm run dev
```

7. Configure CORS:

No backend em produção, `AllowedOrigins` deve conter a origem exata do frontend, por exemplo:

```bash
AllowedOrigins="https://rpg-manager.vercel.app"
```

Para liberar mais de uma origem, separe por vírgula:

```bash
AllowedOrigins="https://rpg-manager.vercel.app,http://localhost:5173"
```

8. Checklist pós-deploy:

- Acesse `https://sua-api-publica.example.com/api/health` e confirme `status: ok`.
- Acesse o frontend na Vercel.
- Faça cadastro/login.
- Crie uma campanha.
- Crie um personagem.
- Faça upload de imagem se o host tiver storage persistente configurado.
- Confirme no console do navegador que não há erro de CORS.
- Confirme que `DATABASE_URL` e `Jwt__Secret` não aparecem em logs.

9. Rode migrations em produção quando houver mudanças de schema:

```bash
export DATABASE_URL="postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require"
dotnet ef database update \
  --project backend/src/RpgManager.Infrastructure/RpgManager.Infrastructure.csproj \
  --startup-project backend/src/RpgManager.Api/RpgManager.Api.csproj
```

Nunca coloque a connection string real no código, no README ou em logs. Use apenas variáveis de ambiente.

## Rodar com Docker Compose

1. Configure o `.env`:

```bash
cp .env.example .env
```

2. Suba tudo:

```bash
docker compose up --build
```

3. Acesse:

- Frontend: http://localhost:8080
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Health: http://localhost:5000/api/health

Se a porta local `5432` já estiver em uso:

```bash
POSTGRES_PORT=15432 docker compose up --build
```

Se quiser alterar as portas externas da aplicação:

```bash
API_PORT=15000 FRONTEND_PORT=18080 POSTGRES_PORT=15432 VITE_API_BASE_URL=http://localhost:15000/api docker compose up --build
```

Parar os containers:

```bash
docker compose down
```

Parar e apagar o banco local:

```bash
docker compose down -v
```

## Rodar Backend Manualmente

1. Suba o PostgreSQL:

```bash
docker compose up -d postgres
```

2. Configure variáveis no shell:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=rpgmanager;Username=rpgmanager;Password=rpgmanager"
export Jwt__Issuer="RpgManager"
export Jwt__Audience="RpgManager"
export Jwt__Secret="change-this-development-secret-with-at-least-32-chars"
export AllowedOrigins="http://localhost:5173"
export LocalFileStorage__RootPath="$(pwd)/backend/uploads"
export LocalFileStorage__PublicBasePath="/uploads"
export LocalFileStorage__MaxBytes="5242880"
```

Para usar Neon em vez do PostgreSQL local, defina `DATABASE_URL` e omita `ConnectionStrings__DefaultConnection`:

```bash
export DATABASE_URL="postgresql://USER:PASSWORD@HOST.neon.tech/DBNAME?sslmode=require"
```

3. Rode a API:

```bash
dotnet run --project backend/src/RpgManager.Api/RpgManager.Api.csproj
```

API local: http://localhost:5000.

## Rodar Frontend Manualmente

```bash
cd frontend
npm install
npm run dev
```

Frontend dev: http://localhost:5173.

Para apontar para outra API:

```bash
VITE_API_BASE_URL=http://localhost:5000/api npm run dev
```

Build de produção:

```bash
cd frontend
npm run build
npm run preview -- --host 0.0.0.0
```

## Migrations

A API executa `Database.Migrate()` ao iniciar. Em desenvolvimento, também é possível aplicar manualmente:

```bash
dotnet tool restore
dotnet ef database update \
  --project backend/src/RpgManager.Infrastructure/RpgManager.Infrastructure.csproj \
  --startup-project backend/src/RpgManager.Api/RpgManager.Api.csproj
```

Criar nova migration:

```bash
dotnet ef migrations add NomeDaMigration \
  --project backend/src/RpgManager.Infrastructure/RpgManager.Infrastructure.csproj \
  --startup-project backend/src/RpgManager.Api/RpgManager.Api.csproj \
  --output-dir Data/Migrations
```

## Criar Usuário Inicial

Não há seed automático. Crie o primeiro usuário pela interface em `/login` alternando para cadastro, ou via API:

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Mestre","email":"mestre@example.com","password":"senha1234","profile":"GameMaster"}'
```

Perfis aceitos:

- `Player`
- `GameMaster`

## Módulos Implementados

- Autenticação JWT: cadastro, login e sessão atual.
- Campanhas: CRUD, convite, membros, painel do mestre.
- Personagens: CRUD, vínculo com campanha, ficha resumida e exportação PDF.
- Atributos e testes de resistência.
- Perícias com proficiência, expertise e bônus customizado.
- Combate: vida, CA, iniciativa, deslocamento, dados de vida, ataques e condições.
- Notebook: notas privadas, visíveis para mestre, categorias, tags e busca.
- Inventário: itens, moedas, peso total, equipado e sintonizado.
- Imagens: avatar, token e galeria local.
- Biblioteca de magias: CRUD, filtros, visibilidade, homebrew e importação SRD Open5e.
- Vínculo de magias ao personagem: conhecidas, preparadas, favoritas, notas e slots.
- Biblioteca de talentos/características: CRUD, filtros, visibilidade e homebrew.
- Vínculo de talentos/características ao personagem: biblioteca ou manual, usos e recuperação.
- Rolador de dados: expressões simples, vantagem/desvantagem em d20 e histórico local no frontend.
- Descanso curto/longo: recuperação simples de recursos, slots e opções de vida/dados de vida.
- Tema claro/escuro, responsividade mobile e PWA básico.

## Endpoints Principais

Autenticação:

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/me`
- `GET /api/health`

Campanhas:

- `GET /api/campaigns`
- `POST /api/campaigns`
- `GET /api/campaigns/{id}`
- `PUT /api/campaigns/{id}`
- `DELETE /api/campaigns/{id}`
- `POST /api/campaigns/join`
- `POST /api/campaigns/{id}/invite/regenerate`
- `GET /api/campaigns/{id}/members`
- `GET /api/campaigns/{id}/characters`
- `GET /api/campaigns/{id}/master-dashboard`

Personagens:

- `GET /api/characters`
- `POST /api/characters`
- `GET /api/characters/{id}`
- `PUT /api/characters/{id}`
- `DELETE /api/characters/{id}`
- `GET|PUT /api/characters/{id}/attributes`
- `GET|PUT /api/characters/{id}/saving-throws`
- `GET|PUT /api/characters/{id}/skills`
- `GET|PUT /api/characters/{id}/combat`
- `GET|POST /api/characters/{id}/attacks`
- `PUT|DELETE /api/characters/{id}/attacks/{attackId}`
- `GET|PUT /api/characters/{id}/conditions`
- `GET|POST /api/characters/{id}/notes`
- `GET|PUT|DELETE /api/characters/{id}/notes/{noteId}`
- `GET|POST /api/characters/{id}/inventory`
- `PUT|DELETE /api/characters/{id}/inventory/{itemId}`
- `GET|PUT /api/characters/{id}/currency`
- `GET|POST /api/characters/{id}/spells`
- `PUT|DELETE /api/characters/{id}/spells/{characterSpellId}`
- `GET|PUT /api/characters/{id}/spell-slots`
- `GET|POST /api/characters/{id}/features`
- `PUT|DELETE /api/characters/{id}/features/{characterFeatureId}`
- `POST /api/characters/{id}/short-rest`
- `POST /api/characters/{id}/long-rest`
- `GET|POST /api/characters/{id}/assets`
- `DELETE /api/characters/{id}/assets/{assetId}`
- `PUT /api/characters/{id}/avatar`
- `PUT /api/characters/{id}/token`

Bibliotecas:

- `GET|POST /api/spells`
- `GET|PUT|DELETE /api/spells/{id}`
- `POST /api/spells/import/open5e`
- `GET|POST /api/features`
- `GET|PUT|DELETE /api/features/{id}`

Dados:

- `POST /api/dice/roll`

Consulte [docs/API.md](docs/API.md) para exemplos de payloads.

## Regras de Permissão

- Usuário autenticado vê suas próprias campanhas e personagens.
- Mestre (`GameMaster`) cria campanha e vira membro com papel `Master`.
- Jogador entra em campanha por código de convite.
- Apenas Mestre edita/exclui campanha e regenera convite.
- Apenas membros acessam detalhes básicos da campanha.
- Apenas Mestre acessa `master-dashboard`.
- Mestre pode visualizar fichas vinculadas à campanha dele, mas não edita personagem do jogador.
- Dono do personagem edita personagem, atributos, perícias, combate, ataques, notas, inventário, imagens, magias, slots, talentos e descansos.
- Usuário externo não acessa personagem sem permissão.
- Nota privada aparece apenas ao dono.
- Nota visível para Mestre aparece no painel se não for privada.
- Magia/talento privado aparece apenas ao criador.
- Magia/talento de campanha aparece aos membros da campanha.
- Conteúdo `LocalPublic` aparece para todos os usuários autenticados.
- Conteúdo de campanha só pode ser criado/editado pelo Mestre daquela campanha.
- Magia duplicada no mesmo personagem é bloqueada.

## Conteúdo Protegido e Licenças

Não cadastre nem distribua texto protegido de livros oficiais sem licença. O projeto foi feito para:

- conteúdo próprio do usuário;
- conteúdo homebrew;
- resumos manuais;
- dados fictícios de exemplo;
- material SRD/aberto quando permitido pela licença.

O importador Open5e deve ser usado apenas com fontes permitidas. Veja [LEGAL_NOTICES.md](LEGAL_NOTICES.md).

## Conteúdo Homebrew

Magias e talentos/características podem ser cadastrados nas bibliotecas:

- Abra `Biblioteca de Magias` ou `Talentos e Características`.
- Clique para criar novo item.
- Use dados próprios/fictícios.
- Marque `IsHomebrew` quando aplicável.
- Escolha visibilidade:
  - `Private`: só o criador vê.
  - `Campaign`: membros da campanha veem.
  - `LocalPublic`: todos os usuários autenticados veem.

Depois, abra a ficha do personagem e adicione o item na aba correspondente.

## Upload de Imagens

Na ficha do personagem, aba `Imagens`:

- envie avatar;
- envie token;
- envie imagens de galeria;
- remova itens da galeria.

Configuração:

- `LOCAL_FILE_STORAGE_ROOT_PATH`: onde os arquivos ficam no servidor.
- `LOCAL_FILE_STORAGE_PUBLIC_BASE_PATH`: rota pública servida pela API.
- `LOCAL_FILE_STORAGE_MAX_BYTES`: tamanho máximo, padrão 5 MB.

No Docker, uploads ficam no volume `character_uploads`.

## Exportar PDF

Na ficha do personagem, clique em `Exportar PDF`. O frontend abre uma versão resumida em nova aba e aciona a impressão do navegador. Escolha `Salvar como PDF`.

Inclui:

- dados gerais;
- jogador e campanha;
- atributos e modificadores;
- perícias principais;
- vida, CA, iniciativa e deslocamento;
- ataques;
- magias vinculadas;
- talentos/características;
- inventário resumido;
- anotações rápidas.

Limitações:

- usa impressão do navegador;
- margens e nome do arquivo variam por sistema;
- imagens não entram para evitar PDFs gigantes;
- pop-ups precisam estar liberados para a aplicação.

## Executar Testes

Backend:

```bash
dotnet test backend/RpgManager.slnx
```

Frontend:

```bash
cd frontend
npm run lint
npm run build
```

Teste rápido local:

```bash
curl http://localhost:5000/api/health
```

## Roadmap Futuro

- Separar `frontend/src/App.tsx` em módulos menores.
- Criar testes de integração para permissões e fluxo completo.
- Criar E2E com navegador para mobile, PWA e exportação PDF.
- Melhorar importação/tradução de conteúdo aberto.
- Expandir automações opcionais de classe, raça/espécie e descanso.
- Melhorar versionamento/auditoria de mudanças na ficha.
- Suportar storage externo para uploads.
- Melhorar gerenciamento de membros da campanha.
- Criar modo offline mais robusto para PWA.
