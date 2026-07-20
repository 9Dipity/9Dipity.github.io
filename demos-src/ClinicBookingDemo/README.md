# Clinic Booking Demo — Rīgas Smaids Clinic

A polished, fully working **sales demo** built by DataCraft Consulting to show clinics what a
modern, self-service booking system looks like. Everything is fictional and runs entirely in
the browser — there is no real backend, no real email/SMS, and no real patient data.

It is built and published as a Blazor WebAssembly app so it can be screen-shared on a sales
call or sent as a link in a cold email, with zero server infrastructure to run.

## What it demonstrates — and the pain point it maps to

Cold-email pitch: *"Your front desk is booking appointments by phone, on a paper diary or a
single shared spreadsheet. That means callers wait on hold, double-bookings happen when two
people write in the same slot, and there's no single view of who's free right now."*

This demo maps directly onto that pitch:

| Client pain point | What the demo shows |
|---|---|
| Phone-only booking, no self-service | A public booking flow (`/`, `/book`) where a patient picks a service, a specialist, and a real open time slot — no phone call. |
| No shared calendar across staff | The admin dashboard (`/admin`) shows every specialist's schedule side by side, color-coded, on one screen. |
| Double-booking risk | `AvailabilityService` in the Core library makes double-booking structurally impossible — it checks working hours, existing bookings, and manually blocked time before a slot is ever offered. Blocking a slot in the admin view removes it from the public flow immediately, in the same session. |
| "Where's today's schedule?" | The **Today** tab in the admin view is the single-screen "no more double-booking, no more juggling by phone" moment. |

The clinic ("Rīgas Smaids Clinic"), its three specialists, and every booking are fictional
seed data — not a real client's system.

## Project layout

```
demos-src/ClinicBookingDemo/
  ClinicBookingDemo.sln
  src/ClinicBookingDemo.Core/      Class library — models, interfaces, availability logic, seed data, translations
  src/ClinicBookingDemo.Client/    Blazor WebAssembly app (standalone, no server project)
  tests/ClinicBookingDemo.Tests/   xUnit tests for the Core availability logic
  README.md
```

### Core (`ClinicBookingDemo.Core`)

- **Models**: `Specialist`, `Service`, `Booking`, `BlockedSlot`, `DayHours`, `TimeSlot`, `Language`.
- **`IAvailabilityService`** — pure, browser-free logic. `GetDaySlots` / `GetAvailableSlots`
  return bookable windows for a specialist/service/day; `CanBook` checks working hours,
  overlapping bookings, and blocked time. No Blazor dependency, so it's testable from plain xUnit.
- **`IClinicDataStore`** — in-memory storage for specialists, services, bookings, and blocked
  slots, seeded on startup. Raises a `Changed` event so the UI re-renders live when the admin
  view changes something the public view depends on. `Reset()` restores the original seed data
  (wired to the "Reset Demo Data" button in `/admin`).
- **`ITranslationService`** — a small dictionary-based LV/EN translation service (default
  language is Latvian). All UI copy, service names, specialist titles/bios, and blocked-slot
  reasons are translation keys, not hardcoded strings, so the whole app switches language live.
- Everything is registered via constructor injection in `Program.cs`
  (`AddSingleton<IClinicDataStore, InMemoryClinicDataStore>()`, etc.) — no static/singleton
  service-locator patterns.

### Client (`ClinicBookingDemo.Client`)

- **Public booking flow** — `/` (landing page) and `/book` (5-step wizard: service → specialist
  → time slot → patient details → confirmation with a mock confirmation-email preview).
