import { faker } from "@faker-js/faker";
import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenConfigurationRules,
  findConfigurationRuleRowByType,
  findConfigurationRuleRowsByType,
} from "../helpers/configuration-rules-list";
import {
  addInputWithTableItemByLabel,
  expectSwitchByIndex,
  setSwitchByIndex,
} from "../helpers/form-controls";
import { createDebugLogger } from "../helpers/debug-log";
import { UI_TEXT } from "../helpers/ui-texts";

type RuleDefinition = {
  ruleType: string;
  optionDisplayName: string;
  resourceType: string;
  issueType: "Warning" | "Recommendation" | "Error";
  messageTemplate: string;
  fixDescription: string;
  fillParameters: (dialog: Locator) => Promise<void>;
  assertParameters: (dialog: Locator) => Promise<void>;
};

function escapeForRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function getRowActionButtons(row: Locator): Locator {
  return row.locator("td").last().getByRole("button");
}

async function openCreateRuleDialog(page: Page): Promise<Locator> {
  await page
    .getByRole("button", { name: new RegExp(UI_TEXT.configurationRules.addNewRule, "i") })
    .click();
  const dialog = page.getByRole("dialog");
  await expect(
    dialog.getByText(UI_TEXT.configurationRules.addNewRule, { exact: true }),
  ).toBeVisible();
  return dialog;
}

async function closeDialog(dialog: Locator): Promise<void> {
  await dialog
    .getByRole("button", { name: new RegExp(UI_TEXT.actions.close, "i") })
    .click();
  await expect(dialog).not.toBeVisible();
}

async function selectRuleType(
  page: Page,
  dialog: Locator,
  optionDisplayName: string,
  resourceType: string,
): Promise<void> {
  const ruleTypeCombobox = dialog.getByRole("combobox").first();
  await ruleTypeCombobox.click();

  const optionPattern = new RegExp(
    `^${escapeForRegex(optionDisplayName)}\\s*\\(${escapeForRegex(resourceType)}\\)$`,
    "i",
  );

  await page.getByRole("option", { name: optionPattern }).click();
  await expect(ruleTypeCombobox).toContainText(optionDisplayName);
}

async function selectIssueType(
  page: Page,
  dialog: Locator,
  issueType: "Warning" | "Recommendation" | "Error",
  isEditMode: boolean,
): Promise<void> {
  const issueTypeCombobox = dialog.getByRole("combobox").nth(isEditMode ? 0 : 1);
  await issueTypeCombobox.click();
  await page.getByRole("option", { name: issueType, exact: true }).click();
  await expect(issueTypeCombobox).toContainText(issueType);
}

async function expectIssueType(
  dialog: Locator,
  issueType: "Warning" | "Recommendation" | "Error",
): Promise<void> {
  await expect(dialog.getByRole("combobox").first()).toContainText(issueType);
}

async function deleteRuleByTypeIfPresent(
  page: Page,
  ruleType: string,
): Promise<void> {
  const rows = findConfigurationRuleRowsByType(page, ruleType);

  while ((await rows.count()) > 0) {
    const existingCount = await rows.count();
    const firstRow = rows.first();
    const actionButtons = getRowActionButtons(firstRow);

    page.once("dialog", (dialog) => {
      void dialog.accept();
    });

    await actionButtons.nth(1).click();
    await expect(rows).toHaveCount(existingCount - 1, {
      timeout: 60_000,
    });
  }
}

async function openEditRuleDialog(
  page: Page,
  ruleType: string,
): Promise<Locator> {
  const row = await findConfigurationRuleRowByType(page, ruleType);
  const actionButtons = getRowActionButtons(row);
  await actionButtons.first().click();

  const dialog = page.getByRole("dialog");
  await expect(
    dialog.getByText(UI_TEXT.configurationRules.editRule, { exact: true }),
  ).toBeVisible();
  return dialog;
}

async function createRule(
  page: Page,
  definition: RuleDefinition,
  allowRetryOnDuplicate: boolean = true,
): Promise<void> {
  const dialog = await openCreateRuleDialog(page);
  await selectRuleType(
    page,
    dialog,
    definition.optionDisplayName,
    definition.resourceType,
  );

  await selectIssueType(page, dialog, definition.issueType, false);
  await dialog.locator('input[name="messageTemplate"]').fill(definition.messageTemplate);
  await dialog.locator('textarea[name="fixDescription"]').fill(definition.fixDescription);
  await definition.fillParameters(dialog);

  const duplicateError = dialog.getByText(
    UI_TEXT.configurationRules.duplicateRuleError,
    { exact: true },
  );
  if ((await duplicateError.count()) > 0) {
    await closeDialog(dialog);

    if (!allowRetryOnDuplicate) {
      throw new Error(
        `Rule '${definition.ruleType}' is still duplicate after cleanup attempt.`,
      );
    }

    await deleteRuleByTypeIfPresent(page, definition.ruleType);
    await createRule(page, definition, false);
    return;
  }

  const saveButton = dialog.locator('button[type="submit"]').first();
  await expect(saveButton).toBeEnabled();
  await saveButton.click();
  await expect(dialog).not.toBeVisible();
}

