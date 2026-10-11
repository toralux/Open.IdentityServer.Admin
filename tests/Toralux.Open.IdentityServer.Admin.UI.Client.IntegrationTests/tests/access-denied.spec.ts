import { expect, test } from "@playwright/test";
import { loadE2ESeedData } from "../utils/seed-data";
import { type LoginCredentials } from "./helpers/auth";
import { ensureLoggedInAndOpenUsers } from "./helpers/users-list";

const seedData = loadE2ESeedData();
const credentials: LoginCredentials = {
  username: seedData.username,
  password: seedData.password,
};

test.describe("Access Denied page", () => {
  test("navigating directly to /access-denied renders the page with correct content", async ({
    page,
  }) => {
    await ensureLoggedInAndOpenUsers(page, credentials);

    await page.goto("/access-denied");
    await expect(page).toHaveURL(/\/access-denied(?:[/?#]|$)/i);

    // Heading and description should be visible
    await expect(
      page.getByRole("heading", { name: /Access Denied/i }),
    ).toBeVisible();
    await expect(page.getByText(/permission|forbidden/i).first()).toBeVisible();

    // Go home link should navigate back to root
    const goHomeLink = page.getByRole("link", { name: /Go to Home/i });
    await expect(goHomeLink).toBeVisible();
    await goHomeLink.click();
    await expect(page).toHaveURL("/");
  });
});
