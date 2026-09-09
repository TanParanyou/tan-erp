import { test, expect, type Page } from "@playwright/test";

async function signIn(page: Page): Promise<void> {
  await page.goto("/th/login");
  await page.getByLabel("อีเมล").fill("foundation-user@example.test");
  await page.locator("input#password").fill("TestPassword123!");
  const meResponse = page.waitForResponse(
    (res) => res.url().includes("/api/v1/me") && res.status() === 200,
  );
  await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
  await meResponse;
}

async function gotoCreate(page: Page): Promise<void> {
  await page.getByRole("link", { name: "ข้อมูลลูกค้า" }).click();
  await page.waitForURL("**/th/customers");
  await page.getByRole("link", { name: "เพิ่มลูกค้าใหม่" }).click();
  await page.waitForURL("**/th/customers/create");
}

async function fillCreateForm(
  page: Page,
  args: { nameTh: string; contactName: string; phone?: string; email?: string },
): Promise<void> {
  await page.locator("input#displayNameTh").fill(args.nameTh);
  await page.locator("input#primaryContactName").fill(args.contactName);
  if (args.phone !== undefined) {
    await page.locator("input#primaryContactPhone").fill(args.phone);
  }
  if (args.email !== undefined) {
    await page.locator("input#primaryContactEmail").fill(args.email);
  }
}

