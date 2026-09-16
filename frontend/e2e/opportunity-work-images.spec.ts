import { test, expect, type Page } from "@playwright/test";
import type { CurrentUserResponse } from "@/lib/api/api-client";

interface SignedInContext {
  apiOrigin: string;
  authorization: string;
  membershipId: string;
  branchId: string | null;
  userId: string;
}

async function signIn(page: Page, email = "foundation-user@example.test"): Promise<SignedInContext> {
  await page.goto("/th/login");
  await page.getByLabel("อีเมล").fill(email);
  await page.locator("input#password").fill("TestPassword123!");
  const meRequest = page.waitForRequest(
    (request) => request.url().includes("/api/v1/me") && request.method() === "GET"
  );
  const meResponse = page.waitForResponse(
    (response) => response.url().includes("/api/v1/me") && response.status() === 200
  );
  await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
  const [request, response] = await Promise.all([meRequest, meResponse]);
  const body = (await response.json()) as CurrentUserResponse;
  const membership = body.memberships?.[0];
  if (!body.user?.id || !membership?.id) throw new Error("Incomplete test identity fixture");
  const authHeader = request.headers()["authorization"] ?? "";
  expect(authHeader).toBeTruthy();
  return {
    apiOrigin: new URL(response.url()).origin,
    authorization: authHeader,
    membershipId: membership.id,
    branchId: membership.branch?.id ?? null,
    userId: body.user.id,
  };
}

test.describe("Opportunity Work Images Journey", () => {
  test("browses opportunity, verifies work images gallery exists, and inspects attachment trigger", async ({ page }) => {
    await signIn(page);

    // Navigate to opportunities list
    await page.getByRole("link", { name: "โอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");

    // Wait for list to load
    await page.waitForSelector("table");

    // Click first opportunity row if exists
    const firstRowLink = page.locator("table tbody tr a").first();
    const count = await firstRowLink.count();

    if (count > 0) {
      await firstRowLink.click();
      await page.waitForURL(/\/th\/opportunities\/[0-9a-f-]+/);

      // Verify Work Images section header is rendered
      const workImagesHeader = page.getByRole("heading", { name: /ภาพถ่ายหน้างาน/i });
      await expect(workImagesHeader).toBeVisible();

      // Check attach button or empty state is rendered properly
      const attachButton = page.getByRole("button", { name: /แนบภาพถ่ายหน้างาน/i });
      const emptyState = page.getByText(/ยังไม่มีภาพถ่ายหน้างาน/i);

      const hasButton = await attachButton.count();
      const hasEmpty = await emptyState.count();

      expect(hasButton > 0 || hasEmpty > 0).toBeTruthy();
    }
  });
});
