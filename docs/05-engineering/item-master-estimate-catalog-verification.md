# Item Master Estimate Catalog Verification

**Code status:** Complete — backend/frontend implementation, contracts, generated OpenAPI types, automated regression tests, private-image authorization, and priced Item → saved BOQ flow are covered. User acceptance remains for native tablet/200% zoom; migration rehearsal against a sanitized copy of legacy file/cost data remains a release-data check.
**Updated:** 2026-09-29
**Evidence captured:** 2026-09-23 12:38 UTC at `a6562d5` with uncommitted remediation changes.
**Migration regression evidence:** 2026-09-23 14:31 UTC at `a6562d5` with uncommitted migration fix.
**SDK:** `/Users/syaco/.dotnet/dotnet` reports `10.0.400` for `backend/global.json`.

This record supports the release decision for the Item Master → Estimate Catalog → BOQ flow. The API and data contracts remain authoritative for behavior; this file records only evidence actually exercised.

## Verified automated behavior

- `CostRecordTests` rejects publish from `Submitted`; only an approved record can publish.
- `CostResolverTests` checks Branch precedence, latest effective date, quantity tier, equal-precedence ambiguity and a future replacement's old-price window. Catalog and Estimate use `ICostResolver`; Estimate revalidates the cost ID, version and amount before snapshotting.
- `CostPublicationPolicyTests` checks intersecting effective/quantity ranges, adjacent quantity bands and replacement coverage. Publishing serializes writes per Item with a PostgreSQL transaction lock, rejects conflicting Published ranges, and closes an older record's effective window immediately before a future replacement starts.
- `CostRecordEndpointsTests` checks maker self-approval, stale/missing `If-Match`, create/publish idempotency replay, overlap rejection and immutability. Item and Cost writes produce AuditEvents in their transactions.
- `EstimateCatalogEndpointsTests` checks branch isolation, priced-item pagination, default priced eligibility, stable facets across category/brand/type/attribute/search filters, exact attribute key/value filtering, key-only filtering, and ambiguous costs excluded from priced pages/facets.
- `Migration_PreservesElevenLegacyUploadedFilesAsPending` starts at the pre-Item-Master migration with 11 synthetic legacy uploaded-file rows, upgrades to latest, and checks that the rows survive with `scan_status = pending`, no SHA-256/verified timestamp, and no database default. It passed in an isolated PostgreSQL Testcontainer; it does not use the developer database's actual upload rows.
- Frontend adapter tests reject missing snapshot metadata and relation/page fields. Modal tests cover pending/error/empty states, locale-aware text, search, selection and cursor navigation.

## Commands and results

| Command | Result |
| --- | --- |
| `/Users/syaco/.dotnet/dotnet build backend/TanErp.slnx --no-restore -m:1 /nodeReuse:false` | PASS, 0 warnings/errors |
| `cd backend && dotnet test TanErp.slnx --no-restore -m:1 /nodeReuse:false` | PASS after migration fix, Architecture 3 / Integration 245 / Unit 210. The first attempt hit an unrelated `SiteStoreTests` Testcontainers connection timeout; a clean retry passed. |
| `cd backend && dotnet test tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-restore -m:1 /nodeReuse:false --filter FullyQualifiedName~ItemSchemaConstraintTests` | PASS, 6 integration tests including legacy-file upgrade |
| `cd backend && dotnet test tests/TanErp.UnitTests/TanErp.UnitTests.csproj --no-restore -m:1 /nodeReuse:false` | PASS, 210 unit tests |
| `cd frontend && npm run lint` | PASS |
| `cd frontend && npm run typecheck` | PASS |
| `cd frontend && npm test -- --run` | PASS, 114 files / 496 tests; jsdom prints canvas-not-implemented diagnostics |
| `cd frontend && npm run build` | PASS; Next reports multiple lockfiles warning |
| `cd frontend && npm run generate:api` | PASS; generated types updated from the changed OpenAPI contract |
| `cd frontend && npm run test:e2e -- estimate-item-catalog.spec.ts --project=chromium` | PASS, 1 browser test against an isolated `tan_erp_catalog_review_20260923` database; Thai/English modal rendering, search input, 320px viewport, keyboard focus and close behavior |
| `git diff --check` | PASS |

