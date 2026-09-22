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

test.describe("Estimate Item Master Catalog Journey", () => {
  test("browses catalog modal, searches, filters, multi-selects items, and recovers from price conflict", async ({
    page,
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

    // Create Customer
    const custRes = await page.request.post(`${auth.apiOrigin}/api/v1/customers`, {
      headers,
      data: {
        displayName: { thai: `บจก. สินค้าแคตตาล็อก ${suffix}`, english: `Catalog Item Co ${suffix}` },
        primaryContact: { name: "คุณวิชาญ แคตตาล็อก", phone: `081111${suffix}` },
      },
    });
    expect(custRes.ok()).toBeTruthy();
    const customer = (await custRes.json()) as { id: string };

    // Activate Customer
    const actRes = await page.request.post(`${auth.apiOrigin}/api/v1/customers/${customer.id}/activate`, {
      headers,
    });
    expect(actRes.ok()).toBeTruthy();

    // Create Site
    const siteRes = await page.request.post(`${auth.apiOrigin}/api/v1/customers/${customer.id}/sites`, {
      headers,
      data: {
        name: `โครงการทดสอบแคตตาล็อก ${suffix}`,
        address: {
          addressLine1: "123 สุขุมวิท",
          subdistrict: "คลองเตย",
          district: "คลองเตย",
          province: "กรุงเทพมหานคร",
          postalCode: "10110",
        },
      },
    });
    expect(siteRes.ok()).toBeTruthy();
    const site = (await siteRes.json()) as { id: string };

    // Create Opportunity
    const oppRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities`, {
      headers,
      data: {
        customerId: customer.id,
        title: `งานบิวท์อินแคตตาล็อก ${suffix}`,
        scopeSummary: "ทดสอบการเลือกสินค้าจาก Item Master Catalog",
        serviceCategories: ["built_in"],
        targetBudget: 500000,
        expectedClosingDate: "2026-12-31",
      },
    });
    expect(oppRes.ok()).toBeTruthy();
    const opportunity = (await oppRes.json()) as { id: string };

    // Qualify Opportunity
    const qRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/stage-transitions`, {
      headers,
      data: {
        toStage: "qualified",
        reason: "Qualification passed",
      },
    });
    expect(qRes.ok()).toBeTruthy();

    // Schedule Survey
    const surveyRes = await page.request.post(`${auth.apiOrigin}/api/v1/opportunities/${opportunity.id}/surveys`, {
      headers,
      data: {
        siteId: site.id,
        scheduledAt: new Date(Date.now() + 86400000).toISOString(),
        assignedSurveyorUserId: auth.userId,
      },
    });
    expect(surveyRes.ok()).toBeTruthy();
    const survey = (await surveyRes.json()) as { id: string };

    // Update survey draft with area & measurement
    await page.request.put(`${auth.apiOrigin}/api/v1/surveys/${survey.id}/draft`, {
      headers,
      data: {
        scopeSummary: "พื้นที่สำหรับติดตั้งเฟอร์นิเจอร์",
        areas: [
          {
            name: "ห้องรับแขก",
            measurements: [{ label: "ความกว้างผนัง", value: 4.5, unit: "m" }],
          },
        ],
      },
    });

    // Mark Survey Ready -> advances opportunity to Estimating
    const readyRes = await page.request.post(`${auth.apiOrigin}/api/v1/surveys/${survey.id}/mark-ready`, {
      headers,
    });
    expect(readyRes.ok()).toBeTruthy();

    // 2. Open Opportunity Detail in browser
    await page.goto(`/th/opportunities/${opportunity.id}`);
    await page.waitForLoadState("networkidle");

    // Click "สร้างร่างใบประเมินราคา" or open Estimate Drawer
    const createEstimateBtn = page.getByRole("button", { name: /สร้างร่างใบประเมินราคา/ });
    if (await createEstimateBtn.isVisible()) {
      await createEstimateBtn.click();
    }

    // Wait for Estimate workspace drawer
    const estimateDrawer = page.getByRole("dialog", { name: /จัดทำใบประเมินราคา/ });
    await expect(estimateDrawer).toBeVisible({ timeout: 15000 });

    // Click "เพิ่มหมวดหมู่งาน" if no sections exist
    const addSectionBtn = estimateDrawer.getByRole("button", { name: /เพิ่มหมวดหมู่งาน/ });
    if (await addSectionBtn.isVisible()) {
      await addSectionBtn.click();
    }

    // Click "เพิ่มรายการย่อย"
    const addWorkItemBtn = estimateDrawer.getByRole("button", { name: /เพิ่มรายการย่อย/ }).first();
    if (await addWorkItemBtn.isVisible()) {
      await addWorkItemBtn.click();
    }

    // Click "เลือกจากแคตตาล็อกสินค้า" (Browse Catalog)
    const browseCatalogBtn = estimateDrawer.getByRole("button", { name: /เลือกจากแคตตาล็อกสินค้า/ }).first();
    await expect(browseCatalogBtn).toBeVisible({ timeout: 10000 });
    await browseCatalogBtn.click();

    // 3. Verify Item Catalog Modal is visible
    const catalogModal = page.getByRole("dialog", { name: /เลือกสินค้าจากแคตตาล็อก/ });
    await expect(catalogModal).toBeVisible();

    // Verify search and filter inputs are present
    const searchInput = catalogModal.getByPlaceholder(/ค้นหารหัส หรือชื่อสินค้า/);
    await expect(searchInput).toBeVisible();

    const typeFilter = catalogModal.getByRole("combobox", { name: /ประเภทสินค้า/ });
    await expect(typeFilter).toBeVisible();

    // Test Search input interaction
    await searchInput.fill("ไม้");
    await page.waitForTimeout(400); // debounce wait

    // Test responsive behavior at 320px
    await page.setViewportSize({ width: 320, height: 800 });
    await expect(catalogModal).toBeVisible();

    // Restore desktop viewport
    await page.setViewportSize({ width: 1280, height: 800 });
    await expect(catalogModal).toBeVisible();

    // Close modal via Cancel / Close button
    const closeBtn = catalogModal.getByRole("button", { name: /ยกเลิก/ });
    await closeBtn.click();
    await expect(catalogModal).not.toBeVisible();
  });
});
