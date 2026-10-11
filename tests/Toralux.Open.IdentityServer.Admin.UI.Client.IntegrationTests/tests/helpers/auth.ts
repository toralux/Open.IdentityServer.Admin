import { expect, type Page } from "@playwright/test";
import { UI_TEXT } from "./ui-texts";

export type LoginCredentials = {
  username: string;
  password: string;
};

function escapeForRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function getStsLoginUrlPattern(): RegExp {
  const stsUrl = (process.env.E2E_STS_URL ?? "https://localhost:44310").replace(
    /\/+$/g,
    "",
  );
  return new RegExp(
    `${escapeForRegex(stsUrl)}/Account/Login(?:[/?#]|$)`,
    "i",
  );
}

function normalizePath(path: string): string {
  const trimmed = path.replace(/\/+$/g, "");
  return trimmed.length === 0 ? "/" : trimmed.toLowerCase();
}

function isPath(urlString: string, expectedPath: string): boolean {
  try {
    const parsedUrl = new URL(urlString);
    return normalizePath(parsedUrl.pathname) === normalizePath(expectedPath);
  } catch {
    return false;
  }
}

function isAppLoginUrl(urlString: string): boolean {
  return isPath(urlString, "/account/login");
}

async function waitForInitialState(
  clientsHeading: ReturnType<Page["getByRole"]>,
  usernameInput: ReturnType<Page["locator"]>,
  consentAllowButton: ReturnType<Page["getByRole"]>,
  page: Page,
): Promise<"clients" | "login"> {
  const timeoutAt = Date.now() + 90_000;

  while (Date.now() < timeoutAt) {
    if (await clientsHeading.isVisible().catch(() => false)) {
      return "clients";
    }

    if (await usernameInput.isVisible().catch(() => false)) {
      return "login";
    }

    if (isAppLoginUrl(page.url())) {
      return "login";
    }

    if (await consentAllowButton.isVisible().catch(() => false)) {
      await consentAllowButton.click();
      continue;
    }

    await page.waitForTimeout(250);
  }

  throw new Error(
    `Neither Clients screen nor STS login form became visible within timeout. Last URL: ${page.url()}`,
  );
}

async function waitForPostLoginResult(
  page: Page,
  clientsHeading: ReturnType<Page["getByRole"]>,
): Promise<"success" | "invalid_credentials" | "timeout"> {
  const invalidCredentials = page.getByText(UI_TEXT.auth.invalidCredentials, {
    exact: false,
  });
  const consentAllowButton = page.getByRole("button", {
    name: UI_TEXT.auth.consentAllow,
  });
  const timeoutAt = Date.now() + 90_000;
  let consentHandled = false;

  while (Date.now() < timeoutAt) {
    if (await clientsHeading.isVisible().catch(() => false)) {
      return "success";
    }

    if (await invalidCredentials.isVisible().catch(() => false)) {
      return "invalid_credentials";
    }

    if (
      !consentHandled &&
      (await consentAllowButton.isVisible().catch(() => false))
    ) {
      await consentAllowButton.click();
      consentHandled = true;
      continue;
    }

    await page.waitForTimeout(250);
  }

  return "timeout";
}

export async function ensureLoggedInAndOpenClients(
  page: Page,
  credentials: LoginCredentials,
): Promise<void> {
  await page.goto("/clients");

  const clientsHeading = page.getByRole("heading", {
    name: UI_TEXT.auth.clientsHeading,
  });
  const usernameInput = page.locator("#Username");
  const passwordInput = page.locator("#Password");
  const consentAllowButton = page.getByRole("button", {
    name: UI_TEXT.auth.consentAllow,
  });
  const initialState = await waitForInitialState(
    clientsHeading,
    usernameInput,
    consentAllowButton,
    page,
  );

  if (initialState === "login") {
    await expect(page).toHaveURL(getStsLoginUrlPattern(), { timeout: 30_000 });

    await usernameInput.fill(credentials.username);
    await passwordInput.fill(credentials.password);
    await page.locator("button[name='button'][value='login']").click();

    const loginResult = await waitForPostLoginResult(
      page,
      clientsHeading,
    );
    if (loginResult === "invalid_credentials") {
      throw new Error(
        `Login failed for user '${credentials.username}'. Provide valid credentials using E2E_USERNAME and E2E_PASSWORD.`,
      );
    }

    if (loginResult !== "success") {
      throw new Error(
        `Login flow timed out before reaching Clients screen. Last URL: ${page.url()}`,
      );
    }
  }

  await expect(page).toHaveURL((url) => normalizePath(url.pathname) === "/clients", {
    timeout: 60_000,
  });
  await expect(clientsHeading).toBeVisible({ timeout: 60_000 });
}