`npm run verify` currently stops at `check:api` because that script compares generated types with committed `HEAD`, and the regenerated OpenAPI/types are uncommitted in this change set. Lint, typecheck, tests and build were run separately as recorded above.

## Historical release evidence (2026-09-23)

At this checkpoint, the Playwright spec was only a modal smoke test. Later 2026-09-26 checkpoints below supersede its journey and release-gate status.

The existing local `tan_erp` development database contains 11 legacy uploaded files and has not applied `AddItemMasterCatalogFoundation`. The original migration added `scan_status` with an empty-string default, which violated the new check. The migration now explicitly backfills legacy rows as `pending` before making the column required; a pre-migration Testcontainer reproduced the original `23514` failure and passed after the change. The original development database was not modified. An upgrade rehearsal against a copy of its actual 11 rows remains necessary before deployment.

The Evidence Gate was open at that checkpoint. Do not use this historical section as the current release status.

## Master Data Maintenance implementation checkpoint

**Updated:** 2026-09-25, working tree uncommitted.

This checkpoint covers Manual Cost Source lifecycle, Cost Record provenance/evidence validation, paged Cost Review Queue projection, idempotent Item/Category/Brand/Unit creation, and Item, Category/Brand/Unit, Cost Source, Cost Record and Review Queue screens. It does not close the priced-Item browser journey or migration rehearsal gates above.

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx --no-restore -m:1` | PASS, 0 warnings/errors |
| `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-restore --filter 'FullyQualifiedName~ItemEndpointsTests\|FullyQualifiedName~CostRecordEndpointsTests\|FullyQualifiedName~CostSourceEndpointsTests\|FullyQualifiedName~OpenApiContractTests' -m:1` | PASS, 28 tests; includes Item idempotency replay/key reuse, Cost Source lifecycle, Cost Record lifecycle, structured Cost Review Queue paging/filtering and OpenAPI contract |
| `cd frontend && npm run typecheck` | PASS |
| `cd frontend && npm run lint` | PASS |
| `cd frontend && npm test -- src/features/item-master/components/cost-record-maintenance.test.tsx src/features/item-master/components/taxonomy-maintenance.test.tsx` | PASS, 4 component tests including deferred evidence upload ordering |
| `cd frontend && npm run test` | FAIL, 498 passed / 1 failed; existing `CustomerEditor` duplicate-confirmation test expects a modal title absent from rendered test UI. |
| `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-restore --filter FullyQualifiedName~OpenApiContractTests -m:1` | PASS, 1 test; regenerated `contracts/openapi/tan-erp.v1.json` from the API contract. |
| `cd frontend && npm run generate:api` | PASS; regenerated types from the verified OpenAPI file. |
| `cd frontend && npm run build -- --webpack` | PASS; all routes including Item Master pages compiled. Default Turbopack cannot bind its local helper port in this environment. |
| `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` | FAIL: Unit 211/211 passed; Integration had 158 failures / 90 passes because Testcontainers could not start PostgreSQL containers (`DockerClient` readiness/exec errors). Focused integration suite above passed. |
| `git diff --check` and JSON parse of Thai/English messages and OpenAPI | PASS |

At this earlier checkpoint, the Item form only edited code, type, category, brand, base unit, localized name and description. The 2026-09-26 Item field maintenance checkpoint below supersedes this limitation and the test status recorded here.

## Item identity foundation design checkpoint

**Updated:** 2026-09-26. The [SKU/Barcode foundation plan](../superpowers/plans/2026-09-26-item-master-foundation-sku-barcode.md) and authoritative Field/API/Data/Governance documents define Item Code as the internal SKU for physical Items and Barcode/GTIN as a scoped child relation. This checkpoint records the initial Barcode foundation; the later SKU, Barcode and Unit Conversion verification below supersedes its remaining-work list.

### SKU and Barcode implementation checkpoint

The migration adds `item_master.item_barcodes` with organization-scoped normalized uniqueness, same-organization Item/Unit foreign keys and one active primary code per packaging level. GTIN length/check digit and internal-code rules live in Domain. Create supports idempotent replay; mutations use `If-Match`; exact scan lookup is organization and selected-branch scoped and returns structured Item/Unit/quantity data. Item Code remains the stable internal SKU; this change does not add a duplicate SKU column.

At this checkpoint, the edit page supported GTIN/internal codes and primary-code maintenance. The later verification below adds Item/shared unit conversions and browser coverage for GTIN creation, scan quantity snapshots and deactivation.

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx --no-restore -m:1` | PASS, 0 warnings/errors after Barcode branch-scope and audit updates |
| `dotnet test backend/TanErp.slnx --no-restore -m:1` | PASS, Architecture 3 / Integration 249 / Unit 215 |
| Focused Barcode + OpenAPI integration tests, `--no-build --no-restore` | PASS, 2 tests after final backend build |
| `cd frontend && npm run typecheck && npm run lint` | PASS |
| `cd frontend && npm run test` | PASS, 116 files / 500 tests |
| `cd frontend && npm run build -- --webpack` | PASS; all Item Master routes compiled. Default Turbopack build cannot bind its helper port in this environment, including when retried with escalation. |
| `cd frontend && npm run generate:api` | PASS; generated client types derive from verified OpenAPI |

