namespace ServiceBoardDemo.Core.Localization;

/// <summary>
/// Simple in-memory dictionary translation service. Default language is Latvian (the
/// primary audience for this demo is Riga-area workshops/service shops); English is
/// available via the language toggle. Switching language only changes CurrentLanguage
/// and fires LanguageChanged - it never touches job-board state.
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
        ["nav.brand"] = New("Baltic Motor Works", "Baltic Motor Works"),
        ["nav.brandSubtitle"] = New("Service Bay Console", "Servisa panelis"),
        ["nav.board"] = New("Job Board", "Darbu panelis"),
        ["nav.parts"] = New("Parts", "Rezerves daļas"),

        ["board.title"] = New("Service Bay — Live Job Board", "Servisa darbnīca — darbu panelis"),
        ["board.subtitle"] = New(
            "Every job, every bay, one shared board — everyone on the floor sees the same status the instant it changes.",
            "Katrs darbs, katra darbnīcas vieta, viens kopīgs panelis — visi redz vienu un to pašu statusu tajā pašā brīdī."),
        ["board.completedToday"] = New("Completed today", "Šodien pabeigti"),
        ["board.newJobButton"] = New("+ New Job", "+ Jauns darbs"),
        ["board.resetButton"] = New("Reset Demo Data", "Atiestatīt demo datus"),
        ["board.noJobsInColumn"] = New("No jobs here", "Šeit nav darbu"),

        ["status.Intake"] = New("Intake", "Pieņemts"),
        ["status.Diagnosis"] = New("Diagnosis", "Diagnostika"),
        ["status.AwaitingParts"] = New("Awaiting Parts", "Gaida daļas"),
        ["status.InProgress"] = New("In Progress", "Darbā"),
        ["status.Ready"] = New("Ready for Pickup", "Gatavs izsniegšanai"),

        ["job.technician"] = New("Technician", "Meistars"),
        ["job.unassigned"] = New("Unassigned", "Nav piešķirts"),
        ["job.estimate"] = New("Estimate", "Aptuvenā izmaksa"),
        ["job.advanceButton"] = New("Advance →", "Virzīt tālāk →"),
        ["job.completeButton"] = New("Mark Picked Up ✓", "Atzīmēt kā izsniegtu ✓"),
        ["job.blockedOnParts"] = New("Waiting on parts — see Parts page", "Gaida detaļas — skatīt sadaļu “Rezerves daļas”"),

        ["newJob.title"] = New("New Job Intake", "Jauna darba pieņemšana"),
        ["newJob.customerLabel"] = New("Customer name", "Klienta vārds"),
        ["newJob.vehicleLabel"] = New("Vehicle (make, model, plate)", "Transportlīdzeklis (marka, modelis, numurs)"),
        ["newJob.issueLabel"] = New("Reported issue", "Norādītā problēma"),
        ["newJob.submitButton"] = New("Add to Board", "Pievienot panelim"),
        ["newJob.cancelButton"] = New("Cancel", "Atcelt"),
        ["newJob.validationError"] = New("Fill in customer, vehicle, and issue before adding.", "Aizpildi klientu, transportlīdzekli un problēmu, pirms pievieno."),

        ["parts.title"] = New("Parts Blocking Active Jobs", "Detaļas, kas kavē aktīvos darbus"),
        ["parts.subtitle"] = New(
            "One shop-wide parts view instead of checking every ticket by hand — see what's holding up the floor before it's a surprise at pickup time.",
            "Viens kopskats visai darbnīcai, nevis katras kartītes pārbaude atsevišķi — redzi, kas kavē darbu, pirms tas kļūst par pārsteigumu izsniegšanas brīdī."),
        ["parts.column.part"] = New("Part", "Detaļa"),
        ["parts.column.qty"] = New("Qty needed", "Nepieciešamais daudzums"),
        ["parts.column.jobs"] = New("Blocking jobs", "Skartie darbi"),
        ["parts.markReceived"] = New("Mark Received", "Atzīmēt kā saņemtu"),
        ["parts.empty"] = New(
            "Nothing blocked right now — every active job has the parts it needs.",
            "Šobrīd nekas nav bloķēts — visiem aktīvajiem darbiem ir nepieciešamās detaļas."),
        ["parts.receivedHint"] = New(
            "Marking a part received unblocks its job on the board — the technician still clicks Advance.",
            "Atzīmējot detaļu kā saņemtu, attiecīgais darbs panelī tiek atbloķēts — meistars joprojām pats nospiež “Virzīt tālāk”."),

        ["footer.disclaimer"] = New(
            "This is a demo built by DataCraft Consulting to illustrate what's possible — not a real client's system or data.",
            "Šī ir DataCraft Consulting izveidota demonstrācijas versija, kas parāda iespējas — nevis reāla klienta sistēma vai dati."),

        ["lang.en"] = New("EN", "EN"),
        ["lang.lv"] = New("LV", "LV"),
    };

    private static Dictionary<Language, string> New(string en, string lv) => new()
    {
        [Language.En] = en,
        [Language.Lv] = lv
    };
}
