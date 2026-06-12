#!/usr/bin/env bash
set -euo pipefail

# Quick local test runner:
# 1) bring up compose, 2) wait for postgres, 3) run dotnet tests, 4) tear down

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

echo "Starting docker-compose..."
docker compose up -d --build

echo "Waiting for Postgres to be ready..."
until docker compose exec -T db pg_isready -U shelfwise -d shelfwise_dev >/dev/null 2>&1; do
  printf '.';
  sleep 1
done
echo " Postgres ready"

echo "Running dotnet tests"
dotnet test ShelfWise.sln

echo "Tests finished; tearing down..."
docker compose down

echo "Done"
