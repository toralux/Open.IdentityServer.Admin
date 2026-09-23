import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import {
  ensureLoggedInAndOpenListPage,
  findSingleRowBySearch,
} from "./list-page";

export async function ensureLoggedInAndOpenApiResources(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/api-resources",
  );
}

export async function findApiResourceRow(
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
    entityName: "API resource",
  });
}

export async function openApiResourceDetailFromList(
  page: Page,
  resourceName: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenApiResources(page, credentials);
  const targetRow = await findApiResourceRow(page, resourceName);
  await targetRow.getByRole("link", { name: resourceName, exact: true }).click();
  await expect(page).toHaveURL(/\/api-resource\/\d+(?:[/?#]|$)/i);
}
