Você é um arquiteto e desenvolvedor full-stack sênior. Crie uma aplicação web responsiva/PWA para gerenciamento de personagens de RPG de mesa, focada em D&D 5e/5.5e, para uso privado entre amigos.

## Objetivo do projeto

Criar uma aplicação parecida com uma ficha digital completa de RPG, onde usuários podem criar conta, entrar em campanhas/mesas, cadastrar personagens, preencher ficha completa, adicionar imagem/avatar, registrar magias, talentos, itens, anotações e acompanhar recursos do personagem.

A aplicação deve ser bem organizada, moderna, responsiva e utilizável tanto no desktop quanto no celular.

Não copie conteúdo protegido de livros oficiais pagos. Use apenas estrutura de dados e deixe espaço para conteúdo customizado/homebrew. A aplicação deve permitir cadastro manual de magias, talentos, itens, espécies, classes, antecedentes e características. Caso exista conteúdo base permitido/legal, deixe o projeto preparado para importação futura via seed controlado.

## Stack desejada

Backend:
- C# com ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- ASP.NET Identity ou autenticação JWT
- Swagger/OpenAPI
- Arquitetura limpa e separada por camadas

Frontend:
- React + TypeScript
- Vite ou Next.js, escolha a opção mais simples e estável para iniciar
- TailwindCSS
- shadcn/ui ou componentes reutilizáveis próprios
- Layout responsivo mobile-first
- Tema claro/escuro

Infra:
- Docker Compose para subir backend, frontend e PostgreSQL localmente
- Variáveis de ambiente para connection string, JWT secret e storage
- Estrutura preparada para deploy futuro em Railway, Render, Azure ou VPS

## Módulos principais

### 1. Usuários e autenticação

Implementar:
- Cadastro de usuário
- Login
- Logout
- Perfil do usuário
- Avatar do usuário
- Alteração básica de dados
- Autenticação por JWT ou Identity

Perfis:
- Mestre
- Jogador

Um usuário pode ser mestre em uma campanha e jogador em outra.

### 2. Campanhas / Mesas

Implementar:
- Criar campanha
- Editar campanha
- Excluir campanha
- Listar minhas campanhas
- Entrar em campanha por código de convite
- Gerar código/link de convite
- Definir membros da campanha
- Definir papel do membro: Mestre ou Jogador

Campos sugeridos:
- Id
- Nome
- Descrição
- Sistema, exemplo: D&D 5e, D&D 5.5e, Homebrew
- Imagem/capa
- Código de convite
- CriadoPorUserId
- DataCriacao

### 3. Personagens

Cada usuário pode criar vários personagens.

Campos principais:
- Id
- UserId
- CampaignId opcional
- Nome
- Apelido
- Imagem/avatar
- TokenImage
- Nível total
- Espécie/Raça
- Classe principal
- Subclasse
- Antecedente
- Alinhamento
- Experiência
- Inspiração
- Bônus de proficiência
- Classe de Armadura
- Iniciativa
- Deslocamento
- Vida máxima
- Vida atual
- Vida temporária
- Dados de vida totais
- Dados de vida disponíveis
- Descrição física
- Personalidade
- Ideais
- Vínculos
- Defeitos
- História
- Anotações rápidas

A ficha deve ter abas:
- Visão Geral
- Atributos e Perícias
- Combate
- Magias
- Talentos e Características
- Inventário
- Notebook
- Imagens/Arquivos

### 4. Atributos e perícias

Atributos:
- Força
- Destreza
- Constituição
- Inteligência
- Sabedoria
- Carisma

Para cada atributo:
- Valor
- Modificador calculado automaticamente

Perícias:
- Acrobacia
- Arcanismo
- Atletismo
- Atuação
- Enganação
- Furtividade
- História
- Intimidação
- Intuição
- Investigação
- Lidar com Animais
- Medicina
- Natureza
- Percepção
- Persuasão
- Prestidigitação
- Religião
- Sobrevivência

Para cada perícia:
- Atributo base
- Proficiente
- Especialista
- Bônus customizado
- Valor final calculado

Testes de resistência:
- Um por atributo
- Proficiente ou não
- Bônus customizado
- Valor final calculado

### 5. Combate

Implementar:
- Ataques
- Armas
- Dano
- Tipo de dano
- Alcance
- Bônus de ataque
- Observações
- Condições ativas
- Resistências
- Imunidades
- Vulnerabilidades
- Rolagens rápidas

Entidade sugerida CharacterAttack:
- Id
- CharacterId
- Nome
- BonusAtaque
- Dano
- TipoDano
- Alcance
- UsaAtributo
- Observacoes

Condições:
- Blinded / Cego
- Charmed / Enfeitiçado
- Deafened / Surdo
- Frightened / Amedrontado
- Grappled / Agarrado
- Incapacitated / Incapacitado
- Invisible / Invisível
- Paralyzed / Paralisado
- Petrified / Petrificado
- Poisoned / Envenenado
- Prone / Caído
- Restrained / Contido
- Stunned / Atordoado
- Unconscious / Inconsciente
- Exhaustion / Exaustão

