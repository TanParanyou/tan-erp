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

test.describe("Site Survey Journey (Slice 4)", () => {
  test("creates appointment, fills survey workspace, marks revision ready, and transitions opportunity to estimating", async ({
    page,
  }) => {
    await signIn(page);

    // 1. Create a customer with a primary site and activate
    const suffix = Date.now().toString().slice(-4);
    const customerNameTh = `บจก. สำรวจทดสอบ ${suffix}`;
    const contactName = "คุณพิเชษฐ์ สำรวจ";
    const phone = `084111${suffix}`;

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

    // Add a Site to this customer via link
    const addSiteBtn = page.getByRole("link", { name: /เพิ่มสถานที่ตั้ง/ });
    await expect(addSiteBtn).toBeVisible();
    await addSiteBtn.click();
    await page.waitForURL(`**/th/customers/${customerId}/sites/create`);

    await page.getByLabel(/ชื่อเรียกสถานที่ตั้ง/).fill(`สถานที่โครงการ ${suffix}`);
    const addressCombobox = page.getByRole("combobox", { name: /ตำบล \/ อำเภอ \/ จังหวัด \/ รหัสไปรษณีย์/ });
    await addressCombobox.fill("10310");
    const addressOption = page.getByRole("option").first();
    await expect(addressOption).toBeVisible({ timeout: 10000 });
    await addressOption.click();
    await page.getByLabel(/ที่อยู่บรรทัดที่ 1/).fill("888 ถนนพระราม 9");

    const createSiteResponsePromise = page.waitForResponse(
      (res) => res.url().includes(`/api/v1/customers/${customerId}/sites`) && res.status() === 201
    );
    await page.getByRole("button", { name: "บันทึกสถานที่ตั้ง" }).click();
    await createSiteResponsePromise;
    await page.waitForURL(`**/th/customers/${customerId}`);

    // 2. Create Opportunity and Qualify it
    await page.getByRole("link", { name: "โอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");
    await page.getByRole("link", { name: /สร้างโอกาสทางการขาย/ }).click();
    await page.waitForURL("**/th/opportunities/create");

    // Use combobox for Customer autocomplete
    await page.getByRole("combobox", { name: /ลูกค้า/ }).fill(customerNameTh);
    await page.getByRole("option", { name: new RegExp(customerNameTh) }).click();
    const oppTitle = `งานสำรวจและประเมินราคาบิวท์อิน ${suffix}`;
    await page.getByLabel(/ชื่อโอกาสทางการขาย/).fill(oppTitle);
    await page.getByLabel(/สรุปขอบเขตงาน/).fill("สำรวจและวัดระยะห้องรับแขกและห้องแต่งตัว");
    await page.getByLabel(/งานบิวท์อิน/).check();
    await page.getByRole("tab", { name: "แผนการค้า & การติดตาม" }).click();
    await page.getByRole("spinbutton", { name: /งบประมาณที่คาดหวัง/ }).fill("500000");

    const datePickerInput = page.getByPlaceholder("เลือกวันที่ (วว/ดด/ปปปป)").last();
    await datePickerInput.fill("20/11/2026");
    await datePickerInput.press("Enter");
    await page.getByLabel(/บันทึกการดำเนินการถัดไป/).fill("เข้าสำรวจและวัดระยะหน้างานจริง");

    const createOppResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/opportunities") && res.request().method() === "POST" && res.status() === 201
    );
    await page.getByRole("button", { name: "บันทึกโอกาสทางการขาย" }).click();
    const oppRes = await createOppResponsePromise;
    const oppData = (await oppRes.json()) as { id: string };

    await page.waitForURL(`**/th/opportunities/${oppData.id}`);

    // Qualify the Opportunity
    const qualifyBtn = page.getByRole("button", { name: /ผ่านเกณฑ์ \(Qualify\)/ });
    await expect(qualifyBtn).toBeVisible();
    await qualifyBtn.click();
    const qualifyConfirmBtn = page.getByRole("dialog").getByRole("button", { name: "ยืนยัน" });
    const qualifyResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/stage-transitions") && res.status() === 200
    );
    await qualifyConfirmBtn.click();
    await qualifyResponsePromise;

    // 3. Schedule Survey Appointment
    const scheduleSurveyBtn = page.getByRole("button", { name: /นัดหมายสำรวจ/ });
    await expect(scheduleSurveyBtn).toBeVisible();
    await scheduleSurveyBtn.click();

    // Modal: Schedule Site Survey
    const scheduleModal = page.getByRole("dialog");
    await expect(scheduleModal).toBeVisible();

    // Select site in dropdown
    await scheduleModal.getByLabel(/สถานที่สำรวจ/).selectOption({ index: 1 });

    // Select surveyor in autocomplete
    const surveyorInput = scheduleModal.getByPlaceholder(/เลือกพนักงานผู้ทำการสำรวจ/);
    await surveyorInput.fill("Foundation");
    const surveyorOption = scheduleModal.getByRole("listbox", { name: /ผู้รับผิดชอบการสำรวจ/ }).getByRole("option").first();
    await expect(surveyorOption).toBeVisible({ timeout: 10000 });
    await surveyorOption.click();

    // Submit appointment
    const createSurveyPromise = page.waitForResponse(
      (res) => res.url().includes("/surveys") && res.request().method() === "POST" && res.status() === 201
    );
    await scheduleModal.getByRole("button", { name: "ยืนยันนัดหมาย" }).click();
    await createSurveyPromise;

    // 4. Verify Opportunity transitioned to 'surveying' and SurveyCard rendered
    await expect(page.getByText(/สำรวจหน้างาน \(Surveying\)/i).first()).toBeVisible();
    const surveyCard = page.getByRole("region", { name: /ข้อมูลการนัดหมายสำรวจหน้างาน/i });
    await expect(surveyCard).toBeVisible();
    await expect(surveyCard.getByText(/รุ่นที่ 1 \(ฉบับร่าง\)/)).toBeVisible();

    // 5. Open Survey Workspace Drawer
    const openWorkspaceBtn = surveyCard.getByRole("button", { name: /เปิดหน้าต่างสำรวจ/ });
    await expect(openWorkspaceBtn).toBeVisible();
    await openWorkspaceBtn.click();

    const workspaceDrawer = page.getByRole("dialog", { name: /บันทึกผลการสำรวจหน้างาน/ });
    await expect(workspaceDrawer).toBeVisible();

    // Fill Workspace form: scope summary
    const scopeInput = workspaceDrawer.locator("#scope-summary-input");
    await scopeInput.fill("วัดระยะพื้นที่ห้องรับแขกและผนังโถงทางเดิน");

    // Add Area
    const addAreaBtn = workspaceDrawer.getByRole("button", { name: "เพิ่มพื้นที่สำรวจ" });
    await addAreaBtn.click();

    const areaNameInput = workspaceDrawer.locator("input[placeholder*='ชื่อพื้นที่']").first();
    await areaNameInput.fill("ห้องรับแขกใหญ่");

    // Fill positive measurement value
    const measurementValInput = workspaceDrawer.locator("input[type='number']").first();
    await measurementValInput.fill("4.5");

    // Save Draft
    const saveDraftBtn = workspaceDrawer.getByRole("button", { name: "บันทึกฉบับร่าง" });
    const updateDraftPromise = page.waitForResponse(
      (res) => res.url().includes("/draft") && res.request().method() === "PUT" && res.status() === 200
    );
    await saveDraftBtn.click();
    await updateDraftPromise;

    // 6. Mark Survey Revision Ready
    const markReadyBtn = workspaceDrawer.getByRole("button", { name: "ยืนยันความพร้อม (Mark Ready)" });
    await expect(markReadyBtn).toBeVisible();
    await markReadyBtn.click();

    // Confirm Modal for Mark Ready
    const confirmReadyModal = page.getByRole("dialog").filter({ hasText: /ยืนยันความพร้อมเพื่อส่งต่อประเมินราคา/ });
    await expect(confirmReadyModal).toBeVisible();
    const confirmReadyBtn = confirmReadyModal.getByRole("button", { name: "ยืนยัน" });

    const markReadyPromise = page.waitForResponse(
      (res) => res.url().includes("/mark-ready") && res.request().method() === "POST" && res.status() === 200
    );
    await confirmReadyBtn.click();
    await markReadyPromise;

    // 7. Verify Transition to Estimating and EstimateCard appearance
    await expect(page.getByText(/ถอดแบบ\/ประเมินราคา \(Estimating\)/i).first()).toBeVisible();
    await expect(surveyCard.getByText(/พร้อมประเมินราคา \(Ready\)/)).toBeVisible();

    // Verify EstimateCard is rendered with Create Estimate action
    await expect(page.getByText(/การประเมินราคาทางการ \(Official Estimate\)/i)).toBeVisible();
    await expect(page.getByRole("button", { name: /สร้างใบประเมินราคา/i })).toBeVisible();
  });
});
