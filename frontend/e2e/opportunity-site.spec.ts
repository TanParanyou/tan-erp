import { test, expect, type Page } from "@playwright/test";

async function signIn(page: Page): Promise<void> {
  await page.goto("/th/login");
  await page.getByLabel("อีเมล").fill("foundation-user@example.test");
  await page.locator("input#password").fill("TestPassword123!");
  const meResponse = page.waitForResponse(
    (res) => res.url().includes("/api/v1/me") && res.status() === 200
  );
  await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
  await meResponse;
}

test.describe("Opportunity and Site Management Journey", () => {
  test("complete slice journey: login -> create customer -> activate customer -> create site -> create opportunity -> view detail -> list", async ({
    page,
  }) => {
    await signIn(page);

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

    await page.waitForURL(`**/th/customers/${customerId}`);
    await expect(page.getByText("ฉบับร่าง")).toBeVisible();

    // 2. Activate Customer with Confirmation Modal
    const activateBtn = page.getByRole("button", { name: "เปิดใช้งานลูกค้า" });
    await expect(activateBtn).toBeVisible();
    await activateBtn.click();

    // Modal appears
    await expect(page.getByRole("dialog")).toBeVisible();
    const modalConfirmBtn = page.getByRole("dialog").getByRole("button", { name: "ยืนยัน" });

    const activateResponsePromise = page.waitForResponse(
      (res) => res.url().includes(`/api/v1/customers/${customerId}/activate`) && res.request().method() === "POST"
    );
    await modalConfirmBtn.click();
    const activateRes = await activateResponsePromise;
    expect(activateRes.status()).toBe(200);
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

    const createSiteResponsePromise = page.waitForResponse(
      (res) => res.url().includes(`/api/v1/customers/${customerId}/sites`) && res.request().method() === "POST"
    );
    await page.getByRole("button", { name: "บันทึกสถานที่ตั้ง" }).click();
    const siteRes = await createSiteResponsePromise;
    expect(siteRes.status()).toBe(201);
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

    const createOppResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/opportunities") && res.request().method() === "POST"
    );
    await page.getByRole("button", { name: "บันทึกโอกาสทางการขาย" }).click();
    const oppRes = await createOppResponsePromise;
    expect(oppRes.status()).toBe(201);
    expect(oppRes.headers()["etag"]).toBeTruthy();
    const oppData = await oppRes.json();
    expect(oppData.stage).toBe("draft");

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

  test("negative guard: site creation blocked if direct route used with invalid ID", async ({ page }) => {
    await signIn(page);
    // Arbitrary ID route must trigger notFound() in this slice
    await page.goto("/th/customers/00000000-0000-0000-0000-000000000001/sites/edit-id");
    await expect(page.getByRole("heading", { name: "404" })).toBeVisible();
    await expect(page.getByText("This page could not be found.")).toBeVisible();
  });
});
