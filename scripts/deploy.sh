#!/usr/bin/env bash
# =============================================================================
#  Deploy de um ambiente (staging | production) com Docker Compose
#  Uso: ./scripts/deploy.sh <staging|production>
#
#  - Lê as variáveis de deploy/<ambiente>.env (cria a partir do .example se faltar)
#  - Se API_IMAGE for de um registry (ex.: ghcr.io/...), faz pull da imagem
#    publicada pelo pipeline; senão, faz build local a partir do Dockerfile
#  - Sobe API + SQL Server e espera o health check responder
# =============================================================================
set -euo pipefail

ENVIRONMENT="${1:-}"
case "$ENVIRONMENT" in
  staging)    OVERRIDE="docker-compose.staging.yml"; EXPECTED="Staging" ;;
  production) OVERRIDE="docker-compose.prod.yml";    EXPECTED="Production" ;;
  *) echo "Uso: $0 <staging|production>"; exit 2 ;;
esac

cd "$(dirname "$0")/.."

ENV_FILE="deploy/${ENVIRONMENT}.env"
if [[ ! -f "$ENV_FILE" ]]; then
  echo "==> $ENV_FILE não existe; usando deploy/${ENVIRONMENT}.env.example"
  cp "deploy/${ENVIRONMENT}.env.example" "$ENV_FILE"
fi

set -a; source "$ENV_FILE"; set +a

PROJECT="ecotrack-${ENVIRONMENT}"
COMPOSE=(docker compose -p "$PROJECT" --env-file "$ENV_FILE" -f docker-compose.yml -f "$OVERRIDE")

echo "==> Deploy em ${ENVIRONMENT^^}"
echo "    projeto : $PROJECT"
echo "    imagem  : $API_IMAGE"
echo "    versão  : ${APP_VERSION:-local}"
echo "    porta   : ${API_PORT}"

if [[ "$API_IMAGE" == *"/"* ]]; then
  echo "==> Baixando imagens do registry"
  "${COMPOSE[@]}" pull
  "${COMPOSE[@]}" up -d --no-build --remove-orphans
else
  echo "==> Build local da imagem"
  "${COMPOSE[@]}" up -d --build --remove-orphans
fi

echo "==> Aguardando a API ficar saudável (http://localhost:${API_PORT}/health)"
for i in $(seq 1 60); do
  if curl -fsS "http://localhost:${API_PORT}/health" > /dev/null 2>&1; then
    echo "==> API respondendo após $((i * 5))s"
    curl -s "http://localhost:${API_PORT}/health"; echo
    "${COMPOSE[@]}" ps
    exit 0
  fi
  sleep 5
done

echo "==> ERRO: a API não respondeu em 300s. Logs:"
"${COMPOSE[@]}" ps
"${COMPOSE[@]}" logs --tail=200
exit 1