Deixar as descrições editáveis para evitar depender de conteúdo protegido.

### 6. Magias

Criar módulo completo de magias com busca e filtros.

Campos da entidade Spell:
- Id
- Nome
- NomeIngles
- Nivel
- Escola
- TempoConjuracao
- Alcance
- Componentes
- Material
- Duracao
- Concentracao
- Ritual
- Descricao
- DescricaoEmNivelSuperior
- ClassesDisponiveis
- Fonte
- IsHomebrew
- CriadoPorUserId
- Visibilidade: privada, campanha, pública-local

Filtros:
- Nome
- Nível
- Escola
- Classe
- Concentração
- Ritual
- Tempo de conjuração
- Alcance
- Homebrew ou base

Na ficha do personagem:
- Magias conhecidas
- Magias preparadas
- Magias favoritas
- Truques
- Slots por nível
- Slots usados
- CD de magia
- Bônus de ataque mágico
- Atributo de conjuração

Entidades:
- Spell
- CharacterSpell
- CharacterSpellSlots

CharacterSpell:
- CharacterId
- SpellId
- IsKnown
- IsPrepared
- IsFavorite
- Notes

CharacterSpellSlots:
- CharacterId
- SpellLevel
- TotalSlots
- UsedSlots

### 7. Talentos e características

Criar módulo para:
- Talentos
- Características de classe
- Traços de espécie/raça
- Características de antecedente
- Características homebrew

Entidade Feature:
- Id
- Nome
- Tipo: Talento, Classe, Subclasse, Espécie, Antecedente, Homebrew
- Descricao
- Fonte
- PreRequisitos
- IsHomebrew
- CriadoPorUserId

Na ficha:
- Adicionar/remover talentos
- Adicionar/remover características
- Marcar recursos com usos limitados

Entidade CharacterFeature:
- Id
- CharacterId
- FeatureId
- NomeCustomizado
- DescricaoCustomizada
- UsosMaximos
- UsosAtuais
- RecuperaEm: descanso curto, descanso longo, manual
- Observacoes

### 8. Inventário

Implementar:
- Itens
- Equipamentos
- Armas
- Armaduras
- Itens mágicos
- Consumíveis
- Moedas
- Peso

Entidade Item:
- Id
- Nome
- Tipo
- Descricao
- Peso
- Valor
- Raridade
- RequerSintonia
- IsHomebrew
- CriadoPorUserId

Entidade CharacterInventoryItem:
- Id
- CharacterId
- ItemId opcional
- Nome
- Descricao
- Quantidade
- Peso
- Valor
- Equipado
- Sintonizado
- Observacoes

Moedas:
- Cobre
- Prata
- Electrum
- Ouro
- Platina

### 9. Notebook do personagem

Criar um caderno interno para cada personagem.

Recursos:
- Criar páginas
- Editar páginas
- Excluir páginas
- Categorias
- Tags
- Busca textual
- Marcar como privado
- Marcar como visível para o mestre

Categorias sugeridas:
- Diário
- NPCs
- Missões
- Lugares
- Segredos
- Itens
- Teorias
- Sessões
- Outros

Entidade CharacterNote:
- Id
- CharacterId
- Titulo
- Conteudo
- Categoria
- Tags
- IsPrivate
- IsVisibleToMaster
- CreatedAt
- UpdatedAt

O editor pode começar como textarea/markdown simples. Depois preparar para editor rico.

### 10. Imagens e arquivos

Implementar:
- Upload de avatar do personagem
- Upload de token
- Galeria de imagens do personagem
- Anexos opcionais
- Armazenamento local em desenvolvimento
- Preparar interface para trocar depois por Azure Blob, S3 ou Bunny Storage

Entidade CharacterAsset:
- Id
- CharacterId
- FileName
- FileUrl
- FileType
- AssetType: Avatar, Token, Galeria, Documento
- UploadedAt

### 11. Rolador de dados

Criar rolador simples:
- d4
- d6
- d8
- d10
- d12
- d20
- d100
- Rolagem customizada, exemplo: 2d6+3
- Vantagem
- Desvantagem
- Histórico de rolagens local

Na ficha:
- Botão para rolar atributo
- Botão para rolar perícia
- Botão para rolar ataque
- Botão para rolar dano
- Botão para rolar teste de resistência

Não precisa ter rolagem em tempo real no MVP, mas deixar arquitetura preparada.

### 12. Descansos e recursos

Implementar:
- Descanso curto
- Descanso longo
- Recuperação de slots
- Recuperação de recursos marcados
- Reset manual de usos

Inicialmente, fazer regras simples e editáveis pelo usuário, sem tentar automatizar todas as exceções do sistema.

## Telas principais

Frontend deve ter:

