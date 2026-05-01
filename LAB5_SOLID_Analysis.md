# LAB 5 — Analiză SOLID: Flyweight, Decorator, Bridge, Proxy

> **Framework:** ASP.NET Core 8.0 | **Data:** 2026-04-26

---

## Legendă

| Simbol | Semnificație |
|--------|-------------|
| ✅ | Respectă |
| ⚠️ | Parțial respectă |
| ❌ | Nu respectă |

---

## 1. Flyweight Pattern

### 1.1 Analiza principiilor SOLID

| Principiu | Status | Motivație |
|-----------|--------|-----------|
| **SRP** — Single Responsibility | ✅ | `SkillFlyweight` stochează doar stare intrinsecă (Name, Category). `SkillFlyweightFactory` gestionează exclusiv pool-ul de obiecte. `JobSkillRequirement` deține doar starea extrinsecă. Fiecare clasă are un singur motiv de schimbare. |
| **OCP** — Open/Closed | ⚠️ | Se pot adăuga noi tipuri de flyweight-uri fără a modifica `SkillFlyweightFactory`. Totuși, dacă structura cheii din pool (ex: `name+category`) trebuie schimbată, factory-ul necesită modificare internă — nu este complet închis la modificare. |
| **LSP** — Liskov Substitution | ✅ | `SkillFlyweight` este imutabil (readonly properties), deci nu există riscul ca o subclasă să violeze contractul. Obiectele partajate se comportă identic în orice context. |
| **ISP** — Interface Segregation | ⚠️ | Pattern-ul nu definește o interfață explicită pentru `IFlyweight`. `SkillFlyweight` este o clasă concretă directă. Clienții care utilizează pool-ul sunt forțați să treacă prin `SkillFlyweightFactory`, chiar dacă au nevoie doar de un subset din metodele sale (`GetSkill`, `GetPoolSize`, `GetPoolStatistics`, `Clear`). |
| **DIP** — Dependency Inversion | ❌ | `JobSkillRequirement` depinde direct de clasa concretă `SkillFlyweight`, nu de o abstracție `ISkillFlyweight`. `JobPosting` referințiază direct `JobSkillRequirement` concret. Nu există inversiune a dependenței — modulele de nivel înalt depind de detalii de implementare. |

### 1.2 Avantaje

