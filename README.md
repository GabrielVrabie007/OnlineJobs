
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
- ✅ In-memory data storage (with future moving to MS SQL database)

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


E