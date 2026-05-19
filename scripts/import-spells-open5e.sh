#!/usr/bin/env bash
set -euo pipefail

API_URL="${API_URL:-http://localhost:5000}"

if [[ -z "${TOKEN:-}" ]]; then
  echo "Defina TOKEN com JWT de usuário GameMaster."
  echo "Exemplo: TOKEN=... API_URL=http://localhost:5000 scripts/import-spells-open5e.sh"
  exit 1
fi

curl -fsS -X POST "$API_URL/api/spells/import/open5e" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
