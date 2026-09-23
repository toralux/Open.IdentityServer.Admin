import { faker } from "@faker-js/faker";
import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenIdentityProviders,
  openIdentityProviderDetailFromList,
} from "../helpers/identity-providers-list";
import {
  expectSwitchByIndex,
  setSwitchByIndex,
} from "../helpers/form-controls";
import { createDebugLogger } from "../helpers/debug-log";
import { clickPageSave, openTabAndWait } from "../helpers/ui-navigation";

async function addProperty(
  page: Page,
  propertiesPanel: Locator,
  key: string,
  value: string,
) {
  await propertiesPanel.getByRole("button", { name: /Add/i }).first().click();

  const addPropertyDialog = page
    .getByRole("dialog")
    .filter({ has: page.locator("#key") });
  await expect(addPropertyDialog).toBeVisible();

  await addPropertyDialog.locator("#key").fill(key);
  await addPropertyDialog.locator("#value").fill(value);
  await addPropertyDialog
    .getByRole("button", { name: /Add Property|Add/i })
    .first()
    .click();

  await expect(addPropertyDialog).not.toBeVisible();
}

async function removePropertyByKey(propertiesPanel: Locator, key: string) {
  const propertyCell = propertiesPanel
    .getByRole("cell", { name: key, exact: true })
    .first();
  await expect(propertyCell).toBeVisible();
  const propertyRow = propertyCell.locator("xpath=ancestor::tr[1]");
  await propertyRow.getByRole("button").first().click();
}

export async function runCreateUpdateAndVerifyIdentityProviderPersistence(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("identity-provider-flow");

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });

  const createdScheme = `idp_ui_test_${marker}`;
  const createdType = "oidc";
  const createdDisplayName = `Identity Provider ${marker}`;
  const createdPropertyKey = `idp_prop_create_${marker}`;
  const createdPropertyValue = `idp_prop_create_value_${marker}`;

  const updatedScheme = `${createdScheme}_updated`;
  const updatedType = `oidc_${marker}`;
  const updatedDisplayName = `Identity Provider ${marker} Updated`;
  const updatedPropertyKey = `idp_prop_updated_${marker}`;
  const updatedPropertyValue = `idp_prop_updated_value_${marker}`;

  await ensureLoggedInAndOpenIdentityProviders(page, credentials);
  logStep("opened identity providers list");

  await page.goto("/identity-provider");
  await expect(page).toHaveURL(/\/identity-provider(?:[/?#]|$)/i);
  logStep("opened identity provider create form");

  const basicsPanel = page.locator('[role="tabpanel"]:visible').first();
  await expect(basicsPanel).toBeVisible();

  await page.locator('input[name="scheme"]').fill(createdScheme);
  await page.locator('input[name="type"]').fill(createdType);
  await page.locator('input[name="displayName"]').fill(createdDisplayName);
  await setSwitchByIndex(basicsPanel, 0, false);
  logStep("filled create basics");

  const propertiesPanel = await openTabAndWait(page, /Properties/i, {
    useVisiblePanelFallback: true,
  });
  await addProperty(page, propertiesPanel, createdPropertyKey, createdPropertyValue);
  await expect(
    propertiesPanel.getByRole("cell", { name: createdPropertyKey }),
  ).toBeVisible();
  await expect(
    propertiesPanel.getByRole("cell", { name: createdPropertyValue }),
  ).toBeVisible();
  logStep("added create property");

  await Promise.all([
    page.waitForURL(/\/identity-provider\/\d+(?:[/?#]|$)/i, {
      timeout: 60_000,
    }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created identity provider and navigated to edit");

  const editBasicsPanel = page.locator('[role="tabpanel"]:visible').first();
  await expect(editBasicsPanel).toBeVisible();

  await page.locator('input[name="scheme"]').fill(updatedScheme);
  await page.locator('input[name="type"]').fill(updatedType);
  await page.locator('input[name="displayName"]').fill(updatedDisplayName);
  await setSwitchByIndex(editBasicsPanel, 0, true);
  logStep("updated basics");

  const editPropertiesPanel = await openTabAndWait(page, /Properties/i, {
    useVisiblePanelFallback: true,
  });
  await expect(
    editPropertiesPanel.getByRole("cell", { name: createdPropertyKey }),
  ).toBeVisible();

  await addProperty(page, editPropertiesPanel, updatedPropertyKey, updatedPropertyValue);
  await removePropertyByKey(editPropertiesPanel, createdPropertyKey);

  await expect(
    editPropertiesPanel.getByRole("cell", { name: updatedPropertyKey }),
  ).toBeVisible();
  await expect(
    editPropertiesPanel.getByRole("cell", { name: updatedPropertyValue }),
  ).toBeVisible();
  await expect(
    editPropertiesPanel.getByRole("cell", { name: createdPropertyKey }),
  ).toHaveCount(0);
  logStep("updated properties tab (add + remove)");

  await Promise.all([
    page.waitForURL(/\/identity-providers(?:[/?#]|$)/i, { timeout: 60_000 }),
    clickPageSave(page),
  ]);
  logStep("saved identity provider updates");

  await openIdentityProviderDetailFromList(page, updatedScheme, credentials);
  logStep("reopened updated identity provider detail");

  const reopenedBasicsPanel = page.locator('[role="tabpanel"]:visible').first();
  await expect(reopenedBasicsPanel).toBeVisible();
  await expect(page.locator('input[name="scheme"]')).toHaveValue(updatedScheme);
  await expect(page.locator('input[name="type"]')).toHaveValue(updatedType);
  await expect(page.locator('input[name="displayName"]')).toHaveValue(
    updatedDisplayName,
  );
  await expectSwitchByIndex(reopenedBasicsPanel, 0, true);
  logStep("verified basics tab");

  const reopenedPropertiesPanel = await openTabAndWait(page, /Properties/i, {
    useVisiblePanelFallback: true,
  });
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: updatedPropertyKey }),
  ).toBeVisible();
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: updatedPropertyValue }),
  ).toBeVisible();
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: createdPropertyKey }),
  ).toHaveCount(0);
  logStep("verified properties persistence");
}
