#!/usr/bin/env bash
#
# tools/rename.sh — replayable upstream-sync rename pass
# =====================================================
#
# Purpose
# -------
# Mechanically rewrites an upstream (skoruba/Duende.IdentityServer.Admin)
# working tree into its Toralux.Open.IdentityServer.Admin fork identity.
# This is the REPLAY tool for absorbing future upstream releases — NOT a
# one-off. See "Upstream sync" in CONTRIBUTING.md for the full workflow:
#
#     git fetch upstream
#     git checkout -b sync/upstream-<date> upstream/main
#     tools/rename.sh                      # this script (mechanical pass)
#     git add -A && git commit             # mechanical rename commit
#     # ...then rebase/apply semantic commits (package versions, feature
#     #     gates, UI changes) on top, then run build+test gates.
#
# Properties
# ----------
#   * IDEMPOTENT — re-running on a renamed tree produces zero changes.
#   * ORDERED    — rules are applied longest/most-specific first; do not
#                  reorder them casually.
#   * SAFE       — never touches copyright notice lines (Apache 2.0 §4(c)),
#                  upstream attribution links, or CHANGELOG history.
#   * EXTERNAL PACKAGES stay untouched by design (they are real upstream
#     NuGet/npm dependencies, not our identity):
#       Skoruba.AuditLogging.*        (nuget)
#       Duende.IdentityModel.*        (nuget)
#       Duende.AccessTokenManagement  (nuget)
#       DuendeSoftware/* links        (Quickstart.UI attribution)
#
# What it does NOT do (deliberately out of scope — semantic, human-decided):
#   * package version alignment (upstream bumps → Open.IdentityServer 2.x)
#   * feature gates / removed-feature reconciliation
#   * npm dependency spec semantics (e.g. file: vs registry — see commit
#     history; content pass only renames the package *name*)
#
# Usage:  tools/rename.sh          (from anywhere inside the repo)
# Exit:   0 on success; prints a summary of edits/renames.

set -euo pipefail

# ── locate repo ────────────────────────────────────────────────────────────
REPO_ROOT="$(git rev-parse --show-toplevel 2>/dev/null || true)"
if [ -z "${REPO_ROOT}" ]; then
    echo "error: not inside a git repository" >&2
    exit 1
fi
cd "${REPO_ROOT}"

if git ls-files | grep -q ' '; then
    echo "error: tracked filenames containing spaces are not supported" >&2
    exit 1
fi

# ── token map (single source of truth for content AND path renames) ────────
#
# Order matters: most-specific first. Attribution URLs are protected by
# placeholder round-trip so history links keep pointing at upstream.
#
# Line-level protection (branch to end of script — no substitutions):
#   * any line containing "Copyright"/"copyright"  → Apache 2.0 §4(c) notices
#   * any line containing "originally Duende"      → fork provenance notes
#
# The map below is plain sed (ERE). @@ATTR_*@@ placeholders are restored
# verbatim at the end; they cannot occur in real source files.
read -r -d '' MAP <<'SED' || true
/[Cc]opyright/b
/originally Duende/b
s|github\.com/skoruba/Duende\.IdentityServer\.Admin/issues|@@ATTR_ISSUES@@|g
s|github\.com/skoruba/Duende\.IdentityServer\.Admin/pull|@@ATTR_PULL@@|g
s|github\.com/skoruba/Duende\.IdentityServer\.Admin/graphs|@@ATTR_GRAPHS@@|g
s|github\.com/skoruba/Duende\.IdentityServer\.Admin/blob|@@ATTR_BLOB@@|g
s|github\.com/skoruba/Duende\.IdentityServer\.Admin/tree|@@ATTR_TREE@@|g
s|skoruba/Duende\.IdentityServer\.Admin|toralux/Open.IdentityServer.Admin|g
s|SkorubaDuende\.IdentityServerAdmin|ToraluxOpen.IdentityServerAdmin|g
s|SkorubaDuende\.IdentityServer|ToraluxOpen.IdentityServer|g
s|Skoruba\.Duende\.IdentityServer|Toralux.Open.IdentityServer|g
s|Duende\.IdentityServer|Open.IdentityServer|g
s|SkorubaIdentityAdminAdministrator|ToraluxIdentityAdminAdministrator|g
s|Skoruba Duende IdentityServer Admin|Toralux Open IdentityServer Admin|g
s|Skoruba Duende IdentityServer|Toralux Open IdentityServer|g
s|Duende IdentityServer|Open IdentityServer|g
s|@skoruba/duende\.identityserver\.admin\.api\.client|@toralux/open.identityserver.admin.api.client|g
s|skoruba-duende-identity-server-admin|toralux-open-identity-server-admin|g
s|skoruba-duende-identityserver|toralux-open-identityserver|g
s|skoruba\.duende\.identityserver|toralux.open.identityserver|g
s|skoruba\.duende\.isadmin|toralux.open-isadmin|g
s|skoruba\.local|toralux.local|g
s|skoruba_identity_admin|toralux_identity_admin|g
s|skoruba_admin_client_secret|toralux_admin_client_secret|g
s|skoruba-icon|toralux-icon|g
s|"infoTitle": "Skoruba"|"infoTitle": "Toralux"|g
s|@@ATTR_ISSUES@@|github.com/skoruba/Duende.IdentityServer.Admin/issues|g
s|@@ATTR_PULL@@|github.com/skoruba/Duende.IdentityServer.Admin/pull|g
s|@@ATTR_GRAPHS@@|github.com/skoruba/Duende.IdentityServer.Admin/graphs|g
s|@@ATTR_BLOB@@|github.com/skoruba/Duende.IdentityServer.Admin/blob|g
s|@@ATTR_TREE@@|github.com/skoruba/Duende.IdentityServer.Admin/tree|g
SED

