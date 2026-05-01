you# Design Patterns - Lab 4: Structural Patterns

## Overview
This document demonstrates the implementation of three structural design patterns in the OnlineJobs platform:
1. **Adapter Pattern** - Payment Gateway Integration
2. **Composite Pattern** - Job Category Hierarchy
3. **Façade Pattern** - Job Application Workflow Simplification

---

## 1. Adapter Pattern - Payment Gateway Integration

### Purpose
The Adapter pattern allows incompatible interfaces to work together. In this implementation, we integrate multiple payment gateways (PayPal, Stripe, Google Pay) with different APIs through a unified interface.

### Business Scenario
Job seekers can pay to reveal which company posted a job listing. This feature allows:
- Anonymous job postings where the company name is hidden
- Job seekers pay $4.99 to unlock company information
- Payment through multiple gateways (PayPal, Stripe, Google Pay)
- Each gateway has a different API that needs to be adapted

### Components

#### Target Interface (`IPaymentProcessor`)
```csharp
public interface IPaymentProcessor
{
    PaymentGateway Gateway { get; }
    Task<PaymentResult> ProcessPaymentAsync(string userEmail, decimal amount, string currency, string description);
    Task<bool> VerifyPaymentAsync(string transactionId);
}
```

#### Adaptees (External Gateway SDKs)
- **PayPalGatewaySDK** - Uses `CreatePayment()`, returns `PayPalPaymentResponse`
- **StripeGatewaySDK** - Uses `Charge()` with amount in cents, returns `StripeChargeResult`
- **GooglePayGatewaySDK** - Uses `ProcessTransaction()` with request object, returns `GooglePayTransactionResult`

#### Adapters
- **PayPalAdapter** - Adapts PayPal SDK to `IPaymentProcessor`
- **StripeAdapter** - Adapts Stripe SDK to `IPaymentProcessor` (converts decimal to cents)
- **GooglePayAdapter** - Adapts Google Pay SDK to `IPaymentProcessor` (uses request object)

#### Services
- **PaymentService** - Orchestrates payment processing using adapters
- **CompanyRevealService** - Business logic for revealing company information

### Class Diagram

