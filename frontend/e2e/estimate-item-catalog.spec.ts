import { test, expect, type APIResponse, type Page } from "@playwright/test";
import type { CurrentUserResponse } from "@/lib/api/api-client";

interface SignedInContext {
  apiOrigin: string;
  authorization: string;
  membershipId: string;
  branchId: string | null;
  userId: string;
}

async function signIn(page: Page, email = "foundation-user@example.test", branchId?: string): Promise<SignedInContext> {
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
  const membership = branchId
    ? body.memberships?.find((candidate) => candidate.branch?.id === branchId)
    : body.memberships?.[0];
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

async function expectApiSuccess(response: APIResponse, operation: string) {
  expect(response.ok(), `${operation}: HTTP ${response.status()} ${await response.text()}`).toBeTruthy();
}

test.describe("Estimate Item Master Catalog Journey", () => {
  test("selects a priced Item, saves and reloads BOQ, and preserves its cost snapshot", async ({
    page,
    browser,
  }) => {
    test.setTimeout(120000);
    const auth = await signIn(page);

    // 1. Setup Customer, Site, Opportunity in Estimating stage via API
    const suffix = Date.now().toString().slice(-4);
    const headers = {
      Authorization: auth.authorization,
      "X-Membership-Id": auth.membershipId,
      "Content-Type": "application/json",
    };
    const commandHeaders = () => ({ ...headers, "Idempotency-Key": crypto.randomUUID() });

    const reviewerPage = await browser.newPage();
    const reviewer = await signIn(reviewerPage, "foundation-user-b@example.test", "019a3cf8-96f0-7c9f-b207-93aa818f4a13");
    await reviewerPage.close();
    const reviewerHeaders = {
      Authorization: reviewer.authorization,
      "X-Membership-Id": reviewer.membershipId,
      "Content-Type": "application/json",
    };
    const reviewerCommandHeaders = () => ({ ...reviewerHeaders, "Idempotency-Key": crypto.randomUUID() });

    // Seed a real active Item and publish its first cost through maker-checker APIs.
    const taxonomyResponse = await page.request.post(`${auth.apiOrigin}/api/v1/item-categories`, {
      headers: commandHeaders(),
      data: { code: `E2E-${suffix}`, name: { thai: `หมวดทดสอบ ${suffix}`, english: `E2E Category ${suffix}` }, sortOrder: 1 },
    });
    await expectApiSuccess(taxonomyResponse, "create Item category");
    const taxonomy = (await taxonomyResponse.json()) as { id: string };
    const unitResponse = await page.request.post(`${auth.apiOrigin}/api/v1/units-of-measure`, {
      headers: commandHeaders(),
      data: { code: `U${suffix}`, name: { thai: "ชิ้น", english: "Piece" }, symbol: "pcs", dimension: "Count", decimalScale: 0, roundingMode: "HalfUp" },
    });
    await expectApiSuccess(unitResponse, "create Item unit");
    const unit = (await unitResponse.json()) as { id: string; code: string };
    const itemName = `BOQ priced item ${suffix}`;
    const itemResponse = await page.request.post(`${auth.apiOrigin}/api/v1/items`, {
      headers: commandHeaders(),
      data: {
        itemType: "material",
        categoryId: taxonomy.id,
        baseUnitId: unit.id,
        name: { thai: `สินค้าทดสอบ ${suffix}`, english: itemName },
        availabilityMode: "all_branches",
        capabilities: { canSell: true, canCost: true, canPurchase: true, canStock: true, canProduce: false },
      },
    });
    await expectApiSuccess(itemResponse, "create priced Item");
    const item = (await itemResponse.json()) as { id: string; rowVersion: string; code: string };
    expect(item.code).toMatch(/^ITM-\d{5}$/);
    const itemActivation = await page.request.post(`${auth.apiOrigin}/api/v1/items/${item.id}/activate`, {
      headers: { ...commandHeaders(), "If-Match": `"${item.rowVersion}"` },
    });
    await expectApiSuccess(itemActivation, "activate priced Item");

    const publishCost = async (amount: number, effectiveFromUtc: string, sourceReference: string) => {
      const draftResponse = await page.request.post(`${auth.apiOrigin}/api/v1/items/${item.id}/costs`, {
        headers: commandHeaders(),
        data: {
          scope: "organization",
          unitId: unit.id,
          currency: "THB",
          amount,
          effectiveFromUtc,
          costSourceId: "019a3cf8-96f0-7c9f-b207-93aa818f4a14",
          sourceReference,
          reason: "Priced Item → BOQ acceptance fixture",
        },
      });
      await expectApiSuccess(draftResponse, "create cost draft");
      const draft = (await draftResponse.json()) as { id: string; rowVersion: string; version: number };
      const submittedResponse = await page.request.post(`${auth.apiOrigin}/api/v1/items/${item.id}/costs/${draft.id}/submit`, {
        headers: { ...commandHeaders(), "If-Match": `"${draft.rowVersion}"` },
      });
      await expectApiSuccess(submittedResponse, "submit cost draft");
      const submitted = (await submittedResponse.json()) as { rowVersion: string };
      const approvedResponse = await page.request.post(`${auth.apiOrigin}/api/v1/items/${item.id}/costs/${draft.id}/approve`, {
        headers: { ...reviewerCommandHeaders(), "If-Match": `"${submitted.rowVersion}"` },
      });
      await expectApiSuccess(approvedResponse, "approve cost with independent checker");
      const approved = (await approvedResponse.json()) as { rowVersion: string };
      const publishedResponse = await page.request.post(`${auth.apiOrigin}/api/v1/items/${item.id}/costs/${draft.id}/publish`, {
        headers: { ...reviewerCommandHeaders(), "If-Match": `"${approved.rowVersion}"` },
      });
      await expectApiSuccess(publishedResponse, "publish cost with independent checker");
      const published = (await publishedResponse.json()) as { id: string; version: number; amount: number };
      return published;
    };

    const initialCost = await publishCost(2500, new Date(Date.now() - 86400000).toISOString(), `BOQ-${suffix}-V1`);

    // Create Customer
    const custRes = await page.request.post(`${auth.apiOrigin}/api/v1/customers`, {
      headers: commandHeaders(),
      data: {
        customerType: "organization",
        displayNameTh: `บจก. สินค้าแคตตาล็อก ${suffix}`,
        displayNameEn: `Catalog Item Co ${suffix}`,
        preferredLocale: "th",
        primaryContact: { name: "คุณวิชาญ แคตตาล็อก", phone: `081111${suffix}`, preferredChannel: "phone" },
      },
    });
    await expectApiSuccess(custRes, "create customer");
    const customer = (await custRes.json()) as { id: string };

    // Activate Customer
    const actRes = await page.request.post(`${auth.apiOrigin}/api/v1/customers/${customer.id}/activate`, {
      headers: { ...commandHeaders(), "If-Match": custRes.headers().etag },
    });
    await expectApiSuccess(actRes, "activate customer");

    // Create Site
    const siteRes = await page.request.post(`${auth.apiOrigin}/api/v1/customers/${customer.id}/sites`, {
      headers: commandHeaders(),
      data: {
        label: `โครงการทดสอบแคตตาล็อก ${suffix}`,
        addressLine1: "123 สุขุมวิท",
        subdistrict: "คลองเตย",
        district: "คลองเตย",
        province: "กรุงเทพมหานคร",
        postalCode: "10110",
        countryCode: "TH",
      },
    });
    await expectApiSuccess(siteRes, "create site");
    const site = (await siteRes.json()) as { id: string };

    // Create Opportunity
    const oppRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities`, {
      headers: commandHeaders(),
      data: {
        customerId: customer.id,
        primarySiteId: site.id,
        title: `งานบิวท์อินแคตตาล็อก ${suffix}`,
        scopeSummary: "ทดสอบการเลือกสินค้าจาก Item Master Catalog",
        workTypes: ["built-in"],
        expectedBudget: 500000,
        currencyCode: "THB",
        targetDecisionDate: "2026-12-31",
        nextActionAtUtc: new Date(Date.now() + 86400000).toISOString(),
        nextActionNote: "จัดทำใบประเมินราคา",
      },
    });
    await expectApiSuccess(oppRes, "create opportunity");
    const opportunity = (await oppRes.json()) as { id: string; rowVersion: string };

    // Qualify Opportunity
    const qRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/stage-transitions`, {
      headers: commandHeaders(),
      data: {
        targetStage: "qualified",
        expectedVersion: opportunity.rowVersion,
      },
    });
    await expectApiSuccess(qRes, "qualify opportunity");
    const qualified = (await qRes.json()) as { rowVersion: string };

    // Schedule Survey
    const surveyRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/surveys`, {
      headers: commandHeaders(),
      data: {
        siteId: site.id,
        scheduledStartUtc: new Date(Date.now() + 86400000).toISOString(),
        scheduledEndUtc: new Date(Date.now() + 90000000).toISOString(),
        assignedSurveyorId: auth.userId,
        expectedOpportunityVersion: qualified.rowVersion,
      },
    });
    await expectApiSuccess(surveyRes, "create survey");
    const survey = (await surveyRes.json()) as { id: string; currentRevision: { id: string; rowVersion: string } };

    // Update survey draft with area & measurement
    const surveyDraftRes = await page.request.put(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/surveys/${survey.id}/revisions/${survey.currentRevision.id}/draft`, {
      headers: { ...headers, "If-Match": `"${survey.currentRevision.rowVersion}"` },
      data: {
        expectedRevisionVersion: survey.currentRevision.rowVersion,
        visitedAtUtc: new Date().toISOString(),
        scopeSummary: "พื้นที่สำหรับติดตั้งเฟอร์นิเจอร์",
        assumptions: [],
        constraints: [],
        missingDetails: [],
        areas: [
          {
            code: "AREA-01",
            name: "ห้องรับแขก",
            sortOrder: 1,
            measurements: [{ measurementType: "width", value: 4.5, unitCode: "m", captureMethod: "measured", sortOrder: 1 }],
          },
        ],
      },
    });
    await expectApiSuccess(surveyDraftRes, "update survey draft");
    const updatedRevision = (await surveyDraftRes.json()) as { rowVersion: string };

    const currentOpportunityRes = await page.request.get(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}`, { headers });
    await expectApiSuccess(currentOpportunityRes, "read opportunity");
    const currentOpportunity = (await currentOpportunityRes.json()) as { rowVersion: string };

    // Mark Survey Ready -> advances opportunity to Estimating
    const readyRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/surveys/${survey.id}/revisions/${survey.currentRevision.id}/mark-ready`, {
      headers: commandHeaders(),
      data: {
        expectedRevisionVersion: updatedRevision.rowVersion,
        expectedOpportunityVersion: currentOpportunity.rowVersion,
      },
    });
    await expectApiSuccess(readyRes, "mark survey ready");

    // 2. Open Opportunity Detail in browser
    await page.goto(`/th/opportunities/${opportunity.id}`);
    await page.waitForLoadState("networkidle");

    const createEstimateBtn = page.getByRole("button", { name: /สร้างใบประเมินราคา/ });
    await expect(createEstimateBtn).toBeVisible();
    await createEstimateBtn.click();
    const openEstimateWorkspaceBtn = page.getByRole("button", { name: /เปิดหน้าจอคิดราคา/ });
    await expect(openEstimateWorkspaceBtn).toBeVisible();
    await openEstimateWorkspaceBtn.click();

    // Wait for Estimate workspace drawer
    const estimateDrawer = page.getByRole("dialog", { name: /บันทึกและคำนวณราคาต้นทุน-ขาย/ });
    await expect(estimateDrawer).toBeVisible({ timeout: 15000 });

    // Click "เพิ่มหมวดหมู่งาน" if no sections exist
    const addSectionBtn = estimateDrawer.getByRole("button", { name: /เพิ่มหมวดหมู่งาน/ }).first();
    if (await addSectionBtn.isVisible()) {
      await addSectionBtn.click();
    }

    // Click "เพิ่มรายการย่อย"
    const addWorkItemBtn = estimateDrawer.getByRole("button", { name: /เพิ่มรายการงาน/ }).first();
    if (await addWorkItemBtn.isVisible()) {
      await addWorkItemBtn.click();
    }
    await estimateDrawer.getByPlaceholder("เช่น ตู้เสื้อผ้าบานเลื่อน H2400").fill(`รายการ BOQ ทดสอบ ${suffix}`);

    // Click "เลือกจากแคตตาล็อกสินค้า" (Browse Catalog)
    const browseCatalogBtn = estimateDrawer.getByRole("button", { name: /เปิดคลังรายการสินค้า/ }).first();
    await expect(browseCatalogBtn).toBeVisible({ timeout: 10000 });
    await browseCatalogBtn.click();

    // 3. Verify Item Catalog Modal is visible
    const catalogModal = page.getByRole("dialog", { name: /คลังรายการสินค้าและสเปกวัสดุ/ });
    await expect(catalogModal).toBeVisible();

    // Verify search and filter inputs are present
    const searchInput = catalogModal.getByPlaceholder(/ค้นหารายการงานหรือวัสดุ/);
    await expect(searchInput).toBeVisible();

    await expect(catalogModal.getByText("ตัวกรองรายการ", { exact: true })).toBeVisible();

    // Select the uniquely priced Item and verify Catalog returned its published cost.
    await searchInput.fill(`สินค้าทดสอบ ${suffix}`);
    await page.waitForTimeout(400); // debounce wait
    const pricedRow = catalogModal.getByRole("row").filter({ hasText: item.code });
    await expect(pricedRow).toContainText("2,500");

    // Test responsive behavior at 320px
    await page.setViewportSize({ width: 320, height: 800 });
    await expect(catalogModal).toBeVisible();

    // Restore desktop viewport
    await page.setViewportSize({ width: 1280, height: 800 });
    await expect(catalogModal).toBeVisible();

    await searchInput.focus();
    await page.keyboard.press("Tab");
    await expect(catalogModal.locator(":focus")).toHaveCount(1);
    await pricedRow.getByRole("checkbox", { name: `สินค้าทดสอบ ${suffix}` }).check();
    await catalogModal.getByRole("button", { name: /แทรก 1 รายการ/ }).click();
    await expect(catalogModal).not.toBeVisible();
    await expect(estimateDrawer.getByText(/2,500/).first()).toBeVisible();
    const saveResponsePromise = page.waitForResponse((response) => response.request().method() === "PUT" && response.url().includes("/draft"));
    await estimateDrawer.getByRole("button", { name: /บันทึกฉบับร่าง/ }).click();
    const saveResponse = await saveResponsePromise;
    expect(saveResponse.ok()).toBeTruthy();

    const estimateResponse = await page.request.get(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/estimates`, { headers });
    await expectApiSuccess(estimateResponse, "reload saved estimate");
    const savedEstimate = (await estimateResponse.json()) as {
      id: string;
      currentRevision: { sections: Array<{ workItems: Array<{ costComponents: Array<{ itemId: string | null; costRecordId: string | null; costRecordVersion: number | null; unitCostSnapshot: number | null }> }> }> };
    };
    const savedComponents = savedEstimate.currentRevision.sections[0]?.workItems[0]?.costComponents ?? [];
    const savedComponent = savedComponents.find((component) => component.itemId === item.id);
    expect(savedComponent).toMatchObject({ itemId: item.id, costRecordId: initialCost.id, costRecordVersion: initialCost.version, unitCostSnapshot: 2500 });

    // A later published price version must not rewrite an existing BOQ snapshot.
    const replacementDate = new Date(Date.now() - 30000).toISOString();
    await publishCost(3000, replacementDate, `BOQ-${suffix}-V2`);
    await page.reload();
    await page.waitForLoadState("networkidle");
    await page.getByRole("button", { name: /เปิดหน้าจอคิดราคา/ }).click();
    const reloadedDrawer = page.getByRole("dialog", { name: /บันทึกและคำนวณราคาต้นทุน-ขาย/ });
    await expect(reloadedDrawer).toBeVisible();
    await reloadedDrawer.getByRole("tab", { name: /^ต้นทุน/ }).click();
    await expect(reloadedDrawer.getByText(/2,500/).first()).toBeVisible();
    const estimateAfterReplacement = await page.request.get(`${auth.apiOrigin}/api/v1/estimates/${savedEstimate.id}`, { headers });
    await expectApiSuccess(estimateAfterReplacement, "read estimate after later cost publication");
    const preservedEstimate = (await estimateAfterReplacement.json()) as typeof savedEstimate;
    const preservedComponents = preservedEstimate.currentRevision.sections[0]?.workItems[0]?.costComponents ?? [];
    const preservedComponent = preservedComponents.find((component) => component.itemId === item.id);
    expect(preservedComponent).toMatchObject({ itemId: item.id, costRecordId: initialCost.id, costRecordVersion: initialCost.version, unitCostSnapshot: 2500 });

    // A second unsaved BOQ row must keep its entered catalog selection when the published price changes.
    await reloadedDrawer.getByRole("button", { name: /เพิ่มรายการงาน/ }).first().click();
    await reloadedDrawer.getByPlaceholder("เช่น ตู้เสื้อผ้าบานเลื่อน H2400").last().fill(`รายการ BOQ conflict ${suffix}`);
    await reloadedDrawer.getByRole("button", { name: /เปิดคลังรายการสินค้า/ }).last().click();
    const conflictCatalog = page.getByRole("dialog", { name: /คลังรายการสินค้าและสเปกวัสดุ/ });
    await expect(conflictCatalog).toBeVisible();
    const conflictSearch = conflictCatalog.getByPlaceholder(/ค้นหารายการงานหรือวัสดุ/);
    await conflictSearch.fill(`สินค้าทดสอบ ${suffix}`);
    await page.waitForTimeout(400);
    const conflictRow = conflictCatalog.getByRole("row").filter({ hasText: item.code });
    await expect(conflictRow).toContainText("3,000");
    await conflictRow.getByRole("checkbox", { name: `สินค้าทดสอบ ${suffix}` }).check();
    await conflictCatalog.getByRole("button", { name: /แทรก 1 รายการ/ }).click();
    await expect(conflictCatalog).not.toBeVisible();

    const newerEffectiveDate = new Date(Date.now() - 1000).toISOString();
    await publishCost(3500, newerEffectiveDate, `BOQ-${suffix}-V3`);
    const conflictSavePromise = page.waitForResponse((response) => response.request().method() === "PUT" && response.url().includes("/draft"));
    await reloadedDrawer.getByRole("button", { name: /บันทึกฉบับร่าง/ }).click();
    const conflictSave = await conflictSavePromise;
    expect(conflictSave.status()).toBe(409);
    const conflictBody = (await conflictSave.json()) as { code?: string };
    expect(conflictBody.code).toBe("ITEM_COST_VERSION_CONFLICT");
    await expect(page.getByText("ต้นทุนของวัสดุมีการเปลี่ยนแปลง กรุณาเลือกรายการจากคลังวัสดุใหม่อีกครั้ง")).toBeVisible();
    await expect(reloadedDrawer.getByPlaceholder("เช่น ตู้เสื้อผ้าบานเลื่อน H2400").last()).toHaveValue(`รายการ BOQ conflict ${suffix}`);
    const estimateAfterConflict = await page.request.get(`${auth.apiOrigin}/api/v1/estimates/${savedEstimate.id}`, { headers });
    await expectApiSuccess(estimateAfterConflict, "read estimate after catalog cost conflict");
    const unchangedEstimate = (await estimateAfterConflict.json()) as typeof savedEstimate;
    const unchangedComponents = unchangedEstimate.currentRevision.sections[0]?.workItems[0]?.costComponents ?? [];
    const unchangedComponent = unchangedComponents.find((component) => component.itemId === item.id);
    expect(unchangedComponent).toMatchObject({ itemId: item.id, costRecordId: initialCost.id, costRecordVersion: initialCost.version, unitCostSnapshot: 2500 });

    // The saved priced BOQ remains available in English too.
    await page.goto(`/en/opportunities/${opportunity.id}`);
    const estimateCard = page.getByRole("heading", { name: /^EST-/ }).locator("xpath=../../..");
    await estimateCard.getByRole("button", { name: "Open Workspace" }).click();
    const englishDrawer = page.getByRole("dialog", { name: "Estimate & BOQ Workspace" });
    await expect(englishDrawer).toBeVisible();
    await englishDrawer.getByRole("button", { name: "Add Section" }).first().click();
    await englishDrawer.getByRole("button", { name: "Add Item" }).first().click();
    await englishDrawer.getByRole("button", { name: "Open Item Catalog" }).first().click();
    const englishCatalog = page.getByRole("dialog", { name: "Item & Material Catalog" });
    await expect(englishCatalog).toBeVisible();
    await expect(englishCatalog.getByPlaceholder("Search items or materials...")).toBeVisible();
  });
});
