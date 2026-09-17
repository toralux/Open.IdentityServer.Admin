import { faker } from "@faker-js/faker";
import { expect, type Page } from "@playwright/test";
import { type LoginCredentials } from "../helpers/auth";
import {
  ensureLoggedInAndOpenUsers,
  openUserDetailFromList,
} from "../helpers/users-list";
import { openTabAndWait } from "../helpers/ui-navigation";
import { createDebugLogger } from "../helpers/debug-log";

export async function runChangePasswordFlow(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  const logStep = createDebugLogger("change-password-flow");

  const strongPassword = faker.internet.password({
    length: 16,
    memorable: false,
    prefix: "Aa1!",
  });
  const mismatchPassword = faker.internet.password({
    length: 16,
    memorable: false,
    prefix: "Bb2@",
  });

  const marker = faker.string.alphanumeric({ length: 10, casing: "lower" });
  const userName = `ui_cpwd_${marker}`;
  const email = `${userName}@example.local`;

  // Create user
  await ensureLoggedInAndOpenUsers(page, credentials);
  await page.goto("/user-profile");
  await page.locator('input[name="userName"]').fill(userName);
  await page.locator('input[name="email"]').fill(email);
  await Promise.all([
    page.waitForURL(/\/users(?:[/?#]|$)/i, { timeout: 60_000 }),
    page.getByRole("button", { name: /^Create$/i }).click(),
  ]);
  logStep("created test user");

  await openUserDetailFromList(page, userName, credentials);
  logStep("opened user detail");

  const passwordPanel = await openTabAndWait(page, /^Password$/i, {
    useVisiblePanelFallback: true,
  });
  logStep("opened password tab");

  await expect(
    passwordPanel.locator('input[type="password"]').first(),
  ).toBeVisible();
  await expect(
    passwordPanel.locator('input[type="password"]').last(),
  ).toBeVisible();

  // Test validation: mismatched passwords
  await passwordPanel
    .locator('input[type="password"]')
    .first()
    .fill(strongPassword);
  await passwordPanel
    .locator('input[type="password"]')
    .last()
    .fill(mismatchPassword);
  await passwordPanel.getByRole("button", { name: /Change Password/i }).click();
  await expect(passwordPanel.getByText(/do not match/i)).toBeVisible();
  logStep("verified mismatch validation");

  // Test successful password change
  await passwordPanel
    .locator('input[type="password"]')
    .first()
    .fill(strongPassword);
  await passwordPanel
    .locator('input[type="password"]')
    .last()
    .fill(strongPassword);
  await passwordPanel.getByRole("button", { name: /Change Password/i }).click();

  // Expect success toast (Hooray!)
  await expect(page.getByText(/Hooray|Password changed/i).first()).toBeVisible({
    timeout: 15_000,
  });

  // Inputs should be cleared after success
  await expect(
    passwordPanel.locator('input[type="password"]').first(),
  ).toHaveValue("");
  logStep("password changed successfully and form reset");
}
