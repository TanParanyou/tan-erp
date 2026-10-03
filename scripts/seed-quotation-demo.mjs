/**
 * Take the TEST_ONLY demo Estimate through calculate -> submit -> independent approval -> quotation issue
 * by calling the real API, so a customer-safe Quotation Document exists for local preview and UAT rehearsal.
 *
 * Prerequisites (local only):
 *   - Firebase Auth Emulator with users from scripts/seed-emulator-users.mjs
 *   - Backend running in Test mode with SeedTestData=true, SeedEstimateDemoData=true and the dedicated
 *     estimate reviewer seeded (see `make dev-backend-demo`)
 *
 * Usage: node scripts/seed-quotation-demo.mjs
 * Safe to re-run: stops when the demo Estimate is already quoted.
 */

const EMULATOR_HOST = process.env.FIREBASE_AUTH_EMULATOR_HOST || "127.0.0.1:9099";
const API_ORIGIN = process.env.API_ORIGIN || "http://localhost:5005";
const DEMO_ESTIMATE_ID = "019a3cf8-96f0-7c9f-b207-93aa818f4c04";
const PASSWORD = "TestPassword123!";
const MAKER_EMAIL = "foundation-user@example.test";
const REVIEWER_EMAIL = "foundation-estimate-reviewer@example.test";

if (!/^(127\.0\.0\.1|localhost)(:\d+)?$/.test(EMULATOR_HOST) || !/^http:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/.test(API_ORIGIN)) {
  console.error("Refusing to run: this script only targets the local emulator and a local API.");
  process.exit(1);
}

async function signIn(email) {
  const response = await fetch(
    `http://${EMULATOR_HOST}/identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=fake-api-key`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password: PASSWORD, returnSecureToken: true }),
    },
  );
  const body = await response.json();
  if (!response.ok || !body.idToken) {
    throw new Error(`Emulator sign-in failed for ${email}: run scripts/seed-emulator-users.mjs first.`);
  }
  return body.idToken;
}

async function api(session, method, path, { body, ifMatch, idempotent = false } = {}) {
  const headers = {
    Authorization: `Bearer ${session.token}`,
    Accept: "application/json",
    "Accept-Language": "en",
  };
  if (session.membershipId) headers["X-Membership-Id"] = session.membershipId;
  if (ifMatch) headers["If-Match"] = `"${ifMatch}"`;
  if (idempotent) headers["Idempotency-Key"] = crypto.randomUUID();
  if (body !== undefined) headers["Content-Type"] = "application/json";

  const response = await fetch(`${API_ORIGIN}${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  });
  const text = await response.text();
  const payload = text ? JSON.parse(text) : null;
  if (!response.ok) {
    const code = payload && payload.code ? payload.code : "UNKNOWN";
    const detail = payload && payload.detail ? ` (${payload.detail})` : "";
    throw new Error(`${method} ${path} -> ${response.status} ${code}${detail}`);
  }
  return payload;
}

async function openSession(email) {
  const token = await signIn(email);
  const me = await api({ token }, "GET", "/api/v1/me");
  const membership = me.memberships && me.memberships[0];
  if (!membership || !membership.id) {
    throw new Error(`${email} has no membership: start the backend with the dedicated estimate reviewer seed.`);
  }
  return { token, membershipId: membership.id };
}

const maker = await openSession(MAKER_EMAIL);
let estimate = await api(maker, "GET", `/api/v1/estimates/${DEMO_ESTIMATE_ID}`);

if (estimate.currentRevision.status === "quoted") {
  console.log("Demo Estimate is already quoted. Nothing to do.");
  process.exit(0);
}

if (["draft", "returned"].includes(estimate.currentRevision.status)) {
  const calculated = await api(
    maker,
    "POST",
    `/api/v1/estimates/${DEMO_ESTIMATE_ID}/revisions/${estimate.currentRevision.id}/calculate`,
    {
      body: { expectedRevisionVersion: estimate.currentRevision.rowVersion },
      idempotent: true,
    },
  );
  console.log(`Calculated revision ${calculated.revisionNo} (v${calculated.calculationVersion}).`);
  estimate = await api(maker, "GET", `/api/v1/estimates/${DEMO_ESTIMATE_ID}`);

  await api(maker, "POST", `/api/v1/estimates/${DEMO_ESTIMATE_ID}/submit`, {
    body: { revisionNo: estimate.currentRevision.revisionNo, calculationVersion: estimate.currentRevision.calculationVersion },
    ifMatch: estimate.rowVersion,
    idempotent: true,
  });
  console.log("Submitted for review.");
  estimate = await api(maker, "GET", `/api/v1/estimates/${DEMO_ESTIMATE_ID}`);
}

if (estimate.currentRevision.status === "submitted") {
  const reviewer = await openSession(REVIEWER_EMAIL);
  const forReview = await api(reviewer, "GET", `/api/v1/estimates/${DEMO_ESTIMATE_ID}`);
  await api(reviewer, "POST", `/api/v1/estimates/${DEMO_ESTIMATE_ID}/review-decisions`, {
    body: { revisionNo: forReview.currentRevision.revisionNo, decision: "approved" },
    ifMatch: forReview.rowVersion,
    idempotent: true,
  });
  console.log("Approved by the independent reviewer.");
  estimate = await api(maker, "GET", `/api/v1/estimates/${DEMO_ESTIMATE_ID}`);
}

if (estimate.currentRevision.status === "approved") {
  const opportunity = await api(maker, "GET", `/api/v1/opportunities/${estimate.opportunityId}`);
  const quotation = await api(maker, "POST", `/api/v1/estimates/${DEMO_ESTIMATE_ID}/quotation`, {
    body: { expectedEstimateVersion: estimate.rowVersion, expectedOpportunityVersion: opportunity.rowVersion },
    idempotent: true,
  });
  console.log(`Issued quotation ${quotation.number}.`);
  console.log(`Preview: /th/estimates/${DEMO_ESTIMATE_ID}/quotation`);
}
