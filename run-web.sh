#!/usr/bin/env bash
set -euo pipefail

# Run the IssuesReportingWeb app from the repository root.
# Usage:
#   ./run-web.sh                 # starts on http://localhost:5000 and https://localhost:5001
#   ./run-web.sh --http-only     # starts on http://localhost:5000 only
#   ./run-web.sh --trust-https   # trusts local dev HTTPS cert, then starts both http/https

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$ROOT_DIR/IssuesReportingWeb/IssuesReportingWeb.csproj"

URLS_DEFAULT="http://localhost:5000;https://localhost:5001"
URLS_HTTP_ONLY="http://localhost:5000"
URLS="$URLS_DEFAULT"

if [[ ${1:-} == "--http-only" ]]; then
  URLS="$URLS_HTTP_ONLY"
fi

if [[ ${1:-} == "--trust-https" ]]; then
  echo "Trusting local HTTPS development certificate..."
  dotnet dev-certs https --trust || true
fi

echo "Launching IssuesReportingWeb on: $URLS"
exec env ASPNETCORE_URLS="$URLS" dotnet run --project "$PROJECT"