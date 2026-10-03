import { test, expect, type Page } from "@playwright/test";
import type { CurrentUserResponse } from "@/lib/api/api-client";

interface SignedInContext {
  apiOrigin: string;
  authorization: string;
  membershipId: string;
}

interface IdFixture {
  id: string;
}

interface GeneratedItemFixture extends IdFixture {
  code: string;
}

interface HttpResponse {
  ok(): boolean;
  status(): number;
  text(): Promise<string>;
}

interface BarcodeFixture {
  id: string;
  item: { id: string };
  unit: { id: string };
  quantityInBaseUnit: string;
}

interface UploadSessionFixture {
  sessionId: string;
  slots: Array<{ slotId: string }>;
}

interface UploadedFileFixture {
  files: Array<{ fileId: string }>;
}

interface ItemImageFixture {
  fileId: string;
  fileName: string;
}

async function signIn(page: Page): Promise<SignedInContext> {
  await page.goto("/th/login");
  await page.getByLabel("อีเมล").fill("foundation-user@example.test");
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
  const membershipId = body.memberships?.[0]?.id;
  const authorization = request.headers()["authorization"];
  if (!body.user?.id || !membershipId || !authorization) {
    throw new Error("Incomplete Item Master test identity fixture");
  }
  return { apiOrigin: new URL(response.url()).origin, authorization, membershipId };
}

async function expectApiSuccess(response: HttpResponse, operation: string) {
  expect(response.ok(), `${operation}: HTTP ${response.status()} ${await response.text()}`).toBeTruthy();
}

function createGtin13(): string {
  const digits = Array.from(crypto.getRandomValues(new Uint8Array(12)), (digit) => digit % 10);
  const weightedSum = digits.reduce((sum, digit, index) => sum + digit * (index % 2 === 0 ? 1 : 3), 0);
  const checkDigit = (10 - (weightedSum % 10)) % 10;
  return `${digits.join("")}${checkDigit}`;
}

