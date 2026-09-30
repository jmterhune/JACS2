# JACS

The Judicial Automated Calendaring System DotNetNuke module for the Twelfth
Judicial Circuit. Builds to `tjc.Modules.JACS.dll` and installs as the DNN
package `JACS`.

The module's files live at the root of this repository: views (`*.ascx`) and
their code-behind, `Components/`, `Services/`, `Handlers/`, `Scheduled/` and
`js/`.

## This remote holds two unrelated project histories

`master` and every other branch on this remote come from **different root
commits and share no history at all**. `git merge-base` between them returns
nothing, and of the 530 files on `master` and 1,787 on the other lineage, the
only path they have in common is `.gitignore`.

| Lineage | Root commit | Started | Branches | Shape |
| --- | --- | --- | --- | --- |
| This module | `990c12a1c` | 2025-07-09 | `master` | one module, files at the repository root |
| Older solution | `a30d5347` | 2022-10-19 | `DNN-upgrae-10.3.3`, `expert-witness-randomization`, `standardization-refactor`, `Refactor-Floyd`, `Ed-Wilson-Changes`, `claude/goofy-lichterman` | several modules, each in its own folder (`Arbitrators/`, ...) with its own `.csproj` and `.dnn` |

### What this means when auditing branches

A branch listing makes the second lineage look like a pile of stale feature
branches that never landed. It is not. Because there is no shared history:

- **"Not merged into master" means nothing here.** There is no merge base, so
  those branches can never report as merged no matter what they contain.
- **The ahead/behind counts are not divergence.** `git rev-list --count
  master...<branch>` returns each side's *entire* history — for example 82/35,
  not 82 commits of unmerged work.
- **Merging one into master would be wrong.** It needs
  `--allow-unrelated-histories` and would drop a second, differently-arranged
  copy of the codebase alongside this one.
- **They are not necessarily abandoned.** `DNN-upgrae-10.3.3` was still taking
  commits in September 2026.

So do not delete a branch on the strength of a "not merged" report. Decide
instead whether the older solution is still wanted, and treat that as a separate
question from anything on `master`.

`Pre-Permision-Refactor` was the one branch that *did* descend from this
module's root. It was reviewed and removed on 2026-09-30; its tip was
`99ca90e68efd73612fb21a305aed65e1c4d1f9d0` if it is ever needed again.

## Database

Schema for the JACS2 database is not part of the DNN package — DNN runs
`.SqlDataProvider` scripts against the DotNetNuke database, and JACS2 is
separate. Post-deployment scripts and how to run them are in
[`Database/JACS2/README.md`](Database/JACS2/README.md).
