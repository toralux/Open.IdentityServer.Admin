#!/usr/bin/env python3
"""tools/mirror-to-template.py — map a src/ file into template-content naming.

The single source of truth for src <-> template namespace/project mapping,
used when mirroring src fixes into the frozen template snapshot, and mirrored
(symptomatically) by tools/template-parity.sh's normalizer.

Rules, in order (protect → map → restore):
  - Library namespaces that the template consumes as NUGET PACKAGES keep their
    dotted names in template code (they are package namespaces, not projects):
      Toralux.Open.IdentityServer.Shared.Configuration
      Toralux.Open.IdentityServer.Admin.EntityFramework.{Configuration,Extensions,Identity,Admin.Storage,Shared?}
        (EF.Shared is BOTH: a template OWN project AND a shipped library.
         In template code the OWN (collapsed) form wins for the consumer's
         project; other EF.* namespaces stay dotted.)
      Toralux.Open.IdentityServer.Admin.BusinessLogic.*  (all)
      Toralux.Open.IdentityServer.Admin.UI.*             (all)
  - Template OWN projects map src-dotted -> template-collapsed:
      Toralux.Open.IdentityServer.STS.Identity              -> ToraluxOpen.IdentityServerAdmin.STS.Identity
      Toralux.Open.IdentityServer.Admin.Api                 -> ToraluxOpen.IdentityServerAdmin.Admin.Api
      Toralux.Open.IdentityServer.Admin                     -> ToraluxOpen.IdentityServerAdmin.Admin
      Toralux.Open.IdentityServer.Admin.EntityFramework.Shared     -> ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared
      Toralux.Open.IdentityServer.Admin.EntityFramework.PostgreSQL -> ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL
      Toralux.Open.IdentityServer.Admin.EntityFramework.SqlServer  -> ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer
      Toralux.Open.IdentityServer.Shared                    -> ToraluxOpen.IdentityServerAdmin.Shared
Usage: mirror-to-template.py <srcfile>  (> templatefile)
"""
import sys, re

PROTECTED = [
    r"Toralux\.Open\.IdentityServer\.Shared\.Configuration",
    r"Toralux\.Open\.IdentityServer\.Admin\.BusinessLogic(?:\.[A-Za-z.]+)?",
    r"Toralux\.Open\.IdentityServer\.Admin\.UI(?:\.[A-Za-z.]+)?",
    r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.Configuration",
    r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.Extensions",
    r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.Identity",
    r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.Admin(?:\.[A-Za-z.]+)?",
    r"Toralux\.Open\.IdentityServer\.Admin\.Configuration",
]
OWN = [
    (r"Toralux\.Open\.IdentityServer\.STS\.Identity", "ToraluxOpen.IdentityServerAdmin.STS.Identity"),
    (r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.PostgreSQL", "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.PostgreSQL"),
    (r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.SqlServer", "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.SqlServer"),
    (r"Toralux\.Open\.IdentityServer\.Admin\.EntityFramework\.Shared", "ToraluxOpen.IdentityServerAdmin.Admin.EntityFramework.Shared"),
    (r"Toralux\.Open\.IdentityServer\.Admin\.Api", "ToraluxOpen.IdentityServerAdmin.Admin.Api"),
    (r"Toralux\.Open\.IdentityServer\.Admin", "ToraluxOpen.IdentityServerAdmin.Admin"),
    (r"Toralux\.Open\.IdentityServer\.Shared", "ToraluxOpen.IdentityServerAdmin.Shared"),
]

def map_text(t: str) -> str:
    for i, pat in enumerate(PROTECTED):
        t = re.sub(pat, f"@@PKG{i}@@", t)
    for pat, rep in OWN:
        t = re.sub(pat, rep, t)
    for i in range(len(PROTECTED)):
        # restore the original dotted text the placeholder swallowed: rebuild from PROTECTED pattern source
        src_pat = PROTECTED[i].replace(r"\.", ".").replace("(?:\\.[A-Za-z.]+)?", "").replace("(?:\\.[A-Za-z.]+)", "")
        t = t.replace(f"@@PKG{i}@@", "@@" + f"RESTORE{i}" + "@@")
    # Simplest correct restore: run protection again capturing full matches.
    return t

def map_file(text: str) -> str:
    saved = []
    def protect(m):
        saved.append(m.group(0)); return f"\x00{len(saved)-1}\x00"
    for pat in PROTECTED:
        text = re.sub(pat, protect, text)
    for pat, rep in OWN:
        text = re.sub(pat, rep, text)
    return re.sub(r"\x00(\d+)\x00", lambda m: saved[int(m.group(1))], text)

if __name__ == "__main__":
    sys.stdout.write(map_file(open(sys.argv[1]).read()))