These release gates were open at this checkpoint; the later 2026-09-26 sections below record completion of Item field maintenance, unit conversion, Barcode coverage and the priced Item → BOQ journey. Native zoom and migration against copied legacy data remain open.

## Item field maintenance and SKU foundation — latest checkpoint

**Updated:** 2026-09-26. This section supersedes the earlier statement above that the Item editor only supported its basic fields. The editor now round-trips Item Code/SKU, Product type, tax category, capabilities, attributes, active branch availability and create-time aliases. Existing aliases can be added/removed with confirmation. Item images use the verified File Session flow only after the user submits the image section; a verified file ID is retained for retry. Barcode maintenance remains a child section on the same `[id]` route. Updating the Item and selected branches is one backend transaction. The organization branch list is exposed through a scoped reusable `GET /api/v1/branches` endpoint.

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx --no-restore -m:1` | PASS, 0 warnings/errors |
| `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` | PASS, Architecture 3 / Integration 251 / Unit 215 (469 total) |
| Focused Item selected-branch/data round-trip and OpenAPI integration tests | PASS, 2 tests |
| `cd frontend && npm run generate:api && npm run typecheck && npm run lint` | PASS |
| Item form schema tests | PASS, 4 tests for Product type, 50-character SKU limit, selected-branch requirement, and string-only attributes |
| `cd frontend && npm run build -- --webpack` | PASS; Item Master list, edit, cost review and reference data routes compiled |
| `cd frontend && npm run test` | PASS, 117 files / 504 tests. One earlier run had a transient `CustomerEditor` duplicate warning assertion failure; the latest full rerun passed. |
| `git diff --check` and Thai/English/OpenAPI JSON parse | PASS |

This checkpoint's component tests and backend/frontend suites passed at that time. Its remaining gates were later closed by the SKU, Barcode, Unit Conversion and priced Item → BOQ work recorded below; native tablet/200% zoom, private-image/error/empty browser evidence and migration against copied legacy data remain open.

## SKU, Barcode and Unit Conversion — latest verification

**Updated:** 2026-09-26. Shared exact conversions and Item-specific packaging conversions now have separate versioned persistence, same-organization unit FKs, cycle/overlap validation, idempotent create APIs and localized Problem Details. Barcode creation accepts a non-base unit only when an effective item or shared conversion matches its base-unit quantity; scan returns the persisted quantity snapshot. Item detail UI maintains item-specific conversions and barcode identifiers; Reference Data maintains shared exact conversions. OpenAPI and generated TypeScript contracts are synchronized.

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx --no-restore -m:1` | PASS, 0 warnings/errors |
| `dotnet test backend/TanErp.slnx --no-build --no-restore -m:1` | PASS, Architecture 3 / Integration 254 / Unit 221 (478 total) |
| Focused PostgreSQL tests for Item/Shared conversion, cycle, barcode snapshots and database constraints | PASS, 3 tests; OpenAPI contract test PASS |
| `cd frontend && npm run generate:api` | PASS, generated types from current OpenAPI |
| `cd frontend && npm run typecheck && npm run lint && npm run test` | PASS, 118 files / 505 tests |
| `cd frontend && npm run build -- --webpack` | PASS; Item Master and Reference Data routes compiled |

