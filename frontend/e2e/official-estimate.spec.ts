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
    (response) => response.url().includes("/api/v1/me")
  );
  await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
  const [request, response] = await Promise.all([meRequest, meResponse]);
  const currentUserPayload = await response.text();
  expect(response.status(), `Current user request failed with ${response.status()}: ${currentUserPayload}`).toBe(200);
  const body = JSON.parse(currentUserPayload) as CurrentUserResponse;
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
  test("submits, returns, recalculates, and independently approves an estimate before quotation acceptance", async ({
    page,
    browser,
  }) => {
    test.setTimeout(240000);
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

    // Complete the organization billing profile required for quotation issuance.
    const taxIdentifierBody = `0105563${suffix.padStart(5, "0")}`;
    const taxIdentifierCheckDigit = (11 - Array.from(taxIdentifierBody).reduce(
      (sum, digit, index) => sum + Number(digit) * (13 - index),
      0,
    ) % 11) % 10;
    await page.getByRole("button", { name: "แก้ไขข้อมูลลูกค้า" }).click();
    await page.getByRole("tab", { name: "ภาษีและเงื่อนไขการค้า" }).click();
    await page.getByLabel("ชื่อจดทะเบียน").fill(customerNameTh);
    await page.getByLabel("เลขประจำตัวผู้เสียภาษี").fill(`${taxIdentifierBody}${taxIdentifierCheckDigit}`);
    await page.getByLabel("รหัสสาขาภาษี").fill("00000");
    const updateCustomerPromise = page.waitForResponse(
      (res) => res.url().endsWith(`/api/v1/customers/${customerId}`) && res.request().method() === "PATCH" && res.status() === 200
    );
    await page.getByRole("button", { name: /บันทึกการเปลี่ยนแปลง/ }).click();
    await updateCustomerPromise;

    // Add the primary billing address required by quotation issuance.
    await page.getByLabel("ชื่อเรียกที่อยู่").fill("สำนักงานใหญ่");
    await page.getByLabel("เลขที่ อาคาร ถนน").fill("999 ถนนวิภาวดีรังสิต");
    const billingArea = page.getByRole("combobox", { name: /ตำบล \/ อำเภอ \/ จังหวัด \/ รหัสไปรษณีย์/ }).first();
    await billingArea.fill("10310");
    const billingAddressOption = page.getByRole("listbox").getByRole("option").first();
    await expect(billingAddressOption).toBeVisible({ timeout: 10000 });
    await billingAddressOption.click();
    const createBillingAddressPromise = page.waitForResponse(
      (res) => res.url().includes(`/api/v1/customers/${customerId}/addresses`) && res.request().method() === "POST"
    );
    await page.getByRole("button", { name: /บันทึกการเปลี่ยนแปลง/ }).last().click();
    expect((await createBillingAddressPromise).status()).toBe(201);

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
    await estimateDrawer.getByLabel("รหัสเหตุผลรายการงานกำหนดเอง").fill("TEST_ONLY_CUSTOM_WORK_ITEM");
    await estimateDrawer.getByLabel("เหตุผลที่กำหนดรายการงานเอง").fill("รายการเฉพาะโครงการที่ยังไม่มีใน Item Master");

    await estimateDrawer.getByRole("tab", { name: /^ต้นทุน/ }).click();

    // Fill Cost Component
    const costDescInput = estimateDrawer.locator("#cost-component-desc-0-0-0");
    await costDescInput.fill("ไม้โครงและปิดผิวลามิเนต");

    const unitCostInput = estimateDrawer.locator("#cost-component-unit-cost-0-0-0");
    await unitCostInput.fill("20000");
    await estimateDrawer.getByRole("combobox", { name: "เหตุผลที่ใช้ต้นทุนชั่วคราว" }).selectOption("market-benchmark");

    // 7. Save Draft
    const saveDraftBtn = estimateDrawer.getByRole("button", { name: /บันทึกฉบับร่าง/i });
    await expect(saveDraftBtn).toBeEnabled();
    const updateDraftPromise = page.waitForResponse(
      (res) => res.url().includes("/draft") && res.request().method() === "PUT" && res.status() === 200
    );
    await saveDraftBtn.click();
    await updateDraftPromise;

    // 8. Apply Discount & Recalculate
    await estimateDrawer.locator("#estimate-discount-type").selectOption("fixed-amount");
    const discountInput = estimateDrawer.locator("#estimate-discount-value");
    await discountInput.fill("2000");
    await estimateDrawer.locator("#estimate-discount-reason").fill("TEST_ONLY_DISCOUNT");

    const recalculateBtn = estimateDrawer.getByRole("button", { name: /บันทึกและคำนวณ$/i });
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

    // 10. Submit as the estimator, then approve from a separate reviewer identity.
    const submitButton = page.getByRole("button", { name: "ส่งตรวจอนุมัติ" });
    await expect(submitButton).toBeVisible();
    await submitButton.click();
    const submitDialog = page.getByRole("dialog");
    const submitResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/submit") && res.request().method() === "POST" && res.status() === 200
    );
    await submitDialog.getByRole("button", { name: "ส่งตรวจอนุมัติ" }).click();
    await submitResponsePromise;

    const reviewerPage = await browser.newPage();
    await signIn(reviewerPage, "foundation-estimate-reviewer@example.test");
    await reviewerPage.goto("/th/estimates/review-queue");
    await expect(reviewerPage.getByText(estimateData.number)).toBeVisible();
    await reviewerPage.getByRole("button", { name: "รายละเอียดการตรวจสอบ" }).click();
    await expect(reviewerPage.getByText(/SYSTEM_BOOTSTRAP_INDEPENDENT_CHECKER/)).toBeVisible();
    await reviewerPage.getByRole("link", { name: "เปิดพื้นที่ประเมินราคา" }).click();
    await reviewerPage.waitForURL(`**/th/opportunities/${oppData.id}`);

    const returnButton = reviewerPage.getByRole("button", { name: "ส่งกลับแก้ไข" });
    await expect(returnButton).toBeVisible();
    await returnButton.click();
    const returnDialog = reviewerPage.getByRole("dialog");
    await returnDialog.getByLabel("รหัสเหตุผล").fill("E2E_ADJUSTMENT");
    await returnDialog.getByLabel("รายละเอียดที่ต้องแก้ไข").fill("ทบทวนส่วนลดก่อนอนุมัติ");
    const returnResponsePromise = reviewerPage.waitForResponse(
      (res) => res.url().includes("/review-decisions") && res.request().method() === "POST" && res.status() === 200
    );
    await returnDialog.getByRole("button", { name: "ส่งกลับแก้ไข" }).click();
    await returnResponsePromise;

    await page.reload();
    await page.getByRole("button", { name: /เปิดหน้าจอคิดราคา/i }).click();
    const returnedEstimateDrawer = page.getByRole("dialog", { name: /บันทึกและคำนวณราคาต้นทุน-ขาย/i });
    await returnedEstimateDrawer.locator("#estimate-discount-value").fill("2500");
    const recalculateReturnedPromise = page.waitForResponse(
      (res) => res.url().includes("/calculate") && res.request().method() === "POST" && res.status() === 200
    );
    await returnedEstimateDrawer.getByRole("button", { name: /บันทึกและคำนวณ$/i }).click();
    await recalculateReturnedPromise;
    await returnedEstimateDrawer.getByRole("button", { name: /Close drawer/i }).click();

    const resubmitButton = page.getByRole("button", { name: "ส่งตรวจอนุมัติ" });
    await expect(resubmitButton).toBeVisible();
    await resubmitButton.click();
    const resubmitDialog = page.getByRole("dialog");
    const resubmitResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/submit") && res.request().method() === "POST" && res.status() === 200
    );
    await resubmitDialog.getByRole("button", { name: "ส่งตรวจอนุมัติ" }).click();
    await resubmitResponsePromise;

    await reviewerPage.goto("/th/estimates/review-queue");
    await expect(reviewerPage.getByText(estimateData.number)).toBeVisible();
    await reviewerPage.getByRole("button", { name: "รายละเอียดการตรวจสอบ" }).click();
    await reviewerPage.getByRole("link", { name: "เปิดพื้นที่ประเมินราคา" }).click();
    await reviewerPage.waitForURL(`**/th/opportunities/${oppData.id}`);
    const approveButton = reviewerPage.getByRole("button", { name: "อนุมัติ" });
    await expect(approveButton).toBeVisible();
    await approveButton.click();
    const approveDialog = reviewerPage.getByRole("dialog");
    const approveResponsePromise = reviewerPage.waitForResponse(
      (res) => res.url().includes("/review-decisions") && res.request().method() === "POST" && res.status() === 200
    );
    await approveDialog.getByRole("button", { name: "อนุมัติ" }).click();
    await approveResponsePromise;
    await reviewerPage.close();
    await page.reload();
    await expect(page.getByText(/อนุมัติแล้ว/)).toBeVisible();

    // 11. Issue Quotation (Slice 5B: Advances Opportunity to 'proposed')
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

    // 12. Customer Acceptance (Slice 5B: Advances Opportunity to 'won')
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

    // 12b. Customer-safe quotation document: preview, language switch, no internal data, narrow screen.
    await page.getByRole("link", { name: "ดูเอกสารใบเสนอราคา" }).click();
    await page.waitForURL(/\/th\/estimates\/[^/]+\/quotation$/);
    const quotationDocument = page.getByTestId("quotation-document");
    await expect(quotationDocument).toBeVisible();
    await expect(quotationDocument.getByRole("heading", { name: "ใบเสนอราคา" })).toBeVisible();
    await expect(quotationDocument).toContainText(customerNameTh);
    await expect(quotationDocument).not.toContainText(/ต้นทุน|margin|markup|กำไร/i);
    await page.getByRole("button", { name: "English" }).click();
    await expect(quotationDocument.getByRole("heading", { name: "Quotation" })).toBeVisible();
    await page.setViewportSize({ width: 320, height: 800 });
    const documentPageDimensions = await page.evaluate(() => ({
      viewportWidth: document.documentElement.clientWidth,
      contentWidth: document.documentElement.scrollWidth,
    }));
    expect(documentPageDimensions.contentWidth).toBeLessThanOrEqual(documentPageDimensions.viewportWidth);
    await page.setViewportSize({ width: 1280, height: 800 });
    await page.goto(`/th/opportunities/${oppData.id}`);

    // 13. Preserve the accepted quotation and start a separately versioned revision.
    const createRevisionButton = page.getByRole("button", { name: "สร้างรุ่นแก้ไข" });
    await expect(createRevisionButton).toBeVisible();
    await createRevisionButton.click();
    const revisionDialog = page.getByRole("dialog");
    await revisionDialog.getByLabel("เหตุผลที่สร้างรุ่นแก้ไข").fill("ปรับขอบเขตงานหลังลูกค้ายอมรับรุ่นแรก");
    const revisionResponsePromise = page.waitForResponse(
      (res) => res.url().endsWith("/revisions") && res.request().method() === "POST" && res.status() === 201
    );
    await revisionDialog.getByRole("button", { name: "สร้างรุ่นแก้ไข" }).click();
    await revisionResponsePromise;
    await expect(page.getByText("รุ่นที่ 2")).toBeVisible();

    // 14. Check the estimate workspace in English, with keyboard and narrow-screen settings.
    await page.goto(`/en/opportunities/${oppData.id}`);
    const englishWorkspaceButtons = page.getByRole("button", { name: "Open Workspace" });
    await expect(englishWorkspaceButtons).toHaveCount(2);
    await englishWorkspaceButtons.last().click();

    const englishEstimateDrawer = page.getByRole("dialog", { name: "Estimate & BOQ Workspace" });
    await expect(englishEstimateDrawer).toBeVisible();
    await expect(englishEstimateDrawer.getByRole("button", { name: "Add Section" })).toBeVisible();

    await englishEstimateDrawer.screenshot({ path: "test-results/estimate-workspace-desktop.png" });
    await page.emulateMedia({ reducedMotion: "reduce" });
    await page.setViewportSize({ width: 320, height: 800 });
    const narrowScreenDimensions = await page.evaluate(() => ({
      viewportWidth: document.documentElement.clientWidth,
      contentWidth: document.documentElement.scrollWidth,
    }));
    expect(narrowScreenDimensions.contentWidth).toBeLessThanOrEqual(narrowScreenDimensions.viewportWidth);

    await englishEstimateDrawer.getByRole("tab", { name: "BOQ work items", exact: true }).click();
    await expect(englishEstimateDrawer.getByRole("button", { name: /Edit work item/ })).toBeVisible();
    await expect(englishEstimateDrawer.getByRole("heading", { name: "Estimate & BOQ Workspace" })).toBeVisible();
    await expect(englishEstimateDrawer.getByRole("tab", { name: "Selected work item", exact: true })).toBeVisible();
    await englishEstimateDrawer.screenshot({ path: "test-results/estimate-workspace-mobile.png" });
    const sectionNameInput = englishEstimateDrawer.getByRole("textbox", { name: "Section name" });
    await sectionNameInput.focus();
    await page.keyboard.press("Tab");
    const focusedElementIsInsideEstimateDrawer = await englishEstimateDrawer.evaluate((drawer) =>
      drawer.contains(document.activeElement)
    );
    expect(focusedElementIsInsideEstimateDrawer).toBe(true);
  });
});