```plantuml
@startuml Adapter_Pattern

!define ADAPTER_COLOR #FFE6CC
!define ADAPTEE_COLOR #E6F7FF
!define TARGET_COLOR #D4EDDA
!define CLIENT_COLOR #F8D7DA

' Target Interface
interface IPaymentProcessor <<Target>> TARGET_COLOR {
    +Gateway : PaymentGateway
    +ProcessPaymentAsync(email, amount, currency, desc) : Task<PaymentResult>
    +VerifyPaymentAsync(transactionId) : Task<bool>
}

' Adapters
class PayPalAdapter <<Adapter>> ADAPTER_COLOR {
    -payPalSdk : PayPalGatewaySDK
    +Gateway : PaymentGateway
    +ProcessPaymentAsync() : Task<PaymentResult>
    +VerifyPaymentAsync() : Task<bool>
}

class StripeAdapter <<Adapter>> ADAPTER_COLOR {
    -stripeSdk : StripeGatewaySDK
    +Gateway : PaymentGateway
    +ProcessPaymentAsync() : Task<PaymentResult>
    +VerifyPaymentAsync() : Task<bool>
}

class GooglePayAdapter <<Adapter>> ADAPTER_COLOR {
    -googlePaySdk : GooglePayGatewaySDK
    +Gateway : PaymentGateway
    +ProcessPaymentAsync() : Task<PaymentResult>
    +VerifyPaymentAsync() : Task<bool>
}

' Adaptees (External SDKs)
class PayPalGatewaySDK <<Adaptee>> ADAPTEE_COLOR {
    +CreatePayment(email, amount, currency, desc) : PayPalPaymentResponse
    +GetPaymentStatus(paymentId) : string
}

class StripeGatewaySDK <<Adaptee>> ADAPTEE_COLOR {
    +Charge(amountInCents, currency, email, memo) : StripeChargeResult
    +RetrieveCharge(chargeId) : StripeChargeResult
}

class GooglePayGatewaySDK <<Adaptee>> ADAPTEE_COLOR {
    +ProcessTransaction(request) : GooglePayTransactionResult
    +VerifyTransaction(transactionRef) : bool
}

' Client
class PaymentService <<Client>> CLIENT_COLOR {
    -paymentRepository : IRepository<PaymentTransaction>
    -paymentProcessors : Dictionary<PaymentGateway, IPaymentProcessor>
    +ProcessPaymentAsync(userId, amount, gateway, desc) : Task<PaymentTransaction>
    +GetPaymentByIdAsync(id) : Task<PaymentTransaction>
}

class CompanyRevealService <<Client>> CLIENT_COLOR {
    -revealRepository : IRepository<CompanyReveal>
    -paymentService : IPaymentService
    +PurchaseCompanyRevealAsync(seekerId, jobId, gateway) : Task<CompanyReveal>
    +HasAccessToCompanyAsync(seekerId, jobId) : Task<bool>
}

' Relationships
IPaymentProcessor <|.. PayPalAdapter
IPaymentProcessor <|.. StripeAdapter
IPaymentProcessor <|.. GooglePayAdapter

PayPalAdapter o-- PayPalGatewaySDK : adapts
StripeAdapter o-- StripeGatewaySDK : adapts
GooglePayAdapter o-- GooglePayGatewaySDK : adapts

PaymentService o-- IPaymentProcessor : uses
CompanyRevealService --> PaymentService : uses

note right of PayPalAdapter
  Converts our unified interface
  to PayPal's specific API
  (CreatePayment, GetPaymentStatus)
end note

note right of StripeAdapter
  Converts decimal amount to cents
  Adapts to Stripe's Charge API
end note

note right of GooglePayAdapter
  Creates GooglePayRequest object
  Adapts to Google Pay's API
end note

@enduml
```

### Benefits
- **Single Responsibility**: Each adapter handles one gateway
- **Open/Closed**: New payment gateways can be added without modifying existing code
- **Unified Interface**: Client code works with all gateways uniformly
- **Interchangeability**: Payment gateways can be swapped easily

---

## 2. Composite Pattern - Job Category Hierarchy

### Purpose
The Composite pattern allows you to compose objects into tree structures to represent part-whole hierarchies. It lets clients treat individual objects and compositions uniformly.

### Business Scenario
Job postings are organized in a hierarchical category system:
- **Root**: "Technology Jobs" (composite)
- **Branches**: "Software Development", "Data & Analytics", "Design" (composites)
- **Leaves**: "Backend Developer", "Frontend Developer", "Data Scientist" (leafs)

### Components

#### Component (Abstract Base)
```csharp
public abstract class JobCategory
{
    public abstract int GetJobCount();
    public abstract void Display(int depth = 0);
    public abstract List<CategoryLeaf> GetAllLeafCategories();
    public virtual void Add(JobCategory category) { throw new NotSupportedException(); }
    public virtual void Remove(JobCategory category) { throw new NotSupportedException(); }
    public virtual JobCategory GetChild(int index) { throw new NotSupportedException(); }
}
```

#### Leaf
```csharp
public class CategoryLeaf : JobCategory
{
    private readonly List<JobPosting> _jobs;
    // Contains actual job postings
    // Cannot have children
}
```

#### Composite
```csharp
public class CategoryComposite : JobCategory
{
    private readonly List<JobCategory> _children;
    // Can contain both leafs and other composites
    // Delegates operations to children
}
```

### Class Diagram

