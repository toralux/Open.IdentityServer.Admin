import { faker } from "@faker-js/faker";
import { expect, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenApiScopes,
  openApiScopeDetailFromList,
} from "../helpers/api-scopes-list";
import {
  expectSwitchByIndex,
  getDualListSelectedRowCount,
  setDualListToAllSelected,
  setSwitchByIndex,
} from "../helpers/form-controls";
import { createDebugLogger } from "../helpers/debug-log";
import { clickPageSave, openTabAndWait } from "../helpers/ui-navigation";

export async function runCreateUpdateAndVerifyApiScopePersistence(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("api-scope-flow");

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });

  const createdApiScopeName = `api_scope_ui_test_${marker}`;

  const updatedApiScopeName = `${createdApiScopeName}_updated`;
  const updatedDisplayName = `API Scope ${marker} Updated`;
  const updatedDescription = `Updated API scope description ${marker}`;
  const updatedPropertyKey = `api_scope_property_${marker}`;
  const updatedPropertyValue = `api_scope_property_value_${marker}`;

  const switchValues = {
    enabled: false,
    showInDiscoveryDocument: false,
    required: true,
    emphasize: true,
  } as const;

  await ensureLoggedInAndOpenApiScopes(page, credentials);
  logStep("opened API scopes list");

  await page.goto("/api-scope");
  await expect(page).toHaveURL(/\/api-scope(?:[/?#]|$)/i);
  logStep("opened API scope create form");

  await expect(page.locator('input[name="name"]')).toBeVisible({
    timeout: 60_000,
  });
  await page.locator('input[name="name"]').fill(createdApiScopeName);

  await Promise.all([
    page.waitForURL(/\/api-scopes(?:[/?#]|$)/i, { timeout: 60_000 }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created API scope and returned to list");

  await openApiScopeDetailFromList(page, createdApiScopeName, credentials);
  logStep("opened created API scope detail");

  const basicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(basicsPanel).toBeVisible();

  await page.locator('input[name="name"]').fill(updatedApiScopeName);
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
    page.waitForURL(/\/api-scopes(?:[/?#]|$)/i, { timeout: 60_000 }),
    clickPageSave(page),
  ]);
  logStep("saved API scope updates");

  await openApiScopeDetailFromList(page, updatedApiScopeName, credentials);
  logStep("reopened updated API scope detail");

  const reopenedBasicsPanel = page.getByRole("tabpanel", { name: /Basics/i });
  await expect(page.locator('input[name="name"]')).toHaveValue(
    updatedApiScopeName,
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
