import { expect, type Locator, type Page } from "@playwright/test";
import {
  ensureLoggedInAndOpenClients,
  type LoginCredentials,
} from "./auth";
import { findSingleRowBySearch } from "./list-page";

export async function findClientRow(
  page: Page,
  clientId: string,
): Promise<Locator> {
  const targetRow = page.locator("table tbody tr", {
    has: page.locator("code", { hasText: clientId }),
  });

  return findSingleRowBySearch({
    page,
    searchTerm: clientId,
    rowLocator: targetRow,
    entityName: "Client",
  });
}

export async function openClientDetailFromClients(
  page: Page,
  clientId: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenClients(page, credentials);
  const targetRow = await findClientRow(page, clientId);
  await targetRow.getByRole("link").first().click();
  await expect(page).toHaveURL(/\/client\/\d+(?:[/?#]|$)/i);
}
