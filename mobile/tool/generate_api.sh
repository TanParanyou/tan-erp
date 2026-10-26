#!/usr/bin/env bash
# Generates the Dart (dio) API client from the OpenAPI contract into
# lib/generated/api. Never edit generated files by hand.
#
# Requires ONE of: `openapi-generator` (brew install openapi-generator, needs a
# JRE), or Docker. Run from anywhere: ./tool/generate_api.sh
set -euo pipefail

MOBILE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SPEC="$MOBILE_DIR/../contracts/openapi/tan-erp.v1.json"
OUT="$MOBILE_DIR/lib/generated/api"
GENERATOR_VERSION="7.14.0"
PROPS="pubName=tan_erp_api,pubLibrary=tan_erp_api,useEnumExtension=true,nullSafe=true"

rm -rf "$OUT"
mkdir -p "$OUT"

if command -v openapi-generator >/dev/null 2>&1; then
  openapi-generator generate -g dart-dio -i "$SPEC" -o "$OUT" \
    --additional-properties="$PROPS"
elif command -v docker >/dev/null 2>&1; then
  docker run --rm -v "$MOBILE_DIR/..:/work" \
    "openapitools/openapi-generator-cli:v$GENERATOR_VERSION" generate \
    -g dart-dio -i /work/contracts/openapi/tan-erp.v1.json \
    -o /work/mobile/lib/generated/api --additional-properties="$PROPS"
else
  echo "Install openapi-generator (brew install openapi-generator) or Docker." >&2
  exit 1
fi

# NOTE (not yet run in CI/dev): dart-dio emits a standalone package with its own
# pubspec.yaml (built_value + build_runner). After the first successful run, add
# it as `tan_erp_api: {path: lib/generated/api}` in pubspec.yaml and run
# `dart run build_runner build` inside lib/generated/api.
echo "Generated API client in lib/generated/api"
