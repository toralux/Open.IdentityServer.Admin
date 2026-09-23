import { getBaseHref, normalizeBasePath } from "@/lib/utils";

const joinUrl = (base: string, path: string): string => {
  const normalizedBase = normalizeBasePath(base);
  const normalizedPath = path.startsWith("/") ? path.slice(1) : path;
  return normalizedPath ? `${normalizedBase}${normalizedPath}` : normalizedBase;
};

const getCurrentReturnUrl = (): string | null => {
  if (typeof window === "undefined") return null;
  const { pathname, search } = window.location;
  return `${pathname}${search}`;
};

class AuthHelper {
  static getLoginUrl = (returnUrl?: string | null): string => {
    const loginUrl = joinUrl(getBaseHref(), "account/login");
    const resolvedReturnUrl = returnUrl ?? getCurrentReturnUrl();

    return resolvedReturnUrl
      ? `${loginUrl}?returnUrl=${encodeURIComponent(resolvedReturnUrl)}`
      : loginUrl;
  };

  static getLogoutUrl = (): string => {
    return joinUrl(getBaseHref(), "account/logout");
  };

  static getCsrfUrl = (): string => {
    return joinUrl(getBaseHref(), "csrf/gettoken");
  };

  static redirectToLoginUrl = (returnUrl?: string | null): void => {
    if (typeof window === "undefined") return;
    window.location.assign(AuthHelper.getLoginUrl(returnUrl));
  };

  static redirectToLogoutUrl = (): void => {
    if (typeof window === "undefined") return;
    window.location.assign(AuthHelper.getLogoutUrl());
  };
}

export default AuthHelper;
