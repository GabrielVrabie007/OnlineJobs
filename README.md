# Online Jobs Platform - ASP.NET Core MVC

A maintainable, scalable online jobs platform demonstrating SOLID principles, OOP concepts, and clean architecture patterns.

## Project Overview

This is an educational project built as a laboratory assignment to demonstrate proper application of:
- **SOLID Principles** (Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion)
- **OOP Concepts** (Encapsulation, Inheritance, Polymorphism, Abstraction)
- **Clean Architecture** with clear separation of concerns
- **Design Patterns** (Repository, Service Layer, Dependency Injection)

## Features

### For Job Seekers
- ✅ Register and login
- ✅ Browse active job postings
- ✅ Search jobs by title
- ✅ View detailed job descriptions
- ✅ Apply to jobs with cover letter
- ✅ Track application status
- ✅ Withdraw applications

### For Employers
- ✅ Register and login
- ✅ Create job postings
- ✅ Manage posted jobs
- ✅ View received applications
- ✅ Update application status (Review, Interview, Accept, Reject)
- ✅ Close job postings

### General Features
- ✅ User authentication with sessions
- ✅ Role-based access control
- ✅ Company profiles
- ✅ In-memory data storage (easily replaceable with database)

## Architecture

### Project Structure

```
OnlineJobs/
├── Domain/                      # Business entities and core logic
│   ├── Entities/
│   │   ├── User.cs             # Abstract base class
│   │   ├── JobSeeker.cs        # Inherits from User
│   │   ├── Employer.cs         # Inherits from User
│   │   ├── Company.cs
│   │   ├── JobPosting.cs
│   │   └── JobApplication.cs
│   ├── Enums/
│   │   ├── UserType.cs
│   │   ├── JobStatus.cs
│   │   └── ApplicationStatus.cs
│   └── Interfaces/
│       └── IRepository.cs      # Generic repository interface
│
├── Application/                 # Business logic layer
│   ├── Interfaces/
│   │   ├── IUserService.cs
│   │   ├── IJobService.cs
│   │   ├── IApplicationService.cs
│   │   └── ICompanyService.cs
│   └── Services/
│       ├── UserService.cs
│       ├── JobService.cs
│       ├── ApplicationService.cs
│       └── CompanyService.cs
│
├── Infrastructure/              # Data access layer
│   └── Repositories/
│       └── InMemoryRepository.cs
│
├── Web/                         # Presentation layer
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs
│   │   ├── JobController.cs
│   │   └── ApplicationController.cs
│   ├── Models/                 # ViewModels
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   ├── CreateJobViewModel.cs
│   │   └── ApplyJobViewModel.cs
│   └── Views/                  # Razor views (to be implemented)
│
├── Program.cs                  # Application entry point & DI configuration
├── OnlineJobs.csproj          # Project file
├── ARCHITECTURE.md            # Detailed architecture documentation
├── SOLID_PRINCIPLES_EXPLAINED.md  # SOLID principles explained
└── README.md                  # This file
```

### Layers Explanation

1. **Domain Layer**
   - Contains business entities
   - No dependencies on other layers
   - Pure C# classes with business logic
   - Encapsulation, inheritance, polymorphism demonstrated here

2. **Application Layer**
   - Contains business logic and workflows
   - Interfaces and service implementations
   - Depends only on Domain layer
   - Implements SRP, ISP, DIP

3. **Infrastructure Layer**
   - Data access implementations
   - Repository pattern
   - Currently in-memory, easily swappable
   - Implements DIP

4. **Presentation Layer (Web)**
   - ASP.NET Core MVC controllers
   - ViewModels for data transfer
   - Views (Razor pages)
   - Thin controllers - delegates to services

## How to Run

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 / Visual Studio Code / Rider

### Steps

1. **Navigate to project directory**
   ```bash
   cd OnlineJobs
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Open browser**
   - Navigate to: `https://localhost:5001` or `http://localhost:5000`

### Initial Data

The application seeds initial data on startup:

**Companies:**
- TechCorp Solutions (San Francisco, CA)
- Innovate Inc (New York, NY)

**Job Seekers:**
- john.doe@example.com / password123
- jane.smith@example.com / password123

**Employers:**
- hr@techcorp.example.com / password123
- recruiter@innovate.example.com / password123

**Jobs:**
- Senior Software Engineer at TechCorp Solutions
- Frontend Developer at TechCorp Solutions
- DevOps Engineer at Innovate Inc

## SOLID Principles Implementation

