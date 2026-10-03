import { test, expect, type Browser, type Page } from "@playwright/test";

const EMULATOR_HOST = process.env.FIREBASE_AUTH_EMULATOR_HOST || "127.0.0.1:9099";
const PROJECT_ID = process.env.FIREBASE_PROJECT_ID || "tan-erp-test-only";
const TEST_PASSWORD = "TestPassword123!";

async function signInThroughUi(page: Page, email: string): Promise<void> {
  await page.goto("/th/login");
  await page.getByLabel("อีเมล").fill(email);
  await page.locator("input#password").fill(TEST_PASSWORD);
  const meResponse = page.waitForResponse((response) => response.url().includes("/api/v1/me"));
  await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
  expect((await meResponse).status()).toBe(200);
}

/** Creates a verified account in the local Firebase Auth Emulator, like scripts/seed-emulator-users.mjs. */
async function createVerifiedEmulatorAccount(email: string): Promise<void> {
  const response = await fetch(
    `http://${EMULATOR_HOST}/identitytoolkit.googleapis.com/v1/projects/${PROJECT_ID}/accounts?key=fake-api-key`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json", Authorization: "Bearer owner" },
      body: JSON.stringify({
        localId: `invitee-${Date.now()}`,
        email,
        password: TEST_PASSWORD,
        displayName: "Invitee",
        emailVerified: true,
      }),
    },
  );
  expect(response.ok, `Emulator account creation failed with ${response.status}`).toBe(true);
}

async function newSignedInPage(browser: Browser, email: string): Promise<Page> {
  const context = await browser.newContext();
  const page = await context.newPage();
  await signInThroughUi(page, email);
  return page;
}

test.describe("User administration (CP-02)", () => {
  test("invites a user, links them on first sign-in, and revokes access immediately", async ({ page, browser }) => {
    test.setTimeout(180000);
    const email = `invitee-${Date.now()}@example.test`;

    // 1. Administrator invites a user with a low-privilege role.
    await signInThroughUi(page, "foundation-user@example.test");
    await page.goto("/th/settings/users");
    await page.getByRole("button", { name: "เพิ่มผู้ใช้" }).click();
    await page.waitForURL("**/th/settings/users/create");

    await page.getByLabel("ชื่อที่แสดง").fill("ผู้ใช้ทดสอบเชิญ");
    await page.getByLabel("อีเมล").fill(email);
    await page.getByLabel("Test Read Only").check();
    const createResponse = page.waitForResponse(
      (response) => response.url().endsWith("/api/v1/admin/users") && response.request().method() === "POST",
    );
    await page.getByRole("button", { name: "เพิ่มผู้ใช้" }).click();
    expect((await createResponse).status()).toBe(201);

    await page.waitForURL(/\/th\/settings\/users\/[0-9a-f-]{36}$/);
    await expect(page.getByText("รอเข้าระบบครั้งแรก").first()).toBeVisible();

    // 2. A verified Firebase identity with the same email signs in and is linked automatically.
    await createVerifiedEmulatorAccount(email);
    const inviteePage = await newSignedInPage(browser, email);
    await expect(inviteePage.getByRole("link", { name: /โอกาสทางการขาย/ }).first()).toBeVisible();

    await page.reload();
    await expect(page.getByText("ใช้งานอยู่").first()).toBeVisible();

    // 3. Administrator revokes the role behind a confirmation dialog; the invitee loses access on the next load.
    await page.getByRole("button", { name: "ถอนบทบาท" }).click();
    const dialog = page.getByRole("dialog");
    await expect(dialog).toBeVisible();
    const revokeResponse = page.waitForResponse(
      (response) => response.url().includes("/roles/") && response.request().method() === "DELETE",
    );
    await dialog.getByRole("button", { name: "ยืนยัน" }).click();
    expect((await revokeResponse).status()).toBe(204);
    await expect(page.getByText("ยังไม่มีบทบาท")).toBeVisible();

    await inviteePage.reload();
    await expect(inviteePage.getByRole("link", { name: /โอกาสทางการขาย/ })).toHaveCount(0);
  });
});