Additional focused verification on 2026-09-26: the barcode API integration test passes after adding an assertion that a deactivated GTIN remains reserved and cannot be recreated (409). PostgreSQL schema tests pass for same-organization duplicate normalized values, cross-organization reuse, mismatched organization/Item/Unit foreign keys, non-positive barcode quantity, and duplicate active primary Barcodes for one packaging level. Item-specific migration rehearsal passes in Testcontainers: the prior migration has no `item_master.items` table, the upgrade creates Item and Barcode tables, and an existing uploaded file survives. The Item lifecycle API test also activates an Item without adding a Barcode. Focused Item Barcode/Unit Conversion unit tests pass (12). The full backend suite count above is the last full-suite run before the two newest unit cases and these focused integration assertions.

Playwright verification on the disposable `tan_erp_item_master_acceptance_20260926` database:

| Command | Result |
| --- | --- |
| `npx playwright test e2e/item-master-identity.spec.ts e2e/estimate-item-catalog.spec.ts --project=chromium` | PASS, 2 browser tests on disposable PostgreSQL and Firebase Auth Emulator. Covers Item/SKU and GTIN flow, 320px and keyboard focus; deferred private-image upload, authorized image retrieval and anonymous 401; image API error vs empty state; priced Item → BOQ save/reload, immutable cost snapshot, and cost-version conflict with input retention. |
| `npm run typecheck && npm run lint` | PASS after fixing response typing in the new Playwright helper |
| `dotnet test ... --filter FullyQualifiedName~Migration_PreservesElevenLegacyUploadedFilesAsPending` | PASS, 1 isolated PostgreSQL integration test |
| `dotnet test ... --filter 'FullyQualifiedName~ItemBarcodeTests\|FullyQualifiedName~ItemUnitConversionTests'` | PASS, 12 unit tests |

The `auth.spec.ts` preflight did not pass: it expects an obsolete Thai meta description and fails before login; that unrelated expectation was not changed. Current browser evidence now covers private image access, image error/empty state, priced Item → BOQ and cost-version conflict. Status remains **Partial** because native tablet/200% zoom and migration rehearsal against a copy of the local database's 11 legacy uploaded-file rows and cost data remain open. The existing `tan_erp` database was not used.

## Item Master remaining-gates checkpoint

