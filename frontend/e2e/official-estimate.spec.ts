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

test.describe("Official Estimate & Commercial Journey (Slice 5A + 5B)", () => {
  test("creates estimate draft, calculates BOQ, issues quotation, and advances opportunity to proposed then won upon customer acceptance", async ({
    page,
  }) => {
    test.setTimeout(120000);
    await signIn(page);

    // 1. Create a customer with a primary site and activate
    const suffix = Date.now().toString().slice(-4);
    const customerNameTh = `บจก. ประเมินราคา ${suffix}`;
    const contactName = "คุณธนพัฒน์ ประเมิน";
    const phone = `085222${suffix}`;

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
    await page.getByLabel(/ที่อยู่บรรทัดที่ 1/).fill("999 ถนนวิภาวดีรังสิต");

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

    await page.getByRole("combobox", { name: /ลูกค้า/ }).fill(customerNameTh);
    await page.getByRole("option", { name: new RegExp(customerNameTh) }).click();
    const oppTitle = `งานประเมินราคาบิวท์อินคอนโด ${suffix}`;
    await page.getByLabel(/ชื่อโอกาสทางการขาย/).fill(oppTitle);
    await page.getByLabel(/สรุปขอบเขตงาน/).fill("ประเมินราคาทำตู้และชั้นวางทีวี");
    await page.getByLabel(/งานบิวท์อิน/).check();
    await page.getByRole("tab", { name: "แผนการค้า & การติดตาม" }).click();
    await page.getByRole("spinbutton", { name: /งบประมาณที่คาดหวัง/ }).fill("600000");

    const datePickerInput = page.getByPlaceholder("เลือกวันที่ (วว/ดด/ปปปป)").last();
    await datePickerInput.fill("25/11/2026");
    await datePickerInput.press("Enter");
    await page.getByLabel(/บันทึกการดำเนินการถัดไป/).fill("ทำ BOQ และคำนวณราคา");

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

    // 3. Schedule Survey & Mark Ready to advance to Estimating
    const scheduleSurveyBtn = page.getByRole("button", { name: /นัดหมายสำรวจ/ });
    await expect(scheduleSurveyBtn).toBeVisible();
    await scheduleSurveyBtn.click();

    const scheduleModal = page.getByRole("dialog");
    await expect(scheduleModal).toBeVisible();
    await scheduleModal.getByLabel(/สถานที่สำรวจ/).selectOption({ index: 1 });

    const surveyorInput = scheduleModal.getByPlaceholder(/เลือกพนักงานผู้ทำการสำรวจ/);
    await surveyorInput.fill("Foundation");
    const surveyorOption = scheduleModal.getByRole("listbox", { name: /ผู้รับผิดชอบการสำรวจ/ }).getByRole("option").first();
    await expect(surveyorOption).toBeVisible({ timeout: 10000 });
    await surveyorOption.click();

    const createSurveyPromise = page.waitForResponse(
      (res) => res.url().includes("/surveys") && res.request().method() === "POST" && res.status() === 201
    );
    await scheduleModal.getByRole("button", { name: "ยืนยันนัดหมาย" }).click();
    await createSurveyPromise;

    // Open Survey Drawer, fill measurement, and mark ready
    const surveyCard = page.getByRole("region", { name: /ข้อมูลการนัดหมายสำรวจหน้างาน/i });
    const openSurveyWorkspaceBtn = surveyCard.getByRole("button", { name: /เปิดหน้าต่างสำรวจ/ });
    await expect(openSurveyWorkspaceBtn).toBeVisible();
    await openSurveyWorkspaceBtn.click();

    const surveyDrawer = page.getByRole("dialog", { name: /บันทึกผลการสำรวจหน้างาน/ });
    await surveyDrawer.locator("#scope-summary-input").fill("วัดระยะห้องนอน");
    await surveyDrawer.getByRole("button", { name: "เพิ่มพื้นที่สำรวจ" }).click();
    await surveyDrawer.locator("input[placeholder*='ชื่อพื้นที่']").first().fill("ห้องนอนหลัก");
    await surveyDrawer.locator("input[type='number']").first().fill("3.8");

    const saveSurveyDraftBtn = surveyDrawer.getByRole("button", { name: "บันทึกฉบับร่าง" });
    const updateSurveyDraftPromise = page.waitForResponse(
      (res) => res.url().includes("/draft") && res.request().method() === "PUT" && res.status() === 200
    );
    await saveSurveyDraftBtn.click();
    await updateSurveyDraftPromise;

    const markReadyBtn = surveyDrawer.getByRole("button", { name: "ยืนยันความพร้อม (Mark Ready)" });
    await markReadyBtn.click();
    const confirmReadyModal = page.getByRole("dialog").filter({ hasText: /ยืนยันความพร้อมเพื่อส่งต่อประเมินราคา/ });
    const confirmReadyBtn = confirmReadyModal.getByRole("button", { name: "ยืนยัน" });

    const markReadyPromise = page.waitForResponse(
      (res) => res.url().includes("/mark-ready") && res.status() === 200
    );
    await confirmReadyBtn.click();
    await markReadyPromise;

    // 4. Create Official Estimate
    await expect(page.getByText(/ถอดแบบ\/ประเมินราคา \(Estimating\)/i).first()).toBeVisible();
    const createEstimateBtn = page.getByRole("button", { name: /สร้างใบประเมินราคา/i });
    await expect(createEstimateBtn).toBeVisible();

    const createEstimatePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/estimates") && res.request().method() === "POST" && res.status() === 201
    );
    await createEstimateBtn.click();
    const createEstimateRes = await createEstimatePromise;
    const estimateData = (await createEstimateRes.json()) as { id: string; number: string };

    // 5. Verify EstimateCard and open Estimate Workspace Drawer
    await expect(page.getByText(estimateData.number)).toBeVisible();
    const openEstimateWorkspaceBtn = page.getByRole("button", { name: /เปิดหน้าจอคิดราคา/i });
    await expect(openEstimateWorkspaceBtn).toBeVisible();
    await openEstimateWorkspaceBtn.click();

    const estimateDrawer = page.getByRole("dialog", { name: /บันทึกและคำนวณราคาต้นทุน-ขาย/i });
    await expect(estimateDrawer).toBeVisible();

    // 6. Add Section and Work Item
    const addSectionBtn = estimateDrawer.getByRole("button", { name: /เพิ่มหมวดหมู่งาน/i }).first();
    await addSectionBtn.click();

    // Section 1 exists
    await expect(estimateDrawer.locator("input[value*='SEC-1']")).toBeVisible();

    // Add Work Item inside Section
    const addWorkItemBtn = estimateDrawer.getByRole("button", { name: /เพิ่มรายการงาน/i }).first();
    await addWorkItemBtn.click();

    // Fill Item description
    const itemDescInput = estimateDrawer.locator("input[placeholder*='เช่น ตู้เสื้อผ้า']").first();
    await itemDescInput.fill("ตู้เสื้อผ้าขนาดใหญ่ 3 ตอน");

    // Fill Cost Component
    const costDescInput = estimateDrawer.locator("#cost-component-desc-0-0-0");
    await costDescInput.fill("ไม้โครงและปิดผิวลามิเนต");

    const unitCostInput = estimateDrawer.locator("#cost-component-unit-cost-0-0-0");
    await unitCostInput.fill("20000");

    // 7. Save Draft
    const saveDraftBtn = estimateDrawer.getByRole("button", { name: /บันทึกฉบับร่าง/i });
    await expect(saveDraftBtn).toBeEnabled();
    const updateDraftPromise = page.waitForResponse(
      (res) => res.url().includes("/draft") && res.request().method() === "PUT" && res.status() === 200
    );
    await saveDraftBtn.click();
    await updateDraftPromise;

    // 8. Apply Discount & Recalculate
    const discountInput = estimateDrawer.locator("#estimate-discount-input");
    await discountInput.fill("2000");

    const recalculateBtn = estimateDrawer.getByRole("button", { name: /คำนวณราคาใหม่/i });
    await expect(recalculateBtn).toBeEnabled();
    const calculatePromise = page.waitForResponse(
      (res) => res.url().includes("/calculate") && res.request().method() === "POST" && res.status() === 200
    );
    await recalculateBtn.click();
    const calculateRes = await calculatePromise;
    const calcResult = (await calculateRes.json()) as { grandTotal: number; netCost: number };

    expect(calcResult.netCost).toBeGreaterThan(0);
    expect(calcResult.grandTotal).toBeGreaterThan(0);

    // 9. Close Drawer and verify Estimate Card updated with recalculated numbers
    const closeDrawerBtn = estimateDrawer.getByRole("button", { name: /Close drawer/i });
    await closeDrawerBtn.click();

    await expect(page.getByText(estimateData.number)).toBeVisible();
    await expect(page.getByText(/ยอดรวมสุทธิทั้งสิ้น/)).toBeVisible();

    // 10. Issue Quotation (Slice 5B: Advances Opportunity to 'proposed')
    const issueQuotationBtn = page.getByRole("button", { name: /ออกใบเสนอราคา/i });
    await expect(issueQuotationBtn).toBeVisible();
    await issueQuotationBtn.click();

    const issueConfirmModal = page.getByRole("dialog");
    await expect(issueConfirmModal).toBeVisible();
    const issueConfirmBtn = issueConfirmModal.getByRole("button", { name: /ออกใบเสนอราคา/i });
    const issueQuotationPromise = page.waitForResponse(
      (res) => res.url().includes("/quotation") && res.request().method() === "POST" && res.status() === 201
    );
    await issueConfirmBtn.click();
    await issueQuotationPromise;

    // Verify Estimate is now quoted and Opportunity stage is proposed
    await expect(page.getByText(/ออกใบเสนอราคาแล้ว/i)).toBeVisible();
    await expect(page.getByText(/เสนอราคาแล้ว \(Proposed\)/i).first()).toBeVisible();
    const proposedTimelineBadge = page.locator("div[class*='border-l-2']").getByText(/เสนอราคาแล้ว \(Proposed\)/i);
    await expect(proposedTimelineBadge).toBeVisible();

    // 11. Customer Acceptance (Slice 5B: Advances Opportunity to 'won')
    const acceptQuotationBtn = page.getByRole("button", { name: /ลูกค้ายอมรับใบเสนอราคา/i });
    await expect(acceptQuotationBtn).toBeVisible();
    await acceptQuotationBtn.click();

    const acceptConfirmModal = page.getByRole("dialog");
    await expect(acceptConfirmModal).toBeVisible();
    const acceptConfirmBtn = acceptConfirmModal.getByRole("button", { name: /ลูกค้ายอมรับใบเสนอราคา/i });
    const acceptQuotationPromise = page.waitForResponse(
      (res) => res.url().includes("/quotation/accept") && res.request().method() === "POST" && res.status() === 200
    );
    await acceptConfirmBtn.click();
    await acceptQuotationPromise;

    // Verify Opportunity stage is now won!
    await expect(page.getByText(/ปิดการขายสำเร็จ \(Won\)/i).first()).toBeVisible();
    const wonTimelineBadge = page.locator("div[class*='border-l-2']").getByText(/ปิดการขายสำเร็จ \(Won\)/i);
    await expect(wonTimelineBadge).toHaveCount(1);
  });
});
