import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import {
  ensureLoggedInAndOpenClients,
  type LoginCredentials,
} from "./helpers/auth";
import { findClientRow } from "./helpers/client-list";
import { runCreateUpdateAndVerifyClientPersistence } from "./scenarios/client-persistence-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI OIDC login", () => {
  test("logs in via STS and opens client detail from seeded data", async ({
    page,
  }) => {
    await ensureLoggedInAndOpenClients(page, credentials);

    const clientsHeading = page.getByRole("heading", { name: "Clients" });
    await expect(clientsHeading).toBeVisible({ timeout: 60_000 });

    const clientLinks = page.locator('table tbody tr td a[href*="/client/"]');
    await expect(clientLinks.first()).toBeVisible();
    expect(await clientLinks.count()).toBeGreaterThan(0);

    const targetRow = await findClientRow(page, seedData.expectedClientId);
    await targetRow.getByRole("link").first().click();

    await expect(page).toHaveURL(/\/client\/\d+(?:[/?#]|$)/i);
    const clientIdInput = page.locator('input[name="clientId"]').first();
    await expect(clientIdInput).toHaveValue(seedData.expectedClientId, {
      timeout: 60_000,
    });
  });

  test("creates confidential client, updates all editable fields, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(600_000);
    await runCreateUpdateAndVerifyClientPersistence(page, credentials);
  });
});
