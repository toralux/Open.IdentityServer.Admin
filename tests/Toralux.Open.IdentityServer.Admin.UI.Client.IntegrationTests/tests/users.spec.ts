import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import { ensureLoggedInAndOpenUsers, findUserRow } from "./helpers/users-list";
import { runCreateUpdateAndVerifyUserPersistence } from "./scenarios/user-persistence-flow";
import { runChangePasswordFlow } from "./scenarios/change-password-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI Users", () => {
  test("opens users list and seeded user detail", async ({ page }) => {
    await ensureLoggedInAndOpenUsers(page, credentials);

    const userLinks = page.locator(
      'table tbody tr td a[href*="/user-profile/"]',
    );
    await expect(userLinks.first()).toBeVisible();
    expect(await userLinks.count()).toBeGreaterThan(0);

    const targetRow = await findUserRow(page, seedData.expectedUserName);
    await targetRow
      .getByRole("link", { name: seedData.expectedUserName, exact: true })
      .click();

    await expect(page).toHaveURL(/\/user-profile\/[^/?#]+(?:[/?#]|$)/i);
    await expect(page.locator('input[name="userName"]').first()).toHaveValue(
      seedData.expectedUserName,
      {
        timeout: 60_000,
      },
    );
  });

  test("creates user, updates all editable fields, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(180_000);
    await runCreateUpdateAndVerifyUserPersistence(page, credentials);
  });

  test("change password tab validates and submits correctly", async ({
    page,
  }) => {
    test.setTimeout(120_000);
    await runChangePasswordFlow(page, credentials);
  });
});
