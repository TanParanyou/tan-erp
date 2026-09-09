/**
 * Setup TEST_ONLY users in Firebase Auth Emulator.
 * This script runs against the local emulator only (port 9099).
 */

const EMULATOR_HOST = process.env.FIREBASE_AUTH_EMULATOR_HOST || "127.0.0.1:9099";
const PROJECT_ID = process.env.FIREBASE_PROJECT_ID || "tan-erp-test-only";
const BASE_URL = `http://${EMULATOR_HOST}/identitytoolkit.googleapis.com/v1/projects/${PROJECT_ID}/accounts?key=fake-api-key`;

const TEST_USERS = [
  {
    localId: "foundation-user-test-only",
    email: "foundation-user@example.test",
    password: "TestPassword123!",
    displayName: "ผู้ใช้ TEST_ONLY",
    emailVerified: true,
  },
  {
    localId: "nomember-user-test-only",
    email: "nomember-user@example.test",
    password: "TestPassword123!",
    displayName: "Nomember User",
    emailVerified: true,
  },
  {
    localId: "foundation-user-b-test-only",
    email: "foundation-user-b@example.test",
    password: "TestPassword123!",
    displayName: "ผู้ใช้ TEST_ONLY B",
    emailVerified: true,
  },
];

async function seedUsers() {
  console.log(`Seeding test users into Firebase Auth Emulator at ${EMULATOR_HOST}...`);

  for (const user of TEST_USERS) {
    try {
      const response = await fetch(BASE_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer owner",
        },
        body: JSON.stringify(user),
      });

      if (response.ok) {
        console.log(`✓ Created test user: ${user.email} (UID: ${user.localId})`);
      } else {
        const data = await response.json();
        if (data.error?.message?.includes("EMAIL_EXISTS") || data.error?.message?.includes("DUPLICATE_LOCAL_ID")) {
          console.log(`- User ${user.email} already exists.`);
        } else {
          console.error(`✗ Failed to create ${user.email}:`, data);
        }
      }
    } catch (err) {
      console.error(`✗ Error connecting to emulator at ${BASE_URL}:`, err.message);
      process.exit(1);
    }
  }
  console.log("Seeding complete.");
}

seedUsers();
