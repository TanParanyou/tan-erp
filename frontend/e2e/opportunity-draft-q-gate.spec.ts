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

test.describe("Opportunity Draft Q-Gate Completion Journey", () => {
  test("creates incomplete draft, updates Q-gate inline, and transitions to qualified", async ({ page }) => {
    const authContext = await signIn(page);

    // 1. Create a customer and activate
    const suffix = Date.now().toString().slice(-4);
    const customerNameTh = `บริษัท คิวเกตจำกัด ${suffix}`;
    const contactName = "คุณคิวเกต สมบูรณ์";
    const phone = `084222${suffix}`;

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
    const customerRes = await createCustomerResponsePromise;
    const customerData = (await customerRes.json()) as { id: string };
    const customerId = customerData.id;

    await page.waitForURL(`**/th/customers/${customerId}`);

    // Activate Customer
    const activateBtn = page.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    await expect(activateBtn).toBeVisible();
    await activateBtn.click();
    const modalConfirmBtn = page.getByRole("dialog").getByRole("button", { name: "ยืนยัน" });
    const activateResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/activate") && res.status() === 200
    );
    await modalConfirmBtn.click();
    await activateResponsePromise;
    await expect(page.getByText("ใช้งานอยู่")).toBeVisible();

    // 2. Create Incomplete Draft Opportunity (only customer, title, workTypes - omitting scopeSummary and nextAction)
    await page.getByRole("link", { name: "โอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");
    await page.getByRole("link", { name: /สร้างโอกาสทางการขาย/ }).click();
    await page.waitForURL("**/th/opportunities/create");

    await page.getByLabel(/ลูกค้า/).fill(customerNameTh);
    await page.getByRole("option", { name: new RegExp(customerNameTh) }).click();
    const oppTitle = `โครงการทดสอบ Q-Gate Inline ${suffix}`;
    await page.getByLabel(/ชื่อโอกาสทางการขาย/).fill(oppTitle);
    await page.getByLabel(/งานบิวท์อิน/).check();

    const createOppResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/opportunities") && res.request().method() === "POST" && res.status() === 201
    );
    await page.getByRole("button", { name: "บันทึกโอกาสทางการขาย" }).click();
    const oppRes = await createOppResponsePromise;
    const oppData = (await oppRes.json()) as { id: string; rowVersion: string; stage: string; code: string };
    expect(oppData.stage).toBe("draft");

    await page.waitForURL(`**/th/opportunities/${oppData.id}`);

    // 3. Verify Q-Gate checklist guidance banner is shown and Qualify button is disabled/missing
    await expect(page.getByRole("region", { name: /รายการตรวจสอบความพร้อมตามเกณฑ์/ })).toBeVisible();
    const qualifyBtn = page.getByRole("button", { name: /ผ่านเกณฑ์ \(Qualify\)/ });
    await expect(qualifyBtn).toHaveCount(0);

    // 4. Fill inline Q-gate form (scopeSummary, nextAction date and note)
    const scopeInput = page.getByLabel(/สรุปขอบเขตงาน/);
    await scopeInput.fill("งานตกแต่งภายในห้องผู้บริหารและบิวท์อินตู้เอกสาร");

    const datePickerInput = page.getByPlaceholder("เลือกวันที่ (วว/ดด/ปปปป)");
    await datePickerInput.fill("25/09/2026");
    await datePickerInput.press("Enter");

    const noteInput = page.getByLabel(/บันทึกการดำเนินการถัดไป/);
    await noteInput.fill("โทรยืนยันเวลานัดตรวจวัดพื้นที่จริง");

    // Intercept PATCH request
    const patchResponsePromise = page.waitForResponse(
      (res) =>
        res.url().includes(`/api/v1/opportunities/${oppData.id}`) &&
        res.request().method() === "PATCH" &&
        res.status() === 200
    );

    const saveQGateBtn = page.getByRole("button", { name: "บันทึกข้อมูล Q-Gate" });
    await saveQGateBtn.click();

    const patchRes = await patchResponsePromise;
    const patchedData = (await patchRes.json()) as { id: string; rowVersion: string; scopeSummary: string };
    expect(patchedData.scopeSummary).toBe("งานตกแต่งภายในห้องผู้บริหารและบิวท์อินตู้เอกสาร");
    expect(patchedData.rowVersion).not.toBe(oppData.rowVersion);

    // Toast notification should show success
    await expect(page.getByText("บันทึกข้อมูล Q-Gate เรียบร้อยแล้ว")).toBeVisible();

    // 5. Now Qualify button must be visible because Q-gate is satisfied
    await expect(page.getByRole("button", { name: /ผ่านเกณฑ์ \(Qualify\)/ })).toBeVisible();

    // 6. Transition to Qualified
    const transitionPromise = page.waitForResponse(
      (res) =>
        res.url().includes(`/api/v1/opportunities/${oppData.id}/stage-transitions`) &&
        res.request().method() === "POST" &&
        res.status() === 200
    );

    await page.getByRole("button", { name: /ผ่านเกณฑ์ \(Qualify\)/ }).click();
    const dialog = page.getByRole("dialog");
    await expect(dialog).toBeVisible();
    await dialog.getByRole("button", { name: "ยืนยัน" }).click();

    await transitionPromise;

    // 7. Verify UI updates to Qualified stage
    await expect(page.getByText("ผ่านเกณฑ์ (Qualified)").first()).toBeVisible();
  });
});
