export const UI_TEXT = {
  actions: {
    search: "Search",
    save: "Save",
    create: "Create",
    close: "Close",
    next: "Next",
    addItem: "Add Item",
    selectAll: "Select All",
    deselectAll: "Deselect All",
    openMenu: "Open menu",
  },
  auth: {
    clientsHeading: "Clients",
    invalidCredentials: "Invalid username or password",
    consentAllow: "Yes, Allow",
  },
  placeholders: {
    enterItem: "Enter item",
    search: "Search",
    enterValue: "Enter value",
  },
  configurationRules: {
    addNewRule: "Add New Rule",
    editRule: "Edit Rule",
    duplicateRuleError:
      "This rule type already exists. Each rule type can only be configured once.",
  },
  configurationIssues: {
    pageTitle: "Configuration Issues",
    filters: "Filters",
    searchPlaceholder: "Search resources or messages...",
    showIssues: "Show issues",
  },
  clientTabs: {
    showAllSettings: "Show all settings",
  },
  grantTypes: {
    authorizationCode: "Authorization Code",
    clientCredentials: "Client Credentials",
  },
  integration: {
    tab: "Integration",
    settings: "Settings",
    authority: "Authority",
    apiBaseUrl: "API base address",
    appName: "Application name",
    copyAll: "Copy all",
    unsupportedGrantTypes:
      "The setup code is generated for the authorization code and client credentials flows only.",
    scenarios: {
      webApp: "ASP.NET Core web app",
      worker: "Worker / API to API",
    },
    steps: {
      storeSecret: "Store the client secret",
    },
    notes: {
      pkceDisabled: "This client does not require PKCE.",
      publicClient:
        "This client has no secret, so it is treated as a public client.",
      inlineSecret: "The secret is written directly into the code.",
      noApiScope: "No API scope is selected.",
    },
  },
  wizard: {
    addNewClient: "Add New Client",
    newClientDialog: "New Client",
    reviewAndSubmit: "Review and Submit",
    highSecureClientType: "High Secure Web Client",
    sharedSecretWarning: "not be FAPI 2.0 compliant",
    clientIdCopyButton: "Click to copy",
    copiedToClipboard: "Copied to clipboard!",
    dPoPClockSkewLabel: "DPoP Clock Skew",
    highSecureDPoPClockSkewSummary: "30 s",
    highSecureDPoPClockSkewValue: "00:00:30",
  },
  secrets: {
    addSecret: "Add Secret",
    secretTypeLabel: "Secret Type",
    secretDescriptionLabel: "Description",
    sharedSecretType: "Shared Secret",
    plainValueTip:
      "This secret type is stored as it is - it is not hashed and stays readable after saving.",
  },
} as const;
