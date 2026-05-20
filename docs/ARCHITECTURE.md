# Arquitetura

## Visão Geral

O projeto é uma aplicação web/PWA composta por API ASP.NET Core, frontend React e PostgreSQL. A API concentra regras de permissão, persistência e integrações. O frontend fornece a experiência de ficha digital e bibliotecas.

## Backend

Camadas:

- `RpgManager.Api`: controllers, autenticação JWT, CORS, Swagger, arquivos estáticos e inicialização da aplicação.
- `RpgManager.Application`: DTOs, interfaces e contratos entre API e infraestrutura.
- `RpgManager.Domain`: entidades (`User`, `Campaign`, `Character`, `Spell`, `Feature` etc.) e enums.
- `RpgManager.Infrastructure`: EF Core, migrations, serviços de aplicação, importador Open5e, rolagem de dados e storage local.

Fluxo típico:

1. Controller recebe request autenticado.
2. Controller extrai `UserId` do JWT.
3. Serviço valida permissão e dados.
4. Serviço persiste via `AppDbContext`.
5. Controller retorna DTO.

## Frontend

- React + TypeScript + Vite.
- Rotas protegidas por token salvo em `localStorage`.
- Tema claro/escuro salvo em `localStorage`.
- API base configurada por `VITE_API_BASE_URL`.
- PWA básico via `manifest.webmanifest` e `sw.js`.
- Exportação PDF gerada no navegador por página de impressão.

## Banco

- PostgreSQL.
- Migrations versionadas em `RpgManager.Infrastructure/Data/Migrations`.
- A API aplica migrations automaticamente no startup.

## Permissões

As regras ficam nos serviços de infraestrutura, principalmente:

- `CampaignService`
- `CharacterService`
- `SpellService`
- `FeatureService`

Controllers apenas traduzem `ServiceResult` para HTTP.

## Uploads

Uploads usam `IFileStorageService`. A implementação atual é local (`LocalFileStorageService`), servida por `UseStaticFiles`. A interface permite troca futura por S3, Azure Blob, Bunny Storage ou equivalente.

## Pontos de Atenção

- O frontend ainda está concentrado em `App.tsx`; futuras evoluções devem modularizar telas e componentes.
- Não coloque texto protegido oficial em seeds, exemplos ou fixtures.
- Não mova regra de permissão para o frontend. O frontend pode esconder botões, mas a API deve continuar bloqueando.
