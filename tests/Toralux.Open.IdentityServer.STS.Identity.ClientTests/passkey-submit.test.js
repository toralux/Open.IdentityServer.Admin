const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const { JSDOM } = require("jsdom");

function resolvePasskeySubmitScriptPath() {
  const repoRootCandidates = [
    path.resolve(__dirname, "..", ".."),
    process.cwd(),
  ];

  const checkedPaths = [];

  for (const repoRoot of repoRootCandidates) {
    const candidate = path.join(
      repoRoot,
      "src",
      "Toralux.Open.IdentityServer.STS.Identity",
      "wwwroot",
      "js",
      "passkey-submit.js",
    );

    checkedPaths.push(candidate);

    if (fs.existsSync(candidate)) {
      return candidate;
    }
  }

  throw new Error(
    `Could not locate passkey-submit.js. Checked: ${checkedPaths.join(", ")}`,
  );
}

const scriptPath = resolvePasskeySubmitScriptPath();
const scriptContent = fs.readFileSync(scriptPath, "utf8");

function createHarness(options = {}) {
  const {
    conditionalMediationAvailable = false,
    credentialGetImpl = async () => ({
      id: "credential-id",
      rawId: "raw-id",
      response: {},
      type: "public-key",
    }),
    fetchJson = {},
    username = "alice@example.test",
  } = options;

  const dom = new JSDOM("<!doctype html><html><body></body></html>", {
    runScripts: "dangerously",
    url: "https://sts.example.test/Account/Login",
  });

  const { window } = dom;
  const fetchCalls = [];
  const credentialCalls = [];
  let lastFormValue;

  class FakePublicKeyCredential {}

  FakePublicKeyCredential.parseCreationOptionsFromJSON = (json) => json;
  FakePublicKeyCredential.parseRequestOptionsFromJSON = (json) => json;
  FakePublicKeyCredential.isConditionalMediationAvailable = async () => conditionalMediationAvailable;

  Object.defineProperty(window, "PublicKeyCredential", {
    configurable: true,
    value: FakePublicKeyCredential,
  });

  Object.defineProperty(window.navigator, "credentials", {
    configurable: true,
    value: {
      create: async ({ publicKey, signal }) => credentialGetImpl({ operation: "create", publicKey, signal, window }),
      get: async ({ publicKey, mediation, signal }) => {
        credentialCalls.push({ publicKey, mediation, signal });
        return credentialGetImpl({ operation: "request", publicKey, mediation, signal, window });
      },
    },
  });

  window.fetch = async (url, options = {}) => {
    fetchCalls.push({
      url: String(url),
      options,
    });

    return {
      ok: true,
      json: async () => fetchJson,
      text: async () => "",
    };
  };

  Object.defineProperty(window.HTMLElement.prototype, "attachInternals", {
    configurable: true,
    value() {
      const element = this;

      return {
        form: element.closest("form"),
        setFormValue(value) {
          lastFormValue = value;
          element.__formValue = value;
        },
      };
    },
  });

  Object.defineProperty(window.HTMLFormElement.prototype, "submit", {
    configurable: true,
    value() {
      this.__submitted = true;
    },
  });

  window.console.error = () => {};

  window.eval(scriptContent);

  const form = window.document.createElement("form");

  const usernameInput = window.document.createElement("input");
  usernameInput.name = "Username";
  usernameInput.value = username;

  const button = window.document.createElement("button");
  button.type = "submit";
  button.name = "__passkeySubmit";
  button.value = "__passkeySubmit";

  const component = window.document.createElement("passkey-submit");
  component.setAttribute("operation", "Request");
  component.setAttribute("name", "Passkey");
  component.setAttribute("email-name", "Username");
  component.setAttribute("request-token-name", "RequestVerificationToken");
  component.setAttribute("request-token-value", "anti-forgery-token");
  component.setAttribute("request-options-url", "/Identity/Account/PasskeyRequestOptions");
  component.setAttribute("creation-options-url", "/Identity/Account/PasskeyCreationOptions");

  form.append(usernameInput, button, component);
  window.document.body.append(form);

  return {
    dom,
    window,
    form,
    button,
    component,
    fetchCalls,
    credentialCalls,
    get lastFormValue() {
      return lastFormValue;
    },
  };
}

function getFallbackFields(form) {
  return [...form.querySelectorAll("input[data-passkey-fallback-field='1']")];
}

function getFallbackFieldMap(form) {
  return new Map(getFallbackFields(form).map((field) => [field.name, field.value]));
}

function appendFallbackField(window, form, name, value) {
  const input = window.document.createElement("input");
  input.type = "hidden";
  input.name = name;
  input.value = value;
  input.setAttribute("data-passkey-fallback-field", "1");
  form.append(input);
}

async function flushAsyncWork() {
  await new Promise((resolve) => setTimeout(resolve, 0));
  await new Promise((resolve) => setTimeout(resolve, 0));
}

function createManualSerializationCredential(window) {
  return {
    toJSON() {
      throw new TypeError("Illegal invocation");
    },
    authenticatorAttachment: "platform",
    getClientExtensionResults: () => ({ credProps: { rk: true } }),
    id: "credential-id",
    rawId: window.Uint8Array.from([1, 2, 3]),
    response: {
      authenticatorData: window.Uint8Array.from([4, 5]),
      clientDataJSON: window.Uint8Array.from([6, 7]),
      getPublicKey: () => window.Uint8Array.from([10, 11]),
      getPublicKeyAlgorithm: () => -7,
      getTransports: () => ["internal"],
      signature: window.Uint8Array.from([12, 13]),
      userHandle: window.Uint8Array.from([14, 15]),
    },
    type: "public-key",
  };
}