1. Landing/Login
2. Cadastro
3. Dashboard
4. Minhas campanhas
5. Detalhe da campanha
6. Meus personagens
7. Criar/editar personagem
8. Ficha do personagem com abas
9. Biblioteca de magias
10. Biblioteca de talentos/características
11. Biblioteca de itens
12. Cadastro/edição de conteúdo homebrew
13. Perfil do usuário

## Requisitos de UI/UX

- Design moderno, limpo e responsivo
- Mobile-first
- Funcionar bem no celular
- Usar cards, abas e menus claros
- Tema escuro e claro
- Sidebar no desktop
- Bottom navigation ou menu compacto no mobile
- Busca e filtros nas bibliotecas
- Botões grandes e fáceis de usar em celular
- Feedback visual para salvar, erro e carregamento
- Evitar telas muito poluídas

## Arquitetura backend sugerida

Criar solução com projetos separados:

- RpgManager.Api
- RpgManager.Application
- RpgManager.Domain
- RpgManager.Infrastructure

Domain:
- Entidades
- Enums
- Regras básicas de domínio

Application:
- DTOs
- Services
- Interfaces
- Use cases

Infrastructure:
- DbContext
- Migrations
- Repositories
- Implementações de storage
- Identity/Auth

Api:
- Controllers
- Middlewares
- Swagger
- Configuração de DI

## Endpoints mínimos

Auth:
- POST /api/auth/register
- POST /api/auth/login
- GET /api/auth/me

Campaigns:
- GET /api/campaigns
- POST /api/campaigns
- GET /api/campaigns/{id}
- PUT /api/campaigns/{id}
- DELETE /api/campaigns/{id}
- POST /api/campaigns/join
- POST /api/campaigns/{id}/invite/regenerate

Characters:
- GET /api/characters
- POST /api/characters
- GET /api/characters/{id}
- PUT /api/characters/{id}
- DELETE /api/characters/{id}

Character sections:
- GET/PUT /api/characters/{id}/attributes
- GET/PUT /api/characters/{id}/skills
- GET/PUT /api/characters/{id}/combat
- GET/PUT /api/characters/{id}/spell-slots
- GET/POST/DELETE /api/characters/{id}/spells
- GET/POST/DELETE /api/characters/{id}/features
- GET/POST/PUT/DELETE /api/characters/{id}/inventory
- GET/POST/PUT/DELETE /api/characters/{id}/notes
- GET/POST/DELETE /api/characters/{id}/assets

Library:
- GET /api/spells
- POST /api/spells
- GET /api/spells/{id}
- PUT /api/spells/{id}
- DELETE /api/spells/{id}

- GET /api/features
- POST /api/features
- GET /api/features/{id}
- PUT /api/features/{id}
- DELETE /api/features/{id}

- GET /api/items
- POST /api/items
- GET /api/items/{id}
- PUT /api/items/{id}
- DELETE /api/items/{id}

Dice:
- POST /api/dice/roll

## Regras de permissão

- Usuário só pode editar seus próprios personagens.
- Mestre pode visualizar personagens da campanha.
- Mestre pode criar conteúdo visível para a campanha.
- Conteúdo privado só aparece para quem criou.
- Conteúdo de campanha aparece para membros da campanha.
- Conteúdo base aparece para todos.
- Apenas criador pode editar conteúdo homebrew privado.
- Mestre pode editar conteúdo homebrew da campanha.

## Cuidados importantes

- Não colocar secrets no appsettings.json versionado.
- Criar .env.example.
- Criar .gitignore adequado.
- Criar README.md com instruções.
- Usar migrations do EF Core.
- Criar seed inicial mínimo apenas com dados genéricos e próprios, sem copiar descrições oficiais protegidas.
- Preparar projeto para importação futura de SRD aberta, mas não incluir material protegido.
- Validar entradas no backend.
- Usar DTOs, não expor entidades diretamente.
- Tratar erros com respostas padronizadas.
- Usar paginação nas bibliotecas de magias, itens e talentos.
- Criar filtros no backend e frontend.

## MVP obrigatório

Primeiro entregue um MVP funcional com:

- Login/cadastro
- Dashboard
- CRUD de campanhas
- Entrar por código de convite
- CRUD de personagens
- Ficha com dados gerais
- Atributos com cálculo de modificador
- Perícias com proficiência
- Vida, CA, iniciativa e deslocamento
- Notebook do personagem
- Inventário simples
- Biblioteca de magias com CRUD
- Adicionar magia ao personagem
- Upload simples de avatar
- Layout responsivo
- Docker Compose com PostgreSQL
- README com como rodar

Depois, evoluir para:
- Talentos/características
- Slots de magia
- Rolador de dados
- Recursos por descanso
- Painel do mestre
- Tema escuro
- PWA instalável
- Exportar ficha em PDF

## Entregáveis esperados

