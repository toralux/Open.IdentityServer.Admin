import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import {
  ensureLoggedInAndOpenListPage,
  findSingleRowBySearch,
} from "./list-page";

export async function ensureLoggedInAndOpenIdentityResources(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/identity-resources",
  );
}

export async function findIdentityResourceRow(
  page: Page,
  resourceName: string,
): Promise<Locator> {
  const targetRow = page.locator("table tbody tr", {
    has: page.getByRole("link", { name: resourceName, exact: true }),
  });

  return findSingleRowBySearch({
    page,
    searchTerm: resourceName,
    rowLocator: targetRow,
    entityName: "Identity resource",
  });
}

export async function openIdentityResourceDetailFromList(
  page: Page,
  resourceName: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenIdentityResources(page, credentials);
  const targetRow = await findIdentityResourceRow(page, resourceName);
  await targetRow.getByRole("link", { name: resourceName, exact: true }).click();
  await expect(page).toHaveURL(/\/identity-resource\/\d+(?:[/?#]|$)/i);
}