test.describe("Customer and Contact Management Journey", () => {
  test("required validation blocks empty submit without POST", async ({ page }) => {
    await signIn(page);
    await gotoCreate(page);

    let postCount = 0;
    page.on("request", (req) => {
      if (req.url().includes("/api/v1/customers") && req.method() === "POST") {
        postCount += 1;
      }
    });

    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    await expect(page.getByRole("alert").first()).toBeVisible();
    expect(postCount).toBe(0);
  });

  test("email-only create returns 201 with ETag and draft organization labels", async ({ page }) => {
    await signIn(page);
    await gotoCreate(page);

    const suffix = Date.now().toString().slice(-4);
    const nameTh = `บริษัท อีเมลอย่างเดียว TEST_ONLY ${suffix}`;
    const email = `emailonly${suffix}@example.test`;

    await fillCreateForm(page, {
      nameTh,
      contactName: "คุณอีเมล TEST_ONLY",
      email,
    });

    const responsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST",
    );
    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    const response = await responsePromise;

    expect(response.status()).toBe(201);
    expect(response.headers()["etag"]).toBeTruthy();
    const data = await response.json();
    expect(data.status).toBe("draft");

    await page.waitForURL(`**/th/customers/${data.id}`);
    await expect(page.getByText("นิติบุคคล")).toBeVisible();
    await expect(page.getByText("ฉบับร่าง")).toBeVisible();
  });

  test("valid double click sends one POST", async ({ page }) => {
    await signIn(page);
    await gotoCreate(page);

    const suffix = Date.now().toString().slice(-4);
    await fillCreateForm(page, {
      nameTh: `บริษัท ดับเบิลคลิก TEST_ONLY ${suffix}`,
      contactName: "คุณดับเบิล TEST_ONLY",
      phone: `081234${suffix}`,
    });

    let postCount = 0;
    page.on("request", (req) => {
      if (req.url().includes("/api/v1/customers") && req.method() === "POST") {
        postCount += 1;
      }
    });

    const responsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST",
    );
    const saveButton = page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" });
    await saveButton.click();
    await saveButton.click({ force: true }).catch(() => undefined);
    await responsePromise;
    await page.waitForURL("**/th/customers/*");
    expect(postCount).toBe(1);
  });

  test("complete customer lifecycle: view empty state, create customer, see details and list item", async ({
    page,
  }) => {
    await signIn(page);

    // Navigate to Customers via Shell Navigation
    const customerNavLink = page.getByRole("link", { name: "ข้อมูลลูกค้า" });
    await expect(customerNavLink).toBeVisible();
    await customerNavLink.click();

    await page.waitForURL("**/th/customers");
    await expect(page.getByRole("heading", { name: "ข้อมูลลูกค้า" })).toBeVisible();

    // Click Create Customer button
    const createBtn = page.getByRole("link", { name: "เพิ่มลูกค้าใหม่" });
    await expect(createBtn).toBeVisible();
    await createBtn.click();

    await page.waitForURL("**/th/customers/create");
    await expect(page.getByRole("heading", { name: "เพิ่มลูกค้าใหม่" })).toBeVisible();

    // Fill in customer creation form
    const uniqueSuffix = Date.now().toString().slice(-4);
    const customerNameTh = `บริษัท ธนพัฒนา สตีล คอร์ปอเรชั่น จำกัด ${uniqueSuffix}`;
    const customerNameEn = `Tan Pattana Steel Corp Ltd ${uniqueSuffix}`;
    const contactName = "คุณสมศักดิ์ มั่นคง";
    const contactPhone = `081234${uniqueSuffix}`;
    const contactEmail = `somsak${uniqueSuffix}@example.test`;

    await page.locator("input#displayNameTh").fill(customerNameTh);
    await page.locator("input#displayNameEn").fill(customerNameEn);
    await page.locator("input#primaryContactName").fill(contactName);
    await page.locator("input#primaryContactRoleTitle").fill("ผู้จัดการฝ่ายจัดซื้อ");
    await page.locator("input#primaryContactPhone").fill(contactPhone);
    await page.locator("input#primaryContactEmail").fill(contactEmail);

    // Submit form with POST /api/v1/customers
    const createCustomerResponsePromise = page.waitForResponse(
      (res) =>
        res.url().includes("/api/v1/customers") && res.request().method() === "POST" && res.status() === 201,
    );

    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    const createResponse = await createCustomerResponsePromise;
    expect(createResponse.headers()["etag"]).toBeTruthy();
    const responseData = await createResponse.json();

    // Automatically navigates to detail view
    await page.waitForURL(`**/th/customers/${responseData.id}`);
    await expect(page.getByRole("heading", { name: customerNameTh })).toBeVisible();
    await expect(page.getByText(responseData.code).first()).toBeVisible();
    await expect(page.getByText(contactName)).toBeVisible();

    // Detail refresh keeps data
    await page.reload();
    await expect(page.getByRole("heading", { name: customerNameTh })).toBeVisible();

    // Back to customer list and verify new customer appears
    await page.getByRole("link", { name: "กลับหน้ารายการลูกค้า" }).click();
    await page.waitForURL("**/th/customers");

    // Search by code narrows the list
    await page.locator("input#customer-search-input").fill(responseData.code);
    await page.getByRole("button", { name: "ค้นหา" }).click();
    await expect(page.getByText(customerNameTh)).toBeVisible();
    await expect(page.getByText(responseData.code)).toBeVisible();
    await expect(page.getByText(contactName)).toBeVisible();
  });

  test("keyboard-only flow reaches save and surfaces associated errors", async ({ page }) => {
    await signIn(page);
    await gotoCreate(page);

    await page.locator("input#displayNameTh").focus();
    await page.keyboard.press("Tab");
    // Walk focus forward until the save button is focused, then submit with Enter
    for (let i = 0; i < 30; i += 1) {
      const focused = await page.evaluate(() => document.activeElement?.textContent);
      if (focused?.includes("บันทึกข้อมูลลูกค้า")) {
        break;
      }
      await page.keyboard.press("Tab");
    }
    await expect(page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" })).toBeFocused();
    await page.keyboard.press("Enter");
    // Validation errors are exposed with role=alert and aria-invalid association
    await expect(page.getByRole("alert").first()).toBeVisible();
    const invalidCount = await page.locator('[aria-invalid="true"]').count();
    expect(invalidCount).toBeGreaterThan(0);
  });

  test("duplicate candidate path shows masked value without auto-merge", async ({ page }) => {
    await signIn(page);
    await gotoCreate(page);

    const suffix = Date.now().toString().slice(-4);
    const sharedPhone = `089998${suffix}`;
    const firstName = `บริษัท ต้นฉบับซ้ำ TEST_ONLY ${suffix}`;
    const secondName = `บริษัท ซ้ำซ้อน TEST_ONLY ${suffix}`;

    await fillCreateForm(page, {
      nameTh: firstName,
      contactName: "คุณต้นฉบับ TEST_ONLY",
      phone: sharedPhone,
    });
    const firstResponse = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST",
    );
    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    await firstResponse;
    await page.waitForURL("**/th/customers/*");

    await page.getByRole("link", { name: "กลับหน้ารายการลูกค้า" }).click();
    await page.waitForURL("**/th/customers");
    await page.getByRole("link", { name: "เพิ่มลูกค้าใหม่" }).click();
    await page.waitForURL("**/th/customers/create");

    await fillCreateForm(page, {
      nameTh: secondName,
      contactName: "คุณซ้ำ TEST_ONLY",
      phone: sharedPhone,
    });
    const secondResponse = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST",
    );
    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    const created = await (await secondResponse).json();

    expect(created.duplicateCandidates).toHaveLength(1);
    await expect(page.getByText("รายชื่อที่อาจซ้ำซ้อนในระบบ")).toBeVisible();
    await expect(page.getByRole("link", { name: "ดูข้อมูลลูกค้าที่สร้างแล้ว" })).toBeVisible();
    // Full phone must never leak in the duplicate region.
    await expect(page.getByText(sharedPhone, { exact: false })).toHaveCount(0);
    await expect(page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" })).toBeDisabled();
  });
});
