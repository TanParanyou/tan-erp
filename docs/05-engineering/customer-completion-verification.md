# Customer Completion Verification

**สถานะ:** Implementation complete; build gates passed, integration verification is environment-limited

## Scope

Customer profile editing now covers legal/tax data and credit/billing terms; contact and address subresources support CRUD and primary selection; customer deactivation/reactivation use optimistic concurrency and idempotency; site update/deactivation are available; quotation issuance checks billing readiness and stores an immutable customer billing snapshot. Credit read/manage permissions and contact PII permissions are enforced by the application layer.

Customer address editing reuses the shared `AddressAreaField`; Site edit uses the same area selector, and customer contact forms reuse `PhoneInput` and shared Customer identity/contact field components. Duplicate-candidate rows, customer display-name selection, query error messages, and subresource mutation context are shared within the Customer feature. Customer writes invalidate the Customer query root rather than the entire business cache. Contact/address reads use feature query hooks and centralized keys. Customer addresses and Sites share `AddressLocationValidator` for common location-field rules, while each feature retains its own label and address-line validation.

Customer activation still checks only draft state and an active primary contact. Credit terms are stored as master data; credit exposure enforcement and accounts receivable are outside this slice. Existing quotations remain readable and acceptable without retroactive customer billing snapshots.

## Verification gates

| Command | Result |
| --- | --- |
| `dotnet build backend/TanErp.slnx --no-restore --disable-build-servers` | Passed after the final test changes; 0 warnings, 0 errors |
| `dotnet test backend/TanErp.slnx --no-build -m:1` | With Docker available, architecture 3/3 and unit 284/284 passed. Integration 282/284 passed before the fixture update; the two failures were quotation requests rejected because the Estimate fixture lacked a primary billing address. The affected quotation scenarios then passed 2/2. A subsequent full rerun encountered PostgreSQL connection timeouts while initializing unrelated `CurrentUserEndpointTests` and `EstimateCatalogEndpointsTests`; that run was stopped before totals. |
| `CustomerContactMigrationTests` | Passed 7/7, including rollback and reapply after correcting migration constraint drop order. |
| `CustomerEndpointsTests` | Passed 23/23 with Docker available. |
| Estimate quotation fixture regressions | Passed 2/2 after adding a primary billing address to the shared estimating-opportunity fixture. |
| `IssueQuotation_ApprovedEstimateWithoutPrimaryBillingAddress_ReturnsCustomerBillingNotReady` | Passed 1/1; confirms the billing readiness gate returns 422 and creates no quotation without a primary billing address. |
| OpenAPI snapshot integration test with `UPDATE_OPENAPI=1` | Passed after the customer request model updates. Later address validation changes did not alter API schemas. |
| `npm run check:api` | Exit 1 because generated API types have intentional uncommitted changes relative to `HEAD`. Re-running `npm run generate:api` produced the same SHA-256, confirming generated output is stable against the current OpenAPI document. |
| `npm run lint` | Passed after customer UI changes |
| `npx tsc --noEmit` | Passed after generated API refresh |
| `npm test` | Passed; 595 tests across 135 files |
| `npm run build -- --webpack` | Passed, including production compile, TypeScript, and static page generation. Default Turbopack build could not bind its local port in this environment. |
| Reuse changes: `npm run lint`, `npx tsc --noEmit`, `npm test -- --run`, `npm run build -- --webpack` | Passed after shared address/contact/identity fields, query invalidation, duplicate rows, customer display/error helpers, and mutation runner changes; 595 frontend tests across 135 files. |
| Reuse changes: `dotnet build backend/TanErp.slnx --no-restore --disable-build-servers`, `dotnet test backend/tests/TanErp.UnitTests/TanErp.UnitTests.csproj --no-build`, Architecture tests | Passed after shared validation changes; build 0 warnings/errors, unit 284/284, architecture 3/3. |
| Thai/English message key parity | Passed; both locale files parse and have matching key paths |

At the time of this verification (2026-09-29), the full backend test suite had not completed cleanly after the fixture fix; the limitation was superseded by the 2026-09-30 full solution run recorded below. Targeted Customer migration/API tests and quotation readiness scenarios pass.

### Full backend suite refresh (2026-09-30)

- `dotnet build backend/TanErp.slnx --no-restore -m:1` passed with 0 warnings/errors.
- `dotnet test backend/TanErp.slnx --no-restore -m:1` passed IntegrationTests 289/289, UnitTests 287/287, and ArchitectureTests 3/3 with Docker Desktop socket access.
- This verifies the current backend code suite, not Customer business UAT, pilot credit policy, or Production sign-off.

## Security and data notes

- Customer/contact/address mutation uses organization scope and `If-Match`; create and customer lifecycle requests use `Idempotency-Key`.
- Customer deactivation is blocked while open opportunities remain.
- Tax identifiers are checked with the Thai 13-digit checksum and require `customer-contacts.manage` to read or change.
- Credit terms require `customers.credit.read` to read and `customers.credit.manage` to change.
- Audit events record changed field names; they do not contain tax identifiers, contact details, or billing address values.
- Quotation billing snapshots preserve issuance-time customer and billing-address values and include a SHA-256 hash.
