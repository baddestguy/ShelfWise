#!/usr/bin/env bash
set -euo pipefail

# Run EF Core migrations for the repository project using the API as startup project.
# Intended for CI or deployment pipelines where dotnet-ef is available.

echo "Running EF Core database migrations..."

dotnet tool restore || true

# Ensure the ef tool is available (global/local)
if ! command -v dotnet-ef >/dev/null 2>&1; then
  echo "dotnet-ef not found, installing as local tool..."
  dotnet new tool-manifest --force
  dotnet tool install dotnet-ef --version 8.*
fi

# Run the migration update
dotnet ef database update --project src/ShelfWise.Repository --startup-project src/ShelfWise.Api

echo "Migrations applied."
