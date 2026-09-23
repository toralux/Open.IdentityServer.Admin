import { faker } from "@faker-js/faker";
import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenUsers,
  openUserDetailFromList,
} from "../helpers/users-list";
import {
  expectSwitchByIndex,
  setSwitchByIndex,
} from "../helpers/form-controls";
import { createDebugLogger } from "../helpers/debug-log";
import { clickPageSave, openTabAndWait } from "../helpers/ui-navigation";

async function selectLockoutDate(
  page: Page,
  basicsPanel: Locator,
): Promise<string> {
  const lockoutDateButton = basicsPanel.getByRole("button").first();

  await expect(lockoutDateButton).toBeVisible();
  await lockoutDateButton.click();

  const anyVisibleCalendarDay = page
    .getByRole("gridcell")
    .filter({ hasText: /^\d+$/ })
    .first();
  await expect(anyVisibleCalendarDay).toBeVisible();
  await anyVisibleCalendarDay.click();

  await expect(lockoutDateButton).not.toContainText("Pick a date");
  return ((await lockoutDateButton.textContent()) ?? "").trim();
}

export async function runCreateUpdateAndVerifyUserPersistence(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("user-flow");

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });
  const createdUserName = `ui_user_${marker}`;
  const createdUserEmail = `${createdUserName}@example.local`;

  const updatedUserName = `${createdUserName}_updated`;
  const updatedUserEmail = `${updatedUserName}@example.local`;
  const updatedPhoneNumber = `+420777${faker.string.numeric(6)}`;
  const updatedAccessFailedCount = faker.number.int({ min: 1, max: 25 });
  const updatedLockoutEndTime = "13:45";
  const updatedClaimType = `ui_claim_type_${marker}`;
  const updatedClaimValue = `ui_claim_value_${marker}`;

  const switchValues = {
    emailConfirmed: true,
    phoneNumberConfirmed: true,
    lockoutEnabled: true,
    twoFactorEnabled: true,
  } as const;

  await ensureLoggedInAndOpenUsers(page, credentials);
  logStep("opened users list");

  await page.goto("/user-profile");
  await expect(page).toHaveURL(/\/user-profile(?:[/?#]|$)/i);
  logStep("opened user create form");

  await page.locator('input[name="userName"]').fill(createdUserName);
  await page.locator('input[name="email"]').fill(createdUserEmail);

  await Promise.all([
    page.waitForURL(/\/users(?:[/?#]|$)/i, { timeout: 60_000 }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created user and returned to list");

  await openUserDetailFromList(page, createdUserName, credentials);
  logStep("opened created user detail");

  const basicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(basicsPanel).toBeVisible();

  await page.locator('input[name="userName"]').fill(updatedUserName);
  await page.locator('input[name="email"]').fill(updatedUserEmail);
  await page.locator('input[name="phoneNumber"]').fill(updatedPhoneNumber);
  await basicsPanel
    .locator('input[type="number"]')
    .first()
    .fill(String(updatedAccessFailedCount));
  await basicsPanel
    .getByRole("textbox", { name: /Lockout End Time/i })
    .first()
    .fill(updatedLockoutEndTime);

  await setSwitchByIndex(basicsPanel, 0, switchValues.emailConfirmed);
  await setSwitchByIndex(basicsPanel, 1, switchValues.phoneNumberConfirmed);
  await setSwitchByIndex(basicsPanel, 2, switchValues.lockoutEnabled);
  await setSwitchByIndex(basicsPanel, 3, switchValues.twoFactorEnabled);

  const updatedLockoutDateLabel = await selectLockoutDate(page, basicsPanel);
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
  logStep("added user claim");

  const rolesPanel = await openTabAndWait(page, /Roles/i);
  await rolesPanel.getByRole("button", { name: /Add/i }).first().click();

  const addRoleDialog = page
    .getByRole("dialog")
    .filter({ has: page.getByRole("combobox") });
  await expect(addRoleDialog).toBeVisible();

  const roleSelect = addRoleDialog.getByRole("combobox").first();
  await roleSelect.click();

  const roleOptions = page.locator('[role="option"]:visible');
  await expect(roleOptions.first()).toBeVisible();
  const selectedRoleName = ((await roleOptions.first().textContent()) ?? "").trim();
  await roleOptions.first().click();

  await addRoleDialog.getByRole("button", { name: /^Add$/i }).click();
  await expect(addRoleDialog).not.toBeVisible();
  await expect(
    rolesPanel.getByRole("cell", { name: selectedRoleName, exact: true }),
  ).toBeVisible();
  logStep(`added user role (${selectedRoleName})`);

  const externalAppsPanel = await openTabAndWait(page, /External/i);
  await expect(externalAppsPanel.locator("table")).toBeVisible();
  logStep("verified external applications tab is accessible");

  const persistedGrantsPanel = await openTabAndWait(page, /Persisted/i);
  await expect(
    persistedGrantsPanel.getByRole("button", { name: /Delete All/i }),
  ).toBeDisabled();
  logStep("verified persisted grants tab is accessible");

  await openTabAndWait(page, /Basics/i);
  await Promise.all([
    page.waitForURL(/\/users(?:[/?#]|$)/i, { timeout: 60_000 }),
    clickPageSave(page),
  ]);
  logStep("saved user updates");

  await openUserDetailFromList(page, updatedUserName, credentials);
  logStep("reopened updated user detail");

  const reopenedBasicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(page.locator('input[name="userName"]')).toHaveValue(updatedUserName);
  await expect(page.locator('input[name="email"]')).toHaveValue(updatedUserEmail);
  await expect(page.locator('input[name="phoneNumber"]')).toHaveValue(
    updatedPhoneNumber,
  );
  await expect(reopenedBasicsPanel.locator('input[type="number"]').first()).toHaveValue(
    String(updatedAccessFailedCount),
  );
  await expect(
    reopenedBasicsPanel
      .getByRole("textbox", { name: /Lockout End Time/i })
      .first(),
  ).toHaveValue(updatedLockoutEndTime);
  await expectSwitchByIndex(
    reopenedBasicsPanel,
    0,
    switchValues.emailConfirmed,
  );
  await expectSwitchByIndex(
    reopenedBasicsPanel,
    1,
    switchValues.phoneNumberConfirmed,
  );
  await expectSwitchByIndex(
    reopenedBasicsPanel,
    2,
    switchValues.lockoutEnabled,
  );
  await expectSwitchByIndex(
    reopenedBasicsPanel,
    3,
    switchValues.twoFactorEnabled,
  );

  const reopenedLockoutDateButton = reopenedBasicsPanel.getByRole("button").first();
  await expect(reopenedLockoutDateButton).toContainText(updatedLockoutDateLabel);
  logStep("verified basics tab");

  const reopenedClaimsPanel = await openTabAndWait(page, /Claims/i);
  await expect(
    reopenedClaimsPanel.getByRole("cell", { name: updatedClaimType }),
  ).toBeVisible();
  await expect(
    reopenedClaimsPanel.getByRole("cell", { name: updatedClaimValue }),
  ).toBeVisible();
  logStep("verified claims tab");

  const reopenedRolesPanel = await openTabAndWait(page, /Roles/i);
  await expect(
    reopenedRolesPanel.getByRole("cell", { name: selectedRoleName, exact: true }),
  ).toBeVisible();
  logStep("verified roles tab");

  const reopenedExternalAppsPanel = await openTabAndWait(page, /External/i);
  await expect(reopenedExternalAppsPanel.locator("table")).toBeVisible();

  const reopenedPersistedGrantsPanel = await openTabAndWait(page, /Persisted/i);
  await expect(
    reopenedPersistedGrantsPanel.getByRole("button", { name: /Delete All/i }),
  ).toBeDisabled();
  logStep("verified external applications and persisted grants tabs");
}