1. Criar a estrutura do projeto.
2. Implementar backend com entidades, DbContext, migrations, controllers e services.
3. Implementar frontend com telas principais.
4. Criar componentes reutilizáveis.
5. Criar Docker Compose.
6. Criar README.md.
7. Criar .env.example.
8. Criar .gitignore.
9. Garantir que o projeto rode localmente.
10. Explicar no README como criar o banco, aplicar migrations e iniciar backend/frontend.

## Critérios de qualidade

- Código limpo e organizado.
- Sem gambiarras.
- Separação clara entre frontend e backend.
- Nomes em inglês no código, mas interface em português.
- Componentes reutilizáveis.
- Responsivo no celular.
- Backend validando permissões.
- Sem conteúdo protegido copiado.
- Pronto para evoluir sem reescrever tudo.

Comece criando a estrutura inicial do projeto, depois implemente o MVP em etapas pequenas e testáveis.

## Objetivo

Este projeto é uma aplicação web para gerenciamento de campanhas de RPG.  
O objetivo desta etapa é implementar quatro grandes módulos novos:

1. Integração/estrutura para anotações com Obsidian/Notion.
2. Combat tracker inspirado em ferramentas como Improved Initiative.
3. Criação de personagens com seletores de raça, classe, antecedente e dados relacionados.
4. Criador/organizador de NPCs exclusivo para o MESTRE.

Além da implementação, o agente deve preparar e executar o fluxo de deploy da aplicação, respeitando a arquitetura atual do projeto.

---

## Regras principais

Antes de alterar código:

1. Analise a estrutura atual do projeto.
2. Identifique backend, frontend, banco, autenticação, autorização e fluxo de deploy existente.
3. Não quebre funcionalidades já implementadas.
4. Siga o padrão atual de pastas, nomes, services, repositories, controllers, DTOs, hooks e componentes.
5. Não duplique entidades se já existir algo equivalente.
6. Não exponha secrets, tokens ou connection strings no frontend ou no repositório.
7. Qualquer alteração de banco deve ser feita por migration.
8. Após cada módulo, rode build/test/lint quando existirem scripts disponíveis.
9. Ao final, atualize documentação de ambiente e deploy.

---

## Modelo de permissão obrigatório

A aplicação deve respeitar rigidamente a separação entre MESTRE e JOGADOR.

### Roles

- MASTER
- PLAYER

### Regra geral

MASTER:

- Pode ver tudo dentro da campanha.
- Pode criar, editar e excluir anotações da campanha.
- Pode ver anotações próprias e anotações dos jogadores.
- Pode criar, editar, excluir e buscar NPCs.
- Pode criar e controlar encontros.
- Pode visualizar dados privados de NPCs, encontros e notas.
- Pode publicar dados para jogadores.

PLAYER:

- Só pode ver dados próprios.
- Só pode ver dados publicados para jogadores.
- Não pode acessar anotações privadas do mestre.
- Não pode acessar NPCs `MasterOnly`.
- Não pode acessar campos secretos.
- Não pode criar/editar/excluir NPCs.
- Não pode controlar encontros, salvo se já existir regra específica no projeto permitindo isso.

A validação deve existir no backend.  
Não confiar apenas no frontend.

---

## Enum de visibilidade

Criar ou reaproveitar um enum semelhante:

```csharp
public enum Visibility
{
    Private = 0,
    Campaign = 1,
    MasterOnly = 2,
    PlayerOnly = 3,
    PublicToPlayers = 4
}
```

Caso o projeto não use C#, adapte para o padrão da stack atual.

### Significado

- `Private`: visível apenas para o criador.
- `Campaign`: visível conforme regras internas da campanha.
- `MasterOnly`: visível apenas para mestre.
- `PlayerOnly`: visível apenas para jogador dono ou jogadores permitidos.
- `PublicToPlayers`: visível para jogadores da campanha.

---
# Nova etapa de implementação — Features de campanha RPG

## Contexto importante

O projeto já está em andamento, já foi publicado e está funcionando em produção.

Não refatorar o projeto inteiro.
Não recriar arquitetura.
Não recriar autenticação.
Não recriar deploy.
Não alterar fluxo de produção sem necessidade.
Não apagar ou substituir funcionalidades existentes.

O objetivo é apenas implementar novas funcionalidades de forma incremental, respeitando a arquitetura atual.

Antes de codar:
1. Ler a estrutura atual do projeto.
2. Identificar como já funcionam usuários, campanhas, permissões, personagens, banco e deploy.
3. Reaproveitar padrões existentes.
4. Criar migrations apenas para as novas tabelas/campos necessários.
5. Preservar compatibilidade com produção.
6. Garantir que o build continue funcionando.
7. Atualizar variáveis de ambiente somente se necessário.
8. Não commitar secrets.

## Features a implementar

Implementar, em fases, os módulos:

1. Anotações com estrutura para Obsidian/Notion.
2. Combat tracker estilo Improved Initiative.
3. Criação de personagens com seletor de raça/classe/antecedente.
4. Organizador de NPCs exclusivo para o mestre.

