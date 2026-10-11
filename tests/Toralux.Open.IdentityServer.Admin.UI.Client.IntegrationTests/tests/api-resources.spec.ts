import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import {
  ensureLoggedInAndOpenApiResources,
  findApiResourceRow,
} from "./helpers/api-resources-list";
import { runCreateUpdateAndVerifyApiResourcePersistence } from "./scenarios/api-resource-persistence-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI API Resources", () => {
  test("opens API resources list and seeded resource detail", async ({ page }) => {
    await ensureLoggedInAndOpenApiResources(page, credentials);

    const resourceLinks = page.locator('table tbody tr td a[href*="/api-resource/"]');
    await expect(resourceLinks.first()).toBeVisible();
    expect(await resourceLinks.count()).toBeGreaterThan(0);

    const targetRow = await findApiResourceRow(
      page,
      seedData.expectedApiResourceName,
    );
    await targetRow
      .getByRole("link", { name: seedData.expectedApiResourceName, exact: true })
      .click();

    await expect(page).toHaveURL(/\/api-resource\/\d+(?:[/?#]|$)/i);
    await expect(page.locator('input[name="name"]').first()).toHaveValue(
      seedData.expectedApiResourceName,
      {
        timeout: 60_000,
      },
    );
  });

  test("creates API resource, updates all editable fields, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(180_000);
    await runCreateUpdateAndVerifyApiResourcePersistence(page, credentials);
  });
});
