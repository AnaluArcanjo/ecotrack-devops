#!/usr/bin/env bash
# =============================================================================
#  Smoke tests pós-deploy — valida se o ambiente está realmente funcionando
#  Uso: ./scripts/smoke-test.sh <BASE_URL> [AMBIENTE_ESPERADO]
#  Ex.: ./scripts/smoke-test.sh http://localhost:8081 Staging
#
#  Se a variável SMOKE_REPORT apontar para um arquivo, um relatório em Markdown
#  é gravado nele (o pipeline usa isso no resumo do GitHub Actions).
# =============================================================================
set -uo pipefail

BASE_URL="${1:-http://localhost:8080}"
EXPECTED_ENV="${2:-}"
REPORT="${SMOKE_REPORT:-/dev/null}"
FAILURES=0
RUN_ID="$(date +%s)$RANDOM"

{
  echo "| # | Verificação | Requisição | Esperado | Obtido | Resultado |"
  echo "|---|---|---|---|---|---|"
} >> "$REPORT"

N=0
check() {
  # check "<descrição>" "<requisição>" <esperado> <obtido>
  local desc="$1" req="$2" expected="$3" got="$4" result
  N=$((N + 1))
  if [[ "$got" == "$expected" ]]; then
    result="✅ OK"
  else
    result="❌ FALHOU"
    FAILURES=$((FAILURES + 1))
  fi
  printf '%-3s %-48s %-42s esperado=%-4s obtido=%-4s %s\n' "$N" "$desc" "$req" "$expected" "$got" "$result"
  echo "| $N | $desc | \`$req\` | $expected | $got | $result |" >> "$REPORT"
}

http_code() { curl -s -o /tmp/smoke_body.json -w '%{http_code}' "$@"; }

echo "==> Smoke tests em $BASE_URL (ambiente esperado: ${EXPECTED_ENV:-qualquer})"

# 1. Health check
code=$(http_code "$BASE_URL/health")
check "Health check da API" "GET /health" 200 "$code"
echo "    resposta: $(cat /tmp/smoke_body.json)"

# 2. Ambiente correto (Staging / Production)
if [[ -n "$EXPECTED_ENV" ]]; then
  env_got=$(sed -n 's/.*"environment":"\([^"]*\)".*/\1/p' /tmp/smoke_body.json)
  check "Ambiente reportado pela API" "GET /health -> environment" "$EXPECTED_ENV" "${env_got:-vazio}"
fi

# 3. Documentação OpenAPI
code=$(http_code "$BASE_URL/swagger/v1/swagger.json")
check "Swagger / OpenAPI disponível" "GET /swagger/v1/swagger.json" 200 "$code"

# 4. Banco + migrations + seed (endpoint público que lê do SQL Server)
code=$(http_code "$BASE_URL/api/tipos-residuo")
check "Leitura no banco (tipos de resíduo / seed)" "GET /api/tipos-residuo" 200 "$code"

# 5. Paginação de pontos de coleta
code=$(http_code "$BASE_URL/api/pontos-coleta?pageNumber=1&pageSize=5")
check "Listagem paginada de pontos de coleta" "GET /api/pontos-coleta" 200 "$code"

# 6. Cadastro de usuário (escrita no banco)
EMAIL="smoke.${RUN_ID}@ecotrack.com.br"
SENHA="Smoke@${RUN_ID: -6}a"
code=$(http_code -X POST "$BASE_URL/api/auth/cadastro" -H 'Content-Type: application/json' \
  -d "{\"nome\":\"Smoke Test\",\"email\":\"$EMAIL\",\"senha\":\"$SENHA\",\"perfil\":\"Cidadao\"}")
check "Cadastro de usuário (escrita no banco)" "POST /api/auth/cadastro" 201 "$code"

# 7. Login JWT
code=$(http_code -X POST "$BASE_URL/api/auth/login" -H 'Content-Type: application/json' \
  -d "{\"email\":\"$EMAIL\",\"senha\":\"$SENHA\"}")
check "Login e geração de token JWT" "POST /api/auth/login" 200 "$code"
TOKEN=$(sed -n 's/.*"token":"\([^"]*\)".*/\1/p' /tmp/smoke_body.json)

# 8. Endpoint protegido COM token
code=$(http_code "$BASE_URL/api/dashboard" -H "Authorization: Bearer $TOKEN")
check "Dashboard autenticado" "GET /api/dashboard (Bearer)" 200 "$code"

# 9. Endpoint protegido SEM token (segurança)
code=$(http_code "$BASE_URL/api/dashboard")
check "Dashboard sem token é bloqueado" "GET /api/dashboard (sem token)" 401 "$code"

echo "" >> "$REPORT"
if [[ $FAILURES -gt 0 ]]; then
  echo "**Resultado: $FAILURES verificação(ões) falharam.**" >> "$REPORT"
  echo "==> $FAILURES verificação(ões) FALHARAM"
  exit 1
fi
echo "**Resultado: todas as $N verificações passaram.**" >> "$REPORT"
echo "==> Todas as $N verificações passaram"
