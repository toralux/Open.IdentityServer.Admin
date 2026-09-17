import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import {
  ensureLoggedInAndOpenIdentityResources,
  findIdentityResourceRow,
} from "./helpers/identity-resources-list";
import { runCreateUpdateAndVerifyIdentityResourcePersistence } from "./scenarios/identity-resource-persistence-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI Identity Resources", () => {
  test("opens identity resources list and seeded resource detail", async ({ page }) => {
    await ensureLoggedInAndOpenIdentityResources(page, credentials);

    const resourceLinks = page.locator(
      'table tbody tr td a[href*="/identity-resource/"]',
    );
    await expect(resourceLinks.first()).toBeVisible();
    expect(await resourceLinks.count()).toBeGreaterThan(0);

    const targetRow = await findIdentityResourceRow(
      page,
      seedData.expectedIdentityResourceName,
    );
    await targetRow
      .getByRole("link", {
        name: seedData.expectedIdentityResourceName,
        exact: true,
      })
      .click();

    await expect(page).toHaveURL(/\/identity-resource\/\d+(?:[/?#]|$)/i);
    await expect(page.locator('input[name="name"]').first()).toHaveValue(
      seedData.expectedIdentityResourceName,
      {
        timeout: 60_000,
      },
    );
  });

  test("creates identity resource, updates all editable fields, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(180_000);
    await runCreateUpdateAndVerifyIdentityResourcePersistence(
      page,
      credentials,
    );
  });
});
