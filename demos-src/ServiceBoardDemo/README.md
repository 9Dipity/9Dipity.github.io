# Service Board Demo — Baltic Motor Works

A self-contained sales demo built by DataCraft Consulting. It is screen-shared live on
sales calls and linked from cold emails to workshops/repair shops — it is **not** a real
product and uses 100% fake, seeded data with zero real backend or payment integration.

## What it demonstrates

**"Baltic Motor Works"** is a fictional auto service center. The whole app is a Blazor
WebAssembly SPA that runs entirely in the visitor's browser; there is no server, no
database, no HTTP calls for app data.

This is DataCraft's third sales demo, and deliberately a different shape of problem than
the first two (a booking calendar, a multi-channel stock sync) — it demonstrates
**job/work-order tracking**, which fits any SMB that runs job-based service work, not
just auto repair: workshops, contractors, appliance/electronics repair, print shops. It
opens a genuinely new outreach segment rather than re-pitching the same two problems.

Two concrete pain points are dramatized:

1. **A paper job board (or a whiteboard) doesn't update itself, and nobody upstream can
   see it.** The board (`/`) is a live kanban of every active job — Intake, Diagnosis,
   Awaiting Parts, In Progress, Ready for Pickup. Click "Advance →" on any card and watch
   it move columns instantly, with a flash so the change is obvious even from across the
   room. Add a new job with "+ New Job" and it appears in Intake immediately. That's the
   pitch: one board everyone on the floor reads from, not a whiteboard only visible to
   whoever's standing next to it.
2. **Parts blocking a job are invisible until pickup day.** A job that needs a part not
   currently in stock automatically lands in "Awaiting Parts" (jobs with no missing parts
   skip that column entirely) and shows a "waiting on parts" flag. The Parts page
   (`/parts`) aggregates every part blocking *any* active job into one list — quantity
   needed and which job numbers are waiting — instead of checking each ticket by hand.
   Click "Mark Received" and the job unblocks on the board immediately (the technician
   still clicks Advance manually — the tool surfaces the block, it doesn't decide the
   job is done). That's the pitch: shop-wide visibility instead of tribal knowledge.

The UI has an EN/LV language toggle (defaults to Latvian) since the primary audience is
Riga-area workshops.

## Project layout

```
demos-src/ServiceBoardDemo/
  ServiceBoardDemo.sln
  src/ServiceBoardDemo.Core/      class library — models, IJobBoardService, localization
  src/ServiceBoardDemo.Client/    Blazor WebAssembly standalone app (references Core)
  tests/ServiceBoardDemo.Tests/   xUnit tests against Core (no browser involved)
```

Key files in Core:
- `Models/` — `RepairJob`, `JobStatus` (the 5-stage pipeline), `PartRequirement`,
  `PartDemand` (aggregated view for the Parts page)
- `Services/JobBoardService.cs` — the shared in-memory job board (singleton), raises
  `JobsChanged` + exposes `LastChangedJobId` so both pages re-render and know which card
  to flash. `AdvanceStatus` walks a fixed pipeline order and auto-skips `AwaitingParts`
  when a job has no parts or all its parts are already in stock; advancing past `Ready`
  completes the job and removes it from the active board.
- `Services/SeedData.cs` — 10 fictional jobs spread across every column, including two
  genuinely blocked `AwaitingParts` jobs
- `Localization/DictionaryLocalizer.cs` — EN/LV string dictionary + `LanguageChanged`
  event, same "subscribe in OnInitialized, StateHasChanged" pattern as job-board changes

Key files in Client:
- `Pages/Board.razor` (route `/`) — the kanban board, new-job intake form, reset button
- `Pages/Parts.razor` (route `/parts`) — aggregated parts-demand table with a
  "Mark Received" action per part
- `Shared/JobCard.razor` — one job's card: vehicle/customer/issue, technician, estimate,
  blocked flag, and the Advance/Mark Picked Up button
- `Layout/MainLayout.razor` — header, nav, EN/LV toggle, footer disclaimer banner

## Running locally

```bash
dotnet run --project src/ServiceBoardDemo.Client
```

The source `wwwroot/index.html` and `404.html` are kept at `<base href="/" />` so
`dotnet run` serves correctly from the dev server root. The production
`<base href="/demo/servisa-panelis/" />` is applied only to the copy published into
`demo/servisa-panelis/` (see "Publishing" below) — don't hand-edit the base href back to
the production path in the source tree, or local dev will 404 on `_framework/*` and
`css/app.css`.

## Running tests

```bash
dotnet test
```

