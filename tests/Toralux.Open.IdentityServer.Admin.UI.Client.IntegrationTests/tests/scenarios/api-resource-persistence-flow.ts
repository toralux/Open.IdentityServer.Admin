import { faker } from "@faker-js/faker";
import { expect, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenApiResources,
  openApiResourceDetailFromList,
} from "../helpers/api-resources-list";
import {
  addInputWithTableItemByLabel,
  expectInputWithTableHasItem,
  expectSwitchByIndex,
  getDualListSelectedRowCount,
  setDualListToAllSelected,
  setSwitchByIndex,
} from "../helpers/form-controls";
import { createDebugLogger } from "../helpers/debug-log";
import { clickPageSave, openTabAndWait } from "../helpers/ui-navigation";

export async function runCreateUpdateAndVerifyApiResourcePersistence(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("api-resource-flow");

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });

  const createdApiResourceName = `api_resource_ui_test_${marker}`;

  const updatedApiResourceName = `${createdApiResourceName}_updated`;
  const updatedDisplayName = `API Resource ${marker} Updated`;
  const updatedDescription = `Updated API resource description ${marker}`;
  const updatedSigningAlgorithm = "RS256";
  const updatedSecretValue = `ApiResourceSecret_${marker}_A1!`;
  const updatedSecretDescription = `Api resource secret ${marker}`;
  const updatedPropertyKey = `api_resource_property_${marker}`;
  const updatedPropertyValue = `api_resource_property_value_${marker}`;

  const switchValues = {
    enabled: false,
    showInDiscoveryDocument: false,
    requireResourceIndicator: true,
  } as const;

  await ensureLoggedInAndOpenApiResources(page, credentials);
  logStep("opened API resources list");

  await page.goto("/api-resource");
  await expect(page).toHaveURL(/\/api-resource(?:[/?#]|$)/i);
  logStep("opened API resource create form");

  await expect(page.locator('input[name="name"]')).toBeVisible({
    timeout: 60_000,
  });
  await page.locator('input[name="name"]').fill(createdApiResourceName);

  await Promise.all([
    page.waitForURL(/\/api-resources(?:[/?#]|$)/i, { timeout: 60_000 }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created API resource and returned to list");

  await openApiResourceDetailFromList(page, createdApiResourceName, credentials);
  logStep("opened created API resource detail");

  const basicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(basicsPanel).toBeVisible();

  await page.locator('input[name="name"]').fill(updatedApiResourceName);
  await page.locator('input[name="displayName"]').fill(updatedDisplayName);
  await page.locator('textarea[name="description"]').fill(updatedDescription);

  await setSwitchByIndex(basicsPanel, 0, switchValues.enabled);
  await setSwitchByIndex(basicsPanel, 1, switchValues.showInDiscoveryDocument);
  await setSwitchByIndex(basicsPanel, 2, switchValues.requireResourceIndicator);

  await addInputWithTableItemByLabel(
    basicsPanel,
    "Signing Algorithms",
    updatedSigningAlgorithm,
    "Search",
  );
  await expectInputWithTableHasItem(
    basicsPanel,
    "Signing Algorithms",
    updatedSigningAlgorithm,
  );
  logStep("updated basics tab");

  const scopesPanel = await openTabAndWait(page, /Scopes/i);
  await setDualListToAllSelected(scopesPanel, "Allowed Scopes");
  const selectedScopesCount = await getDualListSelectedRowCount(scopesPanel);
  expect(selectedScopesCount).toBeGreaterThan(0);
  logStep(`updated scopes tab (${selectedScopesCount} selected)`);

  const userClaimsPanel = await openTabAndWait(page, /User\s*Claims/i);
  await setDualListToAllSelected(userClaimsPanel, "User Claims");
  const selectedUserClaimsCount =
    await getDualListSelectedRowCount(userClaimsPanel);
  expect(selectedUserClaimsCount).toBeGreaterThan(0);
  logStep(`updated user claims tab (${selectedUserClaimsCount} selected)`);

  const secretsPanel = await openTabAndWait(page, /Secrets/i);
  await secretsPanel.getByRole("button", { name: /Add/i }).first().click();

  const addSecretDialog = page
    .getByRole("dialog")
    .filter({ has: page.locator('input[name="secretValue"]') });
  await expect(addSecretDialog).toBeVisible();

  await addSecretDialog
    .locator('input[name="secretValue"]')
    .fill(updatedSecretValue);
  await addSecretDialog
    .locator('textarea[name="secretDescription"]')
    .fill(updatedSecretDescription);
  await addSecretDialog.getByRole("button", { name: /^Save$/i }).click();

  await expect(addSecretDialog).not.toBeVisible();
  await expect(
    secretsPanel.getByRole("cell", { name: updatedSecretDescription }),
  ).toBeVisible();
  logStep("added secret");

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
    page.waitForURL(/\/api-resources(?:[/?#]|$)/i, { timeout: 60_000 }),
    clickPageSave(page),
  ]);
  logStep("saved API resource updates");

  await openApiResourceDetailFromList(page, updatedApiResourceName, credentials);
  logStep("reopened updated API resource detail");

  const reopenedBasicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(page.locator('input[name="name"]')).toHaveValue(
    updatedApiResourceName,
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
  await expectSwitchByIndex(
    reopenedBasicsPanel,
    2,
    switchValues.requireResourceIndicator,
  );
  await expectInputWithTableHasItem(
    reopenedBasicsPanel,
    "Signing Algorithms",
    updatedSigningAlgorithm,
  );
  logStep("verified basics tab");

  const reopenedScopesPanel = await openTabAndWait(page, /Scopes/i);
  const reopenedScopesCount = await getDualListSelectedRowCount(reopenedScopesPanel);
  expect(reopenedScopesCount).toBe(selectedScopesCount);
  logStep("verified scopes tab");

  const reopenedUserClaimsPanel = await openTabAndWait(page, /User\s*Claims/i);
  const reopenedUserClaimsCount =
    await getDualListSelectedRowCount(reopenedUserClaimsPanel);
  expect(reopenedUserClaimsCount).toBe(selectedUserClaimsCount);
  logStep("verified user claims tab");

  const reopenedSecretsPanel = await openTabAndWait(page, /Secrets/i);
  await expect(
    reopenedSecretsPanel.getByRole("cell", { name: updatedSecretDescription }),
  ).toBeVisible();
  logStep("verified secrets tab");

  const reopenedPropertiesPanel = await openTabAndWait(page, /Properties/i);
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: updatedPropertyKey }),
  ).toBeVisible();
  await expect(
    reopenedPropertiesPanel.getByRole("cell", { name: updatedPropertyValue }),
  ).toBeVisible();
  logStep("verified properties tab");
}
