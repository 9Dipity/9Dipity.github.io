namespace DbHealthCheckDemo.Core.Localization;

/// <summary>
/// Simple in-memory dictionary translation service, same pattern as the other two demos.
/// Finding sentences are templates with {0}/{1} placeholders - the Client formats them
/// against Finding.Args via string.Format, so the underlying rule logic in Core never
/// touches localized text.
/// </summary>
public sealed class DictionaryLocalizer : ILocalizer
{
    public Language CurrentLanguage { get; private set; } = Language.Lv;

    public event Action? LanguageChanged;

    public void SetLanguage(Language language)
    {
        if (CurrentLanguage == language) return;
        CurrentLanguage = language;
        LanguageChanged?.Invoke();
    }

    public string T(string key)
    {
        if (Translations.TryGetValue(key, out var byLanguage) &&
            byLanguage.TryGetValue(CurrentLanguage, out var value))
        {
            return value;
        }

        return key; // fallback so a missing key is visible rather than throwing
    }

    private static readonly Dictionary<string, Dictionary<Language, string>> Translations = new()
    {
        ["nav.brand"] = New("DataCraft Consulting", "DataCraft Consulting"),
        ["nav.brandSubtitle"] = New("Diagnostic Preview", "Diagnostikas priekšskatījums"),

        ["page.title"] = New("Database Health Check", "Datubāzes stāvokļa pārbaude"),
        ["page.subtitle"] = New(
            "Pick a sample scenario, run the check, and see exactly what gets flagged — adjust two of the numbers yourself and watch it re-score live.",
            "Izvēlies parauga scenāriju, palaid pārbaudi un redzi, kas tieši tiek atzīmēts — pats maini divus skaitļus un vēro, kā vērtējums pārrēķinās uzreiz."),

        ["scenario.pickLabel"] = New("Choose a sample scenario", "Izvēlies parauga scenāriju"),
        ["scenario.retail.name"] = New("Retail order-processing database", "Mazumtirdzniecības pasūtījumu datubāze"),
        ["scenario.retail.blurb"] = New(
            "A single-store retailer's order system, a few years old.",
            "Viena veikala pasūtījumu sistēma, dažus gadus veca."),
        ["scenario.clinic.name"] = New("Clinic scheduling and billing database", "Klīnikas pierakstu un norēķinu datubāze"),
        ["scenario.clinic.blurb"] = New(
            "A multi-location clinic's scheduling and billing system that's outgrown its original design.",
            "Vairāku nodaļu klīnikas pierakstu un norēķinu sistēma, kas pārauguse sākotnējo dizainu."),
        ["scenario.distribution.name"] = New("Distribution reporting database", "Izplatīšanas pārskatu datubāze"),
        ["scenario.distribution.blurb"] = New(
            "An import/distribution company's nightly reporting database.",
            "Importa/izplatīšanas uzņēmuma nakts pārskatu datubāze."),

        ["runButton"] = New("Run the Check →", "Palaist pārbaudi →"),
        ["rerunButton"] = New("Re-run with these numbers →", "Palaist vēlreiz ar šiem skaitļiem →"),
        ["resetButton"] = New("Reset Demo Data", "Atiestatīt demo datus"),

        ["adjust.title"] = New("Adjust and re-run", "Pielāgo un palaid vēlreiz"),
        ["adjust.subtitle"] = New(
            "These two numbers feed directly into the rules above — change them and the findings recompute for real.",
            "Šie divi skaitļi tieši ietekmē augstāk redzamos noteikumus — mainot tos, konstatējumi patiešām pārrēķinās."),
        ["adjust.rowCountLabel"] = New("Largest table row count", "Lielākās tabulas rindu skaits"),
        ["adjust.backupAutomatedLabel"] = New("Automated backup job configured?", "Vai konfigurēts automātisks dublēšanas uzdevums?"),
        ["adjust.backupYes"] = New("Yes", "Jā"),
        ["adjust.backupNo"] = New("No", "Nē"),
        ["adjust.daysSinceBackupLabel"] = New("Days since last full backup", "Dienas kopš pēdējās pilnās dublēšanas"),

        ["results.heading"] = New("Findings", "Konstatējumi"),

        ["tier.QuickCheck.banner"] = New(
            "Findings at this depth are what a Quick Check typically surfaces.",
            "Šāda dziļuma konstatējumus parasti atklāj Quick Check pārbaude."),
        ["tier.StandardAudit.banner"] = New(
            "Findings at this depth are what a Standard Audit typically surfaces.",
            "Šāda dziļuma konstatējumus parasti atklāj Standard Audit pārbaude."),
        ["tier.DeepDive.banner"] = New(
            "Findings at this depth are what a Deep Dive typically surfaces.",
            "Šāda dziļuma konstatējumus parasti atklāj Deep Dive pārbaude."),

        ["severity.Ok"] = New("OK", "Labi"),
        ["severity.Info"] = New("Info", "Info"),
        ["severity.Warning"] = New("Warning", "Brīdinājums"),
        ["severity.Critical"] = New("Critical", "Kritiski"),

        ["methodology.toggle"] = New("How this scoring works", "Kā veidojas šis vērtējums"),
        ["methodology.missingIndex"] = New(
            "Missing indexes: a table over 500K rows with 0-1 non-clustered indexes is flagged critical; over 100K is a warning.",
            "Trūkstoši indeksi: tabula ar vairāk nekā 500 000 rindu un 0-1 papildu indeksiem tiek atzīmēta kā kritiska; virs 100 000 — kā brīdinājums."),
        ["methodology.fragmentation"] = New(
            "Index fragmentation: 30%+ average fragmentation is critical, 10-30% is a warning — these match Microsoft's own rebuild/reorganize guidance.",
            "Indeksu fragmentācija: vidējā fragmentācija virs 30% ir kritiska, 10-30% — brīdinājums; tas atbilst Microsoft ieteikumiem par pārbūvi/reorganizāciju."),
        ["methodology.backupRecency"] = New(
            "Backup coverage: no automated job is always critical; an automated job with no successful run in 7+ days is a warning.",
            "Dublēšanas nodrošinājums: automātiska uzdevuma trūkums vienmēr ir kritisks; automātisks uzdevums bez veiksmīgas izpildes 7+ dienas ir brīdinājums."),
        ["methodology.slowQuery"] = New(
            "Slow queries: a tracked report at 60+ seconds is critical, 10-60 seconds is a warning.",
            "Lēni vaicājumi: pārskats, kas ilgst 60+ sekundes, ir kritisks, 10-60 sekundes — brīdinājums."),
        ["methodology.schemaStaleness"] = New(
            "Schema staleness: no end-to-end review in 4+ years is critical, 2-4 years is a warning.",
            "Shēmas novecošana: bez pilnas pārskatīšanas 4+ gadus — kritiski, 2-4 gadus — brīdinājums."),
        ["methodology.concurrencyRisk"] = New(
            "Concurrency risk: 40%+ table-scan queries with 10+ concurrent users is critical, 20%+ with 5+ users is a warning.",
            "Vienlaicīgas slodzes risks: 40%+ vaicājumu ar pilnu tabulas skenēšanu un 10+ vienlaicīgiem lietotājiem ir kritiski, 20%+ ar 5+ lietotājiem — brīdinājums."),

        ["footer.disclaimer"] = New(
            "This is a demo built by DataCraft Consulting to illustrate how a diagnostic actually scores a system — not a live connection to a real database.",
            "Šī ir DataCraft Consulting izveidota demonstrācijas versija, kas parāda, kā diagnostika patiešām novērtē sistēmu — nevis reāls pieslēgums reālai datubāzei."),

        ["lang.en"] = New("EN", "EN"),
        ["lang.lv"] = New("LV", "LV"),

        // --- Finding templates: {0}/{1}/{2} filled from Finding.Args via string.Format ---

        ["finding.missingIndex.ok.title"] = New("Indexing looks adequate", "Indeksācija izskatās pietiekama"),
        ["finding.missingIndex.ok.detail"] = New(
            "{0} has {1:N0} rows and {2} non-clustered index(es) — reasonable coverage for a table this size.",
            "{0} ir {1:N0} rindas un {2} papildu indekss(-i) — pietiekams nodrošinājums šāda izmēra tabulai."),
        ["finding.missingIndex.ok.impact"] = New("No action needed here right now.", "Šeit rīcība nav nepieciešama."),
        ["finding.missingIndex.risk.title"] = New("Missing indexes on your busiest table", "Trūkst indeksu visnoslogotākajai tabulai"),
        ["finding.missingIndex.risk.detail"] = New(
            "{0} has {1:N0} rows and only {2} non-clustered index(es). Almost every query against it that isn't a lookup by primary key ends up scanning the full table.",
            "{0} ir {1:N0} rindas un tikai {2} papildu indekss(-i). Gandrīz katrs vaicājums, kas nav meklēšana pēc primārās atslēgas, skenē visu tabulu."),
        ["finding.missingIndex.risk.impact"] = New(
            "Lookups and reports against {0} get slower every month as the table grows — and there's usually no warning until someone finally complains.",
            "Meklēšana un pārskati par {0} kļūst lēnāki ar katru mēnesi, tabulai augot — un parasti par to uzzina tikai tad, kad kāds sūdzas."),

        ["finding.fragmentation.ok.title"] = New("Index fragmentation is under control", "Indeksu fragmentācija ir kontrolē"),
        ["finding.fragmentation.ok.detail"] = New(
            "Average index fragmentation is {0:0}% — comfortably under the 10% threshold where a reorganize is usually considered.",
            "Vidējā indeksu fragmentācija ir {0:0}% — droši zem 10% sliekšņa, kur parasti apsver reorganizāciju."),
        ["finding.fragmentation.ok.impact"] = New("No action needed here right now.", "Šeit rīcība nav nepieciešama."),
        ["finding.fragmentation.risk.title"] = New("Index fragmentation", "Indeksu fragmentācija"),
        ["finding.fragmentation.risk.detail"] = New(
            "Average index fragmentation across indexed tables is {0:0}%. Above 30% is generally treated as a rebuild candidate; 10-30% as a reorganize candidate.",
            "Vidējā indeksu fragmentācija indeksētajās tabulās ir {0:0}%. Virs 30% parasti uzskata par pārbūves kandidātu; 10-30% — par reorganizācijas kandidātu."),
        ["finding.fragmentation.risk.impact"] = New(
            "Fragmented indexes mean SQL Server reads more pages than necessary for the same query — a slow, invisible tax on every report and page load.",
            "Fragmentēti indeksi nozīmē, ka SQL Server katram vaicājumam nolasa vairāk lapu nekā nepieciešams — lēns, neredzams slogs katram pārskatam un lapas ielādei."),

        ["finding.backupRecency.manual.title"] = New("No automated backup job", "Nav automātiska dublēšanas uzdevuma"),
        ["finding.backupRecency.manual.detail"] = New(
            "No automated backup job is configured — if a full backup exists at all, its age isn't being tracked.",
            "Nav konfigurēts automātisks dublēšanas uzdevums — ja pilna dublēšana vispār eksistē, tās vecums netiek izsekots."),
        ["finding.backupRecency.manual.impact"] = New(
            "This is the one finding that isn't about speed — it's about how much work you'd lose if the server failed tomorrow.",
            "Šis ir vienīgais konstatējums, kas nav par ātrumu — tas ir par to, cik daudz darba tiktu zaudēts, ja serveris rīt atteiktos."),
        ["finding.backupRecency.automated-ok.title"] = New("Backup coverage looks healthy", "Dublēšanas nodrošinājums izskatās labs"),
        ["finding.backupRecency.automated-ok.detail"] = New(
            "An automated backup job is configured, and the last full backup completed {0} day(s) ago.",
            "Ir konfigurēts automātisks dublēšanas uzdevums, un pēdējā pilnā dublēšana pabeigta pirms {0} dienas(-ām)."),
        ["finding.backupRecency.automated-ok.impact"] = New("No action needed here right now.", "Šeit rīcība nav nepieciešama."),
        ["finding.backupRecency.automated-risk.title"] = New("Backup job may be stale", "Dublēšanas uzdevums var būt novecojis"),
        ["finding.backupRecency.automated-risk.detail"] = New(
            "An automated backup job is configured, but the last successful full backup was {0} days ago.",
            "Automātisks dublēšanas uzdevums ir konfigurēts, taču pēdējā veiksmīgā pilnā dublēšana bija pirms {0} dienām."),
        ["finding.backupRecency.automated-risk.impact"] = New(
            "A job that's configured but hasn't actually succeeded recently is easy to miss — worth confirming it's still running, not just still scheduled.",
            "Uzdevumu, kas ir konfigurēts, bet nesen nav veiksmīgi izpildīts, viegli palaist garām — vērts pārliecināties, ka tas tiešām darbojas, ne tikai ieplānots."),

        ["finding.slowQuery.ok.title"] = New("No standout slow queries", "Nav izteikti lēnu vaicājumu"),
        ["finding.slowQuery.ok.detail"] = New(
            "The slowest tracked report, \"{0}\", runs in {1:0.#}s.",
            "Lēnākais izsekotais pārskats \"{0}\" izpildās {1:0.#}s."),
        ["finding.slowQuery.ok.impact"] = New("No action needed here right now.", "Šeit rīcība nav nepieciešama."),
        ["finding.slowQuery.risk.title"] = New("Slow report/query", "Lēns pārskats/vaicājums"),
        ["finding.slowQuery.risk.detail"] = New(
            "\"{0}\" currently takes {1:0.#}s to run.",
            "\"{0}\" šobrīd izpildās {1:0.#}s."),
        ["finding.slowQuery.risk.impact"] = New(
            "Anyone waiting on this report — or anything sharing the server while it runs — feels this daily.",
            "Ikviens, kurš gaida šo pārskatu — vai jebko citu, kas dala serveri tā izpildes laikā — to izjūt katru dienu."),

        ["finding.schemaStaleness.ok.title"] = New("Schema reviewed recently", "Shēma nesen pārskatīta"),
        ["finding.schemaStaleness.ok.detail"] = New(
            "The schema was last reviewed end-to-end about {0:0.#} year(s) ago.",
            "Shēma pilnībā pārskatīta apmēram pirms {0:0.#} gada(-iem)."),
        ["finding.schemaStaleness.ok.impact"] = New("No action needed here right now.", "Šeit rīcība nav nepieciešama."),
        ["finding.schemaStaleness.risk.title"] = New("Schema hasn't been reviewed in years", "Shēma gadiem nav pārskatīta"),
        ["finding.schemaStaleness.risk.detail"] = New(
            "It's been roughly {0:0.#} years since the schema was last reviewed end-to-end.",
            "Kopš pilnas shēmas pārskatīšanas pagājuši aptuveni {0:0.#} gadi."),
        ["finding.schemaStaleness.risk.impact"] = New(
            "Schemas that grow feature-by-feature for years without a review tend to accumulate exactly the kind of debt that turns a 2-minute report into a 15-minute one.",
            "Shēmas, kas gadiem augušas funkciju pēc funkcijas bez pārskatīšanas, mēdz uzkrāt tieši tāda veida parādu, kas 2 minūšu pārskatu pārvērš 15 minūšu pārskatā."),

        ["finding.concurrencyRisk.ok.title"] = New("Concurrent load looks manageable", "Vienlaicīgā slodze izskatās pārvaldāma"),
        ["finding.concurrencyRisk.ok.detail"] = New(
            "About {0:0}% of queries fall back to a table scan, with {1} concurrent users typically active — not enough overlap to be a concern yet.",
            "Apmēram {0:0}% vaicājumu izmanto pilnu tabulas skenēšanu, ar {1} vienlaicīgiem lietotājiem — pagaidām nepietiekams pārklājums, lai būtu iemesls satraukumam."),
        ["finding.concurrencyRisk.ok.impact"] = New("No action needed here right now.", "Šeit rīcība nav nepieciešama."),
        ["finding.concurrencyRisk.risk.title"] = New("Table scans under concurrent load", "Pilna tabulas skenēšana vienlaicīgas slodzes laikā"),
        ["finding.concurrencyRisk.risk.detail"] = New(
            "About {0:0}% of queries against this database fall back to a table scan, with {1} concurrent users typically active.",
            "Apmēram {0:0}% vaicājumu šajā datubāzē izmanto pilnu tabulas skenēšanu, ar {1} vienlaicīgiem lietotājiem."),
        ["finding.concurrencyRisk.risk.impact"] = New(
            "Table scans under concurrent load are a common cause of blocking — one slow query holds up everyone else waiting on the same table.",
            "Pilna tabulas skenēšana vienlaicīgas slodzes laikā bieži izraisa bloķēšanu — viens lēns vaicājums aiztur visus pārējos, kas gaida to pašu tabulu."),
    };

    private static Dictionary<Language, string> New(string en, string lv) => new()
    {
        [Language.En] = en,
        [Language.Lv] = lv
    };
}