Covers (in `ServiceBoardDemo.Tests`, against `ServiceBoardDemo.Core` directly, no browser
involved):
- A new job lands in `Intake` and fires `JobsChanged`
- New jobs get unique, sequential job numbers
- `AdvanceStatus` skips `AwaitingParts` entirely when a job has no blocking parts
- `AdvanceStatus` stops at `AwaitingParts` when a job genuinely has an out-of-stock part
- Advancing a job past `Ready` removes it from the active board and increments
  `CompletedTodayCount`
- `MarkPartReceived` clears the blocked flag but does **not** auto-advance the job's
  status — the technician still has to click Advance
- `MarkPartReceived` only affects jobs that need that exact part name, not every blocked
  job
- `GetPartsDemand` sums quantity and lists job numbers correctly, and excludes parts
  already in stock
- `GetPartsDemand` excludes completed (removed) jobs
- `Reset` restores the seeded job count, clears `CompletedTodayCount`, and clears
  `LastChangedJobId`
- Seed data has at least one job in every pipeline status, and every seeded
  `AwaitingParts` job is genuinely blocked (not just cosmetically in that column)

Last local run: **12/12 passed.**

## Resetting demo data

Click **"Reset Demo Data"** in the top-right corner of the board (`/`). It calls
`IJobBoardService.Reset()`, which restores the 10 seeded jobs and their original statuses
and clears `CompletedTodayCount`. No page reload needed — it's a live in-memory reset.

## Publishing / GitHub Pages deployment

```bash
dotnet publish demos-src/ServiceBoardDemo/src/ServiceBoardDemo.Client -c Release
```

This produces
`demos-src/ServiceBoardDemo/src/ServiceBoardDemo.Client/bin/Release/net8.0/publish/wwwroot/`.
Copy the **contents** of that `wwwroot` folder (not the folder itself) into
`demo/servisa-panelis/` at the repo root, so `index.html`, `404.html`, `_framework/`,
`css/` end up directly under `demo/servisa-panelis/`, then rewrite the base href from the
dev default (`/`) to the production deploy path in both HTML files:

```bash
rm -rf demo/servisa-panelis && mkdir -p demo/servisa-panelis
cp -r demos-src/ServiceBoardDemo/src/ServiceBoardDemo.Client/bin/Release/net8.0/publish/wwwroot/. demo/servisa-panelis/
sed -i 's#<base href="/" />#<base href="/demo/servisa-panelis/" />#' demo/servisa-panelis/index.html demo/servisa-panelis/404.html
```

The site is served statically by GitHub Pages at
`https://datacraft.lv/demo/servisa-panelis/` with no build step and no server-side
rewrites, so the deployed output relies on:

- **Rewritten base href** in the deployed `demo/servisa-panelis/index.html` and
  `404.html`: `<base href="/demo/servisa-panelis/" />` (source tree keeps
  `<base href="/" />` for local dev — see "Running locally" above)
- **`noindex, nofollow`** meta tag in `wwwroot/index.html`'s `<head>` (belt-and-suspenders
  alongside the site-wide `Disallow: /demo/` in `robots.txt`, which already covers this
  path)
- **SPA-on-GitHub-Pages redirect trick** for deep links (e.g. a direct load/refresh of
  `/demo/servisa-panelis/parts`, which GitHub Pages would otherwise 404 on since it has no
  server-side rewrite):
  - `wwwroot/404.html` — a copy of `index.html` with a redirect script
    (`pathSegmentsToKeep = 2`) inserted as the very first thing in `<head>`, which
    repoints the browser to the SPA root with the original path encoded in the query
    string
  - `wwwroot/index.html` — a matching decode script in `<head>`, before the Blazor
    `<script src="_framework/blazor.webassembly.js">` tag, which restores the real path
    via `history.replaceState` before the Blazor router takes over
- **Footer disclaimer** on every page (in `Layout/MainLayout.razor`): *"This is a demo
  built by DataCraft Consulting to illustrate what's possible — not a real client's
  system or data."*

Verified end-to-end in a real browser after publishing: advancing jobs through the
pipeline, the auto-skip of Awaiting Parts on parts-free jobs, the cross-page unblock
(Parts page → Mark Received → the same job's blocked flag clears on the board), the new
job intake form, the EN/LV toggle, and Reset Demo Data. No console errors.

## Deviations from the two earlier demos

- `AdvanceStatus` is a single linear pipeline with automatic column-skipping, rather than
  free-form drag-and-drop — chosen so the board works identically on touch/tablet (common
  in a service bay) without a drag library, and so the demo can't be left in a confusing
  half-dragged state on a sales call.
- Marking a part received deliberately does **not** auto-advance the job's status. A part
  arriving is shop-wide information; moving a specific ticket forward is still a decision
  a technician makes. Blurring that line would make the tool feel like it's guessing at
  work that hasn't actually happened.
