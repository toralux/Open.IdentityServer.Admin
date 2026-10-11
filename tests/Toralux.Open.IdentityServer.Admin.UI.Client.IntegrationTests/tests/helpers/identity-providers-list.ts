import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import {
  ensureLoggedInAndOpenListPage,
  findSingleRowBySearch,
} from "./list-page";

export async function ensureLoggedInAndOpenIdentityProviders(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/identity-providers",
  );
}

export async function findIdentityProviderRow(
  page: Page,
  scheme: string,
): Promise<Locator> {
  const targetRow = page.locator("table tbody tr", {
    has: page.getByRole("link", { name: scheme, exact: true }),
  });

  return findSingleRowBySearch({
    page,
    searchTerm: scheme,
    rowLocator: targetRow,
    entityName: "Identity provider",
  });
}

export async function openIdentityProviderDetailFromList(
  page: Page,
  scheme: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenIdentityProviders(page, credentials);
  const targetRow = await findIdentityProviderRow(page, scheme);
  await targetRow.getByRole("link", { name: scheme, exact: true }).click();
  await expect(page).toHaveURL(/\/identity-provider\/\d+(?:[/?#]|$)/i);
}