```plantuml
@startuml Composite_Pattern

!define COMPONENT_COLOR #FFF4E6
!define LEAF_COLOR #E8F5E9
!define COMPOSITE_COLOR #E3F2FD

' Component
abstract class JobCategory <<Component>> COMPONENT_COLOR {
    #Id : int
    #Name : string
    #Description : string
    {abstract} +GetJobCount() : int
    {abstract} +Display(depth) : void
    {abstract} +GetAllLeafCategories() : List<CategoryLeaf>
    +Add(category) : void
    +Remove(category) : void
    +GetChild(index) : JobCategory
    +IsComposite() : bool
}

' Leaf
class CategoryLeaf <<Leaf>> LEAF_COLOR {
    -jobs : List<JobPosting>
    +Jobs : IReadOnlyList<JobPosting>
    +AddJob(job) : void
    +RemoveJob(job) : void
    +GetJobCount() : int
    +Display(depth) : void
    +GetAllLeafCategories() : List<CategoryLeaf>
}

' Composite
class CategoryComposite <<Composite>> COMPOSITE_COLOR {
    -children : List<JobCategory>
    +Children : IReadOnlyList<JobCategory>
    +Add(category) : void
    +Remove(category) : void
    +GetChild(index) : JobCategory
    +GetJobCount() : int
    +Display(depth) : void
    +GetAllLeafCategories() : List<CategoryLeaf>
    +IsComposite() : bool
    +GetChildCount() : int
}

class JobPosting {
    +Id : int
    +Title : string
    +Description : string
}

' Relationships
JobCategory <|-- CategoryLeaf
JobCategory <|-- CategoryComposite
CategoryComposite o-- "0..*" JobCategory : children
CategoryLeaf o-- "0..*" JobPosting : jobs

note right of CategoryComposite
  Recursive structure:
  Can contain both CategoryLeaf
  and other CategoryComposite objects

  GetJobCount() sums all children
end note

note right of CategoryLeaf
  Terminal node:
  Cannot have children
  Contains actual job postings
end note

note as N1
  Example Hierarchy:

  Technology Jobs (Composite)
  ├── Software Development (Composite)
  │   ├── Backend Developer (Leaf) - 2 jobs
  │   ├── Frontend Developer (Leaf) - 1 job
  │   └── DevOps Engineer (Leaf) - 1 job
  ├── Data & Analytics (Composite)
  │   ├── Data Scientist (Leaf) - 0 jobs
  │   └── Data Analyst (Leaf) - 0 jobs
  └── Design (Composite)
      ├── UI/UX Designer (Leaf) - 0 jobs
      └── Graphic Designer (Leaf) - 0 jobs
end note

@enduml
```

### Benefits
- **Uniform Treatment**: Treat individual categories and category groups uniformly
- **Recursive Composition**: Build complex hierarchies from simple components
- **Easy Traversal**: Navigate tree structure easily
- **Aggregation**: Calculate totals (job counts) across hierarchy

---

## 3. Façade Pattern - Job Application Workflow

### Purpose
The Façade pattern provides a simplified interface to a complex subsystem. It hides the complexity of multiple services and coordinates their interactions.

### Business Scenario
Submitting a job application involves multiple complex steps:
1. Validate job seeker profile completeness
2. Check eligibility (application limits, account status)
3. Verify job posting is accepting applications
4. Create application record
5. Send notifications to both parties

The Façade simplifies this to one method call: `SubmitJobApplicationAsync()`

### Components

#### Façade
```csharp
public class JobApplicationFacade
{
    private readonly IUserService _userService;
    private readonly IJobService _jobService;
    private readonly IApplicationService _applicationService;
    private readonly INotificationService _notificationService;

    public Task<ApplicationResult> SubmitJobApplicationAsync(
        int jobSeekerId,
        int jobPostingId,
        string coverLetter)
    {
        // Coordinates all subsystems in the correct order
        // Returns simple success/failure result
    }
}
```

#### Subsystems
- **UserService** - Profile validation
- **JobService** - Job posting validation
- **ApplicationService** - Application creation
- **NotificationService** - Email/notifications
- **ApplicationConfiguration** - Business rules (Singleton from Lab 3)

