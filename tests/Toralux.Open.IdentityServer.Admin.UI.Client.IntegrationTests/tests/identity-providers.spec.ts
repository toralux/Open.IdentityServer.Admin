import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import {
  ensureLoggedInAndOpenIdentityProviders,
  findIdentityProviderRow,
} from "./helpers/identity-providers-list";
import { runCreateUpdateAndVerifyIdentityProviderPersistence } from "./scenarios/identity-provider-persistence-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI Identity Providers", () => {
  test("opens identity providers list and provider detail when available", async ({
    page,
  }) => {
    await ensureLoggedInAndOpenIdentityProviders(page, credentials);

    const expectedScheme = process.env.E2E_EXPECTED_IDENTITY_PROVIDER_SCHEME;
    if (expectedScheme) {
      const targetRow = await findIdentityProviderRow(page, expectedScheme);
      await targetRow
        .getByRole("link", { name: expectedScheme, exact: true })
        .click();
      await expect(page.locator('input[name="scheme"]').first()).toHaveValue(
        expectedScheme,
      );
    } else {
      const providerLinks = page.locator(
        'table tbody tr td a[href*="/identity-provider/"]',
      );
      const timeoutAt = Date.now() + 30_000;
      let providersCount = await providerLinks.count();

      while (providersCount === 0 && Date.now() < timeoutAt) {
        await page.waitForTimeout(250);
        providersCount = await providerLinks.count();
      }

      if (providersCount === 0) {
        return;
      }

      const firstScheme = ((await providerLinks.first().textContent()) ?? "").trim();
      await providerLinks.first().click();
      await expect(page.locator('input[name="scheme"]').first()).toHaveValue(
        firstScheme,
      );
    }

    await expect(page).toHaveURL(/\/identity-provider\/\d+(?:[/?#]|$)/i);
  });

  test("creates identity provider, updates basics and properties, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(180_000);
    await runCreateUpdateAndVerifyIdentityProviderPersistence(page, credentials);
  });
});
