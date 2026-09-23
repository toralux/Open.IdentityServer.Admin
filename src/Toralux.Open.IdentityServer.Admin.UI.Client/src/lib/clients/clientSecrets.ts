import { combineDateTimeForUnspecifiedDb } from "@/helpers/DateTimeHelper";

/**
 * The expiration a secret is created with, built from the raw date and time of
 * the secret form. The result is shifted for the database, so it must never go
 * back into the form - combining it a second time moves it by the offset again.
 */
export const resolveSecretExpiration = ({
  addExpiration,
  expiration,
  expirationTime,
}: {
  addExpiration?: boolean;
  expiration?: Date | null;
  expirationTime?: string | null;
}): Date | null =>
  addExpiration
    ? combineDateTimeForUnspecifiedDb(expiration, expirationTime)
    : null;
