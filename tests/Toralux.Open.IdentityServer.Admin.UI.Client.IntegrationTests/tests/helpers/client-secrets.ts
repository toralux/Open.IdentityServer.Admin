import { expect, type Locator, type Page } from "@playwright/test";
import { UI_TEXT } from "./ui-texts";

export async function openSecretsTab(page: Page): Promise<Locator> {
  await page.getByRole("tab", { name: "Secrets", exact: true }).click();

  const panel = page.getByRole("tabpanel", { name: "Secrets", exact: true });
  await expect(panel).toBeVisible();
  return panel;
}

export async function openAddSecretDialog(page: Page): Promise<Locator> {
  await page
    .getByRole("button", { name: UI_TEXT.secrets.addSecret, exact: true })
    .click();

  const dialog = page.getByRole("dialog", { name: UI_TEXT.secrets.addSecret });
  await expect(dialog).toBeVisible();
  return dialog;
}

/**
 * Radix renders each select as a `button[role=combobox]` trigger plus a hidden
 * native `select` for form compatibility. The trigger carries no accessible name
 * - a `label[for]` names form controls, not buttons - so selects can only be told
 * apart by their order, and only the button triggers may be counted.
 */
function getSelectTrigger(scope: Locator, index = 0): Locator {
  return scope.locator('button[role="combobox"]').nth(index);
}

async function pickSelectOption(
  page: Page,
  trigger: Locator,
  optionLabel: string,
  exact = true,
): Promise<void> {
  await expect(trigger).toBeVisible();
  await trigger.click();

  // Scoped to the open listbox so the hidden native select's options cannot match.
  await page
    .getByRole("listbox")
    .getByRole("option", { name: optionLabel, exact })
    .click();
  await expect(trigger).toContainText(optionLabel);
}

/** Secret Type is the first select in the secret form. */
export async function selectSecretType(
  page: Page,
  dialog: Locator,
  optionLabel: string,
): Promise<void> {
  await pickSelectOption(page, getSelectTrigger(dialog), optionLabel);
}

export function getSecretValueInput(dialog: Locator): Locator {
  return dialog.locator('input[name="secretValue"]');
}

async function confirmSecretRowDelete(page: Page, row: Locator): Promise<void> {
  await row.getByRole("button", { name: "Delete", exact: true }).click();

  const confirmDialog = page.getByRole("alertdialog");
  await expect(confirmDialog).toBeVisible();
  await confirmDialog
    .getByRole("button", { name: "Delete", exact: true })
    .click();
  await expect(confirmDialog).toBeHidden();
}

/**
 * Removes what an aborted run left behind, so a test that depends on which
 * secret types the client has starts from a clean slate.
 */
export async function deleteSecretRowsByDescriptionPrefix(
  page: Page,
  secretsPanel: Locator,
  descriptionPrefix: string,
): Promise<void> {
  // Every seeded client has a secret, so a row means the table has loaded.
  await expect(secretsPanel.locator("table tbody tr").first()).toBeVisible();

  const staleRows = secretsPanel.locator("table tbody tr", {
    hasText: descriptionPrefix,
  });

  while ((await staleRows.count()) > 0) {
    const remainingCount = (await staleRows.count()) - 1;
    await confirmSecretRowDelete(page, staleRows.first());
    await expect(staleRows).toHaveCount(remainingCount);
  }
}

export async function deleteSecretRowByDescription(
  page: Page,
  secretsPanel: Locator,
  description: string,
): Promise<void> {
  const row = secretsPanel.locator("table tbody tr", { hasText: description });
  await expect(row).toBeVisible();

  await confirmSecretRowDelete(page, row);

  await expect(row).toHaveCount(0);
}
