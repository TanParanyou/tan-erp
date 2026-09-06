import test from "node:test";
import assert from "node:assert/strict";
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";
import Ajv from "ajv/dist/2020.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const schemaPath = path.join(__dirname, "survey-baseline-v1.schema.json");
const fixturePath = path.join(__dirname, "SURVEY-BASELINE-v1.json");

test("SURVEY-BASELINE-v1 fixture matches schema and invariants", () => {
  assert.ok(fs.existsSync(schemaPath), "Schema file must exist");
  assert.ok(fs.existsSync(fixturePath), "Fixture file must exist");

  const schema = JSON.parse(fs.readFileSync(schemaPath, "utf8"));
  const fixture = JSON.parse(fs.readFileSync(fixturePath, "utf8"));

  const ajv = new Ajv({ allErrors: true });
  const validate = ajv.compile(schema);
  const valid = validate(fixture);

  assert.ok(valid, `Fixture schema validation failed: ${JSON.stringify(validate.errors)}`);
  assert.equal(fixture.templateCode, "SURVEY-BASELINE-v1");
  assert.equal(fixture.owner, "system");
  assert.equal(fixture.classification, "TEST_ONLY");
  assert.equal(fixture.status, "baseline");
  assert.deepEqual(fixture.requiredSections, ["site", "measurement", "checklist", "evidence"]);
  assert.equal(fixture.businessApproved, false);
});