async function verifyRuleRow(page: Page, ruleType: string): Promise<void> {
  const row = await findConfigurationRuleRowByType(page, ruleType);
  await expect(row.getByRole("switch").first()).toHaveAttribute(
    "data-state",
    "checked",
  );
}

async function verifySavedRuleInEditDialog(
  page: Page,
  definition: RuleDefinition,
): Promise<void> {
  const dialog = await openEditRuleDialog(page, definition.ruleType);

  await expect(dialog.locator('input[name="messageTemplate"]')).toHaveValue(
    definition.messageTemplate,
  );
  await expect(dialog.locator('textarea[name="fixDescription"]')).toHaveValue(
    definition.fixDescription,
  );

  await expectIssueType(dialog, definition.issueType);
  await definition.assertParameters(dialog);
  await closeDialog(dialog);
}

async function verifyDuplicateRuleTypePrevention(
  page: Page,
  definition: RuleDefinition,
): Promise<void> {
  const dialog = await openCreateRuleDialog(page);
  await selectRuleType(
    page,
    dialog,
    definition.optionDisplayName,
    definition.resourceType,
  );

  await expect(
    dialog.getByText(
      UI_TEXT.configurationRules.duplicateRuleError,
      { exact: true },
    ),
  ).toBeVisible();

  const saveButton = dialog.locator('button[type="submit"]').first();
  await expect(saveButton).toBeDisabled();
  await closeDialog(dialog);

  const ruleRows = findConfigurationRuleRowsByType(page, definition.ruleType);
  await expect(ruleRows).toHaveCount(1);
}

export async function runCreateAndVerifyConfigurationRulesFlow(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("configuration-rules-flow");

  const marker = faker.string.alphanumeric({ length: 8, casing: "lower" });

  const maxLifetimeSeconds = faker.number.int({ min: 1200, max: 5400 });
  const requiredPrefixA = `scope_${marker}_`;
  const requiredPrefixB = `api_${marker}_`;

  const definitions: RuleDefinition[] = [
    {
      ruleType: "ClientRedirectUrisMustUseHttps",
      optionDisplayName: "Client Redirect URIs Must Use HTTPS",
      resourceType: "Client",
      issueType: "Warning",
      messageTemplate: `UI test message ${marker} (https)`,
      fixDescription: `UI test fix ${marker} (https)`,
      fillParameters: async (dialog) => {
        await setSwitchByIndex(dialog, 1, false);
      },
      assertParameters: async (dialog) => {
        await expectSwitchByIndex(dialog, 1, false);
      },
    },
    {
      ruleType: "ClientAccessTokenLifetimeTooLong",
      optionDisplayName: "Client Access Token Lifetime Too Long",
      resourceType: "Client",
      issueType: "Error",
      messageTemplate: `UI test message ${marker} (access-token)`,
      fixDescription: `UI test fix ${marker} (access-token)`,
      fillParameters: async (dialog) => {
        await dialog.getByRole("spinbutton").first().fill(String(maxLifetimeSeconds));
      },
      assertParameters: async (dialog) => {
        await expect(dialog.getByRole("spinbutton").first()).toHaveValue(
          String(maxLifetimeSeconds),
        );
      },
    },
    {
      ruleType: "ApiScopeNameMustStartWith",
      optionDisplayName: "API Scope Name Must Start With",
      resourceType: "ApiScope",
      issueType: "Recommendation",
      messageTemplate: `UI test message ${marker} (scope-prefix)`,
      fixDescription: `UI test fix ${marker} (scope-prefix)`,
      fillParameters: async (dialog) => {
        const deleteButtons = dialog.getByRole("button", {
          name: /^Delete$/i,
        });

        while ((await deleteButtons.count()) > 0) {
          await deleteButtons.first().click();
        }

        await addInputWithTableItemByLabel(
          dialog,
          "Required Prefixes",
          requiredPrefixA,
        );
        await addInputWithTableItemByLabel(
          dialog,
          "Required Prefixes",
          requiredPrefixB,
        );
      },
      assertParameters: async (dialog) => {
        await expect(
          dialog.getByRole("cell", { name: requiredPrefixA, exact: true }),
        ).toBeVisible();
        await expect(
          dialog.getByRole("cell", { name: requiredPrefixB, exact: true }),
        ).toBeVisible();
      },
    },
  ];

  await ensureLoggedInAndOpenConfigurationRules(page, credentials);
  logStep("opened configuration rules list");

  for (const definition of definitions) {
    await deleteRuleByTypeIfPresent(page, definition.ruleType);
    logStep(`removed existing '${definition.ruleType}' entries`);

    await createRule(page, definition);
    logStep(`created '${definition.ruleType}'`);

    await verifyRuleRow(page, definition.ruleType);
    logStep(`verified '${definition.ruleType}' is present and enabled`);

    await verifySavedRuleInEditDialog(page, definition);
    logStep(`verified persisted values for '${definition.ruleType}'`);

    await verifyDuplicateRuleTypePrevention(page, definition);
    logStep(`verified duplicate prevention for '${definition.ruleType}'`);
  }
}
