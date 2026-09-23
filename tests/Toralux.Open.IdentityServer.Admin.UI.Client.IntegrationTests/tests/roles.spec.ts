import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import {
  ensureLoggedInAndOpenRoles,
  findRoleRow,
  openRoleUsersFromList,
} from "./helpers/roles-list";
import { runCreateUpdateAndVerifyRolePersistence } from "./scenarios/role-persistence-flow";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Admin UI Roles", () => {
  test("opens roles list, seeded role detail, and users in role", async ({ page }) => {
    await ensureLoggedInAndOpenRoles(page, credentials);

    const roleLinks = page.locator('table tbody tr td a[href*="/role/"]');
    await expect(roleLinks.first()).toBeVisible();
    expect(await roleLinks.count()).toBeGreaterThan(0);

    const targetRow = await findRoleRow(page, seedData.expectedRoleName);
    await targetRow
      .getByRole("link", { name: seedData.expectedRoleName, exact: true })
      .click();

    await expect(page).toHaveURL(/\/role\/[^/?#]+(?:[/?#]|$)/i);
    await expect(page.locator('input[name="name"]').first()).toHaveValue(
      seedData.expectedRoleName,
      {
        timeout: 60_000,
      },
    );

    await openRoleUsersFromList(page, seedData.expectedRoleName, credentials);
    const userLinks = page.locator('table tbody tr td a[href*="/user-profile/"]');
    await expect(userLinks.first()).toBeVisible();
    expect(await userLinks.count()).toBeGreaterThan(0);
  });

  test("creates role, updates all editable fields, and verifies persistence", async ({
    page,
  }) => {
    test.setTimeout(180_000);
    await runCreateUpdateAndVerifyRolePersistence(page, credentials);
  });
});