### Class Diagram

```plantuml
@startuml Facade_Pattern

!define FACADE_COLOR #FFF9C4
!define SUBSYSTEM_COLOR #E1F5FE
!define CLIENT_COLOR #F8BBD0

' Façade
class JobApplicationFacade <<Façade>> FACADE_COLOR {
    -userService : IUserService
    -jobService : IJobService
    -applicationService : IApplicationService
    -notificationService : INotificationService
    -jobSeekerRepository : IRepository<JobSeeker>
    -jobPostingRepository : IRepository<JobPosting>

    +SubmitJobApplicationAsync(seekerId, jobId, coverLetter) : Task<ApplicationResult>
    +GetApplicationStatusAsync(applicationId) : Task<string>
}

' Subsystems
class UserService <<Subsystem>> SUBSYSTEM_COLOR {
    +GetUserByIdAsync(id) : Task<User>
    +ValidateUserAsync(id) : Task<bool>
}

class JobService <<Subsystem>> SUBSYSTEM_COLOR {
    +GetJobByIdAsync(id) : Task<JobPosting>
    +IsJobAcceptingApplicationsAsync(id) : Task<bool>
}

class ApplicationService <<Subsystem>> SUBSYSTEM_COLOR {
    +CreateApplicationAsync(application) : Task<JobApplication>
    +GetJobSeekerApplicationsAsync(seekerId) : Task<IEnumerable<JobApplication>>
}

class NotificationService <<Subsystem>> SUBSYSTEM_COLOR {
    +SendApplicationConfirmationAsync(seekerId, jobTitle) : Task
    +NotifyEmployerNewApplicationAsync(employerId, seekerName, jobTitle) : Task
    +SendProfileCompletionReminderAsync(seekerId) : Task
}

class ApplicationConfiguration <<Subsystem>> SUBSYSTEM_COLOR {
    {static} +Instance : ApplicationConfiguration
    +MaxActiveApplicationsPerUser : int
    +JobExpiryDays : int
    +EmailNotificationsEnabled : bool
}

' DTOs
class ApplicationResult <<DTO>> {
    +Success : bool
    +Message : string
    +Application : JobApplication
    +ValidationErrors : List<string>
    {static} +Successful(application) : ApplicationResult
    {static} +Failed(message, errors) : ApplicationResult
}

' Client
class ApplicationController <<Client>> CLIENT_COLOR {
    -applicationFacade : JobApplicationFacade
    +Apply(model) : ActionResult
}

' Relationships
JobApplicationFacade --> UserService : coordinates
JobApplicationFacade --> JobService : coordinates
JobApplicationFacade --> ApplicationService : coordinates
JobApplicationFacade --> NotificationService : coordinates
JobApplicationFacade --> ApplicationConfiguration : uses

JobApplicationFacade ..> ApplicationResult : returns

ApplicationController --> JobApplicationFacade : uses

note right of JobApplicationFacade
  Simplifies complex workflow:

  1. Validate profile (UserService)
  2. Check eligibility (Configuration)
  3. Verify job status (JobService)
  4. Create application (ApplicationService)
  5. Send notifications (NotificationService)

  Client makes one call instead of five!
end note

note bottom of ApplicationResult
  Simple result object that hides
  the complexity of subsystems
end note

@enduml
```

### Workflow Diagram

