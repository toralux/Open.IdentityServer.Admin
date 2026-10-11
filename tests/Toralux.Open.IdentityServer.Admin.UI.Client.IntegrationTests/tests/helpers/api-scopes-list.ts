import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import {
  ensureLoggedInAndOpenListPage,
  findSingleRowBySearch,
} from "./list-page";

export async function ensureLoggedInAndOpenApiScopes(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/api-scopes",
  );
}

export async function findApiScopeRow(
  page: Page,
  scopeName: string,
): Promise<Locator> {
  const targetRow = page.locator("table tbody tr", {
    has: page.getByRole("link", { name: scopeName, exact: true }),
  });

  return findSingleRowBySearch({
    page,
    searchTerm: scopeName,
    rowLocator: targetRow,
    entityName: "API scope",
  });
}

export async function openApiScopeDetailFromList(
  page: Page,
  scopeName: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenApiScopes(page, credentials);
  const targetRow = await findApiScopeRow(page, scopeName);
  await targetRow.getByRole("link", { name: scopeName, exact: true }).click();
  await expect(page).toHaveURL(/\/api-scope\/\d+(?:[/?#]|$)/i);
}
