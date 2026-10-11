import { faker } from "@faker-js/faker";
import { expect, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenIdentityResources,
  openIdentityResourceDetailFromList,
} from "../helpers/identity-resources-list";
import {
  expectSwitchByIndex,
  getDualListSelectedRowCount,
  setDualListToAllSelected,
  setSwitchByIndex,
} from "../helpers/form-controls";
import { createDebugLogger } from "../helpers/debug-log";
import { clickPageSave, openTabAndWait } from "../helpers/ui-navigation";

export async function runCreateUpdateAndVerifyIdentityResourcePersistence(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("identity-resource-flow");

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });

  const createdIdentityResourceName = `identity_resource_ui_test_${marker}`;

  const updatedIdentityResourceName = `${createdIdentityResourceName}_updated`;
  const updatedDisplayName = `Identity Resource ${marker} Updated`;
  const updatedDescription = `Updated identity resource description ${marker}`;
  const updatedPropertyKey = `identity_resource_property_${marker}`;
  const updatedPropertyValue = `identity_resource_property_value_${marker}`;

  const switchValues = {
    enabled: false,
    showInDiscoveryDocument: false,
    required: true,
    emphasize: true,
  } as const;

  await ensureLoggedInAndOpenIdentityResources(page, credentials);
  logStep("opened identity resources list");

  await page.goto("/identity-resource");
  await expect(page).toHaveURL(/\/identity-resource(?:[/?#]|$)/i);
  logStep("opened identity resource create form");

  await expect(page.locator('input[name="name"]')).toBeVisible({
    timeout: 60_000,
  });
  await page.locator('input[name="name"]').fill(createdIdentityResourceName);

  await Promise.all([
    page.waitForURL(/\/identity-resources(?:[/?#]|$)/i, { timeout: 60_000 }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created identity resource and returned to list");

  await openIdentityResourceDetailFromList(
    page,
    createdIdentityResourceName,
    credentials,
  );
  logStep("opened created identity resource detail");

  const basicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(basicsPanel).toBeVisible();

  await page.locator('input[name="name"]').fill(updatedIdentityResourceName);
  await page.locator('input[name="displayName"]').fill(updatedDisplayName);
  await page.locator('textarea[name="description"]').fill(updatedDescription);

  await setSwitchByIndex(basicsPanel, 0, switchValues.enabled);
  await setSwitchByIndex(
    basicsPanel,
    1,
    switchValues.showInDiscoveryDocument,
  );
  await setSwitchByIndex(basicsPanel, 2, switchValues.required);
  await setSwitchByIndex(basicsPanel, 3, switchValues.emphasize);
  logStep("updated basics tab");

  const userClaimsPanel = await openTabAndWait(page, /User\s*Claims/i);
  await setDualListToAllSelected(userClaimsPanel, "User Claims");
  const selectedUserClaimsCount =
    await getDualListSelectedRowCount(userClaimsPanel);
  expect(selectedUserClaimsCount).toBeGreaterThan(0);
  logStep(`updated user claims tab (${selectedUserClaimsCount} selected)`);

  const propertiesPanel = await openTabAndWait(page, /Properties/i);
  await propertiesPanel.getByRole("button", { name: /Add/i }).first().click();

  const addPropertyDialog = page
    .getByRole("dialog")
    .filter({ has: page.locator("#key") });
  await expect(addPropertyDialog).toBeVisible();

  await addPropertyDialog.locator("#key").fill(updatedPropertyKey);
  await addPropertyDialog.locator("#value").fill(updatedPropertyValue);
  await addPropertyDialog.getByRole("button", { name: /Add/i }).click();

  await expect(addPropertyDialog).not.toBeVisible();
  await expect(
    propertiesPanel.getByRole("cell", { name: updatedPropertyKey }),
  ).toBeVisible();
  await expect(
    propertiesPanel.getByRole("cell", { name: updatedPropertyValue }),
  ).toBeVisible();
  logStep("added property");

  await Promise.all([
    page.waitForURL(/\/identity-resources(?:[/?#]|$)/i, { timeout: 60_000 }),
    clickPageSave(page),
  ]);
  logStep("saved identity resource updates");

  await openIdentityResourceDetailFromList(
    page,
    updatedIdentityResourceName,
    credentials,
  );
  logStep("reopened updated identity resource detail");

  const reopenedBasicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(page.locator('input[name="name"]')).toHaveValue(
    updatedIdentityResourceName,
  );
  await expect(page.locator('input[name="displayName"]')).toHaveValue(
    updatedDisplayName,
  );
  await expect(page.locator('textarea[name="description"]')).toHaveValue(
    updatedDescription,
  );
  await expectSwitchByIndex(reopenedBasicsPanel, 0, switchValues.enabled);
  await expectSwitchByIndex(
    reopenedBasicsPanel,
    1,
    switchValues.showInDiscoveryDocument,
  );
  await expectSwitchByIndex(reopenedBasicsPanel, 2, switchValues.required);
  await expectSwitchByIndex(reopenedBasicsPanel, 3, switchValues.emphasize);
  logStep("verified basics tab");

  const reopenedUserClaimsPanel = await openTabAndWait(page, /User\s*Claims/i);
  const reopenedUserClaimsCount =
    await getDualListSelectedRowCount(reopenedUserClaimsPanel);
  expect(reopenedUserClaimsCount).toBe(selectedUserClaimsCount);
  logStep("verified user claims tab");

  const reopenedPropertiesPanel = await openTabAndWait(page, /Properties/i);
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: updatedPropertyKey }),
  ).toBeVisible();
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: updatedPropertyValue }),
  ).toBeVisible();
  logStep("verified properties tab");
}
