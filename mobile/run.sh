#!/usr/bin/env bash
set -e

# Reverse ports for Android emulator/device so localhost can reach host machine
ADB_BIN="adb"
if ! command -v adb >/dev/null 2>&1 && [ -f "$HOME/Library/Android/sdk/platform-tools/adb" ]; then
  ADB_BIN="$HOME/Library/Android/sdk/platform-tools/adb"
fi
if command -v "$ADB_BIN" >/dev/null 2>&1; then
  "$ADB_BIN" reverse tcp:5079 tcp:5079 2>/dev/null || true
  "$ADB_BIN" reverse tcp:5005 tcp:5005 2>/dev/null || true
fi

# Default to dev flavor
TARGET="lib/main_dev.dart"
CONFIG="config/dev.json"

if [ "$1" == "ios" ]; then
  echo "🚀 Running Mobile App on iOS..."
  flutter run -t "$TARGET" --dart-define-from-file="$CONFIG" "${@:2}"
elif [ "$1" == "staging" ]; then
  echo "🚀 Running Mobile App (Staging)..."
  flutter run --flavor staging -t lib/main_staging.dart --dart-define-from-file="config/staging.json" "${@:2}"
elif [ "$1" == "prod" ]; then
  echo "🚀 Running Mobile App (Prod)..."
  flutter run --flavor prod -t lib/main_prod.dart --dart-define-from-file="config/prod.json" "${@:2}"
else
  echo "🚀 Running Mobile App (Dev)..."
  flutter run --flavor dev -t "$TARGET" --dart-define-from-file="$CONFIG" "$@"
fi
