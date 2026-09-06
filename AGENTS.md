# tan-erp Agent Guide

## Start here

Read `CONTEXT.md` for canonical business terms and `docs/README.md` for the documentation map before changing this repository.

## Current phase

This repository is in **Application Implementation**. The authorized implementation boundary is the Foundation Login and Current User vertical slice documented in `docs/superpowers/plans/2026-09-06-foundation-login-current-user.md`. Do not add CRM, Survey, Estimation, Item, Commercial, Project, Procurement, Inventory, Production, or MRP behavior without a separately approved implementation task.

## Product boundaries

- Call the product **Project ERP** or **tan-erp**. MRP is a future module inside the ERP.
- Thai is the default language. Pair important English technical terms with a short Thai explanation.
- Keep one authoritative document per concern. Link to it instead of duplicating rules.
- Record durable trade-off decisions in `docs/adr/` and business vocabulary only in `CONTEXT.md`.

## Documentation workflow

1. Identify the reader and the decision or task the document supports.
2. Update the authoritative document listed in `docs/README.md`.
3. Update related JSON flow data when the business or architecture flow changes.
4. Keep `flow.schema.json` compatible or increment the JSON version when making a breaking change.
5. Verify internal links, JSON syntax, responsive portal behavior, keyboard use, Thai/English text, and print layout.
6. Summarize the decision and list every changed document.

## Portal rules

- Keep the portal dependency-free: semantic HTML, CSS, JavaScript, JSON, and local SVG only.
- JSON is the source for diagrams; HTML must not hard-code business nodes or connections.
- Every visual diagram needs a readable text sequence and accessible labels.
- Preserve visible keyboard focus, 44px touch targets, adequate contrast, and reduced-motion support.
- Do not add external fonts, trackers, analytics, CDNs, or network dependencies.

## Future application guardrails

Read these only when application implementation is explicitly authorized:

- Backend: `docs/02-architecture/backend-architecture.md`
- Frontend: `docs/02-architecture/frontend-architecture.md`
- API and errors: `docs/03-contracts/`
- Data and Raw SQL: `docs/04-data/`
- Testing and completion gates: `docs/05-engineering/`

The backend uses Clean Architecture with four projects and feature folders. EF Core owns writes and transactions; Dapper/parameterized Raw SQL supports justified complex reads in Infrastructure. PostgreSQL owns RBAC data; Firebase provides identity only.
