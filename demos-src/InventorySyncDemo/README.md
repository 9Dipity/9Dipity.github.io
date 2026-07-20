# Inventory Sync Demo — Nordic Fashion House

A self-contained sales demo built by DataCraft Consulting. It is screen-shared live on
sales calls and linked from cold emails to retailers/distributors — it is **not** a real
product and uses 100% fake, seeded data with zero real backend or payment integration.

## What it demonstrates

**"Nordic Fashion House"** is a fictional clothing retailer selling through three
channels — Retail Store A, Retail Store B, and Livestream/Online orders. The whole app
is a Blazor WebAssembly SPA that runs entirely in the visitor's browser; there is no
server, no database, no HTTP calls for app data.

Two concrete pain points are dramatized:

1. **Manual stock tracking across channels goes stale.** Retailers running multiple
   physical stores plus livestream/WhatsApp order-taking typically track stock in
   separate spreadsheets or gut-feel, so overselling and stockouts are common. The
   dashboard (`/`) keeps ONE shared stock pool behind three channel panels. Click
   "Simulate order" in any panel and watch the same SKU's row flash and update
   *instantly* in the catalog table and all three channel panels — because they are all
   reading the same in-memory state, not independent copies. That's the pitch: a real
   sync layer means every channel always shows the truth.
2. **Supplier price lists arrive in inconsistent formats.** Every supplier exports CSVs
   differently — different delimiters, column names, column order, decimal formats. The
   price-list import page (`/price-lists`) ships three sample files from three fictional
   suppliers (Nordic Textiles: comma CSV; Baltic Goods: semicolon CSV with decimal-comma
   prices; Scandi Home: tab-delimited with flexible column order and a currency suffix on
   price) each parsed by its own `IPriceListParser` implementation, normalized against
   the shared catalog, and rendered as a clear up/down price-change table. That's the
   pitch: "we normalize whatever format your suppliers send."

The UI has an EN/LV language toggle (defaults to Latvian) since the primary audience is
Riga-area retailers/distributors.

## Project layout

```
demos-src/InventorySyncDemo/
  InventorySyncDemo.sln
  src/InventorySyncDemo.Core/      class library — models, IStockService, price-list
                                    parsers/resolver, price normalization, localization
  src/InventorySyncDemo.Client/    Blazor WebAssembly standalone app (references Core)
  tests/InventorySyncDemo.Tests/   xUnit tests against Core (no browser involved)
```

Key files in Core:
- `Models/` — `Product`, `Channel`, `StockLevel`, `SaleEvent`, `SupplierPriceListEntry`,
  `NormalizedPriceChange`
- `Services/StockService.cs` — the shared in-memory stock pool (singleton), raises
  `StockChanged` + exposes `LastChangedSku` so every subscribed component re-renders and
  knows which row to flash
- `Services/SeedData.cs` — ~18 fictional SKUs and starting stock (several seeded at/below
  their low-stock threshold so the badge is visible on first load)
- `Parsers/SupplierNordicTextilesParser.cs`, `SupplierBalticGoodsParser.cs`,
  `SupplierScandiHomeParser.cs` — one implementation per fictional supplier format
- `Parsers/PriceListParserResolver.cs` — maps a chosen supplier name to its parser
- `Services/PriceNormalizationService.cs` — matches parsed rows to the catalog by SKU and
  computes Up/Down/Same price direction
- `Localization/DictionaryLocalizer.cs` — EN/LV string dictionary + `LanguageChanged`
  event, same "subscribe in OnInitialized, StateHasChanged" pattern as stock changes

Key files in Client:
- `Pages/Dashboard.razor` (route `/`) — shared catalog table + three `ChannelPanel`s
- `Pages/PriceLists.razor` (route `/price-lists`) — supplier picker, `InputFile` upload,
  normalized results table
- `Shared/ChannelPanel.razor` — one channel's controls, compact live stock list, and
  recent-activity feed; reads/writes through the shared `IStockService`
- `Layout/MainLayout.razor` — header, nav, EN/LV toggle, footer disclaimer banner
- `wwwroot/sample-data/*.csv` — the three sample supplier files (download links are on
  the price-list page)

## Running locally

```bash
dotnet run --project src/InventorySyncDemo.Client
```

The source `wwwroot/index.html` and `404.html` are kept at `<base href="/" />` so
`dotnet run` serves correctly from the dev server root. The production
`<base href="/demo/noliktavas-sinhronizacija/" />` is applied only to the copy published
into `demo/noliktavas-sinhronizacija/` (see "Publishing" below) — don't hand-edit the base
href back to the production path in the source tree, or local dev will 404 on
`_framework/*` and `css/app.css`.

