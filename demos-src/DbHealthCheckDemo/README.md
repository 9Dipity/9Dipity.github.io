# DB Health Check Demo — Database Diagnostic Preview

A self-contained sales demo built by DataCraft Consulting. It is screen-shared live on
sales calls and linked from `services.html` and cold emails — it is **not** a real
product and uses 100% fake, seeded data with zero real backend or database connection.

## What makes this demo different from the other three

The clinic booking, inventory sync, and service board demos each illustrate **software
we'd build for a client**. This one doesn't — it illustrates **the audit service itself**.
DataCraft's named tiers (Quick Check / Standard Audit / Deep Dive, described on
`services.html`) are otherwise just labels until a prospect has actually been through
one. This demo makes them concrete without needing a real client's database.

## What it demonstrates

The whole app is a Blazor WebAssembly SPA that runs entirely in the visitor's browser;
there is no server, no database, no HTTP calls for app data.

The core design problem this demo solves: a diagnostic demo is easy to make feel fake.
A booking calendar or a job board is concrete — customers, vehicles, appointments. A SQL
diagnostic is about query plans and index stats, which either reads as jargon or as a
canned "the problem is always critical" script. Two choices avoid that:

1. **The diagnostic engine is real, testable rule logic**, not a scripted animation. Six
   `IDiagnosticRule` implementations (`DbHealthCheckDemo.Core/Rules/`) each evaluate one
   dimension against a `DatabaseProfile`'s numbers — missing/inadequate indexing, index
   fragmentation (using SQL Server's own documented 10%/30% reorganize/rebuild bands),
   backup coverage, slow queries, schema staleness, and concurrency risk from table
   scans. `DiagnosticEngine` runs all six and maps the result to a recommended tier.
2. **Two of the inputs are visitor-adjustable and genuinely change the result.** On the
   Audit page, after picking a sample scenario, the visitor can change the largest
   table's row count and toggle automated backups on/off, then re-run. The findings
   recompute for real — `DiagnosticEngineTests.AdjustingRowCountAndRerunning_...`
   specifically asserts a finding's severity actually flips when the input changes, and
   `TogglingBackupAutomation_...` asserts the same for the backup toggle. This is the
   demo's proof that it isn't hand-wavy.

Three seeded scenarios are tied to the same three verticals as the site's other
illustrative case studies (retail, clinic, distribution) and are deliberately calibrated
to land at three different tiers — Quick Check, Standard Audit, and Deep Dive
respectively — so the demo shows the engine actually discriminating between a mostly
healthy system and a badly overdue one, not always crying wolf.
`SeededScenarios_AreNotAllTheSameSeverity` guards against that regressing.

A "How this scoring works" panel on the page states the actual thresholds in plain
language, reinforcing the same transparent/scrutinized positioning as the rest of the
redesigned site.

The UI has an EN/LV language toggle (defaults to Latvian).

## Project layout

```
demos-src/DbHealthCheckDemo/
  DbHealthCheckDemo.sln
  src/DbHealthCheckDemo.Core/      class library — models, rules, engine, localization
  src/DbHealthCheckDemo.Client/    Blazor WebAssembly standalone app (references Core)
  tests/DbHealthCheckDemo.Tests/   xUnit tests against Core (no browser involved)
```

Key files in Core:
- `Models/DatabaseProfile.cs` — the seeded stand-in for what a real audit gathers before
  drawing any conclusion. `LargestTableRowCount`, `HasAutomatedBackupJob`, and
  `DaysSinceLastFullBackup` are the three mutable fields the demo UI adjusts.
- `Models/Finding.cs` — a rule's verdict. Carries no localized text: `RuleId` + `Variant`
  select which dictionary template the Client renders, `Args` are the raw numbers
  plugged into that template via `string.Format` — the same separation of concerns the
  other two demos use for parameterized strings, just applied more heavily here since
  every finding is a sentence built from live numbers.
- `Rules/` — one class per dimension (`MissingIndexRule`, `FragmentationRule`,
  `BackupRecencyRule`, `SlowQueryRule`, `SchemaStalenessRule`, `ConcurrencyRiskRule`),
  each a pure function from `DatabaseProfile` to `Finding`.
- `Services/DiagnosticEngine.cs` — runs all six rules and maps the result to
  `AuditTier`: 2+ critical findings → Deep Dive; any critical, or 3+ warnings →
  Standard Audit; otherwise → Quick Check (including an all-clear result).
- `Services/SampleProfiles.cs` — the three seeded scenarios.
- `Localization/DictionaryLocalizer.cs` — EN/LV dictionary, same pattern as the other two
  demos, plus the finding sentence templates described above.

Key files in Client:
- `Pages/Audit.razor` (route `/`, the only page) — scenario picker, adjustable inputs,
  the step-by-step findings reveal (each finding appears ~380ms after the last, so a
  visitor watches the check actually run rather than seeing a result dump appear), the
  tier-recommendation banner, and the methodology panel.
- `Shared/FindingCard.razor` — renders one `Finding` by resolving and formatting its
  localized template from `RuleId`/`Variant`/`Args`.

## Running locally

```bash
dotnet run --project src/DbHealthCheckDemo.Client
```

The source `wwwroot/index.html` and `404.html` are kept at `<base href="/" />` so
`dotnet run` serves correctly from the dev server root. The production
`<base href="/demo/datubazes-audits/" />` is applied only to the copy published into
`demo/datubazes-audits/` (see "Publishing" below).

## Running tests

```bash
dotnet test
```

Covers (in `DbHealthCheckDemo.Tests`, against `DbHealthCheckDemo.Core` directly, no
browser involved):
- Each rule's severity thresholds at their exact boundaries (e.g. fragmentation at 9%
  vs. 10% vs. 29% vs. 30%)