1. **Reducere drastică a memoriei** — obiectele cu stare intrinsecă identică (ex: skill-ul „C#/Backend") sunt create o singură dată și partajate între sute de `JobSkillRequirement`-uri, reducând alocările pe heap.
2. **Thread-safety prin imutabilitate** — flyweight-urile sunt `readonly`, eliminând race conditions la acces concurent fără overhead de sincronizare pe fiecare citire.
3. **Performanță la scară mare** — într-un sistem cu mii de postări de joburi, numărul de obiecte `SkillFlyweight` rămâne constant (egal cu numărul de skill-uri unice), nu crește liniar cu numărul de joburi.

### 1.3 Dezavantaje

1. **Complexitate în gestionarea stării** — separarea artificială a stării în intrinsecă/extrinsecă îngreunează codul; programatorul trebuie să știe mereu care date sunt partajate și care sunt contextuale.
2. **Violarea DIP** — dependența directă de `SkillFlyweight` concret face unit testarea dificilă (nu se poate injecta un mock al flyweight-ului).
3. **Overhead al factory-ului** — `SkillFlyweightFactory` necesită lock pentru thread-safety, ceea ce introduce contention la scriere în pool, mai ales la inițializarea aplicației când multe skill-uri sunt înregistrate simultan.

---

## 2. Decorator Pattern

### 2.1 Analiza principiilor SOLID

| Principiu | Status | Motivație |
|-----------|--------|-----------|
| **SRP** — Single Responsibility | ✅ | Fiecare decorator are o singură responsabilitate: `EmailNotificationDecorator` trimite email, `SMSNotificationDecorator` trimite SMS, `LoggingNotificationDecorator` loghează. Nu există clasă care combină mai multe canale. |
| **OCP** — Open/Closed | ✅ | Adăugarea unui nou canal de notificare (ex: `SlackNotificationDecorator`) se face prin creare de clasă nouă, fără a modifica `BaseNotification` sau decoratorii existenți. Acesta este cazul clasic de OCP respectat. |
| **LSP** — Liskov Substitution | ✅ | Toți decoratorii implementează `INotification` și pot substitui orice altă implementare. `NotificationService` folosește `INotification` fără a cunoaște lanțul de decoratori — substituția funcționează corect. |
| **ISP** — Interface Segregation | ✅ | `INotification` conține doar două metode (`SendAsync`, `GetDescription`), focalizate pe o singură responsabilitate. Nu există metode inutile impuse implementatorilor. |
| **DIP** — Dependency Inversion | ✅ | `NotificationDecorator` depinde de abstracția `INotification`, nu de vreo implementare concretă. `NotificationService` construiește lanțul prin interfețe, nu prin clase concrete. Dependențele sunt inversate corect. |

### 2.2 Avantaje

1. **Extensibilitate dinamică la runtime** — comportamentul unui obiect poate fi extins sau restrâns în timp de execuție prin adăugarea/eliminarea decoratorilor, fără a modifica clasele existente.
2. **Evitarea exploziei de subclase** — fără Decorator ar trebui clase precum `EmailAndSMSNotification`, `EmailAndPushNotification`, `EmailAndSMSAndPushNotification` etc. — creștere combinatorică. Cu Decorator, combinațiile se obțin prin compunere.
3. **Conformitate completă cu SOLID** — dintre cele 4 pattern-uri analizate, Decorator este singurul care respectă toate cele 5 principii SOLID simultan, ceea ce îl face cea mai curată soluție arhitectural.

### 2.3 Dezavantaje

1. **Ordinea decoratorilor contează** — dacă `LoggingNotificationDecorator` este adăugat înainte de `EmailNotificationDecorator`, logarea poate înregistra date incomplete. Erorile de ordine sunt greu de depistat la runtime.
2. **Debugging dificil** — call stack-ul devine adânc: `Logging → Push → SMS → Email → Base`. Urmărirea execuției printr-un lanț de 5 decoratori în cod asincron (`Task`) complică diagnosticarea problemelor.
3. **Proliferare de clase mici** — cu multe canale și comportamente (retry, rate-limiting, templating), numărul de clase decorator crește rapid, mărind suprafața codebas-ului și efortul de mentenanță.

---

## 3. Bridge Pattern

### 3.1 Analiza principiilor SOLID

| Principiu | Status | Motivație |
|-----------|--------|-----------|
| **SRP** — Single Responsibility | ✅ | Ierarhia Abstracție (`JobReport`, `ApplicationReport`) se ocupă cu generarea datelor, iar ierarhia Implementare (`PDFExporter`, `ExcelExporter`) cu formatarea output-ului. Cele două responsabilități sunt complet separate. |
| **OCP** — Open/Closed | ✅ | Adăugarea unui nou format (ex: `XMLExporter`) nu necesită modificarea niciunui `Report`. Adăugarea unui nou tip de raport (ex: `UserReport`) nu necesită modificarea exporterilor. Ambele ierarhii sunt deschise la extensie, închise la modificare. |
| **LSP** — Liskov Substitution | ✅ | `PDFExporter`, `ExcelExporter`, `JSONExporter`, `CSVExporter` sunt interschimbabile ca `IReportExporter`. `ReportingService` poate folosi orice exporter fără a-și schimba comportamentul — contractul este respectat. |
| **ISP** — Interface Segregation | ⚠️ | `IReportExporter` conține 5 metode (`ExportAsync`, `GenerateHeader`, `GenerateFooter`, `Format`, `FileExtension`). Un exporter simplu (ex: plain text) poate fi forțat să implementeze `GenerateHeader`/`GenerateFooter` chiar dacă nu are nevoie de ele. Interfața ar putea fi segregată. |
| **DIP** — Dependency Inversion | ✅ | `BaseReport` depinde de `IReportExporter` (abstracție), nu de `PDFExporter` concret. `ReportingService` injectează exporterul în constructor — dependențele sunt inversate. |

### 3.2 Avantaje

1. **Eliminarea exploziei combinatorice de subclase** — fără Bridge, 3 rapoarte × 4 formate = 12 clase (`JobReportPDF`, `JobReportExcel`, ...). Cu Bridge: 3 + 4 = 7 clase, indiferent de câte combinații există.
2. **Dezvoltare paralelă independentă** — echipa de raportare poate lucra pe `ApplicationReport` simultan cu echipa de export care implementează `CSVExporter`, fără conflicte de cod.
3. **Schimbarea implementării la runtime** — proprietatea `Exporter` din `BaseReport` poate fi înlocuită la execuție, permițând comportament dinamic (ex: utilizatorul alege formatul din UI).

### 3.3 Dezavantaje

1. **Complexitate arhitecturală crescută** — pentru scenarii simple (un singur raport, un singur format), Bridge introduce două ierarhii de clase și un nivel de indirectare în plus, fără beneficii reale.
2. **Overhead de indirectare** — fiecare `ExportAsync` al unui `Report` delegă către `_exporter.ExportAsync(...)`, adăugând un nivel de apel suplimentar. În scenarii de export masiv (mii de rapoarte/minut), aceasta poate fi măsurabilă.
3. **ISP parțial nerespectat** — `IReportExporter` impune implementarea lui `GenerateHeader`/`GenerateFooter` tuturor exporterilor, chiar și celor care nu au nevoie de un header explicit (ex: JSON pur).

---

## 4. Proxy Pattern

### 4.1 Analiza principiilor SOLID

| Principiu | Status | Motivație |
|-----------|--------|-----------|
| **SRP** — Single Responsibility | ⚠️ | **Protection Proxy** (`JobPostingProtectionProxy`) are două responsabilități: (1) verificarea autentificării și (2) delegarea către `RealJobPostingAccess`. **Virtual Proxy** (`ApplicationListVirtualProxy`) combină lazy loading, caching și invalidare în aceeași clasă — 3 responsabilități. |
| **OCP** — Open/Closed | ✅ | Conform refactoring.guru: *„You can introduce new proxies without changing the service or clients."* — adăugarea unui `CachingProxy` sau `LoggingProxy` nu necesită modificarea `RealJobPostingAccess` sau a clienților. |
| **LSP** — Liskov Substitution | ✅ | `JobPostingProtectionProxy` și `RealJobPostingAccess` implementează același `IJobPostingAccess`. `ApplicationListVirtualProxy` și `RealApplicationListAccess` implementează același `IApplicationListAccess`. Substituția este transparentă pentru clienți. |
| **ISP** — Interface Segregation | ✅ | `IJobPostingAccess` și `IApplicationListAccess` sunt interfețe separate, focalizate. Proxy-ul de protecție nu este forțat să implementeze metode din proxy-ul virtual și viceversa — segregarea este corectă. |
| **DIP** — Dependency Inversion | ✅ | Clienții depind de `IJobPostingAccess` și `IApplicationListAccess` (abstracții). `JobPostingProtectionProxy` depinde de `RealJobPostingAccess` prin compoziție, nu prin moștenire directă. Dependința este gestionată prin interfețe. |

### 4.2 Avantaje

1. **Securitate prin Protection Proxy** — utilizatorii neautentificați primesc date trunchiate (company name ascuns, salariu ascuns, descriere trunchiată), fără ca logica de securitate să polueze `RealJobPostingAccess`. Controlul accesului este centralizat.
2. **Performanță prin Virtual Proxy + caching** — `ApplicationListVirtualProxy` încarcă datele din DB doar la primul acces și le cacheează pentru cererile ulterioare, cu double-check locking thread-safe, reducând semnificativ numărul de query-uri.
3. **Transparență totală pentru client** — clientul interacționează cu `IJobPostingAccess` fără a ști dacă primește un proxy sau obiectul real. Schimbarea comportamentului (autentificare, lazy loading) nu necesită modificări în codul client.

### 4.3 Dezavantaje

1. **SRP parțial violat în Virtual Proxy** — `ApplicationListVirtualProxy` gestionează simultan: lazy initialization, cache management și invalidare. Aceasta este o supraîncărcare de responsabilități ce ar putea fi separată (ex: un `CacheManager` dedicat).
2. **Latență introdusă** — fiecare apel trece prin proxy înainte de a ajunge la obiectul real. Verificările de autentificare din Protection Proxy sau operațiunile de locking din Virtual Proxy adaugă overhead la fiecare request, vizibil sub load mare.
3. **Complexitate de debugging** — erorile pot fi obscure: un client care primește date trunchiate de la Protection Proxy poate crede că sunt bug-uri în date, nu restricții de acces. Stack trace-ul ascunde că proxy-ul a interceptat cererea.

---

## Sumar comparativ

| Pattern | SRP | OCP | LSP | ISP | DIP | SOLID Score |
|---------|-----|-----|-----|-----|-----|-------------|
| **Flyweight** | ✅ | ⚠️ | ✅ | ⚠️ | ❌ | 3/5 |
| **Decorator** | ✅ | ✅ | ✅ | ✅ | ✅ | **5/5** |
| **Bridge** | ✅ | ✅ | ✅ | ⚠️ | ✅ | 4.5/5 |
| **Proxy** | ⚠️ | ✅ | ✅ | ✅ | ✅ | 4/5 |

---

## Surse

- Refactoring.Guru — [Flyweight](https://refactoring.guru/design-patterns/flyweight), [Decorator](https://refactoring.guru/design-patterns/decorator), [Bridge](https://refactoring.guru/design-patterns/bridge), [Proxy](https://refactoring.guru/design-patterns/proxy)
- GeeksforGeeks — Flyweight, Decorator, Bridge, Proxy Design Pattern
- Codementor / Adrian Bilescu — *Decorator Pattern and SOLID Principles*
- Scaler Topics — Bridge Design Pattern, Proxy Design Pattern
- Belatrix Blog — Decorator Design Pattern, Bridge Design Patterns
- OODesign.com — Flyweight Pattern, Interface Segregation Principle
- S.I.R World — *Software Design Patterns Part 11: Proxy Design Pattern*
