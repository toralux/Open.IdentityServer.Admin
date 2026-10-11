import { expect, type Locator, type Page } from "@playwright/test";
import { type LoginCredentials } from "./auth";
import {
  ensureLoggedInAndOpenListPage,
  findSingleRowBySearch,
} from "./list-page";

export async function ensureLoggedInAndOpenRoles(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenListPage(
    page,
    credentials,
    "/roles",
  );
}

export async function findRoleRow(
  page: Page,
  roleName: string,
): Promise<Locator> {
  const targetRow = page.locator("table tbody tr", {
    has: page.getByRole("link", { name: roleName, exact: true }),
  });

  return findSingleRowBySearch({
    page,
    searchTerm: roleName,
    rowLocator: targetRow,
    entityName: "Role",
  });
}

export async function openRoleDetailFromList(
  page: Page,
  roleName: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenRoles(page, credentials);
  const targetRow = await findRoleRow(page, roleName);
  await targetRow.getByRole("link", { name: roleName, exact: true }).click();
  await expect(page).toHaveURL(/\/role\/[^/?#]+(?:[/?#]|$)/i);
}

export async function openRoleUsersFromList(
  page: Page,
  roleName: string,
  credentials: LoginCredentials,
): Promise<void> {
  await ensureLoggedInAndOpenRoles(page, credentials);
  const targetRow = await findRoleRow(page, roleName);

  const showUsersButton = targetRow.getByRole("button", { name: /Show|Users/i });
  if ((await showUsersButton.count()) > 0) {
    await showUsersButton.first().click();
  } else {
    await targetRow.getByRole("button").first().click();
  }

  await expect(page).toHaveURL(/\/role\/[^/?#]+\/users(?:[/?#]|$)/i);
}