A prioridade é segurança de permissão entre MESTRE e JOGADOR.

# Módulo 1 — Anotações com estrutura para Obsidian/Notion

## Objetivo

Criar um sistema interno de anotações em Markdown, preparado para futura integração com Notion e Obsidian.

A integração externa completa pode ser preparada com interfaces/services, mas não precisa ser finalizada nesta primeira etapa se isso comprometer o escopo.

## Entidade: CampaignNote

Criar entidade:

```txt
CampaignNote
- Id
- CampaignId
- OwnerUserId
- Title
- ContentMarkdown
- Tags
- Visibility
- LinkedEntityType
- LinkedEntityId
- ExternalProvider
- ExternalId
- CreatedAt
- UpdatedAt
```

## ExternalProvider

Criar enum ou equivalente:

```txt
ExternalProvider
- None
- Notion
- Obsidian
```

## Regras

- Nota criada por MASTER deve ser `MasterOnly` por padrão.
- Nota criada por PLAYER deve ser `Private` por padrão.
- MASTER vê todas as notas da campanha.
- PLAYER vê apenas notas próprias ou notas `PublicToPlayers`.
- PLAYER nunca pode acessar nota `MasterOnly`.
- Conteúdo deve aceitar Markdown.
- O campo `ContentMarkdown` deve ser sanitizado/renderizado com segurança no frontend.

## Endpoints esperados

```txt
GET    /api/campaigns/{campaignId}/notes
GET    /api/campaigns/{campaignId}/notes/{noteId}
POST   /api/campaigns/{campaignId}/notes
PUT    /api/campaigns/{campaignId}/notes/{noteId}
DELETE /api/campaigns/{campaignId}/notes/{noteId}
```

## Filtros desejáveis

```txt
?search=
?tag=
?visibility=
?linkedEntityType=
?linkedEntityId=
```

## Frontend

Criar tela ou componente:

```txt
CampaignNotesPage
```

Funcionalidades:

- Listar notas.
- Criar nota.
- Editar nota.
- Excluir nota.
- Buscar por título/conteúdo.
- Filtrar por tag.
- Definir visibilidade.
- Editor Markdown simples.
- Preview Markdown, se a stack já tiver suporte simples para isso.

## Preparação para Notion/Obsidian

Criar interface ou service abstrato:

```txt
INotesExternalProvider
- ExportNoteAsync
- ImportNoteAsync
- SyncNoteAsync
```

Providers planejados:

```txt
NotionNotesProvider
ObsidianMarkdownProvider
```

Nesta etapa, pode implementar apenas stubs seguros.

Não implementar tokens hardcoded.  
Usar variáveis de ambiente futuramente:

```txt
NOTION_API_KEY
NOTION_DATABASE_ID
OBSIDIAN_API_URL
OBSIDIAN_API_KEY
```

---

# Módulo 2 — Organizador de NPCs exclusivo do mestre

## Objetivo

Criar um organizador de NPCs para o mestre cadastrar, buscar e manter fichas de NPCs sem perder informações importantes.

## Entidade: NPC

```txt
NPC
- Id
- CampaignId
- CreatedByUserId
- Name
- Alias
- Race
- Occupation
- Location
- Faction
- Personality
- Appearance
- Motivation
- Secrets
- Notes
- StatBlockJson
- Tags
- IsImportant
- IsAlive
- Visibility
- CreatedAt
- UpdatedAt
```

## Regras

- Apenas MASTER pode criar NPC.
- Apenas MASTER pode editar NPC.
- Apenas MASTER pode excluir NPC.
- PLAYER só pode visualizar NPC se `Visibility = PublicToPlayers`.
- Campo `Secrets` nunca deve ser retornado para PLAYER.
- Campo `Notes` pode ser privado se o projeto separar notas públicas/privadas.
- NPC deve pertencer a uma campanha.
- Busca por nome deve ser obrigatória no MVP.

## Endpoints esperados

```txt
GET    /api/campaigns/{campaignId}/npcs
GET    /api/campaigns/{campaignId}/npcs/{npcId}
POST   /api/campaigns/{campaignId}/npcs
PUT    /api/campaigns/{campaignId}/npcs/{npcId}
DELETE /api/campaigns/{campaignId}/npcs/{npcId}
```

## Filtros

```txt
?search=
?tag=
?location=
?faction=
?isImportant=
?isAlive=
?visibility=
```

## Frontend

Criar tela:

```txt
NpcManagerPage
```

Funcionalidades:

- Criar NPC.
- Editar NPC.
- Excluir NPC.
- Buscar por nome.
- Filtrar por tag.
- Filtrar por localidade.
- Filtrar por facção.
- Marcar como importante.
- Marcar como vivo/morto/desaparecido, se a modelagem permitir.
- Campo de segredos visível apenas ao mestre.
- Campo de ficha/statblock em JSON ou formulário simples.

---

# Módulo 3 — Criação de personagem com seletores

## Objetivo