test.describe("Item Master SKU and Barcode", () => {
  test("creates an Item, maintains its GTIN, and remains usable at 320px", async ({ page }) => {
    const auth = await signIn(page);
    const headers = {
      Authorization: auth.authorization,
      "X-Membership-Id": auth.membershipId,
      "Content-Type": "application/json",
    };

    const suffix = crypto.randomUUID().replaceAll("-", "").slice(0, 12).toUpperCase();
    const categoryResponse = await page.request.post(`${auth.apiOrigin}/api/v1/item-categories`, {
      headers: { ...headers, "Idempotency-Key": crypto.randomUUID() },
      data: {
        code: `E2E-CAT-${suffix}`,
        name: { thai: `หมวดทดสอบ ${suffix}`, english: `Test category ${suffix}` },
        allowedItemTypes: ["material"],
        sortOrder: 0,
      },
    });
    await expectApiSuccess(categoryResponse, "create category");
    const category = (await categoryResponse.json()) as IdFixture;

    const unitResponse = await page.request.post(`${auth.apiOrigin}/api/v1/units-of-measure`, {
      headers: { ...headers, "Idempotency-Key": crypto.randomUUID() },
      data: {
        code: `E2E${suffix.slice(0, 6)}`,
        name: { thai: "ชิ้นทดสอบ", english: "Test piece" },
        symbol: "pcs",
        dimension: "Count",
        decimalScale: 0,
        roundingMode: "HalfUp",
      },
    });
    await expectApiSuccess(unitResponse, "create unit");
    const unit = (await unitResponse.json()) as IdFixture;

    await page.goto("/th/item-master/create");
    await page.setViewportSize({ width: 320, height: 800 });
    await expect(page.getByRole("radio", { name: "สร้างรหัสอัตโนมัติ" })).toBeChecked();
    await page.locator("#item-category").selectOption(category.id);
    await page.locator("#item-unit").selectOption(unit.id);
    await page.getByLabel("ชื่อสินค้า").fill(`วัสดุทดสอบ ${suffix}`);
    const createItemResponse = page.waitForResponse(
      (response) => response.url().endsWith("/api/v1/items") && response.request().method() === "POST"
    );
    await page.getByRole("button", { name: "บันทึกสินค้า" }).click();
    const createdItemResponse = await createItemResponse;
    await expectApiSuccess(createdItemResponse, "create Item SKU through the form");
    const item = (await createdItemResponse.json()) as GeneratedItemFixture;
    expect(item.code).toMatch(/^ITM-\d{5}$/);
    await page.waitForURL(`**/th/item-master/${item.id}`);
    const imageHeading = page.getByRole("heading", { name: "รูปภาพสินค้า" });
    const imageSection = page.locator("section").filter({ has: imageHeading });
    await expect(imageSection.getByText("ยังไม่มีรูปภาพ")).toBeVisible();
    const imageListUrl = `${auth.apiOrigin}/api/v1/items/${item.id}/images`;
    await page.route(imageListUrl, (route) => route.fulfill({ status: 503, contentType: "application/json", body: "{}" }));
    await page.reload();
    const reloadedImageSection = page.locator("section").filter({ has: page.getByRole("heading", { name: "รูปภาพสินค้า" }) });
    await expect(reloadedImageSection.getByRole("alert")).toHaveText("โหลดรูปภาพไม่สำเร็จ");
    await expect(reloadedImageSection.getByText("ยังไม่มีรูปภาพ")).toHaveCount(0);
    await page.unroute(imageListUrl);
    await page.reload();
    const availableImageSection = page.locator("section").filter({ has: page.getByRole("heading", { name: "รูปภาพสินค้า" }) });
    await expect(availableImageSection.getByText("ยังไม่มีรูปภาพ")).toBeVisible();
    let uploadSessionRequests = 0;
    page.on("request", (request) => {
      if (request.url().endsWith("/api/v1/files/upload-sessions") && request.method() === "POST") {
        uploadSessionRequests += 1;
      }
    });
    await imageSection.getByLabel("ไฟล์รูปภาพ").setInputFiles({
      name: "item-private.png",
      mimeType: "image/png",
      buffer: Buffer.from("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+jUQAAAABJRU5ErkJggg==", "base64"),
    });
    await imageSection.getByLabel("ข้อความอธิบายรูปภาพ (ไทย)").fill(`ภาพทดสอบ ${suffix}`);
    await expect(imageSection.getByRole("button", { name: "อัปโหลดและบันทึกรูปภาพ" })).toBeEnabled();
    expect(uploadSessionRequests).toBe(0);

    const sessionResponse = page.waitForResponse(
      (response) => response.url().endsWith("/api/v1/files/upload-sessions") && response.request().method() === "POST"
    );
    const completeResponse = page.waitForResponse(
      (response) => /\/api\/v1\/files\/upload-sessions\/[^/]+\/complete$/.test(response.url()) && response.request().method() === "POST"
    );
    const attachResponse = page.waitForResponse(
      (response) => response.url().endsWith(`/api/v1/items/${item.id}/images`) && response.request().method() === "POST"
    );
    await imageSection.getByRole("button", { name: "อัปโหลดและบันทึกรูปภาพ" }).click();
    const [createdSession, completedUpload, attachedImage] = await Promise.all([sessionResponse, completeResponse, attachResponse]);
    await expectApiSuccess(createdSession, "create deferred image upload session");
    await expectApiSuccess(completedUpload, "complete image upload");
    await expectApiSuccess(attachedImage, "attach uploaded private image");
    const session = (await createdSession.json()) as UploadSessionFixture;
    const uploaded = (await completedUpload.json()) as UploadedFileFixture;
    const image = (await attachedImage.json()) as ItemImageFixture;
    expect(session.slots).toHaveLength(1);
    expect(uploaded.files).toHaveLength(1);
    expect(image.fileId).toBe(uploaded.files[0].fileId);
    await expect(imageSection.getByText("item-private.png")).toBeVisible();
    const privateImage = await page.request.get(`${auth.apiOrigin}/api/v1/files/${image.fileId}/content`, { headers });
    await expectApiSuccess(privateImage, "retrieve private image with membership authorization");
    expect(privateImage.headers()["content-type"]).toContain("image/png");
    const anonymousImage = await page.request.get(`${auth.apiOrigin}/api/v1/files/${image.fileId}/content`);
    expect(anonymousImage.status()).toBe(401);

    const gtin = createGtin13();

    const barcodeHeading = page.getByRole("heading", { name: "SKU และบาร์โค้ด" });
    const barcodeSection = page.locator("section").filter({ has: barcodeHeading });
    await expect(barcodeHeading).toBeVisible();
    await expect(barcodeSection).toBeVisible();
    await page.setViewportSize({ width: 320, height: 800 });
    await expect(barcodeSection).toBeVisible();

    await page.getByLabel("ค่า Barcode / GTIN").fill(gtin);
    await page.locator("#barcode-packaging").selectOption("each");
    await page.getByLabel("รหัสหลักของระดับนี้").check();
    const createBarcodeResponse = page.waitForResponse(
      (response) => response.url().includes(`/api/v1/items/${item.id}/barcodes`) && response.request().method() === "POST"
    );
    await page.getByRole("button", { name: "เพิ่ม Barcode" }).click();
    const createdBarcodeResponse = await createBarcodeResponse;
    await expectApiSuccess(createdBarcodeResponse, "create primary GTIN");
    const createdBarcode = (await createdBarcodeResponse.json()) as BarcodeFixture;
    expect(createdBarcode.item.id).toBe(item.id);
    expect(createdBarcode.unit.id).toBe(unit.id);
    expect(createdBarcode.quantityInBaseUnit).toBe("1.0000");
    await expect(barcodeSection.getByText(gtin)).toBeVisible();
    const activateItemResponse = page.waitForResponse(
      (response) => response.url().endsWith(`/api/v1/items/${item.id}/activate`) && response.request().method() === "POST"
    );
    await page.getByRole("button", { name: "เปิดใช้งาน" }).click();
    await expectApiSuccess(await activateItemResponse, "activate Item with its barcode");
    const scanResponse = await page.request.get(`${auth.apiOrigin}/api/v1/items/by-barcode?value=${gtin}`, { headers });
    await expectApiSuccess(scanResponse, "scan GTIN");
    const scannedBarcode = (await scanResponse.json()) as BarcodeFixture;
    expect(scannedBarcode.id).toBe(createdBarcode.id);
    expect(scannedBarcode.quantityInBaseUnit).toBe("1.0000");

    await page.getByLabel("ค่า Barcode / GTIN").focus();
    await page.keyboard.press("Tab");
    await expect(page.locator("#barcode-unit")).toBeFocused();

    const barcodeRow = barcodeSection.locator("tbody tr").filter({ hasText: gtin });
    await barcodeRow.getByRole("button", { name: "ปิดใช้งาน" }).click();
    const confirmDialog = page.getByRole("dialog");
    await expect(confirmDialog.getByRole("heading", { name: "ยืนยันการปิดใช้งาน Barcode" })).toBeVisible();
    await confirmDialog.getByRole("button", { name: "ยกเลิก" }).click();
    await expect(confirmDialog).not.toBeVisible();

    await barcodeRow.getByRole("button", { name: "ปิดใช้งาน" }).click();
    const deactivateResponse = page.waitForResponse(
      (response) => response.url().includes(`/api/v1/items/${item.id}/barcodes/`) && response.url().endsWith("/deactivate")
    );
    await confirmDialog.getByRole("button", { name: "ปิดใช้งาน" }).click();
    const deactivatedResponse = await deactivateResponse;
    await expectApiSuccess(deactivatedResponse, "deactivate GTIN after confirmation");
    await expect(barcodeRow.getByText("ปิดใช้งาน", { exact: true })).toBeVisible();
    const inactiveScan = await page.request.get(`${auth.apiOrigin}/api/v1/items/by-barcode?value=${gtin}`, { headers });
    expect(inactiveScan.status()).toBe(404);

    await page.goto(`/en/item-master/${item.id}`);
    await expect(page.getByRole("heading", { name: "SKU and barcodes" })).toBeVisible();
    await expect(page.getByText(gtin)).toBeVisible();
  });
});