- **Admin view** — `/admin`, linked only via a small footer link (not the main nav, no auth —
  it's a demo). Tabs for **Today** (single-screen schedule across all specialists),
  **Upcoming** (all future bookings, color-coded by specialist), and **Blocked time**
  (block/unblock a specialist's time — blocking removes it from `/book` immediately).
- Visual design is a custom calm medical/wellness look (soft surfaces, teal accent, rounded
  cards) in `wwwroot/css/app.css` — no Bootstrap, no default Blazor template styling.

## Running locally

```bash
dotnet run --project demos-src/ClinicBookingDemo/src/ClinicBookingDemo.Client
```

Then open the URL it prints (e.g. `http://localhost:5289`). The source `wwwroot/index.html` and
`404.html` are kept at `<base href="/" />` so `dotnet run` serves correctly from the dev server
root; the production `<base href="/demo/klinikas-pieraksts/" />` is applied only to the copy
published into `demo/klinikas-pieraksts/` (see "Publishing" below) — do not hand-edit the base
href back to the production path in the source tree, or local dev will 404 on `_framework/*`.

## Running tests

```bash
dotnet test demos-src/ClinicBookingDemo
```

Covers `AvailabilityService`: exact-overlap double-booking is rejected, partial-overlap is
rejected, bookings outside working hours are rejected, manually blocked slots are neither
offered nor bookable, a different specialist at the same time succeeds (no false cross-specialist
conflicts), non-overlapping back-to-back bookings both succeed, and cancelled bookings don't
block a slot.

## Resetting demo data

Click **"Reset Demo Data"** in the admin view (`/admin`) — it calls `IClinicDataStore.Reset()`,
which discards any bookings/blocks made during the session and regenerates the seed data
(2–3 specialists, ~20+ bookings spread across the past two weeks and the next two weeks, plus
blocked slots) freshly relative to "today," so the demo never looks stale no matter when it's run.

## Publishing / deploying to GitHub Pages

```bash
dotnet publish demos-src/ClinicBookingDemo/src/ClinicBookingDemo.Client -c Release
```

This produces static output at:

```
demos-src/ClinicBookingDemo/src/ClinicBookingDemo.Client/bin/Release/net8.0/publish/wwwroot/
```

Copy the **contents** of that `wwwroot` folder (not the folder itself) into
`demo/klinikas-pieraksts/` at the repo root, so `index.html`, `404.html`, `_framework/`, and
`css/` sit directly under `demo/klinikas-pieraksts/`, then rewrite the base href from the dev
default (`/`) to the production deploy path in both HTML files:

```bash
rm -rf demo/klinikas-pieraksts && mkdir -p demo/klinikas-pieraksts
cp -r demos-src/ClinicBookingDemo/src/ClinicBookingDemo.Client/bin/Release/net8.0/publish/wwwroot/. demo/klinikas-pieraksts/
sed -i 's#<base href="/" />#<base href="/demo/klinikas-pieraksts/" />#' demo/klinikas-pieraksts/index.html demo/klinikas-pieraksts/404.html
```

The site is then live at `https://datacraft.lv/demo/klinikas-pieraksts/`. GitHub Pages serves
this repo directly with no build step and no server-side rewrites, so a few things are baked
into the published output to make that work:

- `demo/klinikas-pieraksts/index.html` and `404.html` have `<base href="/demo/klinikas-pieraksts/" />`
  rewritten to match the final deploy path (the source tree keeps `<base href="/" />` so
  `dotnet run` works locally — see "Running locally" above).
- `wwwroot/index.html` has `<meta name="robots" content="noindex, nofollow">` in the `<head>` —
  this is a sales demo, not content that should be indexed. (The site's root `robots.txt` also
  already covers this; the demo repeats it locally in case the page is ever linked from
  somewhere that doesn't respect the root file.)
- `wwwroot/404.html` is a copy of `index.html` with the standard
  [SPA-on-GitHub-Pages redirect script](https://github.com/rafgraph/spa-github-pages) inserted
  first in `<head>`, and `index.html` has the matching decode script before
  `blazor.webassembly.js`. Without this, a direct load or refresh of `/demo/klinikas-pieraksts/admin`
  would 404, since GitHub Pages has no server-side rewrite and Blazor WASM is a purely
  client-routed SPA.
- Every page has a small footer disclaimer: *"This is a demo built by DataCraft Consulting to
  illustrate what's possible — not a real client's system or data."*

After copying, re-run the sanity checks above (`grep` for `base href` and `noindex` in the
copied `index.html`) if you're doing this by hand — it's easy to accidentally publish a fresh
`wwwroot` with the template's default `<base href="/" />` if the source `index.html` was ever
reverted.