**Updated:** 2026-09-26. Added focused Item editor and image-maintenance tests. Editor coverage includes existing-field round-trip with the current ETag, tab error marker/auto-switch, missing create permission, minimal loading/retryable error, retained input after a rejected save, and duplicate-submit lock. Image coverage proves selection does not upload immediately and that attach uses the verified file returned by File Service. Barcode component coverage verifies a value is required; malformed/check-digit and stale ETag behavior remain covered by API tests. Browser acceptance covers Thai/English, 320px and keyboard focus. The priced Item → BOQ journey creates a published cost with a separate same-organization reviewer, inserts the Item, saves/reloads the estimate, confirms a later published cost version does not rewrite the BOQ snapshot, and confirms a stale catalog cost produces a conflict while retaining form input.

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx -m:1` | PASS, 0 warnings/errors |
| `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --no-build -m:1` | PASS, 223 tests |
| `dotnet test backend/TanErp.slnx --no-build -m:1` | PASS: Architecture 3, Integration 254, Unit 223. |
| `cd frontend && npm run typecheck && npm run lint && npm run test` | PASS; 120 files / 512 tests |
| `cd frontend && npm run build -- --webpack` | PASS; Item Master routes compiled |
| `cd frontend && npx playwright test e2e/item-master-identity.spec.ts e2e/estimate-item-catalog.spec.ts --project=chromium` | PASS, 2 tests: Item/GTIN journey, deferred image upload, authorized private-file read and anonymous 401, image error/empty handling, priced Item → BOQ save/reload, immutable cost snapshot and cost-version conflict; uses isolated disposable database and Firebase emulator. |
| Playwright CLI at `600 CSS px` viewport | PASS: Item create form remained visible and `scrollWidth` equaled `clientWidth` (594px); this verifies 200%-equivalent reflow width from a 1200px CSS viewport, not native browser zoom. |
| `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --filter FullyQualifiedName~Migration_PreservesElevenLegacyUploadedFilesAsPending -m:1` | PASS: pre-Item-Master PostgreSQL Testcontainer migrated 11 synthetic legacy file rows; original paths/names were retained and all rows became pending without hashes, verified timestamps, or a column default. |
| `git diff --check` | PASS |

The development database was not accessed. Synthetic 11-file migration coverage passed; a rehearsal against a sanitized copy of legacy file/cost data remains a release-data check. Native tablet/200% zoom is left for user acceptance. The browser computer-use permission stayed pending, so the 600px viewport check is recorded as equivalent reflow coverage rather than native zoom evidence.

## Quick-create, identifier and navigation checkpoint

**Updated:** 2026-09-26. Item Create/Edit offers permission-gated quick-create actions for Category, Brand, Unit, and Tax Category; creating a Category scopes it to the selected Item Type and writes the created reference back to the current form. The item identifier panel distinguishes the single authoritative Item Code/SKU from multiple alternate internal identifiers and GTIN/barcodes. Sidebar navigation is grouped by Workspace, Customer/Sales, Master Data, and Settings, with nested CRM and Product groups and permission-filtered links.

| Command | Result |
| --- | --- |
| `cd frontend && npm test -- --run` | PASS, 125 files / 533 tests at this checkpoint; later tax-category checkpoint below supersedes this count |
| `cd frontend && npm run typecheck` | PASS |
| `cd frontend && npm run lint` | PASS |
| `cd frontend && npm run build -- --webpack` | PASS; Next reports the existing multiple-lockfiles warning |
| Parse `frontend/src/messages/th.json` and `en.json` | PASS |
| `git diff --check` | Pending at documentation update |

Item Type remains a system-controlled enum because it governs item capabilities. Tax Category is now organization-scoped reference data for classification only; it carries no tax rate and does not change VAT calculation behavior.

## Tax Category reference master checkpoint

**Updated:** 2026-09-26. Added organization-scoped `item_master.item_tax_categories` with unique normalized codes, localized names, status, sort order, audit fields and row-version concurrency. The typed API supports list/get/create/update under the existing Item read and taxonomy-management permissions. Item Create/Edit and Reference Data can create and select a Tax Category; existing codes with no active master entry remain visible as inactive legacy values during editing. Item Type stays system-controlled because it defines item capabilities.

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx --no-restore -m:1 /nodeReuse:false` | PASS, 0 warnings/errors |
| `dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-build --no-restore -m:1 /nodeReuse:false --filter FullyQualifiedName~TaxCategoryMaster_CreatesListsAndUpdatesWithinOrganization` | PASS, 1 API integration test including migration/startup |
| `cd frontend && npm run typecheck && npm run lint` | PASS |
| `cd frontend && npm test -- --run` | PASS, 125 files / 534 tests |
| Parse Thai/English messages and OpenAPI JSON; `git diff --check` | PASS |
| `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-build --no-restore -m:1 /nodeReuse:false --filter FullyQualifiedName~OpenApiContractTests.OpenApi_MeEndpoint_And_Schemas_Exist_And_Are_Committed` | PASS; synchronized the committed contract from the runtime OpenAPI document |
| `cd frontend && npm run build -- --webpack` | PASS; Item Master routes compiled, with the existing multiple-lockfiles warning |
| Full `dotnet test TanErp.slnx --no-build --no-restore -m:1 /nodeReuse:false` | INCOMPLETE: several integration hosts received `NpgsqlException: Received unknown response H for SSLRequest` from disposable PostgreSQL startup; stopped after repeated infrastructure failures. Focused Tax Category and OpenAPI integration tests passed. |
| `cd frontend && npm run generate:api` | BLOCKED: installed TypeScript is 7.0.2, which no longer exposes the compiler factory used by `openapi-typescript` 7.13.0 (requires TypeScript 5.x). The generated API types for this working tree are present and typecheck/build pass, but the generator command itself could not run. |

