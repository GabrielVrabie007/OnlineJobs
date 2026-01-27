# Online Jobs Platform - Architecture Documentation

## Overview
This application demonstrates SOLID principles and OOP concepts in a maintainable, scalable online jobs platform built with ASP.NET Core MVC.

## UML Class Diagram (Text Representation)

```
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer (Entities)                  │
└─────────────────────────────────────────────────────────────┘

┌──────────────────────┐
│   <<abstract>>       │
│      User            │
├──────────────────────┤
│ - Id: Guid           │
│ - Email: string      │
│ - PasswordHash: str  │
│ - FirstName: string  │
│ - LastName: string   │
│ - CreatedAt: DateTime│
│ - UserType: UserType │
├──────────────────────┤
│ + GetFullName(): str │
│ + IsActive(): bool   │
└──────────────────────┘
         △
         │ (inheritance)
    ┌────┴────┐
    │         │
┌───┴────┐ ┌──┴────────┐
│JobSeeker│ │ Employer  │
├─────────┤ ├───────────┤
│-Resume  │ │-CompanyId │
│-Skills  │ │-Company   │
└─────────┘ └───────────┘


┌──────────────────────┐         ┌──────────────────────┐
│      Company         │◄────────│     Employer         │
├──────────────────────┤  owns   └──────────────────────┘
│ - Id: Guid           │
│ - Name: string       │         ┌──────────────────────┐
│ - Description: str   │         │    JobPosting        │
│ - Website: string    │         ├──────────────────────┤
│ - Location: string   │         │ - Id: Guid           │
└──────────────────────┘         │ - Title: string      │
         │                       │ - Description: str   │
         │ creates               │ - Requirements: str  │
         │ (1:N)                 │ - Salary: decimal?   │
         ▼                       │ - Location: string   │
┌──────────────────────┐         │ - EmployerId: Guid   │
│    JobPosting        │         │ - CompanyId: Guid    │
└──────────────────────┘         │ - PostedDate: Date   │
         │                       │ - Status: JobStatus  │
         │                       └──────────────────────┘
         │                                △
         │ receives                       │
         │ (1:N)                          │
         ▼                                │
┌──────────────────────┐                 │
│  JobApplication      │─────────────────┘
├──────────────────────┤    applies to
│ - Id: Guid           │
│ - JobPostingId: Guid │
│ - JobSeekerId: Guid  │
│ - CoverLetter: str   │
│ - AppliedDate: Date  │
│ - Status: AppStatus  │
└──────────────────────┘


┌─────────────────────────────────────────────────────────────┐
│                    Interfaces (ISP)                         │
└─────────────────────────────────────────────────────────────┘

<<interface>>              <<interface>>
IRepository<T>             IUserService
├─GetAll()                 ├─Register()
├─GetById(id)              ├─Login()
├─Add(entity)              ├─GetUserById()
├─Update(entity)           └─ValidateCredentials()
└─Delete(id)

<<interface>>              <<interface>>
IJobService                IApplicationService
├─CreateJob()              ├─SubmitApplication()
├─UpdateJob()              ├─GetApplicationsByJobSeeker()
├─GetAllJobs()             ├─GetApplicationsByEmployer()
├─SearchByTitle()          └─UpdateApplicationStatus()
└─DeleteJob()


┌─────────────────────────────────────────────────────────────┐
│              Service Layer (SRP, DIP)                       │
└─────────────────────────────────────────────────────────────┘

Each service class has a SINGLE responsibility:
- UserService: User authentication and management
- JobService: Job posting CRUD operations
- ApplicationService: Job application management
- SearchService: Search and filtering logic

All services depend on ABSTRACTIONS (interfaces), not concrete implementations.
```

## SOLID Principles Application

### 1. Single Responsibility Principle (SRP)
- **User**: Manages only user identity and authentication data
- **JobPosting**: Handles only job posting information
- **JobApplication**: Manages application data and state
- **UserService**: Handles ONLY user-related business logic
- **JobService**: Handles ONLY job posting operations
- **ApplicationService**: Handles ONLY application workflows

### 2. Open/Closed Principle (OCP)
- Abstract `User` class allows extension (JobSeeker, Employer) without modification
- Strategy pattern for different user types
- New job statuses can be added via enum extension
- New search strategies can be added without modifying existing code

### 3. Liskov Substitution Principle (LSP)
- `JobSeeker` and `Employer` can replace `User` without breaking functionality
- All derived classes honor base class contracts
- Repository implementations can be swapped (in-memory → database)

### 4. Interface Segregation Principle (ISP)
- `IRepository<T>`: Generic data access operations
- `IUserService`: Only user-specific operations
- `IJobService`: Only job-specific operations
- `IApplicationService`: Only application-specific operations
- No class is forced to implement methods it doesn't use

### 5. Dependency Inversion Principle (DIP)
- Controllers depend on `IService` interfaces, not concrete services
- Services depend on `IRepository` interfaces, not concrete repositories
- High-level modules don't depend on low-level modules
- Dependencies injected via constructor injection

## Additional Principles

### KISS (Keep It Simple, Stupid)
- Clear, straightforward class hierarchies
- Simple method names that describe their purpose
- No over-engineering or unnecessary complexity

### DRY (Don't Repeat Yourself)
- Generic `IRepository<T>` prevents code duplication
- Base `User` class contains common user properties
- Shared validation logic in base classes

### Separation of Concerns
- **Domain Layer**: Business entities (no dependencies)
- **Application Layer**: Business logic and services
- **Infrastructure Layer**: Data access implementation
- **Presentation Layer**: MVC controllers and views

## Project Structure

```
OnlineJobs/
├── Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── JobSeeker.cs
│   │   ├── Employer.cs
│   │   ├── Company.cs
│   │   ├── JobPosting.cs
│   │   └── JobApplication.cs
│   ├── Enums/
│   │   ├── UserType.cs
│   │   ├── JobStatus.cs
│   │   └── ApplicationStatus.cs
│   └── Interfaces/
│       └── IRepository.cs
├── Application/
│   ├── Interfaces/
│   │   ├── IUserService.cs
│   │   ├── IJobService.cs
│   │   └── IApplicationService.cs
│   └── Services/
│       ├── UserService.cs
│       ├── JobService.cs
│       └── ApplicationService.cs
├── Infrastructure/
│   └── Repositories/
│       ├── InMemoryRepository.cs
│       ├── UserRepository.cs
│       ├── JobRepository.cs
│       └── ApplicationRepository.cs
├── Web/
│   ├── Controllers/
│   │   ├── AccountController.cs
│   │   ├── JobController.cs
│   │   └── ApplicationController.cs
│   ├── Views/
│   └── Models/ (ViewModels)
└── Program.cs
```

## Key Design Decisions

1. **Abstract User Class**: Allows polymorphic behavior for different user types
2. **Repository Pattern**: Abstracts data access, easy to swap implementations
3. **Service Layer**: Encapsulates business logic, keeps controllers thin
4. **Dependency Injection**: Built-in ASP.NET Core DI container
5. **In-Memory Storage**: Simplifies demo, easily replaceable with EF Core

## Benefits of This Architecture

- ✓ **Maintainable**: Clear separation of concerns, easy to locate and fix bugs
- ✓ **Scalable**: Can add new features without modifying existing code
- ✓ **Testable**: Interfaces allow easy mocking for unit tests
- ✓ **Flexible**: Repository implementations can be swapped
- ✓ **Understandable**: Clear naming and structure following conventions