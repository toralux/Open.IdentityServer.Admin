import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import {
  ensureLoggedInAndOpenListPage,
  findSingleRowBySearch,
} from "./list-page";

export async function ensureLoggedInAndOpenUsers(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/users",
  );
}

export async function findUserRow(
  page: Page,
  userName: string,
): Promise<Locator> {
  const targetRow = page.locator("table tbody tr", {
    has: page.getByRole("link", { name: userName, exact: true }),
  });

  return findSingleRowBySearch({
    page,
    searchTerm: userName,
    rowLocator: targetRow,
    entityName: "User",
  });
}

export async function openUserDetailFromList(
  page: Page,
  userName: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenUsers(page, credentials);
  const targetRow = await findUserRow(page, userName);
  await targetRow.getByRole("link", { name: userName, exact: true }).click();
  await expect(page).toHaveURL(/\/user-profile\/[^/?#]+(?:[/?#]|$)/i);
}