Melhorar a criação de personagens adicionando seletores de raça, classe, antecedente e dados relacionados.

Não inserir conteúdo oficial protegido por copyright.  
Usar dados fictícios, vazios, homebrew ou conteúdo legalmente permitido.

## Entidades

Criar ou ajustar:

```txt
Race
- Id
- Name
- Description
- Source
- IsHomebrew
- CreatedByUserId
- CreatedAt
- UpdatedAt
```

```txt
CharacterClass
- Id
- Name
- HitDie
- Description
- Source
- IsHomebrew
- CreatedByUserId
- CreatedAt
- UpdatedAt
```

```txt
Background
- Id
- Name
- Description
- Source
- IsHomebrew
- CreatedByUserId
- CreatedAt
- UpdatedAt
```

Atualizar Character, se necessário:

```txt
Character
- RaceId
- ClassId
- BackgroundId
- Level
- Strength
- Dexterity
- Constitution
- Intelligence
- Wisdom
- Charisma
```

Adaptar para a modelagem atual do projeto.

## Endpoints esperados

```txt
GET  /api/races
POST /api/races

GET  /api/classes
POST /api/classes

GET  /api/backgrounds
POST /api/backgrounds
```

Se o projeto usa escopo por campanha, os endpoints podem ser:

```txt
GET  /api/campaigns/{campaignId}/races
POST /api/campaigns/{campaignId}/races
```

Mesma lógica para classes e backgrounds.

## Regras

- MASTER pode cadastrar opções homebrew para a campanha.
- PLAYER pode selecionar opções disponíveis.
- Não usar descrições oficiais completas de livros pagos.
- Permitir campos vazios para futura importação.
- Se existir seed inicial, usar nomes genéricos/fictícios.

## Frontend

Criar ou ajustar:

```txt
CharacterBuilderPage
```

Fluxo sugerido:

```txt
1. Dados básicos
2. Raça
3. Classe
4. Antecedente
5. Atributos
6. Revisão
```

Funcionalidades mínimas:

- Select de raça.
- Select de classe.
- Select de antecedente.
- Salvar personagem.
- Editar personagem existente.
- Validar campos obrigatórios.
- Respeitar dono do personagem e campanha.

---

# Módulo 4 — Combat tracker estilo Improved Initiative

## Objetivo

Criar um combat tracker interno para o mestre controlar encontros, iniciativa, turnos, rodadas, HP e condições.

Não precisa copiar nenhuma ferramenta externa.  
Criar uma versão própria adaptada ao projeto.

## Entidade: Encounter

```txt
Encounter
- Id
- CampaignId
- Name
- Status
- RoundNumber
- CurrentTurnIndex
- CreatedByUserId
- CreatedAt
- UpdatedAt
```

Status:

```txt
Draft
Active
Finished
```

## Entidade: EncounterParticipant

```txt
EncounterParticipant
- Id
- EncounterId
- Type
- CharacterId
- NpcId
- Name
- ArmorClass
- MaxHp
- CurrentHp
- TemporaryHp
- Initiative
- DexterityModifier
- IsVisibleToPlayers
- NotesMasterOnly
- ConditionsJson
- SortOrder
```

Type:

```txt
PlayerCharacter
NPC
Monster
```

## Regras

- Apenas MASTER pode criar, editar, iniciar ou finalizar encontros.
- Apenas MASTER pode alterar HP de qualquer participante.
- Apenas MASTER pode avançar turno e rodada.
- PLAYER só vê encontros ativos se o mestre permitir.
- PLAYER só vê participantes com `IsVisibleToPlayers = true`.
- PLAYER nunca vê `NotesMasterOnly`.
- Participantes devem ser ordenados por iniciativa.
- Empates podem usar `DexterityModifier` como critério secundário.

## Endpoints esperados

```txt
GET    /api/campaigns/{campaignId}/encounters
GET    /api/campaigns/{campaignId}/encounters/{encounterId}
POST   /api/campaigns/{campaignId}/encounters
PUT    /api/campaigns/{campaignId}/encounters/{encounterId}
DELETE /api/campaigns/{campaignId}/encounters/{encounterId}
```

Participantes:

```txt
POST   /api/campaigns/{campaignId}/encounters/{encounterId}/participants
PUT    /api/campaigns/{campaignId}/encounters/{encounterId}/participants/{participantId}
DELETE /api/campaigns/{campaignId}/encounters/{encounterId}/participants/{participantId}
```

Ações:

```txt
POST /api/campaigns/{campaignId}/encounters/{encounterId}/start
POST /api/campaigns/{campaignId}/encounters/{encounterId}/finish
POST /api/campaigns/{campaignId}/encounters/{encounterId}/next-turn
POST /api/campaigns/{campaignId}/encounters/{encounterId}/previous-turn
POST /api/campaigns/{campaignId}/encounters/{encounterId}/apply-damage
POST /api/campaigns/{campaignId}/encounters/{encounterId}/apply-healing
POST /api/campaigns/{campaignId}/encounters/{encounterId}/roll-initiative
```

