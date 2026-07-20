namespace ClinicBookingDemo.Core.Services;

/// <summary>
/// The full Latvian/English translation table for the demo, keyed by dotted keys
/// (e.g. "home.heroTitle"). Kept as one flat dictionary for simplicity — this is a demo,
/// not a product with a translation pipeline.
/// </summary>
public static class Translations
{
    public static readonly IReadOnlyDictionary<string, (string Lv, string En)> Map = new Dictionary<string, (string Lv, string En)>
    {
        // Common
        ["common.clinicName"] = ("Rīgas Smaids Clinic", "Rīgas Smaids Clinic"),
        ["common.tagline"] = ("Zobārstniecības un veselības klīnika Rīgā", "Dental & wellness clinic in Riga"),
        ["common.langEn"] = ("EN", "EN"),
        ["common.langLv"] = ("LV", "LV"),
        ["common.resetDemoData"] = ("Atiestatīt demo datus", "Reset demo data"),
        ["common.back"] = ("Atpakaļ", "Back"),
        ["common.next"] = ("Tālāk", "Next"),
        ["common.minutesShort"] = ("min", "min"),
        ["common.close"] = ("Aizvērt", "Close"),

        // Nav / layout
        ["nav.book"] = ("Pieteikties vizītei", "Book an appointment"),
        ["nav.home"] = ("Sākums", "Home"),
        ["nav.adminLink"] = ("Administrācija", "Admin view"),

        // Footer
        ["footer.disclaimer"] = (
            "Šī ir DataCraft Consulting izveidota demonstrācijas versija, kas parāda iespējas — nevis reāla klienta sistēma vai dati.",
            "This is a demo built by DataCraft Consulting to illustrate what's possible — not a real client's system or data."),

        // Home / landing page
        ["home.heroTitle"] = ("Piesakieties vizītei tiešsaistē — jebkurā laikā", "Book your visit online — anytime"),
        ["home.heroSubtitle"] = (
            "Izvēlieties speciālistu, pakalpojumu un ērtāko laiku. Bez zvaniem, bez gaidīšanas rindā.",
            "Pick your specialist, service, and a time that works. No phone calls, no waiting on hold."),
        ["home.cta"] = ("Pieteikties vizītei", "Book an appointment"),
        ["home.whyTitle"] = ("Kāpēc pieteikties tiešsaistē?", "Why book online?"),
        ["home.why1Title"] = ("Pieejams 24/7", "Available 24/7"),
        ["home.why1Body"] = ("Piesakieties jebkurā diennakts laikā, bez gaidīšanas pa tālruni.", "Book any time of day — no waiting on hold."),
        ["home.why2Title"] = ("Redzama reālā pieejamība", "See real availability"),
        ["home.why2Body"] = ("Katra speciālista brīvie laiki redzami uzreiz, nevis pēc zvana uz reģistratūru.", "Each specialist's real open slots, right away — no calling the front desk."),
        ["home.why3Title"] = ("Bez dubultas rezervēšanas", "No double-booking"),
        ["home.why3Body"] = ("Sistēma automātiski novērš pārklāšanos grafikā starp visiem speciālistiem.", "The system automatically prevents scheduling conflicts across every specialist."),
        ["home.specialistsTitle"] = ("Mūsu speciālisti", "Our specialists"),

        // Specialist titles / bios
        ["specialist.title.generalDentist"] = ("Vispārējais zobārsts", "General Dentist"),
        ["specialist.title.orthodontist"] = ("Ortodonts", "Orthodontist"),
        ["specialist.title.hygienist"] = ("Zobu higiēnists", "Dental Hygienist"),
        ["specialist.bio.kalnina"] = ("Draudzīgas un rūpīgas pārbaudes un tīrīšanas. Klīnikā kopš 2016. gada.", "Friendly, thorough checkups and cleanings. With the clinic since 2016."),
        ["specialist.bio.ozols"] = ("Specializējas balināšanā un estētiskiem risinājumiem. Populārs pēcpusdienas stundās.", "Specializes in whitening and cosmetic work. Popular with afternoon patients."),
        ["specialist.bio.berzina"] = ("Agra rītdaris — pieejama no rīta, kā arī sestdienās strādājošiem pacientiem.", "Early riser — mornings, plus a Saturday clinic for working patients."),

        // Services
        ["service.checkup.name"] = ("Pārbaude", "Checkup"),
        ["service.checkup.desc"] = ("Ikdienas zobu pārbaude un konsultācija.", "Routine dental checkup and consultation."),
        ["service.cleaning.name"] = ("Tīrīšana", "Cleaning"),
        ["service.cleaning.desc"] = ("Profesionāla tīrīšana un pulēšana.", "Professional cleaning and polish."),
        ["service.whitening.name"] = ("Balināšana", "Whitening"),
        ["service.whitening.desc"] = ("Zobu balināšanas procedūra klīnikā.", "In-clinic teeth whitening treatment."),
        ["service.emergency.name"] = ("Neatliekamā palīdzība", "Emergency"),
        ["service.emergency.desc"] = ("Steidzama vizīte sāpju vai traumas gadījumā.", "Urgent same-window appointment for pain or injury."),

        // Blocked slot reasons
        ["blocked.onLeave"] = ("Atvaļinājumā", "On leave"),
        ["blocked.training"] = ("Darbinieku apmācība", "Staff training"),

        // Booking statuses
        ["status.booked"] = ("Rezervēts", "Booked"),
        ["status.completed"] = ("Pabeigts", "Completed"),
        ["status.noshow"] = ("Neieradās", "No-show"),
        ["status.cancelled"] = ("Atcelts", "Cancelled"),

        // Short weekday labels for the slot-picker day tabs
        ["day.monday"] = ("P", "Mon"),
        ["day.tuesday"] = ("O", "Tue"),
        ["day.wednesday"] = ("T", "Wed"),
        ["day.thursday"] = ("C", "Thu"),
        ["day.friday"] = ("Pk", "Fri"),
        ["day.saturday"] = ("S", "Sat"),
        ["day.sunday"] = ("Sv", "Sun"),

        // Booking wizard
        ["booking.title"] = ("Piesakieties vizītei", "Book an appointment"),
        ["booking.step1.title"] = ("1. Izvēlieties pakalpojumu", "1. Choose a service"),
        ["booking.step2.title"] = ("2. Izvēlieties speciālistu", "2. Choose a specialist"),
        ["booking.step3.title"] = ("3. Izvēlieties laiku", "3. Choose a time"),
        ["booking.step4.title"] = ("4. Jūsu dati", "4. Your details"),
        ["booking.step5.title"] = ("Apstiprinājums", "Confirmation"),
        ["booking.selectDate"] = ("Izvēlieties dienu", "Select a day"),
        ["booking.noSlots"] = ("Šajā dienā brīvu laiku nav.", "No available times on this day."),
        ["booking.available"] = ("Pieejams", "Available"),
        ["booking.unavailable"] = ("Aizņemts", "Unavailable"),
        ["booking.selectedSlot"] = ("Izvēlētais laiks", "Selected time"),
        ["booking.form.name"] = ("Vārds, uzvārds", "Full name"),
        ["booking.form.phone"] = ("Tālrunis", "Phone"),
        ["booking.form.email"] = ("E-pasts", "Email"),
        ["booking.form.submit"] = ("Apstiprināt pieteikumu", "Confirm booking"),
        ["booking.summary.service"] = ("Pakalpojums", "Service"),
        ["booking.summary.specialist"] = ("Speciālists", "Specialist"),
        ["booking.summary.datetime"] = ("Datums un laiks", "Date & time"),
        ["booking.summary.duration"] = ("Ilgums", "Duration"),
        ["booking.chooseServiceFirst"] = ("Vispirms izvēlieties pakalpojumu.", "Choose a service first."),
        ["booking.chooseSpecialistFirst"] = ("Vispirms izvēlieties speciālistu.", "Choose a specialist first."),
        ["booking.chooseSlotFirst"] = ("Vispirms izvēlieties laiku.", "Choose a time first."),

        // Confirmation
        ["confirm.title"] = ("Vizīte apstiprināta!", "Appointment confirmed!"),
        ["confirm.subtitle"] = ("Jūs saņemtu apstiprinājumu e-pastā. Zemāk ir tā priekšskatījums.", "You'd receive a confirmation by email. Here's a preview of what it would look like."),
        ["confirm.bookAnother"] = ("Pieteikt vēl vienu vizīti", "Book another appointment"),
        ["confirm.backHome"] = ("Uz sākumu", "Back to home"),

        // Mock confirmation email
        ["email.from"] = ("Rīgas Smaids Clinic <no-reply@rigas-smaids.example>", "Rīgas Smaids Clinic <no-reply@rigas-smaids.example>"),
        ["email.subject"] = ("Jūsu vizīte Rīgas Smaids klīnikā ir apstiprināta", "Your appointment at Rīgas Smaids Clinic is confirmed"),
        ["email.greeting"] = ("Sveiki, {0}!", "Hi {0},"),
        ["email.body"] = ("Jūsu vizīte ir veiksmīgi rezervēta. Detaļas:", "Your appointment has been successfully booked. Details:"),
        ["email.footer"] = ("Ja nepieciešams pārcelt vai atcelt vizīti, sazinieties ar mums.", "Need to reschedule or cancel? Just get in touch."),
        ["email.signature"] = ("Uz tikšanos!\nRīgas Smaids klīnikas komanda", "See you soon!\nThe Rīgas Smaids Clinic team"),

        // Admin
        ["admin.title"] = ("Administrācijas panelis", "Admin dashboard"),
        ["admin.subtitle"] = ("Nav publiski redzams — tikai iekšējai lietošanai.", "Not publicly linked — for internal use only."),
        ["admin.tab.today"] = ("Šodien", "Today"),
        ["admin.tab.upcoming"] = ("Gaidāmās vizītes", "Upcoming"),
        ["admin.tab.blocked"] = ("Bloķētie laiki", "Blocked time"),
        ["admin.today.title"] = ("Šodienas grafiks", "Today's schedule"),
        ["admin.today.empty"] = ("Šodien vizīšu nav.", "No appointments today."),
        ["admin.upcoming.title"] = ("Visas gaidāmās vizītes", "All upcoming appointments"),
        ["admin.upcoming.empty"] = ("Gaidāmu vizīšu nav.", "No upcoming appointments."),
        ["admin.markCompleted"] = ("Pabeigts", "Completed"),
        ["admin.markNoShow"] = ("Neieradās", "No-show"),
        ["admin.blockSlot.title"] = ("Bloķēt laiku", "Block a time slot"),
        ["admin.blockSlot.specialist"] = ("Speciālists", "Specialist"),
        ["admin.blockSlot.date"] = ("Datums", "Date"),
        ["admin.blockSlot.start"] = ("No", "Start"),
        ["admin.blockSlot.end"] = ("Līdz", "End"),
        ["admin.blockSlot.reason"] = ("Iemesls", "Reason"),
        ["admin.blockSlot.submit"] = ("Bloķēt", "Block"),
        ["admin.blockSlot.list.title"] = ("Aktīvie bloķējumi", "Active blocks"),
        ["admin.blockSlot.remove"] = ("Noņemt", "Remove"),
        ["admin.blockSlot.empty"] = ("Bloķētu laiku nav.", "No blocked slots."),
        ["admin.resetConfirmToast"] = ("Demo dati atiestatīti.", "Demo data has been reset."),
        ["admin.column.patient"] = ("Pacients", "Patient"),
        ["admin.column.service"] = ("Pakalpojums", "Service"),
        ["admin.column.specialist"] = ("Speciālists", "Specialist"),
        ["admin.column.time"] = ("Laiks", "Time"),
        ["admin.column.status"] = ("Statuss", "Status"),
        ["admin.column.actions"] = ("Darbības", "Actions"),
        ["admin.legendTitle"] = ("Speciālisti", "Specialists"),
    };
}
