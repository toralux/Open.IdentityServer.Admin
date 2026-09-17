import { faker } from "@faker-js/faker";
import { expect, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenRoles,
  openRoleDetailFromList,
  openRoleUsersFromList,
} from "../helpers/roles-list";
import { createDebugLogger } from "../helpers/debug-log";
import { clickPageSave, openTabAndWait } from "../helpers/ui-navigation";

export async function runCreateUpdateAndVerifyRolePersistence(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("role-flow");

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });
  const createdRoleName = `role_ui_test_${marker}`;
  const updatedRoleName = `${createdRoleName}_updated`;
  const updatedClaimType = `role_claim_type_${marker}`;
  const updatedClaimValue = `role_claim_value_${marker}`;

  await ensureLoggedInAndOpenRoles(page, credentials);
  logStep("opened roles list");

  await page.goto("/role");
  await expect(page).toHaveURL(/\/role(?:[/?#]|$)/i);
  logStep("opened role create form");

  await expect(page.locator('input[name="name"]')).toBeVisible({
    timeout: 60_000,
  });
  await page.locator('input[name="name"]').fill(createdRoleName);

  await Promise.all([
    page.waitForURL(/\/roles(?:[/?#]|$)/i, { timeout: 60_000 }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created role and returned to list");

  await openRoleDetailFromList(page, createdRoleName, credentials);
  logStep("opened created role detail");

  const basicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(basicsPanel).toBeVisible();
  await page.locator('input[name="name"]').fill(updatedRoleName);
  logStep("updated basics tab");

  const claimsPanel = await openTabAndWait(page, /Claims/i);
  await claimsPanel.getByRole("button", { name: /Add/i }).first().click();

  const addClaimDialog = page
    .getByRole("dialog")
    .filter({ has: page.locator("#value") });
  await expect(addClaimDialog).toBeVisible();

  await addClaimDialog.locator('input[type="text"]').first().fill(updatedClaimType);
  await addClaimDialog.locator("#value").fill(updatedClaimValue);
  await addClaimDialog
    .getByRole("button", { name: /Add Claim|Add/i })
    .first()
    .click();

  await expect(addClaimDialog).not.toBeVisible();
  await expect(
    claimsPanel.getByRole("cell", { name: updatedClaimType }),
  ).toBeVisible();
  await expect(
    claimsPanel.getByRole("cell", { name: updatedClaimValue }),
  ).toBeVisible();
  logStep("added role claim");

  await openTabAndWait(page, /Basics/i);
  await Promise.all([
    page.waitForURL(/\/roles(?:[/?#]|$)/i, { timeout: 60_000 }),
    clickPageSave(page),
  ]);
  logStep("saved role updates");

  await openRoleDetailFromList(page, updatedRoleName, credentials);
  logStep("reopened updated role detail");

  const reopenedBasicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(reopenedBasicsPanel).toBeVisible();
  await expect(page.locator('input[name="name"]')).toHaveValue(updatedRoleName);
  logStep("verified basics tab");

  const reopenedClaimsPanel = await openTabAndWait(page, /Claims/i);
  await expect(
    reopenedClaimsPanel.getByRole("cell", { name: updatedClaimType }),
  ).toBeVisible();
  await expect(
    reopenedClaimsPanel.getByRole("cell", { name: updatedClaimValue }),
  ).toBeVisible();
  logStep("verified claims tab");

  await openRoleUsersFromList(page, updatedRoleName, credentials);
  await expect(page.locator("table")).toBeVisible();
  logStep("verified users-in-role page is accessible");
}
