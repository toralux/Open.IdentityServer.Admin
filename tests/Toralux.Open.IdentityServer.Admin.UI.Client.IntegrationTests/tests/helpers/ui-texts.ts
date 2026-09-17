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
  wizard: {
    addNewClient: "Add New Client",
    newClientDialog: "New Client",
    reviewAndSubmit: "Review and Submit",
  },
} as const;
