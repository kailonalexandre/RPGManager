# Importação de magias Open5e

## Fonte

O importador usa Open5e API V2:

```text
https://api.open5e.com/v2/spells/
```

Por padrão, somente documentos SRD são importados:

```text
srd-2014,srd-2024
```

Não use este importador para trazer material pago/protegido sem licença.

## Como executar

Suba o projeto:

```bash
docker compose up --build
```

Faça login como usuário `GameMaster`, copie o JWT e execute:

```bash
curl -X POST http://localhost:5000/api/spells/import/open5e \
  -H "Authorization: Bearer $TOKEN"
```

Script:

```bash
TOKEN="cole-o-jwt-aqui" scripts/import-spells-open5e.sh
```

## Duplicidade

Chave principal:

```text
ExternalSource = Open5e
ExternalId = key da API
```

Rodar importação várias vezes não duplica. Magias importadas da mesma fonte são atualizadas.

Magias manuais/homebrew não são sobrescritas. Se existir conflito por nome + nível com homebrew, o item importado é ignorado.

## Campos importados

- `Name`
- `EnglishName`
- `Level`
- `School`
- `CastingTime`
- `Range`
- `Components`
- `Material`
- `Duration`
- `IsConcentration`
- `IsRitual`
- `Description`
- `HigherLevelDescription`
- `AvailableClasses`
- `Source`
- `IsHomebrew = false`
- `Visibility = LocalPublic`
- `ExternalSource`
- `ExternalId`
- `Slug`
- `RulesVersion`
- `IsImported = true`
- `IsSrd`
- `Language = en`
- `TranslationMissing = true`
- `ImportedAt`

## Idioma

Open5e retorna conteúdo em inglês. O sistema salva `Name` e `EnglishName` com o nome original, marca `Language = en` e `TranslationMissing = true`.

Tradução pt-BR fica para etapa futura. Não há tradução automática.

## Configuração

```json
{
  "Open5e": {
    "SpellsUrl": "https://api.open5e.com/v2/spells/",
    "DocumentKeys": "srd-2014,srd-2024",
    "PageSize": 100,
    "MaxPages": 100,
    "TimeoutSeconds": 30
  }
}
```

Variáveis:

```bash
Open5e__DocumentKeys="srd-2014,srd-2024"
Open5e__PageSize=100
```

## Limpar/reimportar em desenvolvimento

```sql
DELETE FROM spells
WHERE "ExternalSource" = 'Open5e'
  AND "IsImported" = true;
```

Depois rode importação novamente.

## Filtros

Após importação, `/spells` usa o mesmo CRUD/filtros:

- busca por nome
- nível
- escola
- classe
- fonte
- concentração
- ritual
- homebrew
- visibilidade