## Final Item Master code completion checkpoint

**Updated:** 2026-09-26. This checkpoint covers the final taxonomy and field round-trip hardening: Category now supports Product, parent selection, description and sort order; Brand description/sort order and Unit precision/rounding survive edits; Item Category options respect the selected Item Type; and taxonomy code limits are enforced by the API. The edit UI rejects missing row versions and reports save failures without labeling every failure as a concurrency conflict. API contract, generated TypeScript and th/en messages are synchronized.

| Command | Result |
| --- | --- |
| `/Users/syaco/.dotnet/dotnet build backend/TanErp.slnx --no-restore -m:1 /nodeReuse:false` | PASS, 0 warnings/errors |
| `/Users/syaco/.dotnet/dotnet test backend/TanErp.slnx --no-build --no-restore -m:1 /nodeReuse:false` | PASS, Architecture 3 / Integration 255 / Unit 223 (481 total) |
| Focused API/OpenAPI integration filter for taxonomy code length and OpenAPI contracts | PASS, 2 tests |
| `cd frontend && npm run generate:api && npm run typecheck && npm run lint && npm test -- --run` | PASS, 120 files / 515 tests |
| `cd frontend && npm run build -- --webpack` | PASS; Item Master routes compiled. Next reports multiple lockfiles warning. |
| `git diff --check`; parse Thai/English message JSON and OpenAPI JSON | PASS |

The two browser journeys recorded in the previous checkpoint passed before this last taxonomy-only change. A fresh rerun was attempted, but the isolated browser app could not start because another Next development server held the default build lock; Docker is unavailable in the current sandbox to recreate the disposable API/database. No existing server or development database was stopped or modified. The latest code is therefore covered by the full backend/frontend automated suites, while the earlier browser journey remains the latest successful E2E evidence. User-owned native tablet/200% zoom acceptance and the sanitized-copy legacy file/cost data rehearsal remain release checks; the developer database was not accessed.

## Item list standards completion checkpoint

**Updated:** 2026-09-26. The Item list now uses the shared debounced search, URL-synced item type/status filters with removable filter chips, first-use `EmptyState`, `DataTable`-preserved loading/error/filtered-empty states, shared `TableAction`, row selection and `BulkActionToolbar`, full-dataset and selected-row CSV export, and an explicitly sticky action column. Selection is cleared when the membership, query, sort, or page changes so selected-row export cannot silently omit rows from a prior page. The editor route now wraps Create with `items.create` and existing Item routes with `items.read` through the shared `PermissionGuard`.

Server-side sorting is part of the list contract rather than a page-only UI sort. `GET /api/v1/items` whitelists `code`, `itemType`, and `status`, validates `asc`/`desc`, uses Item ID as a stable tie-breaker, and caps `pageSize` at 100. The frontend sends URL-synced sorting through the typed API client; CSV export follows the active search, filters, and sort while fetching every page.