# Grep alternation used to select only files that actually contain a token.
read -r -d '' TOKEN_RE <<'GREP' || true
skoruba/Duende\.IdentityServer\.Admin|SkorubaDuende\.IdentityServer|Skoruba\.Duende\.IdentityServer|Duende\.IdentityServer|SkorubaIdentityAdminAdministrator|Skoruba Duende IdentityServer|Duende IdentityServer|@skoruba/duende\.identityserver|skoruba-duende-identity-server-admin|skoruba-duende-identityserver|skoruba\.duende\.identityserver|skoruba\.duende\.isadmin|skoruba\.local|skoruba_identity_admin|skoruba_admin_client_secret|skoruba-icon|"infoTitle": "Skoruba"
GREP

# Files never rewritten (history / vendored):
#   CHANGELOG.md — upstream release history keeps original package names.
EXCLUDE_PATHSPEC=(':(exclude)CHANGELOG.md')

# ── pass 1: file contents ──────────────────────────────────────────────────
CONTENT_HITS=0
EDITED=0

# shellcheck disable=SC2207
HITS=($(git grep -lIE "${TOKEN_RE}" -- . "${EXCLUDE_PATHSPEC[@]}" 2>/dev/null || true))
CONTENT_HITS=${#HITS[@]}

for f in "${HITS[@]}"; do
    if sed -E -i "${MAP}" "${f}"; then
        EDITED=$((EDITED + 1))
    fi
done

# ── pass 2: file & directory names (tracked files; deepest paths first) ────
map_path() {
    local path="$1" out="" comp
    local IFS='/'
    for comp in ${path}; do
        out+="${out:+/}$(printf '%s' "${comp}" | sed -E "${MAP}")"
    done
    printf '%s' "${out}"
}

RENAMED=0
git ls-files | sort -r | while IFS= read -r f; do
    new="$(map_path "${f}")"
    if [ "${f}" != "${new}" ]; then
        mkdir -p "$(dirname "${new}")"
        git mv "${f}" "${new}"
        echo "  rename: ${f} -> ${new}"
    fi
done > /tmp/rename-sh-moves.$$
RENAMED=$(grep -c '^  rename:' /tmp/rename-sh-moves.$$ || true)
cat /tmp/rename-sh-moves.$$
rm -f /tmp/rename-sh-moves.$$

# Remove now-empty directories left behind by the renames.
find . -path ./.git -prune -o -type d -empty -print0 2>/dev/null \
    | xargs -0 -r rmdir 2>/dev/null || true

# ── summary ────────────────────────────────────────────────────────────────
cat <<SUMMARY

rename.sh summary
-----------------
  files containing tokens : ${CONTENT_HITS}
  files rewritten         : ${EDITED}
  paths renamed           : ${RENAMED}
Re-running on an already-renamed tree yields zero changes (idempotent).
SUMMARY