- `BackupRecencyRule`'s three variants (no automated job / automated and recent /
  automated but stale)
- `ConcurrencyRiskRule` requires both a high table-scan percentage *and* enough
  concurrent users — high scan percentage alone with few users stays Ok
- `DiagnosticEngine`'s tier mapping at each boundary (all-healthy → Quick Check, exactly
  one critical → Standard Audit, three warnings with no criticals → Standard Audit, two
  criticals → Deep Dive)
- **Adjusting the row count or the backup toggle and re-running genuinely changes the
  relevant finding's severity** — the demo's core honesty claim, tested directly
- The three seeded scenarios land at their intended tier (retail → Quick Check, clinic →
  Standard Audit, distribution → Deep Dive) and aren't all the same severity

Last local run: **34/34 passed.**

## Publishing / GitHub Pages deployment

```bash
dotnet publish demos-src/DbHealthCheckDemo/src/DbHealthCheckDemo.Client -c Release
```

This produces
`demos-src/DbHealthCheckDemo/src/DbHealthCheckDemo.Client/bin/Release/net8.0/publish/wwwroot/`.
Copy the **contents** of that `wwwroot` folder (not the folder itself) into
`demo/datubazes-audits/` at the repo root, then rewrite the base href from the dev
default (`/`) to the production deploy path in both HTML files:

```bash
rm -rf demo/datubazes-audits && mkdir -p demo/datubazes-audits
cp -r demos-src/DbHealthCheckDemo/src/DbHealthCheckDemo.Client/bin/Release/net8.0/publish/wwwroot/. demo/datubazes-audits/
sed -i 's#<base href="/" />#<base href="/demo/datubazes-audits/" />#' demo/datubazes-audits/index.html demo/datubazes-audits/404.html
```

Same production pattern as the other two demos: `noindex, nofollow` in
`wwwroot/index.html` (belt-and-suspenders alongside the site-wide `Disallow: /demo/` in
`robots.txt`), and the SPA-on-GitHub-Pages 404 redirect trick (`pathSegmentsToKeep = 2`)
kept for parity even though this demo is currently single-page.

Verified end-to-end in a real browser after publishing: selecting each scenario,
running the check and watching findings reveal one at a time, adjusting row count and
backup toggle and re-running (confirmed a finding's severity and text both changed live),
the methodology panel, and the EN/LV toggle. No console errors.

## Where it's linked

- `services.html` (EN+LV), next to the Quick Check / Standard Audit / Deep Dive tier
  descriptions — its natural home, since it demonstrates that service directly.
- `case-studies.html` (EN+LV), as a fifth card explicitly framed differently from the
  other four ("What a Quick Check finds" rather than "What we'd build"), since it isn't
  illustrating client software.
