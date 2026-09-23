import { describe, expect, it } from "vitest";
import { combineDateTimeForUnspecifiedDb } from "@/helpers/DateTimeHelper";
import { resolveSecretExpiration } from "./clientSecrets";

describe("resolveSecretExpiration", () => {
  // What the date picker hands over - local midnight of the picked day. Late
  // evening is the case that used to roll over to the next day east of UTC.
  const pickedDay = () => new Date(2026, 8, 18);
  const wizardState = () => ({
    addExpiration: true,
    expiration: pickedDay(),
    expirationTime: "22:30",
  });

  it("stores the picked local date and time as they were entered", () => {
    const expiration = resolveSecretExpiration(wizardState())!;

    // Read through the UTC getters, so the check holds in any machine time zone.
    expect([
      expiration.getUTCFullYear(),
      expiration.getUTCMonth(),
      expiration.getUTCDate(),
      expiration.getUTCHours(),
      expiration.getUTCMinutes(),
    ]).toEqual([2026, 8, 18, 22, 30]);
  });

  it("matches the single combine the secret step used to do on submit", () => {
    expect(resolveSecretExpiration(wizardState())).toEqual(
      combineDateTimeForUnspecifiedDb(pickedDay(), "22:30"),
    );
  });

  it("gives the same value however many times the wizard state is resolved", () => {
    const state = wizardState();

    const first = resolveSecretExpiration(state);
    // Back and Next again - the step submits the very same raw values.
    const second = resolveSecretExpiration({ ...state });

    expect(second).toEqual(first);
    expect(state.expiration).toEqual(pickedDay());
    expect(state.expirationTime).toBe("22:30");
  });

  it("falls back to midnight without a time", () => {
    const expiration = resolveSecretExpiration({
      ...wizardState(),
      expirationTime: null,
    })!;

    expect([expiration.getUTCDate(), expiration.getUTCHours()]).toEqual([18, 0]);
  });

  it("has no expiration when it is switched off or no date is picked", () => {
    expect(
      resolveSecretExpiration({ ...wizardState(), addExpiration: false }),
    ).toBeNull();
    expect(
      resolveSecretExpiration({ ...wizardState(), expiration: null }),
    ).toBeNull();
    expect(resolveSecretExpiration({})).toBeNull();
  });
});
