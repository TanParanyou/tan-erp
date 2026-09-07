import { test, expect } from "@playwright/test";

test.describe("Customer and Contact Management Journey", () => {
  test("complete customer lifecycle: view empty state, create customer, see details and list item", async ({ page }) => {
    // 1. Sign in as foundation test user with customers.read and customers.create permissions
    await page.goto("/th/login");
    const emailInput = page.getByLabel("อีเมล");
    const passwordInput = page.locator("input#password");
    await emailInput.fill("foundation-user@example.test");
    await passwordInput.fill("TestPassword123!");

    const meResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/me") && res.status() === 200
    );
    await page.getByRole("button", { name: "เข้าสู่ระบบ" }).click();
    await meResponsePromise;

    // 2. Navigate to Customers via Shell Navigation
    const customerNavLink = page.getByRole("link", { name: "ข้อมูลลูกค้า" });
    await expect(customerNavLink).toBeVisible();
    await customerNavLink.click();

    await page.waitForURL("**/th/customers");
    await expect(page.getByRole("heading", { name: "ข้อมูลลูกค้า" })).toBeVisible();

    // 3. Click Create Customer button
    const createBtn = page.getByRole("button", { name: "เพิ่มลูกค้าใหม่" });
    await expect(createBtn).toBeVisible();
    await createBtn.click();

    await page.waitForURL("**/th/customers/create");
    await expect(page.getByRole("heading", { name: "เพิ่มลูกค้าใหม่" })).toBeVisible();

    // 4. Fill in customer creation form
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

    // 5. Submit form with POST /api/v1/customers
    const createCustomerResponsePromise = page.waitForResponse(
      (res) => res.url().includes("/api/v1/customers") && res.request().method() === "POST" && res.status() === 201
    );

    await page.getByRole("button", { name: "บันทึกข้อมูลลูกค้า" }).click();
    const createResponse = await createCustomerResponsePromise;
    const responseData = await createResponse.json();

    // 6. Automatically navigates to detail view
    await page.waitForURL(`**/th/customers/${responseData.id}`);
    await expect(page.getByRole("heading", { name: customerNameTh })).toBeVisible();
    await expect(page.getByText(responseData.code)).toBeVisible();
    await expect(page.getByText(contactName)).toBeVisible();
    await expect(page.getByText(contactPhone)).toBeVisible();

    // 7. Back to customer list and verify new customer appears
    await page.getByRole("link", { name: "กลับหน้ารายการลูกค้า" }).click();
    await page.waitForURL("**/th/customers");

    await expect(page.getByText(customerNameTh)).toBeVisible();
    await expect(page.getByText(responseData.code)).toBeVisible();
    await expect(page.getByText(contactName)).toBeVisible();
  });
});
