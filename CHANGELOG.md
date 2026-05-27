# Changelog

All notable changes to ChuA.ERP.Web.Mvc are documented in this file. The format
is inspired by [Keep a Changelog](https://keepachangelog.com/), and the project
uses phase-based milestone tags (no semantic version yet).

## [phase-j-du] — 2026-05-27

Companion to the same-named tag on `chu-a-api`. The MVC now delegates
authentication to Auth0 (cookie + OIDC) and reads authorization from the ERP
database via `/api/v1/users/me`. Nav rendering uses canonical colon-form
permission claims throughout.

### Added

- Auth0 cookie + OIDC wiring via `ChuA.Authentication`'s `Auth0WebApp`
  configurator. `appsettings.Development.json` carries the dev tenant
  (`dev-stqmesqj1jnfr6sz.us.auth0.com`) and the API audience
  (`https://api.chua-erp.com`).
- `ErpClaimsTransformation` (`Security/`) — `IClaimsEnricher` that calls
  `/api/v1/users/me`, stamps `role` / `permission` / `erp_user_id` /
  `company_id` / `companies` claims onto the cookie principal. Cached per
  request in `HttpContext.Items` (via the library's transformation guard) and
  per user in `IMemoryCache` (30 s TTL).
- `AccessDenied.cshtml` "Access pending" page rendered when a caller
  authenticates but lacks an `[identity].[ExternalLogins]` row.
- `/AuthDebug` Development-only page surfacing the principal's claims, roles,
  permissions, and scopes — primary tool for diagnosing why a nav item is
  missing or a `[Authorize(Policy = …)]` denies.
- `RoleClaimType`-aware `CurrentUserService.Roles` — reads using the identity's
  configured `RoleClaimType` rather than the legacy `ClaimTypes.Role` URI so
  Auth0's namespaced `http://localhost:4200/roles` claims flow correctly to
  `User.IsInRole`, `[Authorize(Roles=…)]`, and `RequireRole`.

### Changed

- `AuthorizePolicyTagHelper` simplified to a synchronous `Process` that just
  splits the comma-separated colon-form policy list and forwards to
  `ICurrentUserService.HasAnyPermission`. The earlier per-render
  `LoadProfileAsync` fallback (one API hit per nav item, ~20 per page) is
  gone — `ErpClaimsTransformation` already hydrated the principal once on
  the way in.
- All `asp-authorize-policy="…"` attributes across views migrated from
  PascalCase to colon-form (`vendor:view` instead of `VendorRead`, etc.).
  `Security/AuthorizationPolicies.cs` is a colon-form mirror of
  `ChuA.ERP.Domain.Authorization.ErpPermissions`.
- `_LoginPartial.cshtml` + `_Navigation.cshtml` updates: `asp-area=""` on
  non-Dashboard links so Dashboard-area ambient route values stop polluting
  the URL generator (the cause of `/Dashboard/Vendors` instead of `/Vendors`
  on nav clicks).
- Sidebar section toggles (Purchasing / Sales / Inventory / Finance / Workflow
  / Reports) now bind `bootstrap.Collapse.getOrCreateInstance(target).toggle()`
  explicitly in `site.js` rather than relying on Bootstrap's data API. Same
  external behavior; deterministic against whatever was swallowing the click.

### Fixed

- `ErpClaimsTransformation` not running at all — `ChuA.Authentication` registered
  its own `ChuAClaimsTransformation` which shadowed ours via
  `GetRequiredService<IClaimsTransformation>()`. Once the library exposed the
  `IClaimsEnricher` hook, our enricher plugs in alongside the library's claim
  mapping rather than replacing it.
- Stack overflow (~253 recursion frames) when `_users.GetMeAsync()` triggered
  `ctx.GetTokenAsync("access_token")` → `AuthenticateAsync` → re-entrant
  `TransformAsync`. Handled by the library's per-request stash in
  `HttpContext.Items` once we became a `IClaimsEnricher`.
- Auth0 issuing an opaque access_token because no `audience` parameter was
  passed in the OIDC challenge — added the audience to
  `ChuAAuthentication:Providers:Auth0:Audience`. Library routes it to
  `AdditionalAuthorizationParameters["audience"]` (the canonical pattern), no
  longer to `TokenValidationParameters.ValidAudience` which was the source of
  the earlier `IDX10214` failures.

### Removed

- Pre-library-update workarounds: the manual
  `services.RemoveAll<IClaimsTransformation>()` plus re-add, the
  `IClaimsMappingService` chain inside `ErpClaimsTransformation`, and the
  `PostConfigure<OpenIdConnectOptions>` that hardcoded the audience. All
  replaced by the library's `IClaimsEnricher` extension contract.
- Temporary `[token-shape]` diagnostic in `CookieTokenAcquisitionService` that
  logged the access-token header + payload during live debugging.

## [pre-phase-j] — historical

Original Phase 5 MVC UI. No formal release tag was cut before phase-j-du.