## Running tests

```bash
dotnet test
```

Covers (in `InventorySyncDemo.Tests`, against `InventorySyncDemo.Core` directly, no
browser involved):
- Selling the last unit brings stock to exactly zero; further sells are rejected, return
  `false`, and stock never goes negative
- A sale made through one channel is reflected identically via `GetStock` regardless of
  which channel sold it (shared pool, not per-channel copies)
- Selling more than available stock in one call fails with no partial deduction
- Unknown SKU / zero-or-negative quantity sells are rejected
- `Reset()` restores seed stock and clears sales history
- `StockChanged` fires on a successful sale, does not fire on a failed one
- Each of the 3 `IPriceListParser` implementations correctly parses its sample CSV format
  into normalized entries with correct Sku/Supplier/NewPrice
- `NormalizedPriceChange.ComputeDirection` returns Up/Down/Same correctly
- `PriceNormalizationService` matches by SKU against the catalog and skips unknown SKUs

Last local run: **16/16 passed.**

## Resetting demo data

Click **"Reset Demo Data"** in the top-right corner of the dashboard (`/`). It calls
`IStockService.Reset()`, which restores every SKU's seeded starting quantity and clears
the simulated sales/activity history. No page reload needed — it's a live in-memory
reset.

## Publishing / GitHub Pages deployment

```bash
dotnet publish demos-src/InventorySyncDemo/src/InventorySyncDemo.Client -c Release
```

This produces
`demos-src/InventorySyncDemo/src/InventorySyncDemo.Client/bin/Release/net8.0/publish/wwwroot/`.
Copy the **contents** of that `wwwroot` folder (not the folder itself) into
`demo/noliktavas-sinhronizacija/` at the repo root, so `index.html`, `404.html`,
`_framework/`, `css/`, `sample-data/` end up directly under
`demo/noliktavas-sinhronizacija/`, then rewrite the base href from the dev default (`/`)
to the production deploy path in both HTML files:

```bash
rm -rf demo/noliktavas-sinhronizacija && mkdir -p demo/noliktavas-sinhronizacija
cp -r demos-src/InventorySyncDemo/src/InventorySyncDemo.Client/bin/Release/net8.0/publish/wwwroot/. demo/noliktavas-sinhronizacija/
sed -i 's#<base href="/" />#<base href="/demo/noliktavas-sinhronizacija/" />#' demo/noliktavas-sinhronizacija/index.html demo/noliktavas-sinhronizacija/404.html
```

The site is served statically by GitHub Pages at
`https://datacraft.lv/demo/noliktavas-sinhronizacija/` with no build step and no
server-side rewrites, so the deployed output relies on:

- **Rewritten base href** in the deployed `demo/noliktavas-sinhronizacija/index.html` and
  `404.html`: `<base href="/demo/noliktavas-sinhronizacija/" />` (source tree keeps
  `<base href="/" />` for local dev — see "Running locally" above)
- **`noindex, nofollow`** meta tag in `wwwroot/index.html`'s `<head>`:
  ```html
  <meta name="robots" content="noindex, nofollow">
  ```
- **SPA-on-GitHub-Pages redirect trick** for deep links (e.g. a direct load/refresh of
  `/demo/noliktavas-sinhronizacija/price-lists`, which GitHub Pages would otherwise 404
  on since it has no server-side rewrite):
  - `wwwroot/404.html` — a copy of `index.html` with a redirect script (`pathSegmentsToKeep = 2`)
    inserted as the very first thing in `<head>`, which repoints the browser to the SPA
    root with the original path encoded in the query string
  - `wwwroot/index.html` — a matching decode script in `<head>`, before the Blazor
    `<script src="_framework/blazor.webassembly.js">` tag, which restores the real path
    via `history.replaceState` before the Blazor router takes over
- **Footer disclaimer** on every page (in `Layout/MainLayout.razor`): *"This is a demo
  built by DataCraft Consulting to illustrate what's possible — not a real client's
  system or data."*

## Deviations from the original spec

- Used `dotnet new blazorwasm --empty` instead of the plain default template — the empty
  template already omits `Counter.razor`/`FetchData.razor`/Bootstrap/open-iconic, so
  there was no template cruft to strip afterward.
- Added a lightweight `ILocalizer`/`DictionaryLocalizer` (EN/LV) in Core that wasn't
  explicitly requested, to fit the Riga-based target audience mentioned in the project
  brief; it follows the same subscribe/`StateHasChanged` pattern as `IStockService` and
  doesn't affect stock/sales state when the language is switched.
- `IStockService.LastChangedSku` was added (not in the original interface sketch) so
  every subscriber knows exactly which row to flash without extra plumbing.
