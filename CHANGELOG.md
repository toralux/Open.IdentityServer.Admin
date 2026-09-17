# Changelog

## [0.1.0] - 2026-09-17

### Fork point

Toralux Open IdentityServer Admin **0.1.0** is forked from
[skoruba/Duende.IdentityServer.Admin](https://github.com/skoruba/Duende.IdentityServer.Admin)
**v3.0.0** (commit `45da4dc4`). This is the first Toralux release; upstream
history follows below. Not affiliated with or endorsed by Rock Solid Knowledge /
IdentityServer.com.

### Added

- `tools/rename.sh` + `patches/` replay mechanism for upstream syncs
- Toralux package metadata (Authors, Repository, Apache-2.0 license expression, package icon)
- NOTICE file with upstream attribution
- README fork-lineage, disclaimer, connection-string (×7), and secret-hashing migration notes

### Changed

- Retargeted from Duende IdentityServer 7.4.7 to **Open IdentityServer 2.0.0**
  (namespaces `Duende.IdentityServer` → `Open.IdentityServer`; `Duende.IdentityModel`
  and `Duende.AccessTokenManagement.*` kept)
- Package IDs rebranded `Skoruba.*`/`Duende.*` → `Toralux.Open.IdentityServer.Admin.*` (14 packages), versioned **0.1.0**
- `dotnet new` template renamed to `toralux.open-isadmin`; template package
  `Toralux.Open.IdentityServer.Admin.Templates` 0.1.0; `--requirepushedauthorization` now defaults to `false`
- npm packages rebranded (`toralux-open-identityserver-admin`, `@toralux/open-identityserver.admin.api.client`)
- Seed identifiers renamed: clients `toralux_identity_admin_*`, admin `admin@example.com`,
  role `ToraluxIdentityAdminAdministrator`
- UI.Spa packaging now builds SPA assets (`npm run build:spa`) before `dotnet pack`

### Removed

- IdentityProvider and Keys administration pages (endpoints absent in Open IdentityServer)
- Dynamic providers / server-side sessions wiring (returns when Open IdentityServer ships them)
- PAR enforcement in generated projects (unsupported by Open IdentityServer)

## [3.0.0] - 2026-07-15

### Added

- New React + TypeScript Admin UI with a modern SPA architecture, client creation wizard, monitoring dashboard, configuration rules, dark mode support, and improved client/resource management UX
- Passkey (WebAuthn) authentication support in STS Identity, including registration, rename, removal, localization, configuration, and SQL Server/PostgreSQL migrations
- New Mapperly-based identity data mapping pipeline with customization extension points
- Expanded test coverage across Playwright UI flows, Admin API integration tests, STS integration tests, repository validation, and audit event security scenarios

### Changed

- Updated the solution to .NET 10 and Duende IdentityServer 7.4.7
- Replaced AutoMapper with Mapperly across Admin BusinessLogic, Identity BusinessLogic, and Admin UI API mappers
- Migrated Admin UI Client from `react-query` v3 to `@tanstack/react-query` v5
- Migrated STS UI styling from Bootstrap/Gulp to Tailwind-based tooling
- Improved build, versioning, package publishing, and release automation scripts
- Updated solution/template/frontend package version references to `3.0.0`

### Fixed

- Hardened audit event payload sanitization to avoid persisting sensitive values such as client secrets, API secrets, client property values, pairwise subject salts, identity provider property values, persisted grant data/session identifiers, and identity user security fields
- Fixed Admin UI behavior when API sessions expire or return unauthorized/forbidden responses
- Fixed Admin UI data-grid delete actions and unsaved-changes confirmation flows that could leave the page blocked
- Fixed tabbed client form validation visibility and optional numeric field handling
- Prevented invalid configuration where `ApiScope` and `IdentityResource` share the same name
- Fixed client claims update tracking issues and improved mapper null handling/runtime diagnostics
- Resolved npm audit issues in Admin UI Client and STS Identity dependencies

### Breaking Changes

- New `AdminConfigurationDbContext` for monitoring/configuration rules requires new EF migrations
- Passkey support adds a new Identity persistence schema (`AddUserPasskeys`) for SQL Server/PostgreSQL
- Solution structure and NuGet package layout were reorganized with new Admin, Admin.Storage, and UI SPA projects
- MySQL support was removed because the Pomelo.MySql package is not available for .NET 10
- AutoMapper dependency was removed; custom mapping extensions must use the new Mapperly customization interfaces

---

## [3.0.0-rc4]

### Fixed

- Fixed Admin UI data-grid delete actions leaving the page blocked after confirming deletion from the row action menu
- Corrected shared modal/dropdown state handling to properly release pointer interaction after dialog close
- Added Playwright regression coverage for the delete-from-grid interaction flow

---

## [3.0.0-rc3]

### Changed

- Improved Admin UI authentication and authorization UX by separating `401 Unauthorized` and `403 Forbidden` flows
- Updated the default seeded Admin UI client (`skoruba_identity_admin_v3`) to use `RequireConsent = false`
- Added focused Playwright regression coverage for the new auth and form-validation behaviors

### Fixed

- Fixed Admin UI behavior when API sessions expire or become invalid, preventing confusing partially authenticated states
- Fixed dashboard and configuration-issue loading/error states to avoid misleading empty content and layout glitches
- Fixed tabbed client form validation visibility by adding a summary for hidden-tab errors and friendlier numeric validation messages
- Resolved npm audit issues in `Skoruba.Duende.IdentityServer.STS.Identity`

---

## [3.0.0-rc2]

This release candidate focuses on stabilization before the final `3.0.0` release.

The main highlight of this release is a significant improvement in test coverage. We added new Playwright UI integration tests, extended API integration tests, and added more STS integration tests to better verify the full Admin UI, API, and IdentityServer integration scenarios.

This release also includes stricter validation and clearer error handling for user and role claim updates, improved API feedback, updated documentation, and version updates across the solution.

If no critical issues are reported, this release candidate is intended to be promoted to the final `3.0.0` release after a short stabilization period.

---

## [3.0.0-preview.24]

### Changed

- Updated solution/template/frontend package version references to `3.0.0-preview.24`

### Fixed

- Prevented invalid configuration where `ApiScope` and `IdentityResource` share the same name by extending repository `CanInsert` validation across both entity types
- Added repository unit tests for cross-entity name collision checks between API scopes and identity resources
- Improved optional numeric field handling in Admin UI Client (`FormRow` number mode) so clearing a value keeps it nullable instead of immediately restoring the previous number
- Updated client and configuration-rule form schemas to correctly accept nullable optional numeric values for advanced settings and rule parameters

---

## [3.0.0-preview.23]

### Added

- Passkey (WebAuthn) authentication support in STS Identity (register, rename, and remove passkeys)
- New STS passkey pages, localization resources, and client-side helpers
- User passkey persistence model (`UserIdentityPasskey`) with SQL Server and PostgreSQL migrations
- New Mapperly-based identity data mapping pipeline with customization extension points

### Changed

- Replaced AutoMapper with Mapperly across Admin BusinessLogic, Identity BusinessLogic, and Admin UI API mappers ([#287](https://github.com/skoruba/Duende.IdentityServer.Admin/pull/287))
- Updated package references across the solution (including Duende IdentityServer 7.4.7, EF Core/ASP.NET Core 10.0.7, NSwag 14.7.1) ([#288](https://github.com/skoruba/Duende.IdentityServer.Admin/pull/288))
- Migrated Admin UI Client from `react-query` v3 to `@tanstack/react-query` v5
- Upgraded frontend linting/tooling to ESLint v10 and refreshed related TypeScript dependencies
- Updated v3 template and documentation references to `3.0.0-preview.23`

### Fixed

- Duplicate type mapping configuration for `UserLoginInfo -> TUserProviderDto` ([#271](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/271))
- Client retrieval generating overly complex SQL query ([#265](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/265))
- Admin UI dark-mode background rendering for audit log JSON data ([#284](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/284))
- Client claims update flow now avoids reusing existing claim IDs in update graphs (prevents EF tracking/concurrency conflicts)
- Improved mapper null handling and runtime error messages for dynamic instance creation
- Resolved npm audit vulnerabilities in `Admin.UI.Client` dependencies (`i18next-http-backend`, `uuid`)

### Breaking Changes

- Added passkey persistence schema for Identity (`AddUserPasskeys`) – apply new EF migrations for SQL Server/PostgreSQL
- AutoMapper dependency removed from mapper projects; custom mapping extensions should use the new Mapperly customization interfaces

---

## [3.0.0-preview.21]

### Added

- New React + TypeScript Admin UI with modern design
- Client creation wizard with step-by-step flow
- Monitoring dashboard with configuration rules engine
- Configuration issues tracking and alerts
- 15+ built-in configuration rules (PKCE, implicit grant, secret expiration, HTTPS enforcement, etc.)
- Configuration rules metadata provider with severity levels
- Tailwind CSS + shadcn/ui design system
- Dark mode support with semantic color tokens
- Enhanced TreeView for client summary
- Filtering system for configuration issues
- New Admin UI host (SPA served by .NET)
- Migrated STS UI from Bootstrap to Tailwind CSS with modern tooling (replaced Gulp)
- Pushed Authorization Requests (PAR) template option
- Code-splitting for improved frontend performance
- Forwarded headers configuration for reverse proxy scenarios

### Changed

- Updated to .NET 10
- Updated Duende IdentityServer to 7.4.5
- Updated solution structure for new frontend architecture
- Major UX improvements for client and resource management
- Improved form layouts and compact designs
- Enhanced navigation and user experience

### Breaking Changes

- **New AdminConfigurationDbContext** for monitoring feature – requires new EF migrations to be applied
- Solution structure reorganized with new projects (Admin.Storage, UI.Spa)
- NuGet package structure updated (new Admin and Admin.Storage packages)

---

## [2.6.0] – 2024-11-12

### Changed

- Updated to .NET 9
- Updated Duende IdentityServer to 7.2.1

---

## [2.5.0] – 2024-08-20

### Fixed

- Error when deleting users from the Admin UI ([#214](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/214))

---

## [2.4.0] – 2024-07-15

### Fixed

- Client update failing due to duplicate ClientId validation ([#227](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/227))
- Docker Compose nginx targeting wrong port ([#222](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/222))
- Method CanInsert..Property of controllers always returning true ([#235](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/235))

### Changed

- Migrated to new Azure Key Vault API ([#224](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/224))
- Replaced deprecated Microsoft.Extensions.Configuration.AzureKeyVault package ([#234](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/234))
- Updated all NuGet packages (including CVE-2024-39694 fix) ([#236](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/236))
- Updated Duende IdentityServer to 7.0.7

---

## [2.3.0] – 2024-05-10

### Changed

- Updated Duende IdentityServer to 7.0.5
- Updated all NuGet packages to latest versions

### Fixed

- Dashboard endpoint for Identity data retrieval

---

## [2.2.2] – 2024-03-25

### Added

- New Admin UI API project shipped as NuGet package
- Dashboard API endpoint
- TypeScript client generation for Admin API ([#215](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/215))

### Fixed

- Dockerfiles for multi-platform builds (linux/amd64, linux/arm64) ([#194](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/194))
- NSwag TypeScript definition dayjs import issues

---

## [2.1.0] – 2024-01-18

### Added

- Role users pagination ([#169](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/169))
- Secure secret generation with `secret_` prefix ([#153](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/153))
- Validation/list endpoints for clients, API resources, and scopes ([#213](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/213))

### Changed

- Increased client name prominence ([#154](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/154))
- Named arguments in IdentityServer health checks ([#201](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/201))
- Identity table names configurable via appsettings ([#196](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/196))

### Fixed

- UserLoginSuccessEvent not raised for 2FA/recovery login ([#202](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/202))

---

## [2.0.0] – 2023-11-20

### Changed

- Updated to .NET 8 ([#180](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/180))
- Updated to Duende IdentityServer v7 ([#181](https://github.com/skoruba/Duende.IdentityServer.Admin/issues/181))

---

## [1.2.0] – 2023-02-15

### Added

- Dynamic Identity Providers support

### Changed

- Updated to Duende IdentityServer 6.2.1

---

## [1.1.0] – 2022-11-08

### Changed

- Updated to .NET 6
- Updated to Duende IdentityServer v6

---

## [1.0.0] – 2021-11-15

### Added

- Initial Admin UI for Duende IdentityServer
- ASP.NET Core Identity management
- Client, API and Identity resource management
- Audit logging via skoruba/AuditLogging
- Docker support with nginx-proxy
- Health checks for databases and IdentityServer
- Multiple database providers (SQL Server, PostgreSQL)
- External authentication providers (GitHub, Azure AD)
- Two-Factor Authentication (2FA)
- User registration and password reset
- Email support (SendGrid, SMTP)
- Azure Key Vault integration
- Localization support (multiple languages)
- Serilog logging with multiple sinks
- Swagger API documentation
- Project templates via dotnet CLI

### Security

- Added support for loading signing key from Azure Key Vault ([#533](https://github.com/skoruba/IdentityServer4.Admin/issues/533))
- Data Protection keys in Azure Key Vault ([#715](https://github.com/skoruba/IdentityServer4.Admin/pull/715))

---

## Historical Notes

For history before the Duende rebranding, see the IdentityServer4.Admin repository history at:  
https://github.com/skoruba/IdentityServer4.Admin
