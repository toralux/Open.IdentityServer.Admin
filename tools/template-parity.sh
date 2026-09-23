#!/usr/bin/env bash
#
# tools/template-parity.sh — Gate A: template snapshot equivalence with src
# =============================================================================
# Fails (exit 1) when any file under templates/template-publish/content/src
# diverges from its mapped counterpart in src/, beyond the entries allowlisted
# in tools/template-parity.allowlist.
#
# The template is a frozen SUBSET of the solution (7 app-facing projects; the
# library projects ship as NuGet packages). Every src change to a mapped file
# MUST be mirrored here — this gate makes a forgotten mirror a build failure.
#
# Normalization applied before diffing (expected, mechanical differences):
#   - collapsed template names  ->  dotted src names
#     (ToraluxOpen.IdentityServerAdmin.X  ->  Toralux.Open.IdentityServer.X)
#   - GUIDs -> @@GUID@@ (template regenerates them)
#   - trailing \r stripped
# Usage: tools/template-parity.sh [--verbose]
set -euo pipefail
cd "$(git rev-parse --show-toplevel)"

declare -A MAP=(
  ["ToraluxOpen.IdentityServerAdmin.Admin"]="Toralux.Open.IdentityServer.Admin"
  ["ToraluxOpen.IdentityServerAdmin.Admin.Api"]="Toralux.Open.IdentityServer.Admin.Api"
  ["ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL"]="Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL"
  ["ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared"]="Toralux.Open.IdentityServer.Admin.EntityFramework.Shared"
  ["ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer"]="Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer"
  ["ToraluxOpen.IdentityServerAdmin.STS.Identity"]="Toralux.Open.IdentityServer.STS.Identity"
  ["ToraluxOpen.IdentityServerAdmin.Shared"]="Toralux.Open.IdentityServer.Shared"
)
TPL_ROOT="templates/template-publish/content/src"
BASELINE="tools/template-parity.baseline"

normalize() { # stdin -> stdout: make template text comparable to src text
  sed -e 's|ToraluxOpen\.IdentityServerAdmin\.Admin\.EntityFramework\.PostgreSQL|Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin\.Admin\.EntityFramework\.SqlServer|Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin\.Admin\.EntityFramework\.Shared|Toralux.Open.IdentityServer.Admin.EntityFramework.Shared|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin\.STS\.Identity|Toralux.Open.IdentityServer.STS.Identity|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin\.Admin\.Api|Toralux.Open.IdentityServer.Admin.Api|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin\.Shared|Toralux.Open.IdentityServer.Shared|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin\.Admin|Toralux.Open.IdentityServer.Admin|g' \
      -e 's|ToraluxOpen\.IdentityServerAdmin|Toralux.Open.IdentityServer|g' \
      -e 's/[0-9a-fA-F]\{8\}-[0-9a-fA-F]\{4\}-[0-9a-fA-F]\{4\}-[0-9a-fA-F]\{4\}-[0-9a-fA-F]\{12\}/@@GUID@@/g' \
      -e 's/
$//' | sort # sort: tolerate declaration-order churn in generated files
}

drifts=0; checked=0; baseline=0
for tpl_proj in "${!MAP[@]}"; do
  src_proj="${MAP[$tpl_proj]}"
  while IFS= read -r -d '' f; do
    rel="${f#"$TPL_ROOT/$tpl_proj/"}"
    rel_src="$rel"
    case "$rel" in *.csproj) rel_src="${tpl_proj}.csproj"; rel_src="src/$src_proj/${MAP[$tpl_proj]}.csproj"; src_f="$rel_src";; *) src_f="src/$src_proj/$rel";; esac
    checked=$((checked+1))
    if [ ! -f "$src_f" ]; then
      echo "PARITY-FAIL (no src counterpart): $tpl_proj/$rel"
      drifts=$((drifts+1)); continue
    fi
    if ! diff -q <(normalize < "$f") <(normalize < "$src_f") >/dev/null 2>&1; then
      if grep -qxF "$tpl_proj/$rel" "$BASELINE" 2>/dev/null; then
        baseline=$((baseline+1))
      else
        echo "PARITY-FAIL (new drift not in baseline): $tpl_proj/$rel"
        drifts=$((drifts+1))
      fi
    fi
  done < <(find "$TPL_ROOT/$tpl_proj" -type f \( -name '*.cs' -o -name '*.csproj' -o -name '*.json' -o -name '*.js' -o -name '*.ts' -o -name '*.tsx' \) -print0)
done

echo "---"
echo "checked: $checked files | new-drift: $drifts | inherited-baseline: $baseline ($(wc -l < "$BASELINE") recorded)"
if [ "$drifts" -gt 0 ]; then
  echo "GATE A: FAIL — NEW asymmetry vs the recorded baseline. Mirror the src change into template content, or (intentional divergence only) regenerate the baseline in the same commit with justification."
  exit 1
fi
echo "GATE A: PASS — template snapshot is equivalent to src"
