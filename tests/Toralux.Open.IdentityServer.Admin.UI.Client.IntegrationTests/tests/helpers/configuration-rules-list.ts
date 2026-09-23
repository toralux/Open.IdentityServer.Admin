import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import { ensureLoggedInAndOpenListPage } from "./list-page";

export async function ensureLoggedInAndOpenConfigurationRules(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/configuration-rules",
  );
  await expect(
    page.getByRole("heading", { name: /Configuration Rules/i }),
  ).toBeVisible();
}

export function findConfigurationRuleRowsByType(
  page: Page,
  ruleType: string,
): Locator {
  return page.locator("table tbody tr").filter({ hasText: ruleType });
}

export async function findConfigurationRuleRowByType(
  page: Page,
  ruleType: string,
): Promise<Locator> {
  const rows = findConfigurationRuleRowsByType(page, ruleType);
  const timeoutAt = Date.now() + 90_000;

  while (Date.now() < timeoutAt) {
    const count = await rows.count();
    if (count > 0) {
      return rows.first();
    }

    await page.waitForTimeout(500);
  }

  throw new Error(`Configuration rule '${ruleType}' was not found in list.`);
}