| Command | Result |
| --- | --- |
| `/Users/syaco/.dotnet/dotnet build backend/TanErp.slnx --no-restore -m:1 /nodeReuse:false` | PASS, 0 warnings/errors |
| `/Users/syaco/.dotnet/dotnet test backend/TanErp.slnx --no-build --no-restore -m:1 /nodeReuse:false` | PASS, Architecture 3 / Integration 256 / Unit 223 (482 total) |
| `UPDATE_OPENAPI=1 dotnet test backend/tests/TanErp.IntegrationTests/TanErp.IntegrationTests.csproj --no-restore --filter 'FullyQualifiedName~OpenApiContractTests\|FullyQualifiedName~ListItems_SortsWithStablePaging_AndRejectsUnknownSortKeys' -m:1 /nodeReuse:false` | PASS, 2 tests; verifies sort order, stable item paging size cap, invalid sort key/order Problem Details and OpenAPI output |
| `cd frontend && npm run generate:api && npm run typecheck && npm run lint` | PASS after adding typed sort query parameters |
| `cd frontend && npm test -- --run src/features/item-master/components/item-master-list.test.tsx 'src/app/[locale]/(erp)/item-master/[id]/page.test.tsx'` | PASS, 5 tests for first-use EmptyState, list actions/filters/selection/export, and route permissions |
| `cd frontend && npm test -- --run` | PASS, 122 test files / 520 tests |
| `cd frontend && npm run build -- --webpack` | PASS; Item Master routes compiled. Next reports the existing multiple-lockfiles warning. |
| `git diff --check` and Thai/English/OpenAPI JSON parse | PASS |

React Doctor could not be downloaded in this environment: the registry invocation did not complete and offline mode returned `ENOTCACHED`. Existing `npm run lint`, typecheck, full tests and production build passed. Native zoom remains for user acceptance; sanitized-copy legacy file/cost migration rehearsal remains a release-data check.

## TEST_ONLY material catalog fixture

Enable `SeedItemCatalogDemoData=true` together with `ASPNETCORE_ENVIRONMENT=Test` and `SeedTestData=true` to add a local test catalog. It contains 75 items: three canonical active materials (18 mm moisture-resistant plywood, white matte laminate, and 22 mm PVC edge band), plus 12 filter examples for each of the six item types (`material`, `labor`, `service`, `product`, `subcontract`, and `other`). Within those 72 filter examples, each type has four active, four draft, and four inactive rows, one category, and rows spread across three brands; the fixtures also add service tax, hour/job/each/day units, material specification variants, and internal barcodes. The three canonical materials and the 24 active filter examples have 27 published THB organization costs with independent approval reviews. All item codes, source references, and prices are fictional `TEST_ONLY` values for Item Catalog and estimate cost-selection tests; they are not procurement prices. The seed is additive and leaves existing values untouched.

The focused PostgreSQL integration test verifies all 75 rows, all six item types and three statuses, category/brand coverage, specification values, 27 published costs, 75 barcodes, Test-only environment lock, and repeat-seed idempotence. With the Item Catalog default page size of 25, the fixtures span three pages for pagination checks. Do not enable the seed against Production or a database containing operational data.

**Local seed execution (2026-09-28):** The configured local PostgreSQL cluster initially had no `tan_erp` database or `postgres` login role. After confirming both were absent, the local role/database were prepared to match `appsettings.Development.json`; Test-only migrations and the Item Catalog/Estimate fixtures were seeded into `tan_erp`. The expanded Item Catalog fixture now contains 75 rows across six types and three statuses, with 27 published fixture costs and 75 internal barcodes. The canonical examples include `TEST-MAT-PLY-MR18` (THB 1,250), `TEST-MAT-LAM-WHITE` (THB 580), and `TEST-MAT-EDGE-PVC22` (THB 18). These are fabricated test values. The local PostgreSQL cluster remains running for the app to use.

### Estimate Catalog Product response regression

`GET /api/v1/estimate-catalog/items` now maps Item Type `product` to Estimate `CostComponentType.material`; the Estimate cost-component contract has no separate product type. The previous default mapping threw for priced products, returning HTTP 500 when a product appeared in the result page. `SearchCatalog_ProductItem_MapsToMaterialCostComponent` reproduces that request path and verifies the product projection; it failed with HTTP 500 before the mapper fix and passed afterward. The full `EstimateCatalogEndpointsTests` class passed 12/12, the solution build passed with 0 warnings/errors, and the user's request parameters were replayed against the restarted local API: HTTP 200, 20 items, including four products mapped to `material`.

### Estimate Catalog attribute filters