## Frontend

Criar tela:

```txt
EncounterTrackerPage
```

Funcionalidades:

- Criar encontro.
- Adicionar personagens da campanha.
- Adicionar NPCs.
- Adicionar monstro manual.
- Rolar iniciativa.
- Editar iniciativa manualmente.
- Ordenar participantes.
- Iniciar combate.
- Exibir rodada atual.
- Exibir turno atual.
- Avançar turno.
- Voltar turno.
- Aplicar dano.
- Aplicar cura.
- Editar HP.
- Adicionar/remover condições.
- Mostrar/ocultar participante para jogadores.
- Campo de notas secretas do mestre.

UI mínima esperada:

```txt
[Nome do encontro] [Status]
Rodada: X

Participantes ordenados:
- Indicador do turno atual
- Nome
- Tipo
- CA
- HP atual / HP máximo
- Iniciativa
- Condições
- Botões: dano, cura, editar, visível/invisível
```

---

# Segurança

## Backend

Obrigatório:

- Validar usuário autenticado.
- Validar se usuário pertence à campanha.
- Validar role dentro da campanha.
- Aplicar filtro por visibilidade.
- Nunca retornar campo secreto para PLAYER.
- Não aceitar `OwnerUserId` vindo diretamente do frontend quando puder ser inferido do token.
- Não aceitar `CampaignId` sem validar acesso.
- Não expor stack trace em produção.

## Frontend

Obrigatório:

- Esconder menus não permitidos para PLAYER.
- Tratar erro 401/403 corretamente.
- Não guardar secrets em código.
- Não confiar em regra visual como segurança.

---

# Banco de dados

O agente deve:

1. Criar migrations para novas entidades.
2. Rodar migration localmente se o ambiente permitir.
3. Garantir chaves estrangeiras.
4. Garantir índices úteis.

Índices sugeridos:

```txt
CampaignNote:
- CampaignId
- OwnerUserId
- Visibility
- Title

NPC:
- CampaignId
- Name
- Location
- Faction
- Visibility

Encounter:
- CampaignId
- Status

EncounterParticipant:
- EncounterId
- Initiative
- SortOrder
```

---

# Testes mínimos

Criar ou ajustar testes quando o projeto tiver estrutura de testes.

## Casos obrigatórios

Permissões:

```txt
PLAYER não acessa nota MasterOnly.
PLAYER não acessa NPC MasterOnly.
PLAYER não recebe campo Secrets.
MASTER acessa todas as notas da campanha.
MASTER acessa todos os NPCs da campanha.
```

Notas:

```txt
MASTER cria nota com MasterOnly por padrão.
PLAYER cria nota com Private por padrão.
Busca por tag funciona.
Busca por título funciona.
```

NPCs:

```txt
MASTER cria NPC.
PLAYER não cria NPC.
Busca por nome funciona.
Filtro por facção funciona.
```

Personagem:

```txt
Lista raças/classes/antecedentes.
Cria personagem com RaceId/ClassId/BackgroundId.
Valida campos obrigatórios.
```

Encontros:

```txt
MASTER cria encontro.
MASTER adiciona participante.
MASTER inicia encontro.
Participantes são ordenados por iniciativa.
Next turn avança corretamente.
Ao passar do último participante, incrementa rodada.
PLAYER só vê participantes visíveis.
```

---

# Deploy

O projeto já está publicado e funcionando.

O agente deve apenas ajustar o deploy existente para suportar as novas features.

Não trocar provedor de deploy.
Não recriar ambiente.
Não alterar banco de produção sem migration.
Não mudar domínio, pipeline ou configuração atual sem necessidade.

Ao final da implementação:

1. Rodar build do frontend.
2. Rodar build do backend.
3. Rodar testes, se existirem.
4. Criar migrations necessárias.
5. Atualizar `.env.example` se novas variáveis forem necessárias.
6. Conferir se a API continua apontando para o banco correto.
7. Conferir se o CORS continua permitindo o domínio atual da aplicação.
8. Publicar usando o mesmo fluxo de deploy já usado no projeto.
9. Validar em produção:
   - login
   - campanhas
   - permissões MASTER/PLAYER
   - notas
   - NPCs
   - criação de personagem
   - combat tracker

Se houver risco de quebrar produção, parar e documentar o risco antes de aplicar.

## Premissas

Projeto esperado:

```txt
Frontend: Vercel
Banco: Neon PostgreSQL
Backend/API: Azure App Service, Render, Railway, Koyeb, Fly.io ou outro host já usado no projeto
```

O agente deve detectar a estrutura real antes de aplicar alterações.

---

## Variáveis de ambiente

Criar ou atualizar `.env.example`.

Nunca commitar `.env` real.

Variáveis comuns:

