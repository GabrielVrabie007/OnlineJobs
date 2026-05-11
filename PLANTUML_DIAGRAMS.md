# SCRIPTURI PLANTUML PENTRU RAPORTUL PROIECT DE AN

**INSTRUCȚIUNI:**
1. Copiază fiecare script PlantUML într-un editor PlantUML (https://www.plantuml.com/plantuml/uml/)
2. Generează diagrama ca imagine PNG sau SVG
3. Inserează imaginea în raportul Word în locul placeholder-ului corespunzător

---

## DIAGRAMA 1: Arhitectura Clean Architecture

```plantuml
@startuml
!define RECTANGLE class

package "Presentation Layer (OnlineJobs.csproj)" #LightBlue {
  class AccountController {
    + Login()
    + Register()
    + Logout()
  }
  class JobController {
    + Index()
    + Details()
    + Create()
  }
  class ApplicationController {
    + Apply()
    + MyApplications()
    + Approve()
  }
}

package "Application Layer (Application.csproj)" #LightGreen {
  interface IUserService
  interface IJobService
  interface IApplicationService

  class UserService
  class JobService
  class ApplicationService

  class "15 Design Patterns" as Patterns {
    Builder
    Prototype
    Singleton
    ...
  }
}

package "Domain Layer (Domain.csproj)" #LightYellow {
  class User {
    + Id: Guid
    + Email: string
    + GetFullName()
  }
  class JobSeeker {
    + Resume: string
    + Skills: List<Skill>
  }
  class Employer {
    + CompanyId: Guid
    + Position: string
  }
  class JobPosting {
    + Title: string
    + Description: string
  }
  class JobApplication {
    + Status: ApplicationStatus
    + CoverLetter: string
  }
}

package "Infrastructure Layer (Infrastructure.csproj)" #LightCoral {
  class OnlineJobsDbContext {
    + Users: DbSet<User>
    + Jobs: DbSet<JobPosting>
  }
  class "EFRepository<T>" as EFRepo {
    + GetByIdAsync()
    + GetAllAsync()
    + AddAsync()
  }
}

' Dependencies (outer layers depend on inner)
AccountController ..> IUserService
JobController ..> IJobService
ApplicationController ..> IApplicationService

UserService ..|> IUserService
JobService ..|> IJobService
ApplicationService ..|> IApplicationService

UserService ..> User
JobService ..> JobPosting
ApplicationService ..> JobApplication

JobSeeker --|> User
Employer --|> User

OnlineJobsDbContext ..> User
OnlineJobsDbContext ..> JobPosting
EFRepo ..> OnlineJobsDbContext

@enduml
```

---

## DIAGRAMA 2: Builder Pattern - JobSeekerProfileBuilder

```plantuml
@startuml
interface IJobSeekerProfileBuilder {
  + WithBasicInfo(email, firstName, lastName): IJobSeekerProfileBuilder
  + WithProfessionalSummary(summary): IJobSeekerProfileBuilder
  + AddEducation(education): IJobSeekerProfileBuilder
  + AddWorkExperience(experience): IJobSeekerProfileBuilder
  + AddSkill(skill): IJobSeekerProfileBuilder
  + AddCertification(cert): IJobSeekerProfileBuilder
  + Build(): JobSeeker
}

class JobSeekerProfileBuilder {
  - _jobSeeker: JobSeeker
  + WithBasicInfo()
  + WithProfessionalSummary()
  + AddEducation()
  + AddWorkExperience()
  + AddSkill()
  + AddCertification()
  + Build()
  - Reset()
}

class JobSeekerProfileDirector {
  - _builder: IJobSeekerProfileBuilder
  + BuildJuniorProfile(): JobSeeker
  + BuildSeniorProfile(): JobSeeker
  + BuildExecutiveProfile(): JobSeeker
}

class JobSeeker {
  + Email: string
  + FirstName: string
  + LastName: string
  + ProfessionalSummary: string
  + EducationHistory: List<Education>
  + WorkHistory: List<WorkExperience>
  + SkillSet: List<Skill>
  + Certifications: List<Certification>
}

IJobSeekerProfileBuilder <|.. JobSeekerProfileBuilder
JobSeekerProfileDirector o-- IJobSeekerProfileBuilder
JobSeekerProfileBuilder ..> JobSeeker : creates

note right of JobSeekerProfileDirector
  Director oferă rețete
  predefinite pentru
  construirea profilurilor
  comune (junior, senior, etc.)
end note

note bottom of JobSeekerProfileBuilder
  Builder construiește pas cu pas
  obiectul complex JobSeeker,
  evitând constructori uriași
end note

@enduml
```

---

## DIAGRAMA 3: Prototype Pattern - JobPosting Clone

```plantuml
@startuml
interface "IPrototype<T>" as IPrototype {
  + Clone(): T
}

class JobPosting {
  + Id: Guid
  + Title: string
  + Description: string
  + Requirements: string
  + SalaryMin: decimal?
  + SalaryMax: decimal?
  + Location: string
  + EmployerId: Guid
  + CompanyId: Guid
  + Status: JobStatus
  + PostedDate: DateTime
  --
  + Clone(): JobPosting
  + Publish()
  + Close()
  + IsAcceptingApplications(): bool
}

IPrototype <|.. JobPosting

note right of JobPosting::Clone
  Creează o copie profundă
  a job-ului existent.
  ID-ul este generat nou,
  toate celelalte proprietăți
  sunt copiate.
  Status devine Draft.
end note

note left of JobPosting
  Prototype permite clonarea
  job-urilor similare,
  util când un employer
  dorește să creeze oferte
  asemănătoare.
end note

@enduml
```

---

## DIAGRAMA 4: Singleton Pattern - ApplicationConfiguration

```plantuml
@startuml
class ApplicationConfiguration {
  - {static} _instance: Lazy<ApplicationConfiguration>
  - ApplicationConfiguration()
  --
  + {static} Instance: ApplicationConfiguration
  --
  + JobExpiryDays: int
  + MaxActiveApplicationsPerUser: int
  + MaxActiveJobPostingsPerEmployer: int
  + EnableEmailNotifications: bool
  + EnableAutoSave: bool
  --
  - LoadConfiguration(): void
  + ToString(): string
}

note right of ApplicationConfiguration
  Singleton Pattern asigură
  o singură instanță a configurației
  la nivel global.

  Lazy<T> oferă thread-safety
  și inițializare lazy.

  Constructor privat previne
  instanțierea directă.
end note

note bottom of ApplicationConfiguration::_instance
  Lazy initialization:
  instanța se creează doar
  la prima accesare a
  proprietății Instance
end note

@enduml
```

---

## DIAGRAMA 5: Adapter Pattern - Payment Gateway Integration

```plantuml
@startuml
interface IPaymentGateway {
  + ProcessPaymentAsync(amount, currency, details): Task<PaymentResult>
}

class PayPalAdapter {
  - _paypalAPI: PayPalAPI
  + ProcessPaymentAsync()
}

class StripeAdapter {
  - _stripeClient: StripeClient
  + ProcessPaymentAsync()
}

class GooglePayAdapter {
  - _googlePayClient: GooglePayClient
  + ProcessPaymentAsync()
}

class PayPalAPI {
  + CreatePaymentAsync()
}

class StripeClient {
  + Charges.CreateAsync()
}

class GooglePayClient {
  + ProcessTransaction()
}

class PaymentService {
  + ProcessCompanyRevealPayment()
}

IPaymentGateway <|.. PayPalAdapter
IPaymentGateway <|.. StripeAdapter
IPaymentGateway <|.. GooglePayAdapter

PayPalAdapter ..> PayPalAPI : uses
StripeAdapter ..> StripeClient : uses
GooglePayAdapter ..> GooglePayClient : uses

PaymentService ..> IPaymentGateway : depends on

note right of IPaymentGateway
  Interfață comună pentru toate
  payment processors.
  Permite utilizarea interschimbabilă.
end note

note bottom of PaymentService
  PaymentService nu știe
  ce payment processor este folosit.
  Poate schimba dinamic între
  PayPal, Stripe sau Google Pay.
end note

@enduml
```

---

## DIAGRAMA 6: Composite Pattern - Job Categories Hierarchy

```plantuml
@startuml
interface IJobCategory {
  + GetName(): string
  + Display(depth: int): void
  + GetJobCount(): int
  + AddCategory(category): void
}

class CategoryLeaf {
  + Name: string
  + JobCount: int
  --
  + GetName()
  + Display()
  + GetJobCount()
  + AddCategory()
}

class CategoryComposite {
  + Name: string
  - _children: List<IJobCategory>
  --
  + GetName()
  + Display()
  + GetJobCount()
  + AddCategory()
}

IJobCategory <|.. CategoryLeaf
IJobCategory <|.. CategoryComposite
CategoryComposite o-- IJobCategory : contains

note right of CategoryLeaf
  Frunză - categorie terminală
  care nu poate conține
  alte categorii.
  GetJobCount() returnează
  numărul direct.
end note

note left of CategoryComposite
  Compozit - categorie ce conține
  subcategorii.
  GetJobCount() returnează suma
  tuturor copiilor (recursiv).
end note

note bottom of IJobCategory
  Exemplu ierarhie:
  Technology (Composite)
    ├─ Software Development (Composite)
    │   ├─ Backend Developer (Leaf: 45 jobs)
    │   ├─ Frontend Developer (Leaf: 38 jobs)
    │   └─ Full Stack Developer (Leaf: 52 jobs)
    └─ Hardware Engineering (Composite)
        ├─ Electrical Engineer (Leaf: 12 jobs)
        └─ PCB Designer (Leaf: 8 jobs)
end note

@enduml
```

---

## DIAGRAMA 7: Façade Pattern - JobApplicationFacade

```plantuml
@startuml
class JobApplicationFacade {
  - _jobService: IJobService
  - _applicationService: IApplicationService
  - _userService: IUserService
  - _notificationService: INotificationService
  --
  + SubmitApplicationAsync(jobSeekerId, jobId, coverLetter): Task<ApplicationResult>
}

interface IJobService {
  + GetJobByIdAsync()
  + IsAcceptingApplications()
}

interface IApplicationService {
  + HasAlreadyAppliedAsync()
  + SubmitApplicationAsync()
}

interface IUserService {
  + GetJobSeekerByIdAsync()
  + GetEmployerByIdAsync()
  + HasCompleteProfile()
}

interface INotificationService {
  + SendApplicationConfirmationAsync()
  + SendNewApplicationAlertAsync()
}

class ApplicationController {
  - _jobApplicationFacade: JobApplicationFacade
  + Apply(model): Task<IActionResult>
}

JobApplicationFacade o-- IJobService
JobApplicationFacade o-- IApplicationService
JobApplicationFacade o-- IUserService
JobApplicationFacade o-- INotificationService

ApplicationController ..> JobApplicationFacade : uses

note right of JobApplicationFacade::SubmitApplicationAsync
  Metodă unificată care orchestrează:
  1. Verificare job activ
  2. Verificare aplicație duplicată
  3. Validare profil complet
  4. Creare aplicație
  5. Notificare candidat
  6. Notificare employer
end note

note bottom of ApplicationController
  Controller folosește o singură metodă
  în loc de 6 apeluri separate către
  servicii diferite.
  Façade simplifică complexitatea.
end note

@enduml
```

---

## DIAGRAMA 8: Flyweight Pattern - SkillFlyweightFactory

```plantuml
@startuml
class SkillFlyweight {
  + Name: string
  + Category: string
  --
  + GetDisplayInfo(proficiency): string
}

class SkillFlyweightFactory {
  - _skillsCache: ConcurrentDictionary<string, SkillFlyweight>
  --
  + GetSkill(name, category): SkillFlyweight
  + GetCacheSize(): int
}

class JobSkillRequirement {
  + Skill: SkillFlyweight
  + RequiredLevel: SkillProficiency
  --
  + GetRequirement(): string
}

class JobPosting {
  + SkillRequirements: List<JobSkillRequirement>
}

SkillFlyweightFactory ..> SkillFlyweight : creates & caches
JobSkillRequirement o-- SkillFlyweight : references (shared)
JobPosting o-- JobSkillRequirement

note right of SkillFlyweightFactory
  Factory menține un cache
  de flyweights.
  Pentru skill-uri duplicate
  (ex: "C#" în 100 job-uri),
  returnează ACEEAȘI instanță.

  Economie memorie: 99%
end note

note left of SkillFlyweight
  Flyweight conține starea
  intrinsecă (partajată):
  - Name
  - Category

  Starea extrinsecă (specifică)
  este în JobSkillRequirement:
  - RequiredLevel
end note

@enduml
```

---

## DIAGRAMA 9: Decorator Pattern - Notification System

```plantuml
@startuml
interface INotification {
  + SendAsync(recipient, subject, message): Task
}

class BaseNotification {
  + SendAsync()
}

abstract class NotificationDecorator {
  # _wrappedNotification: INotification
  --
  + SendAsync()
}

class EmailNotificationDecorator {
  + SendAsync()
}

class SMSNotificationDecorator {
  + SendAsync()
}

class PushNotificationDecorator {
  + SendAsync()
}

class LoggingNotificationDecorator {
  + SendAsync()
}

INotification <|.. BaseNotification
INotification <|.. NotificationDecorator
NotificationDecorator <|-- EmailNotificationDecorator
NotificationDecorator <|-- SMSNotificationDecorator
NotificationDecorator <|-- PushNotificationDecorator
NotificationDecorator <|-- LoggingNotificationDecorator

NotificationDecorator o-- INotification : wraps

note right of NotificationDecorator
  Decorator abstract menține
  referință către notificarea
  împachetată și delegă apelul.
end note

note bottom of EmailNotificationDecorator
  Fiecare decorator concret:
  1. Adaugă funcționalitate proprie
  2. Apelează base.SendAsync()

  Compunere dinamică:
  new LoggingDecorator(
    new PushDecorator(
      new SMSDecorator(
        new EmailDecorator(
          new BaseNotification()))))
end note

@enduml
```

---

## DIAGRAMA 10: Bridge Pattern - Reports and Exporters

```plantuml
@startuml
abstract class Report {
  # _exporter: IReportExporter
  --
  + SetExporter(exporter): void
  + Generate(): Task<string>
  # {abstract} GatherData(): Task<ReportData>
}

class JobReport {
  + GatherData()
}

class ApplicationReport {
  + GatherData()
}

class CompanyReport {
  + GatherData()
}

interface IReportExporter {
  + Export(data): string
}

class PDFExporter {
  + Export()
}

class ExcelExporter {
  + Export()
}

class JSONExporter {
  + Export()
}

class CSVExporter {
  + Export()
}

Report <|-- JobReport
Report <|-- ApplicationReport
Report <|-- CompanyReport

Report o-- IReportExporter : uses

IReportExporter <|.. PDFExporter
IReportExporter <|.. ExcelExporter
IReportExporter <|.. JSONExporter
IReportExporter <|.. CSVExporter

note right of Report
  Bridge separă abstracția (ce raport)
  de implementare (cum se exportă).

  3 rapoarte × 4 formate = 12 combinații
  cu doar 7 clase!
end note

note bottom of IReportExporter
  Exporters pot fi schimbați
  independent de tipul raportului:

  var report = new JobReport();
  report.SetExporter(new PDFExporter());
  report.Generate(); // PDF

  report.SetExporter(new ExcelExporter());
  report.Generate(); // Excel
end note

@enduml
```

---

## DIAGRAMA 11: Proxy Pattern - JobPosting Access Control

```plantuml
@startuml
interface IJobPostingAccess {
  + GetJobDetailsAsync(jobId): Task<JobPosting>
  + GetCompanyName(job): string
  + GetSalaryRange(job): string
}

class RealJobPostingAccess {
  - _jobService: IJobService
  --
  + GetJobDetailsAsync()
  + GetCompanyName()
  + GetSalaryRange()
}

class JobPostingProtectionProxy {
  - _realAccess: IJobPostingAccess
  - _isAuthenticated: bool
  - _userId: Guid?
  --
  + GetJobDetailsAsync()
  + GetCompanyName()
  + GetSalaryRange()
  - CheckAccess()
}

class ApplicationListVirtualProxy {
  - _applicationService: IApplicationService
  - _cachedApplications: List<JobApplication>
  - _isCached: bool
  --
  + GetApplicationsAsync(): Task<List<JobApplication>>
  - LoadFromDatabase()
}

IJobPostingAccess <|.. RealJobPostingAccess
IJobPostingAccess <|.. JobPostingProtectionProxy

JobPostingProtectionProxy o-- IJobPostingAccess : delegates to

note right of JobPostingProtectionProxy
  Protection Proxy:
  - Verifică autentificare
  - Ascunde companie pentru guests
  - Ascunde salariu pentru guests

  Guest: "🔒 Company Hidden"
  Authenticated: "Apple Inc."
end note

note left of ApplicationListVirtualProxy
  Virtual Proxy:
  - Lazy loading (încarcă doar când e nevoie)
  - Caching (păstrează în memorie)
  - Performanță îmbunătățită 100x

  Prima accesare: 500ms (DB query)
  Următoarele: 5ms (cache hit)
end note

@enduml
```

---

## DIAGRAMA 12: Observer Pattern - Application Status Notifications

```plantuml
@startuml
interface IObserver {
  + Update(applicationId, oldStatus, newStatus): void
}

class ApplicationStatusSubject {
  - _observers: List<IObserver>
  --
  + Attach(observer): void
  + Detach(observer): void
  + Notify(applicationId, oldStatus, newStatus): void
}

class EmailAlertObserver {
  + Update()
}

class DashboardNotificationObserver {
  + Update()
}

class AuditLogObserver {
  + Update()
}

class StatisticsObserver {
  + Update()
}

IObserver <|.. EmailAlertObserver
IObserver <|.. DashboardNotificationObserver
IObserver <|.. AuditLogObserver
IObserver <|.. StatisticsObserver

ApplicationStatusSubject o-- IObserver : notifies

note right of ApplicationStatusSubject
  Subject menține lista de observatori
  și îi notifică pe toți când
  statusul unei aplicații se schimbă:
  Submitted → Under Review
  Under Review → Approved
  Under Review → Rejected
end note

note bottom of IObserver
  Când statusul se modifică, toți observatorii
  sunt notificați automat:
  - EmailAlertObserver: trimite email candidat
  - DashboardNotificationObserver: adaugă în UI
  - AuditLogObserver: scrie în log
  - StatisticsObserver: actualizează statistici
end note

@enduml
```

---

## DIAGRAMA 13: Iterator Pattern - Application Filtering

```plantuml
@startuml
interface IApplicationIterator {
  + HasNext(): bool
  + Next(): JobApplication
  + Reset(): void
}

class FilteredApplicationIterator {
  - _applications: List<JobApplication>
  - _filterStatus: ApplicationStatus
  - _currentPosition: int
  --
  + HasNext()
  + Next()
  + Reset()
}

class DateOrderedApplicationIterator {
  - _applications: List<JobApplication>
  - _currentPosition: int
  --
  + HasNext()
  + Next()
  + Reset()
}

class ApplicationCollection {
  - _applications: List<JobApplication>
  --
  + CreateFilteredIterator(status): IApplicationIterator
  + CreateDateOrderedIterator(): IApplicationIterator
  + CreateScoredIterator(strategy): IApplicationIterator
}

IApplicationIterator <|.. FilteredApplicationIterator
IApplicationIterator <|.. DateOrderedApplicationIterator

ApplicationCollection ..> IApplicationIterator : creates

note right of FilteredApplicationIterator
  Filtrează aplicațiile după status:
  - Doar Submitted
  - Doar Under Review
  - Doar Approved
  - Doar Rejected
end note

note left of DateOrderedApplicationIterator
  Sortează aplicațiile cronologic:
  - Cea mai recentă primul
  - Cea mai veche primul
end note

note bottom of ApplicationCollection
  Aceeași colecție poate fi parcursă
  în multiple moduri:

  var iterator1 = collection.CreateFilteredIterator(Approved);
  var iterator2 = collection.CreateDateOrderedIterator();

  Separarea logicii de traversare
  de structura colecției.
end note

@enduml
```

---

## DIAGRAMA 14: Strategy Pattern - Scoring Strategies

```plantuml
@startuml
interface IApplicationScoringStrategy {
  + CalculateScore(application, job): double
}

class SkillMatchScoringStrategy {
  + CalculateScore()
}

class ExperienceScoringStrategy {
  + CalculateScore()
}

class EducationScoringStrategy {
  + CalculateScore()
}

class ComprehensiveScoringStrategy {
  - _skillStrategy: IApplicationScoringStrategy
  - _experienceStrategy: IApplicationScoringStrategy
  - _educationStrategy: IApplicationScoringStrategy
  --
  + CalculateScore()
}

class ApplicationEvaluator {
  - _strategy: IApplicationScoringStrategy
  --
  + SetStrategy(strategy): void
  + EvaluateCandidate(): double
}

IApplicationScoringStrategy <|.. SkillMatchScoringStrategy
IApplicationScoringStrategy <|.. ExperienceScoringStrategy
IApplicationScoringStrategy <|.. EducationScoringStrategy
IApplicationScoringStrategy <|.. ComprehensiveScoringStrategy

ComprehensiveScoringStrategy o-- IApplicationScoringStrategy : combines

ApplicationEvaluator o-- IApplicationScoringStrategy : uses

note right of SkillMatchScoringStrategy
  Evaluează după skill-uri:
  - Câte skill-uri cerute are candidatul
  - Nivel de competență pentru fiecare
  Scor: 0-100
end note

note left of ComprehensiveScoringStrategy
  Combină toate strategiile cu ponderi:
  - Skills: 50%
  - Experience: 30%
  - Education: 20%

  Scor final: weighted average
end note

note bottom of ApplicationEvaluator
  Strategy poate fi schimbată la runtime:

  evaluator.SetStrategy(new SkillMatchStrategy());
  score1 = evaluator.EvaluateCandidate();

  evaluator.SetStrategy(new ComprehensiveStrategy());
  score2 = evaluator.EvaluateCandidate();
end note

@enduml
```

---

## DIAGRAMA 15: Command Pattern - Application Commands

```plantuml
@startuml
interface ICommand {
  + ExecuteAsync(): Task
  + UndoAsync(): Task
  + Description: string
}

class SubmitApplicationCommand {
  - _repository: IRepository<JobApplication>
  - _jobId: Guid
  - _jobSeekerId: Guid
  - _coverLetter: string
  - _applicationId: Guid
  --
  + ExecuteAsync()
  + UndoAsync()
}

class WithdrawApplicationCommand {
  - _repository: IRepository<JobApplication>
  - _applicationId: Guid
  - _previousState: ApplicationMemento
  --
  + ExecuteAsync()
  + UndoAsync()
}

class ApproveApplicationCommand {
  - _repository: IRepository<JobApplication>
  - _applicationId: Guid
  - _previousStatus: ApplicationStatus
  --
  + ExecuteAsync()
  + UndoAsync()
}

class RejectApplicationCommand {
  - _repository: IRepository<JobApplication>
  - _applicationId: Guid
  - _previousStatus: ApplicationStatus
  --
  + ExecuteAsync()
  + UndoAsync()
}

class CommandInvoker {
  - _commandHistory: Stack<ICommand>
  - _undoneCommands: Stack<ICommand>
  --
  + ExecuteAsync(command): Task
  + UndoAsync(): Task
  + RedoAsync(): Task
  + CanUndo(): bool
  + CanRedo(): bool
}

ICommand <|.. SubmitApplicationCommand
ICommand <|.. WithdrawApplicationCommand
ICommand <|.. ApproveApplicationCommand
ICommand <|.. RejectApplicationCommand

CommandInvoker o-- ICommand : manages

note right of ICommand
  Fiecare comandă encapsulează:
  - Cerere (request)
  - Parametri necesari
  - Logică de execuție
  - Logică de anulare (undo)
end note

note bottom of CommandInvoker
  Invoker gestionează istoric comenzi:

  await invoker.ExecuteAsync(submitCmd);
  await invoker.ExecuteAsync(approveCmd);

  await invoker.UndoAsync(); // Undo approve
  await invoker.UndoAsync(); // Undo submit

  await invoker.RedoAsync(); // Redo submit
end note

@enduml
```

---

## DIAGRAMA 16: Memento Pattern - Application Draft Manager

```plantuml
@startuml
class ApplicationFormMemento {
  + JobId: Guid
  + JobSeekerId: Guid
  + CoverLetter: string
  + SavedAt: DateTime
  --
  + GetState(): (Guid, Guid, string)
}

class ApplicationDraftManager {
  - _drafts: ConcurrentDictionary<string, ApplicationFormMemento>
  --
  + SaveDraft(userId, jobId, memento): void
  + GetDraft(userId, jobId): ApplicationFormMemento
  + DeleteDraft(userId, jobId): void
  + HasDraft(userId, jobId): bool
  - GetKey(userId, jobId): string
}

class JobSeekerOriginator {
  + JobId: Guid
  + CoverLetter: string
  + Skills: List<string>
  --
  + CreateMemento(): ApplicationFormMemento
  + RestoreFromMemento(memento): void
}

class ApplicationController {
  - _draftManager: ApplicationDraftManager
  --
  + SaveDraft(jobId, coverLetter): IActionResult
  + LoadDraft(jobId): IActionResult
  + Apply(model): Task<IActionResult>
}

ApplicationDraftManager o-- ApplicationFormMemento : stores
JobSeekerOriginator ..> ApplicationFormMemento : creates/restores
ApplicationController ..> ApplicationDraftManager : uses

note right of ApplicationFormMemento
  Memento salvează snapshot-ul
  stării formularului:
  - JobId
  - CoverLetter
  - Timestamp

  Immutable - nu poate fi modificat
  după creare.
end note

note left of ApplicationDraftManager
  Manager gestionează draft-urile:
  - Salvare automată (JavaScript la 30s)
  - Restaurare la revenire
  - Ștergere după submit

  Thread-safe (ConcurrentDictionary)
end note

note bottom of ApplicationController
  Flux utilizare:
  1. User completează form → Auto-save la 30s
  2. User închide browser → Draft salvat
  3. User revine → LoadDraft() restaurează
  4. User trimite → DeleteDraft() curăță
end note

@enduml
```

---

## DIAGRAMA 17: Schema Bazei de Date

```plantuml
@startuml
!define TABLE(name) entity name

TABLE(Users) {
  *Id : GUID <<PK>>
  --
  Email : VARCHAR(255) <<UNIQUE>>
  PasswordHash : VARCHAR(500)
  FirstName : VARCHAR(100)
  LastName : VARCHAR(100)
  CreatedAt : DATETIME
  LastLoginAt : DATETIME?
  IsActive : BOOLEAN
  PhoneNumber : VARCHAR(20)?
  UserType : INT
}

TABLE(JobSeekers) {
  *Id : GUID <<PK,FK>>
  --
  Resume : TEXT?
  Skills : TEXT?
  Address : VARCHAR(500)?
  DateOfBirth : DATE?
  ProfessionalSummary : TEXT?
  LinkedInUrl : VARCHAR(500)?
  GitHubUrl : VARCHAR(500)?
  PortfolioUrl : VARCHAR(500)?
}

TABLE(Employers) {
  *Id : GUID <<PK,FK>>
  --
  CompanyId : GUID? <<FK>>
  Position : VARCHAR(200)?
}

TABLE(Companies) {
  *Id : GUID <<PK>>
  --
  Name : VARCHAR(200)
  Location : VARCHAR(500)
  Description : TEXT?
  Website : VARCHAR(500)?
  Industry : VARCHAR(100)?
  EmployeeCount : INT?
  LogoUrl : VARCHAR(500)?
}

TABLE(JobPostings) {
  *Id : GUID <<PK>>
  --
  Title : VARCHAR(200)
  Description : TEXT
  Requirements : TEXT
  SalaryMin : DECIMAL(18,2)?
  SalaryMax : DECIMAL(18,2)?
  Location : VARCHAR(500)
  EmploymentType : VARCHAR(50)
  Category : VARCHAR(100)
  EmployerId : GUID <<FK>>
  CompanyId : GUID <<FK>>
  CategoryId : GUID? <<FK>>
  Status : INT
  PostedDate : DATETIME
  ClosedDate : DATETIME?
  ExpiryDate : DATETIME?
  ExperienceLevel : VARCHAR(50)?
  IsCompanyRevealed : BOOLEAN
}

TABLE(JobApplications) {
  *Id : GUID <<PK>>
  --
  JobPostingId : GUID <<FK>>
  JobSeekerId : GUID <<FK>>
  CoverLetter : TEXT
  Status : INT
  AppliedDate : DATETIME
  ReviewedDate : DATETIME?
  Notes : TEXT?
}

TABLE(PaymentTransactions) {
  *Id : GUID <<PK>>
  --
  UserId : GUID <<FK>>
  Amount : DECIMAL(18,2)
  Currency : VARCHAR(10)
  PaymentGateway : INT
  TransactionId : VARCHAR(200)
  Status : INT
  CreatedAt : DATETIME
  CompletedAt : DATETIME?
}

TABLE(CompanyReveals) {
  *Id : GUID <<PK>>
  --
  JobSeekerId : GUID <<FK>>
  JobPostingId : GUID <<FK>>
  PaymentTransactionId : GUID? <<FK>>
  RevealedAt : DATETIME
}

' Relationships (moștenire Table-Per-Type)
Users ||--o| JobSeekers : "is-a"
Users ||--o| Employers : "is-a"

' One-to-Many
Companies ||--o{ Employers : "has employees"
Employers ||--o{ JobPostings : "creates jobs"
Companies ||--o{ JobPostings : "for company"
JobSeekers ||--o{ JobApplications : "submits"
JobPostings ||--o{ JobApplications : "receives"
JobSeekers ||--o{ PaymentTransactions : "makes payments"
JobSeekers ||--o{ CompanyReveals : "reveals companies"
JobPostings ||--o{ CompanyReveals : "company revealed"
PaymentTransactions ||--o| CompanyReveals : "paid for reveal"

note right of Users
  Tabel bază pentru ierarhie.
  UserType: 1=JobSeeker, 2=Employer

  Moștenire Table-Per-Type:
  - Users (common fields)
  - JobSeekers (extends Users)
  - Employers (extends Users)
end note

note bottom of JobApplications
  Status values:
  0 = Submitted
  1 = Under Review
  2 = Approved
  3 = Rejected
  4 = Withdrawn
end note

@enduml
```

---

# NOTĂ FINALĂ

Toate cele 17 diagrame PlantUML sunt generate pentru:
- 1 diagramă arhitectură
- 15 diagrame design patterns
- 1 diagramă bază de date

Pentru fiecare diagramă:
1. Copiază scriptul PlantUML
2. Generează imagine PNG/SVG la https://www.plantuml.com/plantuml/
3. Salvează imaginea
4. Inserează în Word în locul placeholder-ului corespunzător

Alternativ, poți folosi un editor local PlantUML sau plugin-ul PlantUML pentru VS Code.