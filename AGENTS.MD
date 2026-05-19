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