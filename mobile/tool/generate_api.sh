#!/usr/bin/env bash
# Generates the Dart (dio) API client from the OpenAPI contract into the local
# path package packages/tan_erp_api. Never edit generated files by hand.
#
# Requires a JDK. Android Studio's bundled JBR works; it is picked up
# automatically on macOS when JAVA_HOME is unset, otherwise export JAVA_HOME.
# The generator version is pinned in mobile/openapitools.json; npx downloads
# the official jar from Maven Central on first run.
set -euo pipefail

MOBILE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SPEC="$MOBILE_DIR/../contracts/openapi/tan-erp.v1.json"
OUT="$MOBILE_DIR/packages/tan_erp_api"
STUDIO_JBR="/Applications/Android Studio.app/Contents/jbr/Contents/Home"
PROPS="pubName=tan_erp_api,pubLibrary=tan_erp_api,serializationLibrary=built_value"

if [ -z "${JAVA_HOME:-}" ] && [ -x "$STUDIO_JBR/bin/java" ]; then
  export JAVA_HOME="$STUDIO_JBR"
fi
if [ -z "${JAVA_HOME:-}" ] || [ ! -x "$JAVA_HOME/bin/java" ]; then
  echo "JAVA_HOME must point to a JDK (e.g. Android Studio's jbr)." >&2
  exit 1
fi
export PATH="$JAVA_HOME/bin:$PATH"

cd "$MOBILE_DIR"
rm -rf "$OUT"
npx --yes @openapitools/openapi-generator-cli --openapitools "$MOBILE_DIR/openapitools.json" \
  generate -g dart-dio -i "$SPEC" -o "$OUT" --additional-properties="$PROPS" \
  --global-property apiDocs=false,modelDocs=false,apiTests=false,modelTests=false

# built_value serializers are generated code too.
cd "$OUT"
dart pub get
dart run build_runner build
echo "Generated API client in packages/tan_erp_api"