### 1. Single Responsibility Principle (SRP)
- Each class has ONE clear responsibility
- `UserService` → User management only
- `JobService` → Job posting management only
- `ApplicationService` → Application workflow only
- `InMemoryRepository<T>` → Data storage/retrieval only

### 2. Open/Closed Principle (OCP)
- `User` abstract class → Open for extension (JobSeeker, Employer)
- Can add new user types without modifying existing code
- Enums allow new statuses without code changes
- Repository interface allows new implementations

### 3. Liskov Substitution Principle (LSP)
- `JobSeeker` and `Employer` can replace `User` seamlessly
- All derived classes honor base class contracts
- Repository implementations are interchangeable

### 4. Interface Segregation Principle (ISP)
- Focused interfaces: `IUserService`, `IJobService`, `IApplicationService`
- No fat interfaces forcing unnecessary dependencies
- Controllers depend only on interfaces they use

### 5. Dependency Inversion Principle (DIP)
- Controllers depend on `IService` abstractions
- Services depend on `IRepository` abstractions
- All dependencies injected via constructor
- Configuration in `Program.cs`

For detailed explanations with code examples, see [SOLID_PRINCIPLES_EXPLAINED.md](SOLID_PRINCIPLES_EXPLAINED.md)

## OOP Concepts Demonstrated

### Encapsulation
- Private fields with public properties
- Validation in property setters
- Example: `User.Email` validates email format

### Inheritance
- `User` base class → `JobSeeker`, `Employer` derived classes
- Promotes code reuse
- Clear "is-a" relationships

### Polymorphism
- Virtual methods in `User`: `CanPostJobs()`, `CanApplyToJobs()`
- Overridden in derived classes with specific behavior
- Generic repository works with any entity type

### Abstraction
- Abstract `User` class cannot be instantiated
- Hides implementation details
- Interfaces define contracts without implementation

## Design Patterns

### Repository Pattern
- Abstracts data access logic
- `IRepository<T>` interface
- `InMemoryRepository<T>` implementation
- Easy to swap with database repository

### Service Layer Pattern
- Business logic separated from controllers
- Reusable service classes
- Easier testing and maintenance

### Dependency Injection
- Constructor injection throughout
- Built-in ASP.NET Core DI container
- Promotes loose coupling

### ViewModel Pattern
- Separate models for views
- Validation attributes
- Don't expose domain entities directly to views

## Benefits of This Architecture

✅ **Maintainable**
- Clear structure - easy to find code
- Single responsibility - easy to fix bugs
- Well-documented

✅ **Scalable**
- Add new features without modifying existing code
- Extensible through inheritance and interfaces
- Layered architecture supports growth

✅ **Testable**
- All dependencies are interfaces
- Easy to mock for unit testing
- Business logic separated from infrastructure

✅ **Flexible**
- Swap in-memory storage for database
- Add new user types (Admin, Moderator)
- Extend with new features (ratings, reviews)

✅ **Professional**
- Industry-standard practices
- Clean code principles
- SOLID principles throughout

## Next Steps for Enhancement

To turn this into a production application, consider:

1. **Database Integration**
   - Replace `InMemoryRepository` with Entity Framework Core
   - Add database migrations
   - Implement `DbContext`

2. **Authentication & Authorization**
   - Implement ASP.NET Core Identity
   - Add JWT tokens for API
   - Role-based authorization attributes

3. **Views Implementation**
   - Create Razor views for all controllers
   - Add Bootstrap for styling
   - Implement responsive design

4. **Additional Features**
   - File upload for resumes
   - Email notifications
   - Advanced search with filters
   - Pagination for job listings
   - Company reviews and ratings

5. **Testing**
   - Unit tests for services
   - Integration tests for controllers
   - Repository tests

6. **API Layer**
   - Add Web API controllers
   - RESTful endpoints
   - Swagger documentation

## Learning Objectives Achieved

✅ Proper application of SOLID principles
✅ Demonstration of OOP concepts (encapsulation, inheritance, polymorphism)
✅ Clean architecture with separation of concerns
✅ Dependency injection and inversion of control
✅ Repository pattern implementation
✅ Service layer pattern
✅ Maintainable, scalable code structure

## Documentation

- [ARCHITECTURE.md](ARCHITECTURE.md) - Detailed architecture with UML diagrams
- [SOLID_PRINCIPLES_EXPLAINED.md](SOLID_PRINCIPLES_EXPLAINED.md) - In-depth SOLID principles explanation
- Code comments throughout explaining design decisions

## Author

Created as a university laboratory project demonstrating software engineering best practices.

## License

Educational use only.