```plantuml
@startuml Facade_Workflow

actor Client
participant "JobApplicationFacade" as Facade
participant "UserService" as User
participant "ApplicationConfig" as Config
participant "JobService" as Job
participant "ApplicationService" as App
participant "NotificationService" as Notify

Client -> Facade : SubmitJobApplicationAsync(seekerId, jobId, coverLetter)

activate Facade

Facade -> User : GetJobSeekerById(seekerId)
User --> Facade : JobSeeker

Facade -> Facade : Validate profile completeness
alt Profile incomplete
    Facade -> Notify : SendProfileCompletionReminderAsync()
    Facade --> Client : Failed("Profile incomplete")
end

Facade -> Config : Get MaxActiveApplicationsPerUser
Config --> Facade : limit

Facade -> App : GetJobSeekerApplicationsAsync(seekerId)
App --> Facade : existing applications

Facade -> Facade : Check application limit
alt Limit exceeded
    Facade --> Client : Failed("Application limit reached")
end

Facade -> Job : GetJobPostingById(jobId)
Job --> Facade : JobPosting

Facade -> Facade : Check if job accepting applications
alt Job not accepting
    Facade --> Client : Failed("Job not accepting applications")
end

Facade -> App : CreateApplicationAsync(application)
App --> Facade : created application

Facade -> Notify : SendApplicationConfirmationAsync()
Facade -> Notify : NotifyEmployerNewApplicationAsync()

Facade --> Client : Successful(application)

deactivate Facade

@enduml
```

### Benefits
- **Simplified Interface**: One method instead of coordinating five services
- **Decoupling**: Client doesn't need to know about subsystem complexity
- **Centralized Logic**: Workflow logic in one place
- **Easy Testing**: Mock the façade instead of all subsystems

---

## 4. Summary of Lab 4 Structural Patterns

### Pattern Comparison

| Pattern | Purpose | Use Case | Key Benefit |
|---------|---------|----------|-------------|
| **Adapter** | Convert incompatible interfaces | Payment gateway integration | Interoperability |
| **Composite** | Compose objects into tree structures | Job category hierarchy | Uniform treatment |
| **Façade** | Provide simplified interface to complex subsystem | Job application workflow | Simplification |

### Implementation Statistics

- **Files Created**: 23 new files
- **Patterns Implemented**: 3 structural patterns
- **Adapters**: 3 payment gateway adapters
- **Category Types**: 2 (Leaf, Composite)
- **Subsystems Coordinated**: 5 (UserService, JobService, ApplicationService, NotificationService, Configuration)

### Architecture Integration

All three structural patterns integrate seamlessly with the existing Clean Architecture:

```
Domain Layer
├── Entities: PaymentTransaction, CompanyReveal, JobCategory, CategoryLeaf, CategoryComposite
├── Enums: PaymentStatus, PaymentGateway
└── Interfaces: IPrototype (Lab 3)

Application Layer
├── Adapters: PayPalAdapter, StripeAdapter, GooglePayAdapter (Lab 4)
├── ExternalGateways: PayPalSDK, StripeSDK, GooglePaySDK (Lab 4)
├── Facades: JobApplicationFacade (Lab 4)
├── Services: PaymentService, CompanyRevealService, NotificationService (Lab 4)
├── Builders: JobSeekerProfileBuilder (Lab 3)
├── Configuration: ApplicationConfiguration (Lab 3 - Singleton)
└── DTOs: ApplicationResult (Lab 4)

Infrastructure Layer
├── Repositories: InMemoryRepository<T> (Generic Repository Pattern)

Presentation Layer
├── Controllers: (Can use Façade for simplified interactions)
```

### Running the Demo

To see all three patterns in action:

1. Run the application: `dotnet run`
2. Observe console output showing:
   - **Composite Pattern**: Category hierarchy with job counts
   - **Adapter Pattern**: Payments processed through 3 different gateways
   - **Façade Pattern**: Simplified application workflow with validation

### Key Takeaways

1. **Adapter Pattern** solved the problem of integrating incompatible third-party APIs
2. **Composite Pattern** enabled hierarchical organization with uniform operations
3. **Façade Pattern** simplified complex multi-service workflows into single method calls
4. All patterns maintain SOLID principles and clean architecture
5. Patterns work together: Façade uses services that might use Adapters internally

---

*This implementation demonstrates professional-grade structural pattern usage in a real-world ASP.NET Core application.*