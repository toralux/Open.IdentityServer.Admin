# Contributing to Toralux.Open.IdentityServer.Admin

This repository is a fork of [skoruba/Duende.IdentityServer.Admin](https://github.com/skoruba/Duende.IdentityServer.Admin)
rebased on the [Open.IdentityServer](https://github.com/toralux/Open.IdentityServer) token service.
It is not affiliated with or endorsed by Rock Solid Knowledge / IdentityServer.com.

Commits follow [Conventional Commits](https://www.conventionalcommits.org/) (`feat:`, `fix:`,
`refactor:`, `build:`, `docs:`, `chore:`), one logical change per commit. Nothing is pushed
without explicit owner approval.

## Upstream sync — the replay workflow

The fork identity (namespaces, package ids, npm names, template names, seed defaults) is
applied by a **scripted, idempotent mechanical pass**: [`tools/rename.sh`](tools/rename.sh).
Semantic fork changes (package swaps, feature gates, removed pages) live as ordinary commits
on top. Absorbing a new upstream release means replaying that structure:

```bash
# 0. Remotes (once): origin = toralux/Open.IdentityServer.Admin,
#                   upstream = skoruba/Duende.IdentityServer.Admin
git fetch upstream

# 1. Start from the upstream tree, not from main
git checkout -b sync/upstream-<YYYY-MM-DD> upstream/main

# 2. Mechanical identity pass (namespaces, file/dir names, npm/docker/template
#    identifiers, seed defaults). Commit it as one mechanical commit:
tools/rename.sh
git add -A && git commit -m "refactor: replay upstream rename pass (upstream <sha>)"

# 3. Replay the semantic fork commits on top. Find the commit that ended the
#    previous sync, then:
git rebase --onto sync/upstream-<YYYY-MM-DD> <previous-sync-tag-or-sha> main
#    (equivalently cherry-pick the semantic series). Resolve conflicts by hand —
#    the recurring hot spots are listed under "Semantic follow-ups" below.

# 4. Gates — all four must pass before a sync branch is merged:
dotnet build Toralux.Open.IdentityServer.Admin.sln          # 0 errors, 0 warnings
npm install && npm run build                                # in src/Toralux.Open.IdentityServer.Admin.UI.Client
tools/rename.sh && git status --porcelain                   # re-run must yield zero diff (idempotency)
grep -rIE 'Skoruba\.Duende|skoruba-duende|@skoruba' . \
  --exclude-dir={.git,node_modules,bin,obj} \
  --exclude=CHANGELOG.md --exclude=rename.sh               # only attribution matches may remain

# 5. Squash-merge or fast-forward per preference; tag the sync point:
git tag sync/upstream-<upstream-version>
```

### What `tools/rename.sh` protects (never rewritten)

- Copyright notice lines (Apache 2.0 §4(c)) — attribution is preserved verbatim.
- Upstream attribution links (`github.com/skoruba/.../{issues,pull,graphs,blob,tree}`).
- `CHANGELOG.md` — history keeps the package names it shipped under.
- Provenance comments (`originally Duende...`).
- External upstream packages that are real dependencies, not fork identity:
  `Skoruba.AuditLogging.*`, `Duende.IdentityModel.*`, `Duende.AccessTokenManagement.*`,
  `DuendeSoftware/...` links, and `jan@skoruba.com` author fields.

### Semantic follow-ups (manual, after every sync)

The script deliberately does not decide semantics. After replaying it, check:

1. **Package versions** — upstream bumps `Duende.IdentityServer.*` versions; the fork uses
   `Open.IdentityServer` 2.x. Align `Directory.Packages.props` / csproj versions by hand.
2. **Feature gates** — Duende-only features (IdentityProvider management, Keys management,
   PAR/DPoP/CIBA where Business-Edition-bound) are stripped or gated in the fork. If upstream
   touched those areas, re-apply the strip: server DTOs/controllers, React pages/routes/menu/i18n,
   and their tests (see commits `bd67dfb0`, `dedfd76d`, `75a7e6cb` for the shape).
3. **NSwag TS client** — if the Admin.Api surface changed, regenerate
   (`src/Toralux.Open.IdentityServer.Admin.Api/nswag.json`) and rebuild
   `TypescriptClient/dist` (git-tracked; the React app consumes it via a `file:` dependency).
4. **Template pack** — template `nuspec` version, `template.json` defaults, and the
   `toralux.open-isadmin` short name; validate with `dotnet new install` + build (Phase 2b pipeline).
5. **Seed defaults** — fork seeds use `toralux_identity_admin_*` client ids and
   `admin@example.com`; confirm upstream seed changes didn't reintroduce old defaults.

## Local development

```bash
dotnet build Toralux.Open.IdentityServer.Admin.sln        # full solution incl. tests
dotnet test  Toralux.Open.IdentityServer.Admin.sln        # SQL Server integration tests need a container
npm install && npm run build                              # React admin UI (src/.../UI.Client)
```

Integration tests requiring SQL Server run against a Docker container (Phase 2b wires the
CI matrix; locally `docker compose up sqlserver` or an equivalent instance works).