```txt
DATABASE_URL=
JWT_SECRET=
JWT_ISSUER=
JWT_AUDIENCE=
CORS_ALLOWED_ORIGINS=
FRONTEND_URL=
API_BASE_URL=
```

Variáveis futuras:

```txt
NOTION_API_KEY=
NOTION_DATABASE_ID=
OBSIDIAN_API_URL=
OBSIDIAN_API_KEY=
```

Frontend Vite, se existir:

```txt
VITE_API_BASE_URL=
```

---

## Checklist antes do deploy

O agente deve executar, quando disponível:

```bash
npm install
npm run lint
npm run build
npm test
```

Para backend .NET, quando aplicável:

```bash
dotnet restore
dotnet build
dotnet test
dotnet ef migrations add AddRpgCampaignFeatures
dotnet ef database update
```

Para outros backends, usar os comandos equivalentes do projeto.

---

## Deploy frontend na Vercel

Verificar:

1. `VITE_API_BASE_URL` aponta para a URL pública da API.
2. Build command está correto.
3. Output directory está correto.
4. Rotas SPA estão funcionando.
5. CORS da API permite o domínio da Vercel.

Exemplo:

```txt
VITE_API_BASE_URL=https://sua-api.com
```

---

## Deploy backend

O agente deve preparar a API para produção:

1. Configurar connection string do Neon.
2. Configurar CORS com domínio da Vercel.
3. Configurar JWT em produção.
4. Rodar migrations no banco de produção.
5. Publicar API no serviço usado pelo projeto.
6. Testar endpoint `/health`, se existir.
7. Testar login.
8. Testar acesso autenticado.
9. Testar fluxo MASTER/PLAYER.

Se não existir `/health`, criar um endpoint simples:

```txt
GET /health
Resposta: 200 OK
```

---

## Deploy banco Neon

O agente deve:

1. Verificar se `DATABASE_URL` está configurada.
2. Rodar migrations.
3. Confirmar criação das tabelas novas.
4. Não apagar dados existentes.
5. Não recriar banco do zero sem autorização.

---

## CORS

Configuração obrigatória:

```txt
Permitir somente:
- domínio local de desenvolvimento
- domínio da Vercel
```

Evitar em produção:

```txt
AllowAnyOrigin
```

Exceto se for temporário e documentado.

---

# Fluxo de trabalho recomendado para o agente

Implementar em fases.

## Fase 1

- Validar permissões.
- Criar enum Visibility.
- Criar helpers/services de autorização por campanha.
- Garantir que MASTER/PLAYER funcionem corretamente.

## Fase 2

- Implementar CampaignNote.
- Criar endpoints.
- Criar tela de anotações.
- Testar acesso MASTER/PLAYER.

## Fase 3

- Implementar NPC.
- Criar endpoints.
- Criar tela de NPCs.
- Testar bloqueio para PLAYER.

## Fase 4

- Implementar Race, CharacterClass e Background.
- Atualizar Character.
- Criar/ajustar tela de criação de personagem.

## Fase 5

- Implementar Encounter.
- Implementar EncounterParticipant.
- Criar tela de combat tracker.
- Testar turnos, rodadas, HP e visibilidade.

## Fase 6

- Preparar stubs de Notion/Obsidian.
- Criar `.env.example`.
- Documentar integração futura.

## Fase 7

- Rodar build/test/lint.
- Rodar migrations.
- Preparar deploy.
- Fazer deploy frontend/backend.
- Validar produção.

---

# Critérios de aceite

A implementação será considerada concluída quando:

```txt
- MASTER consegue criar e ver notas privadas.
- PLAYER não consegue ver notas privadas do MASTER.
- PLAYER consegue criar as próprias notas.
- MASTER consegue ver notas dos jogadores.
- MASTER consegue criar, editar, buscar e excluir NPCs.
- PLAYER não consegue acessar NPC MasterOnly.
- PLAYER não recebe campo Secrets de NPC.
- Tela de personagem possui seletores de raça, classe e antecedente.
- MASTER consegue criar um encontro.
- MASTER consegue adicionar participantes ao encontro.
- MASTER consegue iniciar combate.
- MASTER consegue avançar turnos e rodadas.
- MASTER consegue aplicar dano/cura.
- PLAYER vê apenas participantes publicados no combate.
- Build do frontend passa.
- Build do backend passa.
- Migrations foram criadas.
- Deploy foi preparado/executado.
- `.env.example` foi atualizado.
- README ou documentação de deploy foi atualizada.
```

---

# Instrução final ao agente

Trabalhe de forma incremental.

Não tente reescrever o projeto inteiro.

Ao encontrar divergência entre este documento e a arquitetura real do projeto, priorize a arquitetura real e documente a decisão.

Ao finalizar, entregue um resumo com:

```txt
- Arquivos alterados
- Entidades criadas
- Endpoints criados
- Migrations criadas
- Telas criadas
- Variáveis de ambiente necessárias
- Comandos de build/test executados
- Resultado do deploy
- Pendências conhecidas
```
