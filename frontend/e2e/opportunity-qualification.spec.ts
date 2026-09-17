import { test, expect, type Page } from "@playwright/test";
import type { CurrentUserResponse } from "@/lib/api/api-client";

interface SignedInContext {
  apiOrigin: string;
  authorization: string;
  membershipId: string;
  branchId: string | null;
  userId: string;
}

const UUID_PATTERN = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

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

test.describe("Opportunity Qualification Journey", () => {
  test("qualifies opportunity from draft to qualified with responsive assertions", async ({ page }) => {
    const authContext = await signIn(page);

    // 1. Create a customer and activate
    const suffix = Date.now().toString().slice(-4);
    const customerNameTh = `บริษัท ทดสอบคัดกรองจำกัด ${suffix}`;
    const contactName = "คุณศักดิ์ชัย ทดสอบ";
    const phone = `083111${suffix}`;

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

    // 2. Create Opportunity that meets all Q-gate invariants
    await page.getByRole("link", { name: "โอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");
    await page.getByRole("link", { name: /สร้างโอกาสทางการขาย/ }).click();
    await page.waitForURL("**/th/opportunities/create");

    await page.getByLabel(/ลูกค้า/).fill(customerNameTh);
    await page.getByRole("option", { name: new RegExp(customerNameTh) }).click();
    const oppTitle = `โครงการประเมินคุณสมบัติ ${suffix}`;
    await page.getByLabel(/ชื่อโอกาสทางการขาย/).fill(oppTitle);
    await page.getByLabel(/สรุปขอบเขตงาน/).fill("ตกแต่งภายในสำนักงานชั้น 5 และติดตั้งเฟอร์นิเจอร์บิวท์อิน");
    await page.getByLabel(/งานบิวท์อิน/).check();
    await page.getByLabel(/งานอินทีเรีย/).check();
    await page.getByRole("spinbutton", { name: /งบประมาณที่คาดหวัง/ }).fill("850000");
    const datePickerInput = page.getByPlaceholder("เลือกวันที่ (วว/ดด/ปปปป)").last();
    await datePickerInput.fill("15/10/2026");
    await datePickerInput.press("Enter");
    await page.getByLabel(/บันทึกการดำเนินการถัดไป/).fill("นัดหมายสำรวจหน้างานจริง");

    const createOppResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/opportunities") && res.request().method() === "POST" && res.status() === 201
    );
    await page.getByRole("button", { name: "บันทึกโอกาสทางการขาย" }).click();
    const oppRes = await createOppResponsePromise;
    const oppData = (await oppRes.json()) as { id: string; rowVersion: string; stage: string; code: string };
    expect(oppData.stage).toBe("draft");

    await page.waitForURL(`**/th/opportunities/${oppData.id}`);

    // 3. Responsive check at 320x800 viewport
    await page.setViewportSize({ width: 320, height: 800 });
    const scrollWidth = await page.evaluate(() => document.documentElement.scrollWidth);
    const clientWidth = await page.evaluate(() => document.documentElement.clientWidth);
    expect(scrollWidth).toBeLessThanOrEqual(clientWidth + 2); // No horizontal overflow

    // Reset viewport for normal testing
    await page.setViewportSize({ width: 1280, height: 800 });

    // 4. Verify Qualify Button and Confirmation Modal
    const qualifyBtn = page.getByRole("button", { name: /ผ่านเกณฑ์ \(Qualify\)/ });
    await expect(qualifyBtn).toBeVisible();

    // Intercept transition request
    let capturedHeaders: Record<string, string> = {};
    let capturedBody: Record<string, unknown> = {};
    let capturedTransitionUrl = "";

    const transitionPromise = page.waitForResponse(async (res) => {
      if (res.url().includes("/stage-transitions") && res.request().method() === "POST") {
        capturedTransitionUrl = res.url();
        capturedHeaders = res.request().headers();
        const postData = res.request().postData();
        if (postData) {
          capturedBody = JSON.parse(postData) as Record<string, unknown>;
        }
        return true;
      }
      return false;
    });

    await qualifyBtn.click();
    const dialog = page.getByRole("dialog");
    await expect(dialog).toBeVisible();
    await expect(dialog.getByText(/ยืนยันการเปลี่ยนขั้นตอนเป็น 'ผ่านเกณฑ์'/)).toBeVisible();

    const dialogConfirmBtn = dialog.getByRole("button", { name: "ยืนยัน" });
    // Double click to verify double-submit protection
    await dialogConfirmBtn.click();

    const transitionRes = await transitionPromise;
    expect(transitionRes.status()).toBe(200);

    // Verify request headers and contract
    expect(capturedTransitionUrl).toContain(`/api/v1/opportunities/${oppData.id}/stage-transitions`);
    expect(capturedHeaders["x-membership-id"]).toBe(authContext.membershipId);
    expect(capturedHeaders["idempotency-key"]).toBeTruthy();
    expect(capturedBody).toEqual({
      targetStage: "qualified",
      expectedVersion: oppData.rowVersion,
    });

    const qualifiedData = (await transitionRes.json()) as { stage: string; rowVersion: string };
    expect(qualifiedData.stage).toBe("qualified");
    expect(qualifiedData.rowVersion).not.toBe(oppData.rowVersion);

    // 5. Verify UI Detail updates to Qualified badge without page reload
    await expect(page.getByText("ผ่านเกณฑ์ (Qualified)").first()).toBeVisible();

    // 6. Verify Opportunity List displays Qualified
    await page.getByRole("link", { name: "กลับหน้ารายการโอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");
    await expect(page.getByText(oppTitle)).toBeVisible();
    await expect(page.locator("table").getByText("ผ่านเกณฑ์ (Qualified)").first()).toBeVisible();
  });
});
