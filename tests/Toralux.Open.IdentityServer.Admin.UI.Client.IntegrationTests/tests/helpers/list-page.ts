import { expect, type Locator, type Page } from "@playwright/test";
import {
  ensureLoggedInAndOpenClients,
  type LoginCredentials,
} from "./auth";
import { UI_TEXT } from "./ui-texts";

type FindSingleRowBySearchOptions = {
  page: Page;
  searchTerm: string;
  rowLocator: Locator;
  entityName: string;
  timeoutMs?: number;
};

function normalizePath(path: string): string {
  const trimmed = path.replace(/\/+$/g, "");
  return trimmed.length === 0 ? "/" : trimmed.toLowerCase();
}

export async function ensureLoggedInAndOpenListPage(
  page: Page,
  credentials: LoginCredentials,
  path: string,
): Promise<void> {
  await ensureLoggedInAndOpenClients(page, credentials);
  await page.goto(path);
  await expect(page).toHaveURL((url) => {
    return normalizePath(url.pathname) === normalizePath(path);
  });
}

export async function findSingleRowBySearch({
  page,
  searchTerm,
  rowLocator,
  entityName,
  timeoutMs = 90_000,
}: FindSingleRowBySearchOptions): Promise<Locator> {
  const searchInput = page.locator("input[type='text']").first();
  const searchButton = page.getByRole("button", {
    name: UI_TEXT.actions.search,
  });
  const timeoutAt = Date.now() + timeoutMs;

  while (Date.now() < timeoutAt) {
    await searchInput.fill(searchTerm);
    await searchButton.click();

    if ((await rowLocator.count()) === 1) {
      return rowLocator;
    }

    await page.waitForTimeout(500);
  }

  throw new Error(`${entityName} '${searchTerm}' was not found in list.`);
}
