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

test.describe("Opportunity Outcome & Stage History Journey", () => {
  test("closes an opportunity as lost, verifies stage timeline, and reopens it", async ({ page }) => {
    const authContext = await signIn(page);

    // 1. Create a customer and activate
    const suffix = Date.now().toString().slice(-4);
    const customerNameTh = `บริษัท เอาท์คัมจำกัด ${suffix}`;
    const contactName = "คุณเอาท์คัม ทดสอบ";
    const phone = `085333${suffix}`;

    await page.getByRole("link", { name: "ข้อมูลลูกค้า" }).click();
    await page.waitForURL("**/th/customers");
    await page.getByRole("link", { name: "เพิ่มลูกค้าใหม่" }).click();
    await page.waitForURL("**/th/customers/create");

    await page.locator("input#displayNameTh").fill(customerNameTh);
    await page.locator("input#primaryContactName").fill(contactName);
    await page.locator("input#primaryContactPhone").fill(phone);

    const createCustomerResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST" && res.status() === 201
    );
    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    const customerResponse = await createCustomerResponsePromise;
    const customer = (await customerResponse.json()) as { id: string };

    // Activate customer
    await page.waitForURL(`**/th/customers/${customer.id}`);
    const activateButton = page.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    if (await activateButton.isVisible()) {
      const activatePromise = page.waitForResponse(
        (res) => res.url().includes("/activation") && res.status() === 200
      );
      await activateButton.click();
      await page.getByRole("dialog").getByRole("button", { name: "ยืนยันการเปิดใช้งาน" }).click();
      await activatePromise;
    }

    // 2. Create Opportunity
    await page.getByRole("link", { name: "โอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");
    await page.getByRole("link", { name: "เพิ่มโอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities/create");

    // Select Customer
    await page.locator("input#customer-autocomplete").click();
    await page.locator("input#customer-autocomplete").fill(customerNameTh);
    await page.getByRole("option", { name: new RegExp(customerNameTh) }).first().click();

    await page.locator("input#title").fill(`งานปิดและเปิดใหม่ ${suffix}`);
    await page.locator("textarea#scopeSummary").fill("ทดสอบวงจร Close และ Reopen พร้อมประวัติ");
    await page.locator("input#built-in").check();
    await page.locator("input#nextActionNote").fill("นัดหมายติดตามความคืบหน้า");

    const createOppResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/opportunities") && res.request().method() === "POST" && res.status() === 201
    );
    await page.getByRole("button", { name: "สร้างโอกาสทางการขาย" }).click();
    const oppResponse = await createOppResponsePromise;
    const opp = (await oppResponse.json()) as { id: string };

    await page.waitForURL(`**/th/opportunities/${opp.id}`);

    // 3. Close Opportunity (Lost)
    await page.getByRole("button", { name: "ปิดงาน / ยกเลิก..." }).click();
    const closeDialog = page.getByRole("dialog");
    await expect(closeDialog).toBeVisible();

    // Select reason
    const reasonSelect = closeDialog.locator("select").nth(1);
    await reasonSelect.selectOption({ value: "lost_price_too_high" });
    await closeDialog.locator("textarea").fill("งบประมาณลูกค้าไม่เพียงพอ");

    const closeResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/stage-transitions") && res.status() === 200
    );
    await closeDialog.getByRole("button", { name: "ยืนยัน" }).click();
    await closeResponsePromise;

    // Verify stage badge shows Lost and Reopen button appears
    await expect(page.locator("span.erp-badge").filter({ hasText: /ปิดการขายไม่สำเร็จ|Lost/i })).toBeVisible();
    await expect(page.getByRole("button", { name: "เปิดงานใหม่ (Reopen)" })).toBeVisible();

    // Verify Stage History Timeline contains transition
    await expect(page.getByText("ประวัติขั้นตอนการขาย (Stage History Timeline)")).toBeVisible();
    await expect(page.getByText("ราคาสูงกว่างบประมาณหรือคู่แข่ง")).toBeVisible();

    // 4. Reopen Opportunity
    await page.getByRole("button", { name: "เปิดงานใหม่ (Reopen)" }).click();
    const reopenDialog = page.getByRole("dialog");
    await expect(reopenDialog).toBeVisible();

    const reopenSelect = reopenDialog.locator("select").first();
    await reopenSelect.selectOption({ value: "reopen_budget_adjusted" });

    const reopenResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/stage-transitions") && res.status() === 200
    );
    await reopenDialog.getByRole("button", { name: "ยืนยัน" }).click();
    await reopenResponsePromise;

    // Verify stage badge is back to open/draft
    await expect(page.locator("span.erp-badge").filter({ hasText: /ฉบับร่าง|Draft/i })).toBeVisible();
    await expect(page.getByRole("button", { name: "ปิดงาน / ยกเลิก..." })).toBeVisible();
    await expect(page.getByText("ลูกค้าปรับงบประมาณใหม่")).toBeVisible();
  });
});
