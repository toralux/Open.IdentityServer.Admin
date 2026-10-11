import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import {
  ensureLoggedInAndOpenApiScopes,
  findApiScopeRow,
} from "./helpers/api-scopes-list";
import { runCreateUpdateAndVerifyApiScopePersistence } from "./scenarios/api-scope-persistence-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI API Scopes", () => {
  test("opens API scopes list and seeded scope detail", async ({ page }) => {
    await ensureLoggedInAndOpenApiScopes(page, credentials);

    const scopeLinks = page.locator('table tbody tr td a[href*="/api-scope/"]');
    await expect(scopeLinks.first()).toBeVisible();
    expect(await scopeLinks.count()).toBeGreaterThan(0);

    const targetRow = await findApiScopeRow(page, seedData.expectedApiScopeName);
    await targetRow
      .getByRole("link", { name: seedData.expectedApiScopeName, exact: true })
      .click();

    await expect(page).toHaveURL(/\/api-scope\/\d+(?:[/?#]|$)/i);
    await expect(page.locator('input[name="name"]').first()).toHaveValue(
      seedData.expectedApiScopeName,
      {
        timeout: 60_000,
      },
    );
  });

  test("creates API scope, updates all editable fields, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(180_000);
    await runCreateUpdateAndVerifyApiScopePersistence(page, credentials);
  });
});