The catalog response includes distinct attribute key/value facets with counts. Facet options and counts describe the authorized branch and cost scope before search, category, brand, item type, attribute, or pagination filters; they remain available even when the result list is empty. The BOQ item catalog keeps type/category/brand filters in a compact desktop sidebar, with specification names as visible horizontal toggle buttons above the results and values as radio choices. On mobile, the same filters share one scrollable panel with a View items action that preserves the selection. It submits both as server-side filters, and shows localized attribute names in each row. During filter requests, previous facets remain visible within the same membership, locale, branch, and cost scope while the results show loading. `SearchCatalog_FacetsRemainStableWhenFiltersOrSearchChange` verifies unchanged options and counts alongside filtered and empty results; `SearchCatalog_FiltersByAttributeAndReturnsAttributeFacets` verifies exact key/value and key-only matching with PostgreSQL JSONB.


## Item editor UX and maintenance drawers checkpoint

**Updated:** 2026-09-29, working tree uncommitted.

This checkpoint supports review of the Item editor and Category/Brand/Unit/Tax Category and Cost Source maintenance forms. The layout and interaction rules are recorded in [Item Master Responsive Wireframe](../01-business/item-master-responsive-wireframe.md#item-editor-และ-reference-data-forms).

The 2026-09-28 UI session verified three freely selectable editor tabs, preserved input registration, automatic navigation to the first invalid tab, the fixed FormActionBar, deferred image upload and retry, Drawer footer submission, unsaved-change confirmation, pending-save close protection, focus containment and restoration, and top-dialog Escape handling. Browser checks covered Thai/English rendering, the 320px Item editor without horizontal overflow, the 375px Cost Source Drawer, and retaining draft values after dismissing the discard confirmation.

| Command or check | Result |
| --- | --- |
| Previous UI session: `cd frontend && npm run test` | PASS, 135 files / 594 tests |
| Previous UI session: focused Item editor, taxonomy, cost-source, Drawer and Modal tests | PASS, 5 files / 34 tests after final UI adjustments |
| `cd frontend && npm run lint` | PASS on 2026-09-29 |
| `cd frontend && npm run build` | BLOCKED: Turbopack's CSS helper process cannot bind its local port (`Operation not permitted`), including an escalation retry |
| `cd frontend && npm run build -- --webpack` | PASS on 2026-09-29; Thai/English Item Master routes compiled |
| `cd backend && dotnet build TanErp.slnx --no-restore -m:1 /nodeReuse:false` | PASS, 0 warnings/errors |
| `cd backend && dotnet test tests/TanErp.UnitTests --no-build --no-restore` | PASS, 272 tests |
| Architecture suite in the full Backend runs | PASS, 3 tests |
| Isolated `ItemEndpointsTests.CreateItem_Valid_Returns201_AndCreatesAuditEvent` with Docker/PostgreSQL access | PASS, 1 integration test |
| Isolated `CostRecordEndpointsTests.UpdateDraft_StaleETag_ReturnsConflict` and `ItemEndpointsTests.UpdateItem_InactiveItem_Returns422`, with `xUnit.ParallelizeTestCollections=false` | PASS, 2 integration tests |
| `cd backend && dotnet test TanErp.slnx --no-build --no-restore -m:1 /nodeReuse:false -- xUnit.ParallelizeTestCollections=false` | PASS, Architecture 3 / Integration 284 / Unit 272; 559 tests, 0 failures/skips. Integration duration: 10 minutes 28 seconds |

The default full Backend run and a retry with `xUnit.MaxParallelThreads=2` were stopped after PostgreSQL connection timeouts during host startup/migration, before their affected business assertions ran. The isolated failing cases passed. This evidence does not establish a specific root cause or a passing default full-suite run.

The full non-parallel run passed using the same assertions and disposable PostgreSQL containers. [xUnit RunSettings](https://xunit.net/docs/config-runsettings) documents the CLI property used for this verification. Use the explicit command above to reproduce this result; it does not establish that the default parallel execution is stable. No application or Backend test code was changed during this verification follow-up.
