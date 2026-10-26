# TAN Field (mobile)

Flutter field companion app for tan-erp. Architecture: `docs/02-architecture/mobile-architecture.md`.

## Run

Per-flavor config lives in `config/<flavor>.json` (gitignored; copy from `<flavor>.example.json`).

```bash
cp config/dev.example.json config/dev.json   # set API_BASE_URL to your Mac's LAN IP
flutter run --flavor dev -t lib/main_dev.dart --dart-define-from-file=config/dev.json
```

Without `FIREBASE_*` values the app starts and shows a "not configured" screen.
Firebase options come from the config file; no `google-services.json` / `GoogleService-Info.plist` is committed.

## Flavors

| Flavor | Bundle ID | Entry |
|---|---|---|
| dev | `com.tanerp.field.dev` | `lib/main_dev.dart` |
| staging | `com.tanerp.field.staging` | `lib/main_staging.dart` |
| prod | `com.tanerp.field` | `lib/main_prod.dart` |

- **Android:** full `productFlavors` (`android/app/build.gradle.kts`). Cleartext HTTP is enabled only for dev through `android/app/src/dev/AndroidManifest.xml`.
- **iOS (partial):** a single `Runner` scheme with bundle ID `com.tanerp.field`. Flavor values (API URL, Firebase) are applied with `--dart-define-from-file=config/<flavor>.json`; a different bundle ID per flavor needs Xcode schemes/xcconfigs that are NOT set up yet. ATS: `Info.plist` allows only `NSAllowsLocalNetworking` (HTTP to LAN IPs / `.local`), which applies to every iOS build until per-flavor schemes exist.

## Checks

```bash
flutter analyze
dart run custom_lint                 # bans Color(0x..) and BorderRadius.circular outside lib/core/theme
dart run tool/check_l10n_parity.dart # app_th.arb vs app_en.arb keys
```

## Generated code

- `lib/core/theme/tokens.g.dart`: from `npm run tokens:build` at the repo root. Never edit by hand.
- `lib/generated/api/`: `tool/generate_api.sh` (openapi-generator dart-dio). Until it has run, API calls are generic (`ApiClient.getJson`) and the two mappers marked `PENDING CODEGEN` (`currentUserParserProvider`, `versionPolicyParserProvider`) must be replaced with mappers over the generated types.
- `lib/core/l10n/gen/`: `flutter gen-l10n` (also run by `flutter pub get`/build).
