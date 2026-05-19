# API

Base local padrão:

```text
http://localhost:5000/api
```

Autenticação:

```http
Authorization: Bearer <token>
```

## Auth

- `POST /auth/register`
- `POST /auth/login`
- `GET /auth/me`

Cadastro:

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Mestre","email":"mestre@example.com","password":"senha1234","profile":"GameMaster"}'
```

Login:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"mestre@example.com","password":"senha1234"}'
```

## Health

- `GET /health`

## Campaigns

- `GET /campaigns`
- `POST /campaigns`
- `GET /campaigns/{id}`
- `PUT /campaigns/{id}`
- `DELETE /campaigns/{id}`
- `POST /campaigns/join`
- `POST /campaigns/{id}/invite/regenerate`
- `GET /campaigns/{id}/members`
- `GET /campaigns/{id}/characters`
- `GET /campaigns/{id}/master-dashboard`

Criar campanha:

```json
{
  "name": "Mesa de Sexta",
  "description": "Campanha privada",
  "system": "Sistema fictício",
  "coverImageUrl": null
}
```

Entrar por convite:

```json
{
  "inviteCode": "ABC12345"
}
```

## Characters

- `GET /characters`
- `POST /characters`
- `GET /characters/{id}`
- `PUT /characters/{id}`
- `DELETE /characters/{id}`
- `GET|PUT /characters/{id}/attributes`
- `GET|PUT /characters/{id}/saving-throws`
- `GET|PUT /characters/{id}/skills`
- `GET|PUT /characters/{id}/combat`
- `GET|POST /characters/{id}/attacks`
- `PUT|DELETE /characters/{id}/attacks/{attackId}`
- `GET|PUT /characters/{id}/conditions`
- `GET|POST /characters/{id}/notes`
- `GET|PUT|DELETE /characters/{id}/notes/{noteId}`
- `GET|POST /characters/{id}/inventory`
- `PUT|DELETE /characters/{id}/inventory/{itemId}`
- `GET|PUT /characters/{id}/currency`
- `GET|POST /characters/{id}/spells`
- `PUT|DELETE /characters/{id}/spells/{characterSpellId}`
- `GET|PUT /characters/{id}/spell-slots`
- `GET|POST /characters/{id}/features`
- `PUT|DELETE /characters/{id}/features/{characterFeatureId}`
- `POST /characters/{id}/short-rest`
- `POST /characters/{id}/long-rest`
- `GET|POST /characters/{id}/assets`
- `DELETE /characters/{id}/assets/{assetId}`
- `PUT /characters/{id}/avatar`
- `PUT /characters/{id}/token`

Descanso longo:

```json
{
  "restoreHitPoints": true,
  "restoreHitDice": true
}
```

Upload:

```bash
curl -X PUT http://localhost:5000/api/characters/<id>/avatar \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@avatar.png"
```

## Spells

- `GET /spells`
- `POST /spells`
- `GET /spells/{id}`
- `PUT /spells/{id}`
- `DELETE /spells/{id}`
- `POST /spells/import/open5e`

Filtros:

- `name`
- `level`
- `school`
- `class`
- `isConcentration`
- `isRitual`
- `source`
- `isHomebrew`
- `visibility`
- `page`
- `pageSize`

Criar magia homebrew:

```json
{
  "name": "Luz Inventada",
  "englishName": "Invented Light",
  "level": 1,
  "school": "Evocação",
  "castingTime": "1 ação",
  "range": "18m",
  "components": "V,S",
  "material": "",
  "duration": "1 minuto",
  "isConcentration": false,
  "isRitual": false,
  "description": "Descrição criada pelo usuário.",
  "higherLevelDescription": "",
  "availableClasses": "Classe fictícia",
  "source": "Homebrew",
  "isHomebrew": true,
  "visibility": "Private",
  "campaignId": null
}
```

## Features

- `GET /features`
- `POST /features`
- `GET /features/{id}`
- `PUT /features/{id}`
- `DELETE /features/{id}`

Tipos:

- `Feat`
- `Class`
- `Subclass`
- `Species`
- `Background`
- `Homebrew`

Filtros:

- `name`
- `type`
- `source`
- `isHomebrew`
- `visibility`
- `page`
- `pageSize`

## Dice

- `POST /dice/roll`

Payload:

```json
{
  "expression": "1d20+5",
  "advantage": false,
  "disadvantage": false,
  "label": "Teste"
}
```

Dados suportados: `d4`, `d6`, `d8`, `d10`, `d12`, `d20`, `d100`.

## Códigos Comuns

- `200`: sucesso.
- `201`: criado.
- `204`: removido/sem conteúdo.
- `400`: validação.
- `401`: token ausente ou inválido.
- `403`: sem permissão.
- `404`: recurso não encontrado.
- `409`: conflito, como duplicidade.
