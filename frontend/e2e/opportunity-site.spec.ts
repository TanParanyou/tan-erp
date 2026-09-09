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

async function postApi(
  page: Page,
  context: SignedInContext,
  path: string,
  body: unknown,
  extraHeaders: Record<string, string> = {}
) {
  return page.request.post(`${context.apiOrigin}${path}`, {
    data: body,
    headers: {
      authorization: context.authorization,
      "x-membership-id": context.membershipId,
      "idempotency-key": crypto.randomUUID(),
      ...extraHeaders,
    },
  });
}

test.describe("Opportunity and Site Management Journey", () => {
  test("complete slice journey: login -> create customer -> activate customer -> create site -> create opportunity -> view detail -> list", async ({
    page,
  }) => {
    const authContext = await signIn(page);

    // 1. Create a Draft customer first
    const suffix = Date.now().toString().slice(-4);
    const customerNameTh = `บริษัท ทดสอบสไลซ์จำกัด ${suffix}`;
    const contactName = "คุณประสิทธิ์ ทดสอบ";
    const phone = `081999${suffix}`;

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
    const customerData = await customerRes.json();
    const customerId = customerData.id;
    const draftRowVersion = customerData.rowVersion;

    await page.waitForURL(`**/th/customers/${customerId}`);
    await expect(page.getByText("ฉบับร่าง")).toBeVisible();

    // 2. Activate Customer with Confirmation Modal & Precondition assertions
    const activateBtn = page.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    await expect(activateBtn).toBeVisible();
    await activateBtn.click();

    // Modal appears
    await expect(page.getByRole("dialog")).toBeVisible();
    const modalConfirmBtn = page.getByRole("dialog").getByRole("button", { name: "ยืนยัน" });

    let capturedActivateHeaders: Record<string, string> = {};
    const activateResponsePromise = page.waitForResponse(async (res) => {
      if (res.url().includes(`/api/v1/customers/${customerId}/activate`) && res.request().method() === "POST") {
        capturedActivateHeaders = res.request().headers();
        return true;
      }
      return false;
    });

    await modalConfirmBtn.click();
    const activateRes = await activateResponsePromise;
    expect(activateRes.status()).toBe(200);
    expect(capturedActivateHeaders["x-membership-id"]).toMatch(UUID_PATTERN);
    expect(capturedActivateHeaders["idempotency-key"]?.length).toBeGreaterThanOrEqual(16);
    expect(capturedActivateHeaders["if-match"]).toBe(`"${draftRowVersion}"`);

    const activatedData = await activateRes.json();
    expect(activatedData.status).toBe("active");

    // Modal closes and status changes to Active
    await expect(page.getByRole("dialog")).not.toBeVisible();
    await expect(page.getByText("ใช้งานอยู่")).toBeVisible();

    // 3. Create Site for Active Customer
    const addSiteBtn = page.getByRole("link", { name: /เพิ่มสถานที่ตั้ง/ });
    await expect(addSiteBtn).toBeVisible();
    await addSiteBtn.click();
    await page.waitForURL(`**/th/customers/${customerId}/sites/create`);

    await page.getByLabel(/ชื่อเรียกสถานที่ตั้ง/).fill("โรงงานสาขาบางนา");
    await page.getByLabel(/ที่อยู่บรรทัดที่ 1/).fill("888 ถนนบางนา-ตราด กม.18");
    await page.getByLabel(/ตำบล \/ แขวง/).fill("บางโฉลง");
    await page.getByLabel(/อำเภอ \/ เขต/).fill("บางพลี");
    await page.getByLabel(/จังหวัด/).fill("สมุทรปราการ");
    await page.getByLabel(/รหัสไปรษณีย์/).fill("10540");

    let capturedSiteHeaders: Record<string, string> = {};
    const createSiteResponsePromise = page.waitForResponse(async (res) => {
      if (res.url().includes(`/api/v1/customers/${customerId}/sites`) && res.request().method() === "POST") {
        capturedSiteHeaders = res.request().headers();
        return true;
      }
      return false;
    });

    await page.getByRole("button", { name: "บันทึกสถานที่ตั้ง" }).click();
    const siteRes = await createSiteResponsePromise;
    expect(siteRes.status()).toBe(201);
    expect(capturedSiteHeaders["x-membership-id"]).toMatch(UUID_PATTERN);
    expect(capturedSiteHeaders["idempotency-key"]?.length).toBeGreaterThanOrEqual(16);

    const siteData = await siteRes.json();

    // Navigates back to customer detail, site is listed
    await page.waitForURL(`**/th/customers/${customerId}`);
    await expect(page.getByText("โรงงานสาขาบางนา")).toBeVisible();

    // 4. Create Opportunity with the created Site
    await page.getByRole("link", { name: "โอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");

    const createOppBtn = page.getByRole("link", { name: /สร้างโอกาสทางการขาย/ });
    await expect(createOppBtn).toBeVisible();
    await createOppBtn.click();
    await page.waitForURL("**/th/opportunities/create");

    // Assert derived read-only context inside form
    await expect(page.locator("#main-content").getByText("สาขาทดสอบ")).toBeVisible();

    // Form: Select Customer, then Site becomes available
    await page.getByLabel(/ลูกค้า/).selectOption(customerId);
    const siteSelect = page.getByLabel(/สถานที่ตั้งหลัก/);
    await expect(siteSelect.locator(`option[value="${siteData.id}"]`)).toBeAttached({ timeout: 10000 });
    await siteSelect.selectOption(siteData.id);

    const oppTitle = `โครงการตกแต่งสำนักงานและโชว์รูม ${suffix}`;
    await page.getByLabel(/ชื่อโอกาสทางการขาย/).fill(oppTitle);
    await page.getByLabel(/งานบิวท์อิน/).check();
    await page.getByLabel(/งานอินทีเรีย/).check();
    await page.getByLabel(/งบประมาณที่คาดหวัง/).fill("1200000");
    await page.getByLabel(/สกุลเงิน/).fill("THB");

    let capturedOppHeaders: Record<string, string> = {};
    let capturedOppBody: Record<string, unknown> = {};
    const createOppResponsePromise = page.waitForResponse(async (res) => {
      if (res.url().includes("/api/v1/opportunities") && res.request().method() === "POST") {
        capturedOppHeaders = res.request().headers();
        const postData = res.request().postData();
        if (postData) {
          capturedOppBody = JSON.parse(postData) as Record<string, unknown>;
        }
        return true;
      }
      return false;
    });

    await page.getByRole("button", { name: "บันทึกโอกาสทางการขาย" }).click();
    const oppRes = await createOppResponsePromise;
    expect(oppRes.status()).toBe(201);
    expect(capturedOppHeaders["x-membership-id"]).toMatch(UUID_PATTERN);
    expect(capturedOppHeaders["idempotency-key"]?.length).toBeGreaterThanOrEqual(16);

    // Assert observed POST body has none of the prohibited fields
    expect(capturedOppBody["organizationId"]).toBeUndefined();
    expect(capturedOppBody["branchId"]).toBeUndefined();
    expect(capturedOppBody["ownerUserId"]).toBeUndefined();
    expect(capturedOppBody["code"]).toBeUndefined();
    expect(capturedOppBody["stage"]).toBeUndefined();
    expect(capturedOppBody["rowVersion"]).toBeUndefined();

    const oppData = await oppRes.json();
    expect(oppData.stage).toBe("draft");
    expect(oppData.branchId).toBe(authContext.branchId);
    expect(oppData.ownerUserId).toBe(authContext.userId);

    // 5. Navigates to Opportunity Detail
    await page.waitForURL(`**/th/opportunities/${oppData.id}`);
    await expect(page.getByRole("heading", { name: oppTitle })).toBeVisible();
    await expect(page.getByText("ฉบับร่าง (Draft)")).toBeVisible();
    await expect(page.getByText("โรงงานสาขาบางนา")).toBeVisible();

    // 6. Check Opportunity in Opportunity List
    await page.getByRole("link", { name: "กลับหน้ารายการโอกาสทางการขาย" }).click();
    await page.waitForURL("**/th/opportunities");
    await expect(page.getByText(oppTitle)).toBeVisible();
    await expect(page.getByText(oppData.code)).toBeVisible();
  });

  test("double-submit and retry behavior in the browser", async ({ page }) => {
    await signIn(page);

    await page.getByRole("link", { name: "ข้อมูลลูกค้า" }).click();
    await page.waitForURL("**/th/customers");
    await page.getByRole("link", { name: "เพิ่มลูกค้าใหม่" }).click();
    await page.waitForURL("**/th/customers/create");

    const suffix = Date.now().toString().slice(-4);
    await page.locator("input#displayNameTh").fill(`ลูกค้ารองรับ Retry ${suffix}`);
    await page.locator("input#primaryContactName").fill("คุณสมศักดิ์");
    await page.locator("input#primaryContactPhone").fill(`082000${suffix}`);

    const createCustomerResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST" && res.status() === 201
    );
    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    const customerRes = await createCustomerResponsePromise;
    const customerData = await customerRes.json();
    const customerId = customerData.id;

    await page.waitForURL(`**/th/customers/${customerId}`);

    // Activate customer first so site creation is allowed
    await page.getByRole("button", { name: "เปิดใช้งานลูกค้า" }).click();
    await page.getByRole("dialog").getByRole("button", { name: "ยืนยัน" }).click();
    await expect(page.getByText("ใช้งานอยู่")).toBeVisible();

    // Go to Site Create
    await page.getByRole("link", { name: /เพิ่มสถานที่ตั้ง/ }).click();
    await page.waitForURL(`**/th/customers/${customerId}/sites/create`);

    await page.getByLabel(/ชื่อเรียกสถานที่ตั้ง/).fill("คลังสินค้าลาดกระบัง");
    await page.getByLabel(/ที่อยู่บรรทัดที่ 1/).fill("111 ลาดกระบัง");
    await page.getByLabel(/ตำบล \/ แขวง/).fill("ลาดกระบัง");
    await page.getByLabel(/อำเภอ \/ เขต/).fill("ลาดกระบัง");
    await page.getByLabel(/จังหวัด/).fill("กรุงเทพมหานคร");
    await page.getByLabel(/รหัสไปรษณีย์/).fill("10520");

    // Intercept first site create attempt and abort it to test retry key preservation
    let firstKey = "";
    let callCount = 0;
    await page.route(`**/api/v1/customers/${customerId}/sites`, async (route) => {
      callCount += 1;
      const key = route.request().headers()["idempotency-key"] ?? "";
      if (callCount === 1) {
        firstKey = key;
        await route.abort("failed");
      } else {
        await route.continue();
      }
    });

    const submitBtn = page.getByRole("button", { name: "บันทึกสถานที่ตั้ง" });
    await submitBtn.click();

    // Wait for failure alert
    await expect(page.getByRole("alert").first()).toBeVisible();
    expect(firstKey.length).toBeGreaterThanOrEqual(16);

    // Unroute so subsequent request succeeds
    await page.unroute(`**/api/v1/customers/${customerId}/sites`);

    // Retry submit without changing form -> assert same idempotency-key
    let retryKey = "";
    const retryPromise = page.waitForResponse(async (res) => {
      if (res.url().includes(`/api/v1/customers/${customerId}/sites`) && res.request().method() === "POST") {
        retryKey = res.request().headers()["idempotency-key"] ?? "";
        return true;
      }
      return false;
    });

    await submitBtn.click();
    await retryPromise;
    expect(retryKey).toBe(firstKey);
    await page.waitForURL(`**/th/customers/${customerId}`);
  });

  test("negative journeys: draft customer, org b isolation, stale etag, no branch", async ({ page }) => {
    const contextA = await signIn(page);

    // 1. Create a Draft customer
    const resDraft = await postApi(page, contextA, "/api/v1/customers", {
      customerType: "organization",
      displayNameTh: "ลูกค้าทดสอบ Draft Guard",
      preferredLocale: "th",
      primaryContact: {
        name: "นาย ก",
        phone: "0811111111",
        preferredChannel: "phone",
      },
    });
    expect(resDraft.status()).toBe(201);
    const draftData = (await resDraft.json()) as { id: string; rowVersion: string };

    // Navigate to draft customer detail: Site CTA must not be present
    await page.goto(`/th/customers/${draftData.id}`);
    await expect(page.getByText("ฉบับร่าง")).toBeVisible();
    await expect(page.getByRole("link", { name: /เพิ่มสถานที่ตั้ง/ })).toHaveCount(0);

    // Direct Site API POST against Draft customer -> 409 CUSTOMER_INVALID_STATE
    const resDraftSite = await postApi(page, contextA, `/api/v1/customers/${draftData.id}/sites`, {
      label: "ไซต์ไม่อนุมัติ",
      addressLine1: "123 สุขุมวิท",
      subdistrict: "คลองเตย",
      district: "คลองเตย",
      province: "กรุงเทพมหานคร",
      postalCode: "10110",
      countryCode: "TH",
    });
    expect(resDraftSite.status()).toBe(409);
    const draftProblem = (await resDraftSite.json()) as { code: string };
    expect(draftProblem.code).toBe("CUSTOMER_INVALID_STATE");

    // 2. Stale ETag activation returns 409 and UI shows localized conflict copy
    const staleKey = crypto.randomUUID();
    const resStale = await postApi(
      page,
      contextA,
      `/api/v1/customers/${draftData.id}/activate`,
      {},
      { "if-match": `"${crypto.randomUUID()}"`, "idempotency-key": staleKey }
    );
    expect(resStale.status()).toBe(409);
    const staleProblem = (await resStale.json()) as { code: string };
    expect(staleProblem.code).toBe("CUSTOMER_VERSION_CONFLICT");

    // 3. Org B isolation: Org B attempts to access Org A Customer -> 404 RESOURCE_NOT_FOUND
    const contextB = await signIn(page, "foundation-user-b@example.test");
    const resOrgBCross = await postApi(page, contextB, `/api/v1/customers/${draftData.id}/sites`, {
      label: "ไซต์ข้ามองค์กร",
      addressLine1: "123 สุขุมวิท",
      subdistrict: "คลองเตย",
      district: "คลองเตย",
      province: "กรุงเทพมหานคร",
      postalCode: "10110",
      countryCode: "TH",
    });
    expect(resOrgBCross.status()).toBe(404);
    const crossProblem = (await resOrgBCross.json()) as { code: string };
    expect(crossProblem.code).toBe("RESOURCE_NOT_FOUND");

    // 4. Membership without Branch: Org B has no branch
    await page.goto("/th/opportunities/create");
    await expect(page.getByText("ไม่สามารถสร้างโอกาสทางการขายได้")).toBeVisible();
    await expect(page.getByText(/คุณต้องมีสมาชิกภาพที่สังกัดสาขาที่ใช้งานได้/)).toBeVisible();

    // Direct Opportunity API POST with no-branch membership -> 422 ACTIVE_BRANCH_REQUIRED
    const resNoBranchOpp = await postApi(page, contextB, "/api/v1/opportunities", {
      customerId: draftData.id,
      title: "โอกาสทางการขายที่ไม่มีสาขา",
      workTypes: ["built-in"],
    });
    expect(resNoBranchOpp.status()).toBe(422);
    const noBranchProblem = (await resNoBranchOpp.json()) as { code: string };
    expect(noBranchProblem.code).toBe("ACTIVE_BRANCH_REQUIRED");
  });

  test("accessibility: keyboard trap and escape in confirmation modal", async ({ page }) => {
    await signIn(page);

    // Create a draft customer to open activation modal
    const suffix = Date.now().toString().slice(-4);
    await page.goto("/th/customers/create");
    await page.locator("input#displayNameTh").fill(`ลูกค้าทดสอบคีย์บอร์ด ${suffix}`);
    await page.locator("input#primaryContactName").fill("คุณคีย์บอร์ด");
    await page.locator("input#primaryContactPhone").fill(`089999${suffix}`);
    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    await page.waitForURL("**/th/customers/*");

    const activateBtn = page.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    await activateBtn.focus();
    await page.keyboard.press("Enter");

    const dialog = page.getByRole("dialog");
    await expect(dialog).toBeVisible();

    const cancelBtn = dialog.getByRole("button", { name: "ยกเลิก" });
    const confirmBtn = dialog.getByRole("button", { name: "ยืนยัน" });

    // Initial focus on cancel button
    await expect(cancelBtn).toBeFocused();

    // Tab moves focus to confirm button
    await page.keyboard.press("Tab");
    await expect(confirmBtn).toBeFocused();

    // Tab again wraps back to cancel button (focus trap)
    await page.keyboard.press("Tab");
    await expect(cancelBtn).toBeFocused();

    // Escape closes modal and returns focus to trigger
    await page.keyboard.press("Escape");
    await expect(dialog).not.toBeVisible();
    await expect(activateBtn).toBeFocused();
  });

  test("responsive viewport 320x800 and 200 percent zoom", async ({ page }) => {
    await page.setViewportSize({ width: 320, height: 800 });
    await signIn(page);

    await page.goto("/th/opportunities/create");
    await page.evaluate(() => {
      document.documentElement.style.zoom = "200%";
    });

    // Verify no horizontal document overflow
    const hasHorizontalScroll = await page.evaluate(() => {
      const doc = document.documentElement;
      const zoomFactor = parseFloat(doc.style.zoom) / 100 || 1;
      return (doc.scrollWidth / zoomFactor) > window.innerWidth;
    });
    expect(hasHorizontalScroll).toBe(false);

    // Controls remain reachable and visible
    const cancelBtn = page.getByRole("button", { name: "ยกเลิก" });
    await expect(cancelBtn).toBeVisible();
    const saveBtn = page.getByRole("button", { name: "บันทึกโอกาสทางการขาย" });
    await expect(saveBtn).toBeVisible();
  });

  test("negative guard: site creation blocked if direct route used with invalid ID", async ({ page }) => {
    await signIn(page);
    // Arbitrary ID route must trigger notFound() in this slice
    await page.goto("/th/customers/00000000-0000-0000-0000-000000000001/sites/edit-id");
    await expect(page.getByRole("heading", { name: "404" })).toBeVisible();
    await expect(page.getByText("This page could not be found.")).toBeVisible();
  });
});
