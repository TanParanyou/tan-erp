import { test, expect } from "@playwright/test";

test.describe("Foundation Login and Current User Journey", () => {
  test("complete auth lifecycle with localized ERP shell and membership states", async ({ page }) => {
    // 1. open /th/login
    await page.goto("/th/login");
    await expect(page.locator("html")).toHaveAttribute("lang", "th");
    await expect(page.getByRole("heading", { name: "เข้าสู่ระบบ Project ERP" })).toBeVisible();

    // 2. sign in through Firebase Auth Emulator with seeded test user
    const emailInput = page.getByLabel("อีเมล");
    const passwordInput = page.getByLabel("รหัสผ่าน");
    await emailInput.fill("foundation-user@example.test");
    await passwordInput.fill("TestPassword123!");

    // 3. wait for GET /api/v1/me
    const meResponsePromise = page.waitForResponse(
      (response) => response.url().includes("/api/v1/me") && response.status() === 200
    );
    await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
    await meResponsePromise;

    // 4. assert TEST_ONLY Project ERP and สาขาทดสอบ are visible
    await expect(page.locator('[data-testid="org-name"]')).toHaveText("TEST_ONLY Project ERP");
    await expect(page.locator('[data-testid="branch-name"]')).toHaveText("สาขาทดสอบ");
    await expect(page.getByText("organizations.read")).toBeVisible();

    // 5. switch to /en and assert English shell labels while data remains unchanged
    await page.getByRole("link", { name: /Switch to English/i }).click();
    await page.waitForURL("**/en");
    await expect(page.locator("html")).toHaveAttribute("lang", "en");
    await expect(page.locator('[data-testid="org-name"]')).toHaveText("TEST_ONLY Project ERP");
    await expect(page.locator('[data-testid="branch-name"]')).toHaveText("สาขาทดสอบ");
    await expect(page.getByRole("button", { name: "Sign Out" })).toBeVisible();

    // 6. log out
    await page.getByRole("button", { name: "Sign Out" }).click();

    // 7. assert login is visible and previous Organization text is absent
    await page.waitForURL("**/login");
    await expect(page.getByRole("heading", { level: 1 })).toBeVisible();
    await expect(page.locator('[data-testid="org-name"]')).toHaveCount(0);
    await expect(page.getByText("TEST_ONLY Project ERP")).toHaveCount(0);

    // 8. sign in as a valid Firebase user with no Membership
    const emailInputNomember = page.getByLabel("Email");
    const passwordInputNomember = page.getByLabel("Password");
    await emailInputNomember.fill("nomember-user@example.test");
    await passwordInputNomember.fill("TestPassword123!");
    await page.getByRole("button", { name: "Sign In" }).click();

    // 9. assert the no-membership recovery state is visible
    await expect(page.getByRole("heading", { name: /No Active Membership|ไม่พบสมาชิกภาพที่ใช้งานได้/i })).toBeVisible();
    await expect(page.getByRole("button", { name: /Sign Out|ออกจากระบบ/i })).toBeVisible();
  });
});
