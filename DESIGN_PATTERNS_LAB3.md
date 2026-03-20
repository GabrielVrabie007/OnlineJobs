
# Laborator 3 - Paternurile de Proiectare Creaționale: Builder, Prototype și Singleton

## Cuprins
- [1. Introducere](#1-introducere)
- [2. Builder Pattern](#2-builder-pattern)
- [3. Prototype Pattern](#3-prototype-pattern)
- [4. Singleton Pattern](#4-singleton-pattern)
- [5. Implementare în OnlineJobs](#5-implementare-în-onlinejobs)
- [6. Testare și Demonstrare](#6-testare-și-demonstrare)
- [7. Beneficii și Concluzii](#7-beneficii-și-concluzii)

---

## 1. Introducere

Acest laborator implementează trei paternuri de proiectare creaționale esențiale în cadrul platformei **OnlineJobs**:

### Paternurile Implementate

1. **Builder Pattern** - Construirea pas cu pas a profilurilor complexe de JobSeeker
2. **Prototype Pattern** - Clonarea entităților JobPosting și Company
3. **Singleton Pattern** - Gestionarea configurației aplicației

### Obiective

- ✅ Separarea logicii de construire de reprezentarea finală (Builder)
- ✅ Permiterea clonării obiectelor existente (Prototype)
- ✅ Asigurarea unei singure instanțe a clasei de configurare (Singleton)
- ✅ Respectarea principiilor SOLID
- ✅ Menținerea clean architecture (Domain → Application → Infrastructure → Presentation)

---

## 2. Builder Pattern

### 2.1 Descriere

**Builder Pattern** este un patern de proiectare creațional care permite construirea obiectelor complexe pas cu pas. Separă construcția unui obiect complex de reprezentarea sa, astfel încât același proces de construcție poate crea diferite reprezentări.

### 2.2 Problemă Rezolvată

**Fără Builder:**
```csharp
// Crearea unui JobSeeker cu toate detaliile este complexă și prone la erori
var jobSeeker = new JobSeeker("email@example.com", "John", "Doe");
jobSeeker.ProfessionalSummary = "Long summary...";
jobSeeker.LinkedInUrl = "...";
jobSeeker.GitHubUrl = "...";
jobSeeker.EducationHistory.Add(new Education(...)); // parametri complecși
jobSeeker.WorkHistory.Add(new WorkExperience(...));
// Posibilitate de erori: uităm să adăugăm skill-uri, certificări, etc.
```

**Cu Builder:**
```csharp
var jobSeeker = new JobSeekerProfileBuilder()
    .WithBasicInfo("email@example.com", "John", "Doe", "+1234567890")
    .WithProfessionalSummary("Experienced developer...")
    .AddEducation(new Education("BSc", "MIT", "Computer Science", ...))
    .AddWorkExperience(new WorkExperience("Google", "Senior Engineer", ...))
    .AddSkill(new Skill("C#", SkillProficiency.Expert, 8))
    .Build();
```

### 2.3 Structură

#### A. Builder Interface

**Fișier:** `Application/Interfaces/IJobSeekerProfileBuilder.cs`

```csharp
public interface IJobSeekerProfileBuilder
{
    IJobSeekerProfileBuilder WithBasicInfo(string email, string firstName, string lastName, string? phoneNumber = null);
    IJobSeekerProfileBuilder WithPersonalDetails(DateTime? dateOfBirth, string? address);
    IJobSeekerProfileBuilder WithProfessionalSummary(string summary);
    IJobSeekerProfileBuilder WithOnlinePresence(string? linkedInUrl = null, string? gitHubUrl = null, string? portfolioUrl = null);
    IJobSeekerProfileBuilder AddEducation(Education education);
    IJobSeekerProfileBuilder AddWorkExperience(WorkExperience experience);
    IJobSeekerProfileBuilder AddSkill(Skill skill);
    IJobSeekerProfileBuilder AddCertification(Certification certification);
    void Reset();
    JobSeeker Build();
}
```

**Caracteristici:**
- **Fluent Interface** - Fiecare metodă returnează `IJobSeekerProfileBuilder` pentru method chaining
- **Flexibilitate** - Permite adăugarea opțională a diferitelor componente
- **Validare** - Fiecare metodă poate valida datele înainte de a le adăuga

#### B. Concrete Builder

**Fișier:** `Application/Builders/JobSeekerProfileBuilder.cs`

```csharp
public class JobSeekerProfileBuilder : IJobSeekerProfileBuilder
{
    private JobSeeker _jobSeeker;

    public JobSeekerProfileBuilder()
    {
        Reset();
    }

    public void Reset()
    {
        _jobSeeker = new JobSeeker();
    }

    public IJobSeekerProfileBuilder WithBasicInfo(string email, string firstName, string lastName, string? phoneNumber = null)
    {
        _jobSeeker.Email = email;
        _jobSeeker.FirstName = firstName;
        _jobSeeker.LastName = lastName;
        _jobSeeker.PhoneNumber = phoneNumber;
        return this;
    }

    // ... alte metode de construire

    public JobSeeker Build()
    {
        // Validare înainte de construire
        if (string.IsNullOrWhiteSpace(_jobSeeker.Email))
            throw new InvalidOperationException("Cannot build JobSeeker profile without email");

        var result = _jobSeeker;
        Reset(); // Pregătire pentru următoarea construire
        return result;
    }
}
```

**Responsabilități:**
- Menține starea obiectului în construcție (`_jobSeeker`)
- Implementează fiecare pas de construire
- Validează datele în timpul construcției
- Resetează builderul după `Build()` pentru reutilizare

#### C. Director

**Fișier:** `Application/Builders/JobSeekerProfileDirector.cs`

```csharp
public class JobSeekerProfileDirector
{
    private readonly IJobSeekerProfileBuilder _builder;

    public JobSeekerProfileDirector(IJobSeekerProfileBuilder builder)
    {
        _builder = builder;
    }

    public JobSeeker ConstructEntryLevelProfile(
        string email, string firstName, string lastName,
        string phoneNumber, Education education,
        string summary, List<Skill> skills)
    {
        return _builder
            .WithBasicInfo(email, firstName, lastName, phoneNumber)
            .AddEducation(education)
            .WithProfessionalSummary(summary)
            .AddSkills(skills)
            .Build();
    }

    public JobSeeker ConstructSeniorTechnicalProfile(...)
    {
        // Construiește profil pentru dezvoltatori seniori
    }

    public JobSeeker ConstructExperiencedProfile(...)
    {
        // Construiește profil complet pentru profesioniști experimentați
    }
}
```

**Scop:**
- Încapsulează algoritmii de construcție comuni
- Simplific codul client
- Oferă scenarii predefinite de construcție

#### D. Value Objects

**Fișiere:** `Domain/ValueObjects/`

Crearea următoarelor obiecte valoare imutabile:

1. **Education** - Educație (diplomă, instituție, domeniu, date, GPA)
2. **WorkExperience** - Experiență profesională (companie, poziție, responsabilități, realizări)
3. **Skill** - Competențe (nume, nivel de competență, experiență, categorie)
4. **Certification** - Certificări (nume, emitent, date, URL credential)

**Exemplu - Skill:**
```csharp
public class Skill
{
    public string Name { get; private set; }
    public SkillProficiency Proficiency { get; private set; }
    public int? YearsOfExperience { get; private set; }
    public string? Category { get; private set; }

    public Skill(string name, SkillProficiency proficiency,
                 int? yearsOfExperience = null, string? category = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty");

        Name = name;
        Proficiency = proficiency;
        YearsOfExperience = yearsOfExperience;
        Category = category;
    }
}
```

**Caracteristici Value Objects:**
- ✅ **Imutabile** - Proprietățile au `private set`
- ✅ **Validare în constructor** - Asigură integritatea datelor
- ✅ **Equality bazată pe valoare** - Override `Equals()` și `GetHashCode()`
- ✅ **Metode helper** - Ex: `IsCurrentlyEnrolled()`, `GetDuration()`

### 2.4 Utilizare

#### Exemplu 1: Utilizare Directă a Builder-ului

```csharp
var builder = new JobSeekerProfileBuilder();

var developer = builder
    .WithBasicInfo("john@example.com", "John", "Doe", "+1234567890")
    .WithProfessionalSummary("Experienced full-stack developer with 5+ years...")
    .AddEducation(new Education(
        "Bachelor of Science",
        "Stanford University",
        "Computer Science",
        new DateTime(2015, 9, 1),
        new DateTime(2019, 6, 1),
        3.8
    ))
    .AddWorkExperience(new WorkExperience(
        "Google",
        "Software Engineer",
        "Mountain View, CA",
        new DateTime(2019, 7, 1),
        null, // Current position
        "Developing cloud-based solutions"
    ))
    .AddSkill(new Skill("C#", SkillProficiency.Expert, 5, "Backend"))
    .AddSkill(new Skill("React", SkillProficiency.Advanced, 3, "Frontend"))
    .AddCertification(new Certification(
        "AWS Certified Solutions Architect",
        "Amazon Web Services",
        new DateTime(2021, 3, 15),
        new DateTime(2024, 3, 15)
    ))
    .Build();
```

#### Exemplu 2: Utilizare cu Director

```csharp
var builder = new JobSeekerProfileBuilder();
var director = new JobSeekerProfileDirector(builder);

// Profil senior tehnic predefinit
var seniorDev = director.ConstructSeniorTechnicalProfile(
    "sarah@example.com",
    "Sarah",
    "Wilson",
    "+1555-0123",
    "Expert in distributed systems and microservices...",
    workExperiences,  // List<WorkExperience>
    technicalSkills,  // List<Skill>
    "https://github.com/sarahwilson",
    "https://sarah.dev"
);
```

### 2.5 Avantaje

1. **Separare a logicii de construcție**
   - Codul de construcție este izolat de logica de business
   - Mai ușor de menținut și testat

2. **Control asupra procesului de construcție**
   - Construcție pas cu pas
   - Validare la fiecare pas
   - Posibilitate de a sări pași opționali

3. **Cod mai lizibil (Fluent Interface)**
   ```csharp
   builder
       .WithBasicInfo(...)
       .AddEducation(...)
       .AddSkill(...)
       .Build();
   ```

4. **Reutilizare**
   - Director-ul oferă scenarii predefinite
   - Același builder poate construi diferite variante

5. **Testabilitate**
   - Ușor de mockat `IJobSeekerProfileBuilder`
   - Fiecare metodă poate fi testată izolat

---

## 3. Prototype Pattern

### 3.1 Descriere

**Prototype Pattern** este un patern creațional care permite clonarea obiectelor existente fără a face codul dependent de clasele lor concrete. Prototipul delegă procesul de clonare către obiectele clonate.

### 3.2 Problemă Rezolvată

**Fără Prototype:**
```csharp
// Crearea unui job posting similar cu unul existent
var originalJob = jobRepository.GetById(id);

// Trebuie să copiem manual fiecare proprietate
var newJob = new JobPosting();
newJob.Title = "Similar title";
newJob.Description = originalJob.Description;
newJob.Requirements = originalJob.Requirements;
newJob.SalaryMin = originalJob.SalaryMin;
newJob.SalaryMax = originalJob.SalaryMax;
// ... 20+ linii de cod repetitiv
// Risc de a uita unele proprietăți!
```

**Cu Prototype:**
```csharp
// Clonare simplă și sigură
var clonedJob = originalJob.Clone();
clonedJob.Title = "Lead Software Engineer"; // Modifică doar ce e diferit
clonedJob.SalaryMin = 120000;
clonedJob.SalaryMax = 180000;
```

### 3.3 Structură

#### A. Prototype Interface

**Fișier:** `Domain/Interfaces/IPrototype.cs`

```csharp
public interface IPrototype<T> where T : class
{
    /// <summary>
    /// Creează o copie profundă (deep copy) a obiectului curent.
    /// Toate obiectele nested sunt de asemenea clonate.
    /// </summary>
    T Clone();

    /// <summary>
    /// Creează o copie superficială (shallow copy).
    /// Tipurile referință sunt partajate, nu duplicate.
    /// </summary>
    T ShallowCopy();
}
```

**Observație Arhitecturală:**
- Interface-ul este în `Domain/Interfaces/` NU în `Application/Interfaces/`
- **Motivație:** Domain layer NU trebuie să depindă de Application layer
- Respectă **Dependency Inversion Principle**

#### B. Implementare în JobPosting

**Fișier:** `Domain/Entities/JobPosting.cs`

```csharp
public class JobPosting : IPrototype<JobPosting>
{
    // ... proprietăți existente

    public JobPosting Clone()
    {
        var clonedJob = new JobPosting
        {
            // Identitate nouă
            Id = Guid.NewGuid(),

            // Copiere proprietăți
            Title = this.Title,
            Description = this.Description,
            Requirements = this.Requirements,
            SalaryMin = this.SalaryMin,
            SalaryMax = this.SalaryMax,
            Location = this.Location,
            EmploymentType = this.EmploymentType,
            Category = this.Category,

            // Păstrare referințe employer/company
            EmployerId = this.EmployerId,
            CompanyId = this.CompanyId,

            // Reset stare pentru job nou
            Status = JobStatus.Draft,
            PostedDate = DateTime.UtcNow,
            ClosedDate = null,
            ExpiryDate = this.ExpiryDate,

            // Colecții noi (deep copy - nu clonăm aplicațiile)
            Applications = new List<JobApplication>(),

            // Navigation properties null - se încarcă separat
            Employer = null,
            Company = null
        };

        return clonedJob;
    }

    public JobPosting ShallowCopy()
    {
        return (JobPosting)this.MemberwiseClone();
    }

    // Metode helper pentru scenarii comune
    public JobPosting CloneWithNewTitle(string newTitle)
    {
        var clone = this.Clone();
        clone.Title = newTitle;
        return clone;
    }

    public JobPosting CloneWith(Action<JobPosting> modifications)
    {
        var clone = this.Clone();
        modifications?.Invoke(clone);
        return clone;
    }
}
```

**Deep Copy vs Shallow Copy:**

| Aspect | Deep Copy (`Clone()`) | Shallow Copy (`ShallowCopy()`) |
|--------|----------------------|-------------------------------|
| **ID** | Nou `Guid.NewGuid()` | Același ID |
| **Primitive types** | Copiate | Copiate |
| **Colecții** | Noi `List<>()` | Aceeași referință! |
| **Status** | Reset la `Draft` | Același status |
| **PostedDate** | `DateTime.UtcNow` | Aceeași dată |
| **Utilizare** | Recomandat ✅ | Rareori necesar ⚠️ |

#### C. Implementare în Company

**Fișier:** `Domain/Entities/Company.cs`

```csharp
public class Company : IPrototype<Company>
{
    public Company Clone()
    {
        var clonedCompany = new Company
        {
            // Identitate nouă
            Id = Guid.NewGuid(),

            // Copiere detalii companie
            Name = this.Name,
            Description = this.Description,
            Website = this.Website,
            Location = this.Location,
            Industry = this.Industry,
            EmployeeCount = this.EmployeeCount,

            // Timestamp nou
            CreatedAt = DateTime.UtcNow,

            // Colecții noi goale
            JobPostings = new List<JobPosting>(),
            Employers = new List<Employer>()
        };

        return clonedCompany;
    }

    // Metode specializate pentru scenarii business
    public Company CloneWithNewName(string newName)
    {
        var clone = this.Clone();
        clone.Name = newName;
        return clone;
    }

    public Company CloneAsSubsidiary(string subsidiaryName, string newLocation)
    {
        var clone = this.Clone();
        clone.Name = subsidiaryName;
        clone.Location = newLocation;
        return clone;
    }
}
```

### 3.4 Utilizare

#### Exemplu 1: Clonare Job Posting

```csharp
// Employer vrea să posteze un job similar cu unul existent
var originalJob = await jobService.GetJobByIdAsync(existingJobId);

// Clonare cu modificări minime
var newJob = originalJob.Clone();
newJob.Title = "Senior Software Engineer"; // Diferă doar titlul
newJob.SalaryMin = 120000; // Și salariul
newJob.SalaryMax = 180000;

// Salvare
await jobService.UpdateJobAsync(newJob);
await jobService.PublishJobAsync(newJob.Id);
```

#### Exemplu 2: Clonare cu Action Delegate

```csharp
var clonedJob = originalJob.CloneWith(job =>
{
    job.Title = "Lead Developer";
    job.SalaryMin = 140000;
    job.SalaryMax = 200000;
    job.Requirements = "10+ years experience...";
});
```

#### Exemplu 3: Clonare Companie ca Subsidiară

```csharp
// TechCorp vrea să creeze o subsidiară în Europa
var techCorp = await companyService.GetCompanyByIdAsync(companyId);

var techCorpEurope = techCorp.CloneAsSubsidiary(
    "TechCorp Europe",
    "London, UK"
);

techCorpEurope.Description = "European branch of TechCorp Solutions";
await companyService.UpdateCompanyAsync(techCorpEurope);
```

### 3.5 Avantaje

1. **Performanță**
   - Clonarea poate fi mai rapidă decât crearea de la zero
   - Evită inițializări costisitoare (database queries, API calls)

2. **Reduce coupling-ul**
   - Client code nu depinde de clasele concrete
   - Flexibilitate în adăugarea de noi tipuri clon able

3. **Configurare dinamică**
   - Păstrezi starea obiectului original
   - Modifici doar ce e necesar

4. **Cod mai curat**
   - Elimină codul repetitiv de copiere manuală
   - Metode helper pentru scenarii comune

5. **Thread-safety**
   - Fiecare clonat are propriile colecții
   - Nu există partajare accidentală de referințe

---

## 4. Singleton Pattern

### 4.1 Descriere

**Singleton Pattern** este un patern creațional care asigură că o clasă are doar o singură instanță și oferă un punct global de acces la ea.

### 4.2 Problemă Rezolvată

**Fără Singleton:**
```csharp
// Fiecare serviciu creează propria instanță de configurare
var config1 = new ApplicationConfiguration();
var config2 = new ApplicationConfiguration();

// Modificare în config1
config1.JobExpiryDays = 60;

// config2 nu reflectă schimbarea! ❌
Console.WriteLine(config2.JobExpiryDays); // Output: 30 (default)

// Memorie irosită, inconsistență, imposibil de sincronizat
```

**Cu Singleton:**
```csharp
// O singură instanță partajată
var config1 = ApplicationConfiguration.Instance;
var config2 = ApplicationConfiguration.Instance;

// Ambele referă aceeași instanță
config1.UpdateJobSettings(60, 25);

// Schimbarea este vizibilă peste tot ✅
Console.WriteLine(config2.JobExpiryDays); // Output: 60

// Memorie optimizată, consistență garantată
```

### 4.3 Structură

**Fișier:** `Application/Configuration/ApplicationConfiguration.cs`

```csharp
public sealed class ApplicationConfiguration
{
    // Lazy<T> pentru inițializare thread-safe
    private static readonly Lazy<ApplicationConfiguration> _instance =
        new Lazy<ApplicationConfiguration>(
            () => new ApplicationConfiguration(),
            LazyThreadSafetyMode.ExecutionAndPublication);

    // Lock pentru actualizări thread-safe
    private static readonly object _updateLock = new object();

    // Punct de acces global
    public static ApplicationConfiguration Instance => _instance.Value;

    // Constructor privat - previne instanțierea externă
    private ApplicationConfiguration()
    {
        // Inițializare cu valori default
        JobExpiryDays = 30;
        MaxActiveApplicationsPerUser = 50;
        MaxActiveJobPostingsPerEmployer = 20;
        MinPasswordLength = 8;
        MaxResumeSizeMB = 5;
        EmailNotificationsEnabled = true;
        ApplicationName = "OnlineJobs Platform";
        ApplicationVersion = "1.0.0";
        SessionExpiryDays = 7;
        RegistrationEnabled = true;
        MaxSearchResultsPerPage = 20;
        LastUpdated = DateTime.UtcNow;
    }

    // Proprietăți de configurare
    public int JobExpiryDays { get; private set; }
    public int MaxActiveApplicationsPerUser { get; private set; }
    public int MaxActiveJobPostingsPerEmployer { get; private set; }
    public int MinPasswordLength { get; private set; }
    public int MaxResumeSizeMB { get; private set; }
    public bool EmailNotificationsEnabled { get; private set; }
    public string ApplicationName { get; private set; }
    public string ApplicationVersion { get; private set; }
    public int SessionExpiryDays { get; private set; }
    public bool RegistrationEnabled { get; private set; }
    public int MaxSearchResultsPerPage { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // Metode thread-safe pentru actualizări
    public void UpdateJobSettings(int expiryDays, int maxPostingsPerEmployer)
    {
        lock (_updateLock)
        {
            if (expiryDays <= 0)
                throw new ArgumentException("Expiry days must be positive");
            if (maxPostingsPerEmployer <= 0)
                throw new ArgumentException("Max postings must be positive");

            JobExpiryDays = expiryDays;
            MaxActiveJobPostingsPerEmployer = maxPostingsPerEmployer;
            LastUpdated = DateTime.UtcNow;
        }
    }

    public void UpdateUserSettings(int maxApplications, int minPasswordLength, int sessionExpiryDays)
    {
        lock (_updateLock)
        {
            // Validare + actualizare thread-safe
            MaxActiveApplicationsPerUser = maxApplications;
            MinPasswordLength = minPasswordLength;
            SessionExpiryDays = sessionExpiryDays;
            LastUpdated = DateTime.UtcNow;
        }
    }

    // Alte metode de actualizare...
}
```

### 4.4 Implementare Thread-Safe

#### A. Lazy<T> Initialization

```csharp
private static readonly Lazy<ApplicationConfiguration> _instance =
    new Lazy<ApplicationConfiguration>(
        () => new ApplicationConfiguration(),
        LazyThreadSafetyMode.ExecutionAndPublication);
```

**LazyThreadSafetyMode.ExecutionAndPublication:**
- ✅ Doar un thread poate inițializa instanța
- ✅ Toate thread-urile văd aceeași instanță
- ✅ Nu necesită double-check locking manual
- ✅ Zero overhead după inițializare

#### B. Lock pentru Actualizări

```csharp
private static readonly object _updateLock = new object();

public void UpdateJobSettings(int expiryDays, int maxPostingsPerEmployer)
{
    lock (_updateLock)
    {
        // Doar un thread la un moment dat poate actualiza
        JobExpiryDays = expiryDays;
        MaxActiveJobPostingsPerEmployer = maxPostingsPerEmployer;
        LastUpdated = DateTime.UtcNow;
    }
}
```

#### C. Sealed Class

```csharp
public sealed class ApplicationConfiguration
```

**De ce sealed?**
- Previne moștenirea
- Asigură că nu pot exista subclase care să submineze singleton-ul
- Optimizare compiler (devirtualizare)

### 4.5 Utilizare

#### Exemplu 1: Accesare Configurare

```csharp
// În Program.cs - inițializare
var config = ApplicationConfiguration.Instance;
Console.WriteLine($"✓ {config}");
Console.WriteLine($"  - Job Expiry: {config.JobExpiryDays} days");
Console.WriteLine($"  - Max Applications: {config.MaxActiveApplicationsPerUser}");
```

#### Exemplu 2: Utilizare în Service

```csharp
public class JobService : IJobService
{
    private readonly IRepository<JobPosting> _jobRepository;

    public async Task<bool> CanEmployerCreateJobAsync(Guid employerId)
    {
        var config = ApplicationConfiguration.Instance;
        var activeJobs = await GetActiveJobsByEmployerAsync(employerId);

        return activeJobs.Count < config.MaxActiveJobPostingsPerEmployer;
    }

    public async Task SetJobExpiryAsync(JobPosting job)
    {
        var config = ApplicationConfiguration.Instance;
        job.ExpiryDate = DateTime.UtcNow.AddDays(config.JobExpiryDays);
    }
}
```

#### Exemplu 3: Actualizare Runtime

```csharp
// Admin panel - actualizare setări
var config = ApplicationConfiguration.Instance;

// Thread-safe update
config.UpdateJobSettings(
    expiryDays: 45,          // Crește perioada de expirare
    maxPostingsPerEmployer: 30  // Permite mai multe job-uri
);

// Resetare la default
config.ResetToDefaults();

// Obținere snapshot
var settings = config.GetAllSettings();
foreach (var setting in settings)
{
    Console.WriteLine($"{setting.Key}: {setting.Value}");
}
```

### 4.6 Avantaje

1. **Instanță unică garantată**
   - O singură sursă de adevăr pentru configurare
   - Eliminare inconsistențe

2. **Acces global**
   - Disponibil oriunde în aplicație
   - Nu necesită dependency injection (deși poate fi folosit cu DI)

3. **Lazy initialization**
   - Creare doar când este nevoie
   - Economisire memorie și timp de startup

4. **Thread-safety**
   - Safe pentru aplicații multi-threaded
   - Lazy<T> gestionează sincronizarea

5. **Control asupra instanței**
   - Constructor privat previne instanțierea externă
   - Sealed previne moștenirea

### 4.7 Considerații

**Când să folosești Singleton:**
- ✅ Configurare aplicație
- ✅ Logger-e
- ✅ Cache-uri
- ✅ Connection pools
- ✅ Manageri de resurse partajate

**Când să eviți Singleton:**
- ❌ Pentru logică de business
- ❌ Când testabilitatea este critică (dificil de mockat)
- ❌ Când ai nevoie de multiple instanțe în teste
- ❌ State-uri care ar trebui să fie scoped (per-request)

---

## 5. Implementare în OnlineJobs

### 5.1 Structură Proiect

```
OnlineJobs/
├── Domain/
│   ├── Entities/
│   │   ├── JobSeeker.cs (actualizat cu noi proprietăți)
│   │   ├── JobPosting.cs (+ IPrototype<JobPosting>)
│   │   └── Company.cs (+ IPrototype<Company>)
│   ├── Interfaces/
│   │   └── IPrototype.cs (NOU - în Domain, nu Application!)
│   └── ValueObjects/ (NOU)
│       ├── Education.cs
│       ├── WorkExperience.cs
│       ├── Skill.cs
│       └── Certification.cs
│
├── Application/
│   ├── Interfaces/
│   │   └── IJobSeekerProfileBuilder.cs (NOU)
│   ├── Builders/ (NOU)
│   │   ├── JobSeekerProfileBuilder.cs
│   │   └── JobSeekerProfileDirector.cs
│   └── Configuration/ (NOU)
│       └── ApplicationConfiguration.cs (Singleton)
│
└── Program.cs (actualizat cu demo pentru toate paternurile)
```

### 5.2 Dependency Injection

**Program.cs:**

```csharp
using OnlineJobs.Application.Builders;
using OnlineJobs.Application.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Builder Pattern - scoped pentru fiecare request
builder.Services.AddScoped<IJobSeekerProfileBuilder, JobSeekerProfileBuilder>();
builder.Services.AddScoped<JobSeekerProfileDirector>();

// Singleton Configuration - aceeași instanță pentru toată aplicația
// Nu e nevoie să-l înregistrăm în DI - folosim ApplicationConfiguration.Instance

var app = builder.Build();

// Inițializare Singleton
var config = ApplicationConfiguration.Instance;
Console.WriteLine($"✓ {config}");
```

### 5.3 Arhitectură Layered

**Respectarea Clean Architecture:**

```
┌─────────────────────────────────────────────────┐
│              Presentation Layer                 │
│               (Program.cs)                      │
└────────────────┬────────────────────────────────┘
                 │ uses
┌────────────────▼────────────────────────────────┐
│           Application Layer                     │
│  - Builders (Builder Pattern)                   │
│  - Configuration (Singleton Pattern)            │
│  - Services, Factories, Strategies              │
└────────────────┬────────────────────────────────┘
                 │ uses
┌────────────────▼────────────────────────────────┐
│             Domain Layer                        │
│  - Entities (JobPosting, Company + Prototype)   │
│  - ValueObjects (Education, Skill, etc.)        │
│  - Interfaces (IPrototype)                      │
│  - Enums                                        │
└─────────────────────────────────────────────────┘
```

**Respectare principii:**
- ✅ **Dependency Flow:** Presentation → Application → Domain
- ✅ **Domain Independence:** Domain nu depinde de Application
- ✅ **IPrototype în Domain:** Evită circular dependencies

---

## 6. Testare și Demonstrare

### 6.1 Seed Data cu Toate Paternurile

**Program.cs - SeedDataAsync():**

```csharp
async Task SeedDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var companyService = scope.ServiceProvider.GetRequiredService<CompanyService>();
    var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();

    // ============ BUILDER PATTERN DEMO ============
    var builder = new JobSeekerProfileBuilder();
    var director = new JobSeekerProfileDirector(builder);

    var seniorDeveloper = director.ConstructSeniorTechnicalProfile(
        "sarah.wilson@example.com",
        "Sarah",
        "Wilson",
        "+1-555-0123",
        "Experienced full-stack developer with 8+ years of expertise...",
        new List<WorkExperience>
        {
            new WorkExperience("Google", "Senior Software Engineer",
                "Mountain View, CA", new DateTime(2020, 1, 1), null,
                "Leading development of cloud-based solutions",
                new List<string> { "Design and implement microservices" },
                new List<string> { "Reduced API latency by 40%" }
            ),
            new WorkExperience("Microsoft", "Software Engineer",
                "Seattle, WA", new DateTime(2016, 6, 1),
                new DateTime(2019, 12, 31), "Developed enterprise applications"
            )
        },
        new List<Skill>
        {
            new Skill("C#", SkillProficiency.Expert, 8, "Backend"),
            new Skill("ASP.NET Core", SkillProficiency.Expert, 6, "Backend"),
            new Skill("React", SkillProficiency.Advanced, 5, "Frontend")
        },
        "https://github.com/sarahwilson",
        "https://sarahwilson.dev"
    );

    var jobSeekerRepo = scope.ServiceProvider.GetRequiredService<IRepository<JobSeeker>>();
    await jobSeekerRepo.AddAsync(seniorDeveloper);

    Console.WriteLine($"✓ Created detailed profile using Builder Pattern");
    Console.WriteLine($"  - Work Experience: {seniorDeveloper.WorkHistory.Count} positions");
    Console.WriteLine($"  - Skills: {seniorDeveloper.SkillSet.Count} skills");
    Console.WriteLine($"  - Total Experience: {seniorDeveloper.GetTotalYearsOfExperience()} years");

    // ============ PROTOTYPE PATTERN DEMO ============

    // 1. Clonare JobPosting
    var originalJob = await jobService.GetJobByIdAsync(job1.Id);
    var clonedJob = originalJob.Clone();
    clonedJob.Title = "Lead Software Engineer";
    clonedJob.SalaryMin = 120000;
    clonedJob.SalaryMax = 180000;
    await jobService.UpdateJobAsync(clonedJob);
    await jobService.PublishJobAsync(clonedJob.Id);

    Console.WriteLine($"✓ Cloned job posting using Prototype Pattern: '{clonedJob.Title}'");

    // 2. Clonare Company
    var techCorpSubsidiary = techCorp.CloneAsSubsidiary("TechCorp Europe", "London, UK");
    await companyService.UpdateCompanyAsync(techCorpSubsidiary);

    Console.WriteLine($"✓ Cloned company using Prototype Pattern: '{techCorpSubsidiary.Name}'");

    // ============ SINGLETON PATTERN DEMO ============
    // Demonstrat în main: var config = ApplicationConfiguration.Instance;

    Console.WriteLine("\n✓ Initial data seeded successfully");
    Console.WriteLine($"✓ Created {3} companies (including 1 cloned subsidiary)");
    Console.WriteLine($"✓ Created {5} users (3 job seekers, 2 employers)");
    Console.WriteLine($"✓ Created {4} active job postings (including 1 cloned)");
}
```

### 6.2 Output Așteptat

```
✓ OnlineJobs Platform v1.0.0 - Configuration (Last Updated: 2024-03-08 14:25:33 UTC)
  - Job Expiry: 30 days
  - Max Applications per User: 50
  - Max Job Postings per Employer: 20

✓ Created detailed profile for Sarah Wilson using Builder Pattern
  - Work Experience: 2 positions
  - Skills: 3 skills
  - Total Experience: 8 years

✓ Cloned job posting using Prototype Pattern: 'Lead Software Engineer'
✓ Cloned company using Prototype Pattern: 'TechCorp Europe' in London, UK

✓ Initial data seeded successfully
✓ Created 3 companies (including 1 cloned subsidiary)
✓ Created 5 users (3 job seekers, 2 employers)
✓ Created 4 active job postings (including 1 cloned)
```

### 6.3 Verificare Build

```bash
cd "/Users/gabriel/Desktop/Universitate_3/semestrul 2/OnlineJobs"
dotnet build
# Build succeeded - 0 errors (doar warnings nullable)

dotnet run
# Aplicația pornește și afișează output-ul de mai sus
```

---

## 7. Beneficii și Concluzii

### 7.1 Beneficii per Pattern

#### Builder Pattern
- ✅ **Claritate:** Cod foarte lizibil cu fluent interface
- ✅ **Flexibilitate:** Construcție opțională, pas cu pas
- ✅ **Validare:** Detectare erori în timpul construcției
- ✅ **Reut ilizare:** Director oferă scenarii predefinite
- ✅ **Testabilitate:** Ușor de testat și mockat

#### Prototype Pattern
- ✅ **Performanță:** Clonarea e mai rapidă decât crearea
- ✅ **Simplitate:** Elimină cod repetitiv
- ✅ **Safety:** Deep copy previne shared references
- ✅ **Flexibilitate:** Metode helper pentru scenarii comune
- ✅ **Decoupling:** Client nu depinde de clase concrete

#### Singleton Pattern
- ✅ **Consistență:** O singură sursă de adevăr
- ✅ **Memorie:** O singură instanță în toată aplicația
- ✅ **Thread-safety:** Implementare sigură cu Lazy<T>
- ✅ **Global access:** Disponibil oriunde
- ✅ **Control:** Constructor privat + sealed class

### 7.2 Respectarea Principiilor SOLID

#### Single Responsibility Principle (SRP)
- ✅ **Builder:** Are doar responsabilitatea de a construi JobSeeker
- ✅ **Director:** Știe doar cum să orchestreze construcția
- ✅ **Singleton:** Gestionează doar configurarea aplicației

#### Open/Closed Principle (OCP)
- ✅ **Builder:** Poți extinde cu noi metode fără să modifici existente
- ✅ **Director:** Poți adăuga noi scenarii de construcție
- ✅ **Prototype:** Poți adăuga noi metode de clonare

#### Liskov Substitution Principle (LSP)
- ✅ **IJobSeekerProfileBuilder:** Orice implementare poate substitui interfața
- ✅ **IPrototype<T>:** Orice entitate clon abilă respectă contractul

#### Interface Segregation Principle (ISP)
- ✅ Interfețe mici și focusate
- ✅ `IPrototype<T>` are doar 2 metode esențiale
- ✅ `IJobSeekerProfileBuilder` este specific unui scop

#### Dependency Inversion Principle (DIP)
- ✅ **Director depinde de IJobSeekerProfileBuilder**, nu de implementare concretă
- ✅ **IPrototype în Domain**, nu în Application (evită circular dependencies)
- ✅ Services pot depinde de abstracții

### 7.3 Clean Architecture

```
✅ Domain Layer:
   - Entities (JobSeeker, JobPosting, Company)
   - Value Objects (Education, Skill, etc.)
   - Interfaces (IPrototype) - fără dependențe externe

✅ Application Layer:
   - Builders (construcție obiecte complexe)
   - Configuration (Singleton)
   - Services, Factories, Strategies

✅ Infrastructure Layer:
   - Repositories (InMemory)

✅ Presentation Layer:
   - Program.cs (dependency injection, seed data)
```

### 7.4 Comparație cu Lab 2

| Aspect | Lab 2 | Lab 3 |
|--------|-------|-------|
| **Paternuri** | Factory Method, Abstract Factory | Builder, Prototype, Singleton |
| **Focus** | Crearea familiilor de obiecte | Construcție complexă, clonare, instanță unică |
| **Aplicabilitate** | Documents, SearchStrategies | JobSeeker profiles, Entity cloning, Config |
| **Complexitate** | Medie | Medie-Ridicată (Value Objects) |
| **Utilitate** | Runtime type selection | Step-by-step construction, duplication |

### 7.5 Lecții Învățate

1. **Arhitectură importă**
   - IPrototype trebuie în Domain, nu Application
   - Evitarea circular dependencies este critică

2. **Value Objects sunt puternice**
   - Imutabilitate asigură integritate
   - Validare în constructor previne stări invalide
   - Equality bazată pe valoare simplific comparațiile

3. **Thread-safety nu e opțională**
   - Lazy<T> simplifică singleton thread-safe
   - Lock statements pentru actualizări

4. **Fluent interfaces îmbunătățesc UX**
   - Method chaining face codul foarte lizibil
   - Builder pattern excelează la acest lucru

5. **Prototype economisește cod**
   - Eliminarea copierilor manuale
   - Metode helper pentru scenarii comum

### 7.6 Dezavantaje și Anti-Patterns - Perspectiva Senior Developer

#### 🚫 Builder Pattern - Când să NU îl folosești

**1. Over-Engineering pentru Obiecte Simple**

```csharp
// ❌ PROST - Builder pentru obiect simplu cu 2-3 proprietăți
public class UserBuilder
{
    private User _user = new User();

    public UserBuilder WithName(string name) { ... }
    public UserBuilder WithEmail(string email) { ... }
    public User Build() { ... }
}

var user = new UserBuilder()
    .WithName("John")
    .WithEmail("john@example.com")
    .Build();

// ✅ BUN - Constructor simplu e suficient
var user = new User("John", "john@example.com");
```

**Problema:** Complexitate inutilă, cod verbose pentru obiecte simple. Builder adaugă overhead fără beneficii reale.

**2. Verbozitate Excesivă în Teste**

```csharp
// ❌ PROST - Builder face testele verbose
[Test]
public void Test_JobSeeker_Application()
{
    var jobSeeker = new JobSeekerProfileBuilder()
        .WithBasicInfo("test@example.com", "Test", "User", "123")
        .WithProfessionalSummary("Summary here...")
        .AddEducation(new Education(...))
        .AddSkill(new Skill(...))
        .Build();

    // Actual test logic
}

// ✅ BUN - Test data builders sau object mothers
var jobSeeker = JobSeekerMother.CreateDefault();
```

**Problema:** În teste, ai nevoie de date rapide, nu de construcție elaborată. Test Data Builders sau Object Mother pattern sunt mai bune.

**3. Maintenance Burden - Dublarea Proprietăților**

```csharp
// ❌ PROST - Fiecare proprietate nouă necesită:
// 1. Proprietate în entitate
// 2. Metodă în builder
// 3. Logică în Director
// 4. Actualizare documentație
public class JobSeeker
{
    public string NewProperty { get; set; } // 1. Adăugat aici
}

public interface IJobSeekerProfileBuilder
{
    IJobSeekerProfileBuilder WithNewProperty(string value); // 2. Adăugat aici
}

public class JobSeekerProfileDirector
{
    // 3. Actualizat în TOATE metodele constructor
}
```

**Problema:** Cost mare de mentenanță. Fiecare proprietate nouă necesită modificări în 3+ locuri. Breaking changes în interfață.

**4. Confusion cu Partial Object States**

```csharp
// ❌ PROST - Builder permite stări invalide
var jobSeeker = new JobSeekerProfileBuilder()
    .WithBasicInfo("email@test.com", "John", "Doe")
    // Uită să adauge summary (dar e necesar pentru profil complet!)
    .AddSkill(new Skill("C#", SkillProficiency.Expert))
    .Build(); // Build() reușește dar obiectul e incomplet!

// Apoi în service:
if (!jobSeeker.HasCompleteProfile()) // Runtime check necesară!
{
    throw new Exception("Profile incomplete"); // Eroare runtime, nu compile-time
}
```

**Problema:** Builder nu garantează compile-time safety pentru proprietăți obligatorii. Validarea runtime e necesară, dar late failure e rău.

---

#### 🚫 Prototype Pattern - Când să NU îl folosești

**1. Deep Copy Performance Penalties**

```csharp
// ❌ PROST - Clonare deep copy pentru obiecte mari
public class HugeEntity : IPrototype<HugeEntity>
{
    public List<ComplexObject> Items { get; set; } // 10,000 items
    public Dictionary<string, AnotherComplexObject> Data { get; set; } // 50,000 entries
    public byte[] LargeBlob { get; set; } // 100MB

    public HugeEntity Clone()
    {
        // Deep copy = 100MB+ copiat în memorie
        // Performanță catastrofală!
        var clone = new HugeEntity
        {
            Items = this.Items.Select(i => i.Clone()).ToList(), // O(n) copii
            Data = this.Data.ToDictionary(kv => kv.Key, kv => kv.Value.Clone()), // O(n) copii
            LargeBlob = (byte[])this.LargeBlob.Clone() // 100MB copiat
        };
        return clone;
    }
}

// Folosire:
var clone = hugeEntity.Clone(); // 2-3 secunde pentru clonare! ❌
```

**Problema:** Deep copy pentru obiecte mari = memory overhead masiv + latency. Alternative: Copy-on-Write, Lazy cloning, sau evită clonarea.

**2. Referințe Circulare și Stack Overflow**

```csharp
// ❌ PROST - Entități cu referințe circulare
public class JobPosting : IPrototype<JobPosting>
{
    public Company Company { get; set; }

    public JobPosting Clone()
    {
        return new JobPosting
        {
            Company = this.Company.Clone() // Company clonează JobPostings
        };
    }
}

public class Company : IPrototype<Company>
{
    public List<JobPosting> JobPostings { get; set; }

    public Company Clone()
    {
        return new Company
        {
            JobPostings = this.JobPostings.Select(j => j.Clone()).ToList() // StackOverflowException!
        };
    }
}
```

**Problema:** Graph objects cu referințe circulare = infinite recursion. Necesită tracking cu HashSet, complicând masiv implementarea.

**3. Clonarea Ascunde Dependențe Database/External**

```csharp
// ❌ PROST - Clonare cu side-effects
public class JobPosting : IPrototype<JobPosting>
{
    public JobPosting Clone()
    {
        var clone = new JobPosting { ... };

        // Side-effect ascuns!
        _auditService.LogJobCloned(this.Id, clone.Id); // Database write!
        _notificationService.NotifyEmployer(this.EmployerId); // Email sent!

        return clone;
    }
}

// ✅ BUN - Clonarea e pure function, side-effects în service layer
public class JobService
{
    public async Task<JobPosting> CloneJobAsync(Guid jobId)
    {
        var original = await GetJobAsync(jobId);
        var clone = original.Clone(); // Pure

        await _auditService.LogJobCloned(original.Id, clone.Id); // Explicit
        await _notificationService.NotifyEmployer(original.EmployerId); // Explicit

        return clone;
    }
}
```

**Problema:** Clone() ar trebui să fie pure function. Side-effects ascunse (DB, API, files) = surprize, testare dificilă, bugs greu de debugged.

**4. Shallow vs Deep Copy Confusion**

```csharp
// ❌ PROST - Confuzie între shallow și deep copy
var original = new Company
{
    Name = "TechCorp",
    JobPostings = new List<JobPosting> { job1, job2 }
};

var shallowClone = original.ShallowCopy(); // MemberwiseClone()
shallowClone.JobPostings.Add(job3); // Modifică și originalul! ❌

Console.WriteLine(original.JobPostings.Count); // 3, nu 2! BUG!

// Apoi cineva face:
var deepClone = original.Clone();
deepClone.JobPostings.Add(job4); // Nu modifică originalul

// Inconsistență = bugs subtile în producție
```

**Problema:** Existența ambelor metode (Clone + ShallowCopy) creează confuzie. Developers nu înțeleg diferența = shared reference bugs.

---

#### 🚫 Singleton Pattern - Când să NU îl folosești (CELE MAI MULTE PROBLEME!)

**1. Global State = Testare Nightmare**

```csharp
// ❌ PROST - Singleton face teste dependente între ele
public class JobServiceTests
{
    [Test]
    public void Test1_UpdatesConfiguration()
    {
        var config = ApplicationConfiguration.Instance;
        config.UpdateJobSettings(60, 30); // Modifică starea globală

        var service = new JobService();
        Assert.AreEqual(60, service.GetJobExpiryDays());
    } // Starea rămâne modificată!

    [Test]
    public void Test2_UsesDefaultConfiguration()
    {
        var service = new JobService();
        Assert.AreEqual(30, service.GetJobExpiryDays()); // ❌ FAIL! E 60 din Test1!
    }
}

// Teste rulate în paralel = race conditions
// Ordinea testelor importă = flaky tests
// Imposibil de izolat teste
```

**Problema:** Singleton = shared mutable state = imposibil de testat corect. Teste interdependente, race conditions, imposibil de rulat în paralel.

**Alternative:** Dependency Injection cu scoped/transient lifetimes.

**2. Hidden Dependencies și Coupling**

```csharp
// ❌ PROST - Dependență ascunsă
public class JobService
{
    public async Task CreateJobAsync(JobPosting job)
    {
        // Dependență ASCUNSĂ! Nu apare în constructor
        var config = ApplicationConfiguration.Instance;
        job.ExpiryDate = DateTime.UtcNow.AddDays(config.JobExpiryDays);

        await _repository.AddAsync(job);
    }
}

// ✅ BUN - Dependență EXPLICITĂ
public class JobService
{
    private readonly IApplicationConfiguration _config;

    public JobService(IApplicationConfiguration config) // Dependency Injection
    {
        _config = config; // Explicit, testabil, mockabil
    }
}
```

**Problema:** Singleton ascunde dependențele. Constructor nu reflectă ce folosește clasa. Imposibil de înlocuit în teste. Coupling strâns.

**3. Multi-Threading Issues și Deadlocks**

```csharp
// ❌ PROST - Singleton cu locks poate cauza deadlocks
public sealed class ApplicationConfiguration
{
    private static readonly object _updateLock = new object();
    private static readonly object _readLock = new object();

    public void UpdateJobSettings(int expiryDays, int maxPostings)
    {
        lock (_updateLock)
        {
            lock (_readLock) // Nested locks!
            {
                JobExpiryDays = expiryDays;
            }
        }
    }

    public int GetJobExpiryDays()
    {
        lock (_readLock)
        {
            lock (_updateLock) // Nested locks în ordine DIFERITĂ!
            {
                return JobExpiryDays;
            }
        }
    }
}

// Thread 1: UpdateJobSettings() - locked _updateLock, waiting for _readLock
// Thread 2: GetJobExpiryDays() - locked _readLock, waiting for _updateLock
// DEADLOCK! ❌
```

**Problema:** Singleton cu state mutabil + locks = risc mare de deadlocks, race conditions. Debugging e coșmar în producție.

**4. Violează SOLID Principles**

```csharp
// ❌ PROST - Singleton violează:

// 1. Single Responsibility - Face prea multe
public sealed class ApplicationConfiguration
{
    // Gestionează configurare
    public int JobExpiryDays { get; }

    // Gestionează propria sa instanță (al doilea responsibility!)
    public static ApplicationConfiguration Instance { get; }
    private ApplicationConfiguration() { }
}

// 2. Open/Closed - Nu poți extinde
public sealed class ApplicationConfiguration // sealed = nu poți moșteni!
{
    // Vrei să adaugi logging la toate update-uri? TOUGH LUCK!
}

// 3. Dependency Inversion - Clasele depind de concretizare
public class JobService
{
    public void CreateJob()
    {
        var config = ApplicationConfiguration.Instance; // Dependență DIRECTĂ pe clasă concretă!
    }
}

// ✅ BUN - Respectă SOLID
public interface IApplicationConfiguration { ... }
public class ApplicationConfiguration : IApplicationConfiguration { ... }
public class JobService
{
    private readonly IApplicationConfiguration _config;
    public JobService(IApplicationConfiguration config) { ... }
}
```

**Problema:** Singleton = anti-pattern pentru SOLID. Nu poți extinde, nu poți substitui, dependențe concrete, multiple responsabilități.

---

### 7.7 Recomandări Generale - Când să Folosești sau să Eviți Aceste Patterns

#### ✅ Folosește Builder DOAR când:
- Obiectul are **10+ parametri** în constructor
- Multe parametri sunt **opționali**
- Ai nevoie de **validare complexă** în timpul construcției
- Ai **multiple variante** ale aceluiași obiect (Director pattern adaugă valoare)

#### ❌ Evită Builder când:
- Obiectul are **< 5 parametri** → Folosește constructor normal
- Toți parametrii sunt **obligatorii** → Record types sau Data classes
- Ai doar **1-2 variante** → Factory Method e mai simplu
- Lucrezi cu **DTOs simple** → Object initializers sunt suficiente

---

#### ✅ Folosește Prototype DOAR când:
- Clonarea e **semnificativ mai rapidă** decât crearea (profiling proof!)
- Obiectele sunt **relativ mici** (< 1MB)
- **Nu ai referințe circulare** în object graph
- Ai **many similar objects** cu small variations

#### ❌ Evită Prototype când:
- Obiectele sunt **mari** (> 10MB) → Lazy loading, Copy-on-Write
- Ai **referințe circulare** → Serialization/Deserialization mai safe
- Ai **side-effects** în constructori → Clonarea le va sări
- Obiectele au **external dependencies** (DB, API) → Factory e mai bun

---

#### ✅ Folosește Singleton DOAR pentru:
- **Configurare read-only** (sau read-mostly) loaded at startup
- **Stateless services** (Logger fără state)
- **Hardware access** (printer manager, file system watcher)
- **Truly shared resources** care TREBUIE să fie unique

#### ❌ Evită Singleton când (APROAPE ÎNTOTDEAUNA!):
- Lucrezi cu **state mutabil** → Scoped services via DI
- Ai nevoie de **testabilitate** → Dependency Injection
- Lucrezi în **mediu multi-threaded complex** → Evită shared state
- Folosești **ASP.NET Core** → DI Container e mult mai bun

**Regula de aur:** În aplicații moderne ASP.NET Core, **EVITĂ Singleton pattern!**
Folosește Dependency Injection cu:
- `AddSingleton<T>()` pentru servicii stateless
- `AddScoped<T>()` pentru servicii per-request
- `AddTransient<T>()` pentru servicii lightweight

---

### 7.8 Alternative Moderne la Aceste Patterns

#### În loc de Builder:
```csharp
// Record types cu with expressions (C# 9+)
public record JobSeeker(string Email, string FirstName, string LastName)
{
    public List<Skill> Skills { get; init; } = new();
}

var jobSeeker = new JobSeeker("email@test.com", "John", "Doe")
{
    Skills = { new Skill("C#", SkillProficiency.Expert) }
};

var modified = jobSeeker with { FirstName = "Jane" }; // Immutable copy!
```

#### În loc de Prototype:
```csharp
// JSON Serialization pentru deep copy (simplu dar mai lent)
public static T DeepClone<T>(T obj)
{
    var json = JsonSerializer.Serialize(obj);
    return JsonSerializer.Deserialize<T>(json)!;
}

// Sau Reflection-based cloners (AutoMapper, CloneExtensions)
var clone = original.DeepClone(); // Extension method via library
```

#### În loc de Singleton:
```csharp
// Dependency Injection (RECOMANDAT!)
builder.Services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>();

public class JobService
{
    private readonly IApplicationConfiguration _config;

    public JobService(IApplicationConfiguration config)
    {
        _config = config; // Injected, testable, mockable!
    }
}
```

---

## 8. Referințe

### Documentație Oficială
- [Microsoft - Builder Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-model-layer-validations)
- [Microsoft - Lazy<T> Class](https://learn.microsoft.com/en-us/dotnet/api/system.lazy-1)
- [Refactoring Guru - Builder Pattern](https://refactoring.guru/design-patterns/builder)
- [Refactoring Guru - Prototype Pattern](https://refactoring.guru/design-patterns/prototype)
- [Refactoring Guru - Singleton Pattern](https://refactoring.guru/design-patterns/singleton)

### Cărți
- **"Design Patterns: Elements of Reusable Object-Oriented Software"** - Gang of Four
- **"Head First Design Patterns"** - Freeman & Robson
- **"Patterns of Enterprise Application Architecture"** - Martin Fowler

### Articole
- [Martin Fowler - Value Object](https://martinfowler.com/bliki/ValueObject.html)
- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