test("uses remembered passkey click intent when submitter metadata is missing", async () => {
  const harness = createHarness();

  try {
    let obtainCredentialCalls = 0;
    harness.component.obtainAndSubmitCredential = async () => {
      obtainCredentialCalls += 1;
    };

    harness.button.dispatchEvent(new harness.window.MouseEvent("click", { bubbles: true }));

    const submitEvent = new harness.window.Event("submit", { bubbles: true, cancelable: true });
    harness.form.dispatchEvent(submitEvent);

    assert.equal(obtainCredentialCalls, 1);
    assert.equal(harness.component.lastPasskeySubmitIntent, false);
  } finally {
    harness.window.close();
  }
});

test("passes the username query parameter and antiforgery header to request options", async () => {
  const harness = createHarness();

  try {
    await harness.component.obtainCredential(false, new AbortController().signal);

    assert.equal(harness.fetchCalls.length, 1);
    assert.match(harness.fetchCalls[0].url, /\/Identity\/Account\/PasskeyRequestOptions\?username=alice%40example\.test$/);
    assert.equal(harness.fetchCalls[0].options.method, "POST");
    assert.equal(harness.fetchCalls[0].options.credentials, "include");
    assert.equal(harness.fetchCalls[0].options.headers.RequestVerificationToken, "anti-forgery-token");
    assert.equal(Object.keys(harness.fetchCalls[0].options.headers).length, 1);
    assert.equal(harness.credentialCalls.length, 1);
    assert.equal(harness.credentialCalls[0].mediation, undefined);
  } finally {
    harness.window.close();
  }
});

test("does not submit or add fallback fields when the passkey request is aborted", async () => {
  const harness = createHarness({
    credentialGetImpl: async ({ window }) => {
      throw new window.DOMException("The user aborted the request.", "AbortError");
    },
  });

  try {
    await harness.component.obtainAndSubmitCredential();

    assert.equal(harness.form.__submitted, undefined);
    assert.equal(getFallbackFields(harness.form).length, 0);
    assert.equal(harness.lastFormValue, undefined);
  } finally {
    harness.window.close();
  }
});

test("replaces stale fallback fields and posts a passkey error payload on NotAllowedError", async () => {
  const harness = createHarness({
    credentialGetImpl: async ({ window }) => {
      throw new window.DOMException("No credential available.", "NotAllowedError");
    },
  });

  try {
    appendFallbackField(harness.window, harness.form, "Passkey.Error", "stale");

    await harness.component.obtainAndSubmitCredential();

    const fallbackFields = getFallbackFields(harness.form);
    const fallbackFieldMap = getFallbackFieldMap(harness.form);
    const passkeyErrorFields = fallbackFields.filter((field) => field.name === "Passkey.Error");

    assert.equal(harness.form.__submitted, true);
    assert.equal(passkeyErrorFields.length, 1);
    assert.equal(fallbackFieldMap.get("Passkey.Error"), "No passkey was provided by the authenticator.");
    assert.equal(fallbackFieldMap.get("__passkeySubmit"), "1");
    assert.equal(fallbackFieldMap.get("button"), "__passkeySubmit");
  } finally {
    harness.window.close();
  }
});

test("swallows conditional mediation errors without posting fallback fields", async () => {
  const harness = createHarness({
    conditionalMediationAvailable: true,
    credentialGetImpl: async ({ window }) => {
      throw new window.Error("Conditional mediation failed.");
    },
  });

  try {
    await flushAsyncWork();

    assert.equal(harness.fetchCalls.length, 1);
    assert.equal(harness.credentialCalls.length, 1);
    assert.equal(harness.credentialCalls[0].mediation, "conditional");
    assert.equal(harness.form.__submitted, undefined);
    assert.equal(getFallbackFields(harness.form).length, 0);
  } finally {
    harness.window.close();
  }
});

test("falls back to manual credential serialization when JSON.stringify throws a TypeError", async () => {
  const harness = createHarness({
    credentialGetImpl: async ({ window }) => createManualSerializationCredential(window),
  });

  try {
    await harness.component.obtainAndSubmitCredential();

    const fallbackFieldMap = getFallbackFieldMap(harness.form);
    const credentialJson = fallbackFieldMap.get("Passkey.CredentialJson");
    const payload = JSON.parse(credentialJson);

    assert.equal(harness.form.__submitted, true);
    assert.equal(payload.authenticatorAttachment, "platform");
    assert.deepEqual(payload.clientExtensionResults, { credProps: { rk: true } });
    assert.equal(payload.id, "credential-id");
    assert.equal(payload.rawId, "AQID");
    assert.equal(payload.response.authenticatorData, "BAU");
    assert.equal(payload.response.clientDataJSON, "Bgc");
    assert.equal(payload.response.publicKey, "Cgs");
    assert.equal(payload.response.publicKeyAlgorithm, -7);
    assert.deepEqual(payload.response.transports, ["internal"]);
    assert.equal(payload.response.signature, "DA0");
    assert.equal(payload.response.userHandle, "Dg8");
    assert.equal(payload.type, "public-key");
  } finally {
    harness.window.close();
  }
});
