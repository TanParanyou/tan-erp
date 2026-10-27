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

## Mock login (dev only)

Sign in against a local backend without Firebase. Set `"MOCK_AUTH": true` in `config/dev.json` (default `false`; ignored by staging/prod, which never construct the mock repository). The sign-in page then shows a uid field (default `foundation-user-test-only`) and the app sends `Authorization: Bearer dev-uid:<uid>`.

Backend side: run `TanErp.Api` with `ASPNETCORE_ENVIRONMENT=Development`; `appsettings.Development.json` sets `Authentication:DevelopmentTokens:Enabled=true`. The backend throws at startup if that flag is true in any other environment. Users are matched by `FirebaseUid`, so the uid must exist in the database. `TestOnlyDataSeeder` seeds `foundation-user-test-only` (and `foundation-user-b-test-only`) but only runs when the environment is `Test` and `SeedTestData=true`; seed the local DB once with `ASPNETCORE_ENVIRONMENT=Test SeedTestData=true` (same `ConnectionStrings__Database`), then restart in `Development` for mock login.

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
dart run tool/check_plane_measurement.dart # cases live in shared-fixtures/plane-measurement.json (run from mobile/)
```

## Generated code

- `lib/core/theme/tokens.g.dart`: from `npm run tokens:build` at the repo root. Never edit by hand.
- `packages/tan_erp_api/`: `tool/generate_api.sh` (openapi-generator 7.26.0 `dart-dio` + built_value, pinned in `openapitools.json`; needs a JDK, Android Studio's JBR is auto-detected). Path dependency; excluded from analysis. `ApiClient` (`lib/core/network/api_client.dart`) runs the generated client on the app's own dio.
- `lib/core/l10n/gen/`: `flutter gen-l10n` (also run by `flutter pub get`/build).
