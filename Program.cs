using Microsoft.EntityFrameworkCore;
using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Services;
using OnlineJobs.Application.Builders;
using OnlineJobs.Application.Configuration;
using OnlineJobs.Application.Facades;
using OnlineJobs.Application.Factories;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.ValueObjects;
using OnlineJobs.Domain.Enums;
using OnlineJobs.Infrastructure.Repositories;
using OnlineJobs.Infrastructure.Data;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Needed so per-user pattern state (e.g. Command undo history) can resolve the
// current user from the session inside the composition root.
builder.Services.AddHttpContextAccessor();

// Configure SQLite Database. Passing the connection string (not a single shared
// SqliteConnection) lets EF manage a connection per DbContext, which is safe under
// concurrent requests.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OnlineJobsDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// EF Core Repositories
builder.Services.AddScoped<IRepository<User>, EFRepository<User>>();
builder.Services.AddScoped<IRepository<JobSeeker>, EFRepository<JobSeeker>>();
builder.Services.AddScoped<IRepository<Employer>, EFRepository<Employer>>();
builder.Services.AddScoped<IRepository<Company>, EFRepository<Company>>();
builder.Services.AddScoped<IRepository<JobPosting>, EFRepository<JobPosting>>();
builder.Services.AddScoped<IRepository<JobApplication>, EFRepository<JobApplication>>();

// Lab 4 - Adapter Pattern repositories
builder.Services.AddScoped<IRepository<PaymentTransaction>, EFRepository<PaymentTransaction>>();
builder.Services.AddScoped<IRepository<CompanyReveal>, EFRepository<CompanyReveal>>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<CategoryService>(); // Composite-pattern category tree
builder.Services.AddScoped<IDocumentGenerationService, DocumentGenerationService>(); // Abstract Factory Pattern

// Lab 3 - Builder Pattern
builder.Services.AddScoped<IJobSeekerProfileBuilder, JobSeekerProfileBuilder>();
builder.Services.AddScoped<JobSeekerProfileDirector>();

// Lab 4 - Adapter Pattern services
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ICompanyRevealService, CompanyRevealService>();

// Lab 4 - Façade Pattern services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<JobApplicationFacade>();

// In-app notification inbox shown in the navbar bell + Notifications page.
// Fed by the Decorator (NotificationService) and the Observer (DashboardNotificationObserver).
builder.Services.AddSingleton<OnlineJobs.Application.Notifications.NotificationStore>();

// Lab 5 - Flyweight Pattern - Singleton factory for skill sharing
builder.Services.AddSingleton<SkillFlyweightFactory>();

// Lab 5 - Bridge Pattern - Reporting Service
builder.Services.AddScoped<ReportingService>();

// Lab 6 - Observer Pattern
builder.Services.AddSingleton<OnlineJobs.Application.Observers.JobPostingSubject>();
builder.Services.AddSingleton<OnlineJobs.Application.Observers.ApplicationStatusSubject>();
builder.Services.AddTransient<OnlineJobs.Application.Observers.JobSeekerObserver>();
builder.Services.AddTransient<OnlineJobs.Application.Observers.EmailAlertObserver>();
builder.Services.AddTransient<OnlineJobs.Application.Observers.DashboardNotificationObserver>();
builder.Services.AddTransient<OnlineJobs.Application.Observers.StatisticsObserver>();
builder.Services.AddTransient<OnlineJobs.Application.Observers.AuditLogObserver>();

// Lab 6 - Command Pattern.
// History is kept PER USER in a singleton store so Undo/Redo works across requests.
// The scoped CommandHistory resolves to the current user's instance (read from the
// session here in the composition root, keeping the Application layer ASP.NET-free).
builder.Services.AddSingleton<OnlineJobs.Application.Commands.UserCommandHistoryStore>();
builder.Services.AddScoped<OnlineJobs.Application.Commands.CommandHistory>(sp =>
{
    var store = sp.GetRequiredService<OnlineJobs.Application.Commands.UserCommandHistoryStore>();
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    var userIdString = httpContext?.Session.GetString("UserId");
    return Guid.TryParse(userIdString, out var userId)
        ? store.GetForUser(userId)
        : new OnlineJobs.Application.Commands.CommandHistory();
});
builder.Services.AddScoped<OnlineJobs.Application.Commands.CommandInvoker>();

// Lab 6 - Memento Pattern
builder.Services.AddSingleton<OnlineJobs.Application.Mementos.ProfileHistory>();
builder.Services.AddSingleton<OnlineJobs.Application.Mementos.JobPostingDraftManager>();
builder.Services.AddSingleton<OnlineJobs.Application.Mementos.ApplicationDraftManager>();

// Additional patterns wired into the employer review flow:
builder.Services.AddScoped<OnlineJobs.Application.ApprovalChains.ApprovalChainFactory>();   // Chain of Responsibility
builder.Services.AddScoped<OnlineJobs.Application.Mediators.NotificationMediator>();          // Mediator

// Lab 6 - Strategy Pattern (Additional Strategies)
builder.Services.AddTransient<OnlineJobs.Application.Strategies.SalaryStrategies.ISalaryCalculationStrategy, OnlineJobs.Application.Strategies.SalaryStrategies.AnnualSalaryStrategy>();
builder.Services.AddSingleton<OnlineJobs.Application.Strategies.SalaryStrategies.SalaryStrategyFactory>();
builder.Services.AddTransient<OnlineJobs.Application.Strategies.ScoringStrategies.IApplicationScoringStrategy, OnlineJobs.Application.Strategies.ScoringStrategies.ComprehensiveScoringStrategy>();

// Some startup/seed code resolves the concrete services. Alias the concretes to the
// SAME scoped instance the interfaces resolve, instead of registering a second copy.
builder.Services.AddScoped<UserService>(sp => (UserService)sp.GetRequiredService<IUserService>());
builder.Services.AddScoped<JobService>(sp => (JobService)sp.GetRequiredService<IJobService>());
builder.Services.AddScoped<ApplicationService>(sp => (ApplicationService)sp.GetRequiredService<IApplicationService>());
builder.Services.AddScoped<CompanyService>(sp => (CompanyService)sp.GetRequiredService<ICompanyService>());

var app = builder.Build();

// Initialize and configure the Singleton Configuration Manager (Lab 3)
var config = ApplicationConfiguration.Instance;
Console.WriteLine($"✓ {config}");
Console.WriteLine($"  - Job Expiry: {config.JobExpiryDays} days");
Console.WriteLine($"  - Max Applications per User: {config.MaxActiveApplicationsPerUser}");
Console.WriteLine($"  - Max Job Postings per Employer: {config.MaxActiveJobPostingsPerEmployer}");

// Ensure database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OnlineJobsDbContext>();
    try
    {
        Console.WriteLine("\n📊 Initializing database...");
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✓ Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization error: {ex.Message}");
    }
}

await SeedDataAsync(app.Services);
SeedNotifications(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Notifications live in an in-memory store, so we re-seed a realistic set for the two
// demo accounts on each boot. Newest appears last in this list -> shows on top in the bell.
void SeedNotifications(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var store = scope.ServiceProvider.GetRequiredService<OnlineJobs.Application.Notifications.NotificationStore>();
    var db = scope.ServiceProvider.GetRequiredService<OnlineJobsDbContext>();

    void For(string email, (string title, string msg, string icon)[] items)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null) return;
        foreach (var (title, msg, icon) in items)
            store.Add(user.Id, title, msg, icon);
    }

    For("gabriel.vrabie@isa.utm.md", new[]
    {
        ("Welcome to OnlineJobs", "Complete your profile to improve your match score.", "bi-person-lines-fill"),
        ("Application submitted", "Your application for 'Senior Backend Engineer' was received.", "bi-send-check"),
        ("Application update", "Your application is now Under review.", "bi-arrow-repeat"),
        ("Interview invitation", "You've been moved to Interviewing for 'Machine Learning Engineer'.", "bi-calendar-check"),
        ("Great news!", "Your application was Accepted - the employer will be in touch.", "bi-check-circle"),
    });

    For("vrabiegabriel07@gmail.com", new[]
    {
        ("New application received", "Emily Chen applied for 'Senior Backend Engineer'.", "bi-person-plus"),
        ("New application received", "Michael Rodriguez applied for 'Product Designer'.", "bi-person-plus"),
        ("New application received", "Sarah Johnson applied for 'Machine Learning Engineer'.", "bi-person-plus"),
        ("Weekly report ready", "Your applications report is ready to download.", "bi-bar-chart"),
    });
}

async Task SeedDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var companyService = scope.ServiceProvider.GetRequiredService<CompanyService>();
    var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
    var applicationService = scope.ServiceProvider.GetRequiredService<IApplicationService>();
    var dbContext = scope.ServiceProvider.GetRequiredService<OnlineJobsDbContext>();

    try
    {
        // Check if database is already seeded
        if (dbContext.Companies.Any())
        {
            Console.WriteLine("\n✓ Database already contains data. Skipping seeding.\n");
            return;
        }

        Console.WriteLine("\n🌱 Seeding database with realistic data...\n");

        // ============================================
        // CREATE 15 REALISTIC COMPANIES
        // ============================================

        var apple = await companyService.CreateCompanyAsync("Apple Inc.", "Cupertino, CA");
        apple.Description = "Technology company designing and manufacturing consumer electronics, software, and online services";
        apple.Website = "https://apple.com";
        apple.Industry = "Technology";
        apple.EmployeeCount = 164000;
        await companyService.UpdateCompanyAsync(apple);

        var google = await companyService.CreateCompanyAsync("Google", "Mountain View, CA");
        google.Description = "Multinational technology company specializing in Internet-related services and products";
        google.Website = "https://google.com";
        google.Industry = "Technology";
        google.EmployeeCount = 190000;
        await companyService.UpdateCompanyAsync(google);

        var microsoft = await companyService.CreateCompanyAsync("Microsoft Corporation", "Redmond, WA");
        microsoft.Description = "Leading platform and productivity company for the mobile-first, cloud-first world";
        microsoft.Website = "https://microsoft.com";
        microsoft.Industry = "Technology";
        microsoft.EmployeeCount = 221000;
        await companyService.UpdateCompanyAsync(microsoft);

        var amazon = await companyService.CreateCompanyAsync("Amazon", "Seattle, WA");
        amazon.Description = "E-commerce, cloud computing, digital streaming, and artificial intelligence company";
        amazon.Website = "https://amazon.com";
        amazon.Industry = "E-commerce & Cloud";
        amazon.EmployeeCount = 1540000;
        await companyService.UpdateCompanyAsync(amazon);

        var meta = await companyService.CreateCompanyAsync("Meta (Facebook)", "Menlo Park, CA");
        meta.Description = "Social technology company building the metaverse and connecting people worldwide";
        meta.Website = "https://meta.com";
        meta.Industry = "Social Media";
        meta.EmployeeCount = 86000;
        await companyService.UpdateCompanyAsync(meta);

        var netflix = await companyService.CreateCompanyAsync("Netflix", "Los Gatos, CA");
        netflix.Description = "World's leading streaming entertainment service with over 230 million paid memberships";
        netflix.Website = "https://netflix.com";
        netflix.Industry = "Entertainment";
        netflix.EmployeeCount = 12800;
        await companyService.UpdateCompanyAsync(netflix);

        var airbnb = await companyService.CreateCompanyAsync("Airbnb", "San Francisco, CA");
        airbnb.Description = "Online marketplace for lodging, primarily homestays for vacation rentals and tourism";
        airbnb.Website = "https://airbnb.com";
        airbnb.Industry = "Travel & Hospitality";
        airbnb.EmployeeCount = 6800;
        await companyService.UpdateCompanyAsync(airbnb);

        var tesla = await companyService.CreateCompanyAsync("Tesla Inc.", "Austin, TX");
        tesla.Description = "Electric vehicle and clean energy company designing and manufacturing electric cars and sustainable energy solutions";
        tesla.Website = "https://tesla.com";
        tesla.Industry = "Automotive & Energy";
        tesla.EmployeeCount = 127855;
        await companyService.UpdateCompanyAsync(tesla);

        var goldman = await companyService.CreateCompanyAsync("Goldman Sachs", "New York, NY");
        goldman.Description = "Leading global investment banking, securities and investment management firm";
        goldman.Website = "https://goldmansachs.com";
        goldman.Industry = "Financial Services";
        goldman.EmployeeCount = 48500;
        await companyService.UpdateCompanyAsync(goldman);

        var jpmorgan = await companyService.CreateCompanyAsync("JPMorgan Chase & Co.", "New York, NY");
        jpmorgan.Description = "Global financial services firm and one of the largest banking institutions";
        jpmorgan.Website = "https://jpmorganchase.com";
        jpmorgan.Industry = "Banking";
        jpmorgan.EmployeeCount = 293723;
        await companyService.UpdateCompanyAsync(jpmorgan);

        var salesforce = await companyService.CreateCompanyAsync("Salesforce", "San Francisco, CA");
        salesforce.Description = "Cloud-based software company providing customer relationship management (CRM) service";
        salesforce.Website = "https://salesforce.com";
        salesforce.Industry = "Software (CRM)";
        salesforce.EmployeeCount = 79000;
        await companyService.UpdateCompanyAsync(salesforce);

        var adobe = await companyService.CreateCompanyAsync("Adobe Inc.", "San Jose, CA");
        adobe.Description = "Multinational computer software company known for multimedia and creativity software products";
        adobe.Website = "https://adobe.com";
        adobe.Industry = "Software";
        adobe.EmployeeCount = 29239;
        await companyService.UpdateCompanyAsync(adobe);

        var uber = await companyService.CreateCompanyAsync("Uber Technologies", "San Francisco, CA");
        uber.Description = "Mobility as a service provider offering ride-hailing, food delivery, and freight transport";
        uber.Website = "https://uber.com";
        uber.Industry = "Transportation";
        uber.EmployeeCount = 32800;
        await companyService.UpdateCompanyAsync(uber);

        var stripe = await companyService.CreateCompanyAsync("Stripe", "San Francisco, CA");
        stripe.Description = "Financial services and software as a service company offering payment processing for internet businesses";
        stripe.Website = "https://stripe.com";
        stripe.Industry = "Fintech";
        stripe.EmployeeCount = 8000;
        await companyService.UpdateCompanyAsync(stripe);

        var shopify = await companyService.CreateCompanyAsync("Shopify", "Ottawa, Canada");
        shopify.Description = "E-commerce platform helping businesses sell online, in-store, and everywhere in between";
        shopify.Website = "https://shopify.com";
        shopify.Industry = "E-commerce";
        shopify.EmployeeCount = 11600;
        await companyService.UpdateCompanyAsync(shopify);

        Console.WriteLine($"✓ Created {15} companies");

        // ============================================
        // CREATE 20 JOB SEEKERS
        // ============================================

        var jobSeeker1 = await userService.RegisterJobSeekerAsync("emily.chen@email.com", "Emily", "Chen", "password123");
        var jobSeeker2 = await userService.RegisterJobSeekerAsync("michael.rodriguez@email.com", "Michael", "Rodriguez", "password123");
        var jobSeeker3 = await userService.RegisterJobSeekerAsync("sarah.johnson@email.com", "Sarah", "Johnson", "password123");
        var jobSeeker4 = await userService.RegisterJobSeekerAsync("david.kim@email.com", "David", "Kim", "password123");
        var jobSeeker5 = await userService.RegisterJobSeekerAsync("jessica.taylor@email.com", "Jessica", "Taylor", "password123");
        var jobSeeker6 = await userService.RegisterJobSeekerAsync("james.anderson@email.com", "James", "Anderson", "password123");
        var jobSeeker7 = await userService.RegisterJobSeekerAsync("amanda.martinez@email.com", "Amanda", "Martinez", "password123");
        var jobSeeker8 = await userService.RegisterJobSeekerAsync("ryan.thomas@email.com", "Ryan", "Thomas", "password123");
        var jobSeeker9 = await userService.RegisterJobSeekerAsync("nicole.jackson@email.com", "Nicole", "Jackson", "password123");
        var jobSeeker10 = await userService.RegisterJobSeekerAsync("kevin.white@email.com", "Kevin", "White", "password123");
        var jobSeeker11 = await userService.RegisterJobSeekerAsync("lisa.harris@email.com", "Lisa", "Harris", "password123");
        var jobSeeker12 = await userService.RegisterJobSeekerAsync("daniel.clark@email.com", "Daniel", "Clark", "password123");
        var jobSeeker13 = await userService.RegisterJobSeekerAsync("rachel.lewis@email.com", "Rachel", "Lewis", "password123");
        var jobSeeker14 = await userService.RegisterJobSeekerAsync("chris.walker@email.com", "Chris", "Walker", "password123");
        var jobSeeker15 = await userService.RegisterJobSeekerAsync("sophia.hall@email.com", "Sophia", "Hall", "password123");
        var jobSeeker16 = await userService.RegisterJobSeekerAsync("brandon.allen@email.com", "Brandon", "Allen", "password123");
        var jobSeeker17 = await userService.RegisterJobSeekerAsync("olivia.young@email.com", "Olivia", "Young", "password123");
        var jobSeeker18 = await userService.RegisterJobSeekerAsync("matthew.king@email.com", "Matthew", "King", "password123");
        var jobSeeker19 = await userService.RegisterJobSeekerAsync("ashley.wright@email.com", "Ashley", "Wright", "password123");
        var jobSeeker20 = await userService.RegisterJobSeekerAsync("joshua.lopez@email.com", "Joshua", "Lopez", "password123");

        Console.WriteLine($"✓ Created {20} job seekers");

        // ============================================
        // CREATE 15 EMPLOYERS
        // ============================================

        var emp1 = await userService.RegisterEmployerAsync("hr.apple@company.com", "Jennifer", "Smith", "password123", apple.Id);
        var emp2 = await userService.RegisterEmployerAsync("recruiting.google@company.com", "Robert", "Johnson", "password123", google.Id);
        var emp3 = await userService.RegisterEmployerAsync("talent.microsoft@company.com", "Maria", "Garcia", "password123", microsoft.Id);
        var emp4 = await userService.RegisterEmployerAsync("hiring.amazon@company.com", "William", "Brown", "password123", amazon.Id);
        var emp5 = await userService.RegisterEmployerAsync("careers.meta@company.com", "Patricia", "Davis", "password123", meta.Id);
        var emp6 = await userService.RegisterEmployerAsync("jobs.netflix@company.com", "Richard", "Miller", "password123", netflix.Id);
        var emp7 = await userService.RegisterEmployerAsync("talent.airbnb@company.com", "Linda", "Wilson", "password123", airbnb.Id);
        var emp8 = await userService.RegisterEmployerAsync("hr.tesla@company.com", "Thomas", "Moore", "password123", tesla.Id);
        var emp9 = await userService.RegisterEmployerAsync("recruiting.goldman@company.com", "Barbara", "Taylor", "password123", goldman.Id);
        var emp10 = await userService.RegisterEmployerAsync("hr.jpmorgan@company.com", "Christopher", "Anderson", "password123", jpmorgan.Id);
        var emp11 = await userService.RegisterEmployerAsync("talent.salesforce@company.com", "Nancy", "Thomas", "password123", salesforce.Id);
        var emp12 = await userService.RegisterEmployerAsync("careers.adobe@company.com", "Steven", "Jackson", "password123", adobe.Id);
        var emp13 = await userService.RegisterEmployerAsync("jobs.uber@company.com", "Karen", "White", "password123", uber.Id);
        var emp14 = await userService.RegisterEmployerAsync("hiring.stripe@company.com", "Kevin", "Harris", "password123", stripe.Id);
        var emp15 = await userService.RegisterEmployerAsync("talent.shopify@company.com", "Lisa", "Martin", "password123", shopify.Id);

        // Custom user for Gabriel
        var empGabriel = await userService.RegisterEmployerAsync("vrabiegabriel07@gmail.com", "Gabriel", "Vrabie", "123!@#", apple.Id);

        Console.WriteLine($"✓ Created {16} employers");

        // ============================================
        // CREATE 50+ JOB POSTINGS
        // ============================================

        // Apple Jobs (4)
        var job1 = await jobService.CreateJobAsync(
            "Senior iOS Engineer",
            "Join Apple's world-class iOS team to build the next generation of iOS experiences. Work on features used by millions of users daily. You'll collaborate with designers, product managers, and engineers across Apple to create intuitive and innovative iOS applications.",
            emp1.Id, apple.Id);
        job1.Requirements = "5+ years iOS development, Swift, SwiftUI, Objective-C, strong CS fundamentals";
        job1.SalaryMin = 180000; job1.SalaryMax = 250000;
        job1.EmploymentType = "Full-time"; job1.Category = "Software Development";
        await jobService.UpdateJobAsync(job1);
        await jobService.PublishJobAsync(job1.Id);

        var job2 = await jobService.CreateJobAsync(
            "Machine Learning Engineer",
            "Be part of Apple's ML team developing cutting-edge machine learning solutions for products like Siri, Photos, and more. Work on large-scale ML systems that impact billions of users.",
            emp1.Id, apple.Id);
        job2.Requirements = "PhD or MS in CS/ML, TensorFlow, PyTorch, Python, 3+ years ML experience";
        job2.SalaryMin = 200000; job2.SalaryMax = 280000;
        job2.EmploymentType = "Full-time"; job2.Category = "Machine Learning";
        await jobService.UpdateJobAsync(job2);
        await jobService.PublishJobAsync(job2.Id);

        var job3 = await jobService.CreateJobAsync(
            "Hardware Engineer",
            "Design and develop innovative hardware products for the next generation of Apple devices. Work with cutting-edge technology and world-class engineers.",
            emp1.Id, apple.Id);
        job3.Requirements = "BS/MS in EE, 4+ years hardware design, PCB design, signal integrity";
        job3.SalaryMin = 160000; job3.SalaryMax = 220000;
        job3.EmploymentType = "Full-time"; job3.Category = "Hardware Engineering";
        await jobService.UpdateJobAsync(job3);
        await jobService.PublishJobAsync(job3.Id);

        var job4 = await jobService.CreateJobAsync(
            "Product Designer",
            "Create beautiful, intuitive designs for Apple's products. Join a team of world-class designers shaping the future of technology.",
            emp1.Id, apple.Id);
        job4.Requirements = "5+ years product design, Figma, Sketch, strong portfolio, HCI knowledge";
        job4.SalaryMin = 150000; job4.SalaryMax = 210000;
        job4.EmploymentType = "Full-time"; job4.Category = "Design";
        await jobService.UpdateJobAsync(job4);
        await jobService.PublishJobAsync(job4.Id);

        // Google Jobs (4)
        var job5 = await jobService.CreateJobAsync(
            "Senior Software Engineer",
            "Build technologies that help billions of people connect, explore, and interact with information. Work on Google's core products like Search, Chrome, Android, and more.",
            emp2.Id, google.Id);
        job5.Requirements = "BS in CS, 5+ years software development, Java/C++/Python, distributed systems";
        job5.SalaryMin = 190000; job5.SalaryMax = 270000;
        job5.EmploymentType = "Full-time"; job5.Category = "Software Development";
        await jobService.UpdateJobAsync(job5);
        await jobService.PublishJobAsync(job5.Id);

        var job6 = await jobService.CreateJobAsync(
            "Site Reliability Engineer",
            "Ensure Google's services are reliable and scalable. Work on infrastructure that serves billions of users with 99.99% uptime.",
            emp2.Id, google.Id);
        job6.Requirements = "Linux/Unix, Python/Go, networking, 4+ years SRE/DevOps experience";
        job6.SalaryMin = 170000; job6.SalaryMax = 240000;
        job6.EmploymentType = "Full-time"; job6.Category = "DevOps/SRE";
        await jobService.UpdateJobAsync(job6);
        await jobService.PublishJobAsync(job6.Id);

        var job7 = await jobService.CreateJobAsync(
            "UX Researcher",
            "Lead research initiatives to understand user needs and behaviors. Shape product decisions with data-driven insights.",
            emp2.Id, google.Id);
        job7.Requirements = "MS/PhD in HCI or related, 3+ years UX research, quantitative/qualitative methods";
        job7.SalaryMin = 140000; job7.SalaryMax = 190000;
        job7.EmploymentType = "Full-time"; job7.Category = "Research";
        await jobService.UpdateJobAsync(job7);
        await jobService.PublishJobAsync(job7.Id);

        var job8 = await jobService.CreateJobAsync(
            "Product Manager",
            "Drive product strategy and roadmap for key Google products. Work with cross-functional teams to deliver innovative solutions.",
            emp2.Id, google.Id);
        job8.Requirements = "MBA or 5+ years PM experience, technical background, data-driven decision making";
        job8.SalaryMin = 175000; job8.SalaryMax = 260000;
        job8.EmploymentType = "Full-time"; job8.Category = "Product Management";
        await jobService.UpdateJobAsync(job8);
        await jobService.PublishJobAsync(job8.Id);

        // Microsoft Jobs (4)
        var job9 = await jobService.CreateJobAsync(
            "Cloud Solution Architect",
            "Help customers architect and deploy solutions on Microsoft Azure. Lead technical engagements and drive cloud adoption.",
            emp3.Id, microsoft.Id);
        job9.Requirements = "Azure certifications, 5+ years cloud architecture, C#/.NET, microservices";
        job9.SalaryMin = 165000; job9.SalaryMax = 230000;
        job9.EmploymentType = "Full-time"; job9.Category = "Cloud Architecture";
        await jobService.UpdateJobAsync(job9);
        await jobService.PublishJobAsync(job9.Id);

        var job10 = await jobService.CreateJobAsync(
            "Principal Software Engineer",
            "Lead engineering teams building the next generation of Microsoft 365. Drive technical excellence and mentor engineers.",
            emp3.Id, microsoft.Id);
        job10.Requirements = "10+ years software development, C#, TypeScript, distributed systems, leadership";
        job10.SalaryMin = 210000; job10.SalaryMax = 300000;
        job10.EmploymentType = "Full-time"; job10.Category = "Software Development";
        await jobService.UpdateJobAsync(job10);
        await jobService.PublishJobAsync(job10.Id);

        var job11 = await jobService.CreateJobAsync(
            "AI Research Scientist",
            "Conduct cutting-edge AI research at Microsoft Research. Publish papers and develop AI technologies for Microsoft products.",
            emp3.Id, microsoft.Id);
        job11.Requirements = "PhD in CS/ML/AI, published research, deep learning, NLP or computer vision";
        job11.SalaryMin = 190000; job11.SalaryMax = 280000;
        job11.EmploymentType = "Full-time"; job11.Category = "Research";
        await jobService.UpdateJobAsync(job11);
        await jobService.PublishJobAsync(job11.Id);

        var job12 = await jobService.CreateJobAsync(
            "Full Stack Developer",
            "Build modern web applications for Microsoft's cloud services. Work with React, Node.js, and Azure.",
            emp3.Id, microsoft.Id);
        job12.Requirements = "4+ years full-stack development, React, Node.js, SQL, REST APIs";
        job12.SalaryMin = 140000; job12.SalaryMax = 190000;
        job12.EmploymentType = "Full-time"; job12.Category = "Web Development";
        await jobService.UpdateJobAsync(job12);
        await jobService.PublishJobAsync(job12.Id);

        // Amazon Jobs (4)
        var job13 = await jobService.CreateJobAsync(
            "Software Development Engineer",
            "Build and scale Amazon's e-commerce platform. Work on systems handling millions of transactions daily.",
            emp4.Id, amazon.Id);
        job13.Requirements = "CS degree, 3+ years Java/Python/C++, algorithms, data structures, AWS";
        job13.SalaryMin = 150000; job13.SalaryMax = 220000;
        job13.EmploymentType = "Full-time"; job13.Category = "Software Development";
        await jobService.UpdateJobAsync(job13);
        await jobService.PublishJobAsync(job13.Id);

        var job14 = await jobService.CreateJobAsync(
            "Data Scientist",
            "Drive data-driven decisions across Amazon. Build ML models for recommendations, pricing, and forecasting.",
            emp4.Id, amazon.Id);
        job14.Requirements = "MS/PhD, Python, R, SQL, machine learning, statistics, 3+ years experience";
        job14.SalaryMin = 160000; job14.SalaryMax = 230000;
        job14.EmploymentType = "Full-time"; job14.Category = "Data Science";
        await jobService.UpdateJobAsync(job14);
        await jobService.PublishJobAsync(job14.Id);

        var job15 = await jobService.CreateJobAsync(
            "DevOps Engineer",
            "Build and maintain infrastructure for Amazon's services. Automate deployments and ensure high availability.",
            emp4.Id, amazon.Id);
        job15.Requirements = "AWS, Docker, Kubernetes, CI/CD, Python/Bash, 4+ years DevOps";
        job15.SalaryMin = 145000; job15.SalaryMax = 200000;
        job15.EmploymentType = "Full-time"; job15.Category = "DevOps";
        await jobService.UpdateJobAsync(job15);
        await jobService.PublishJobAsync(job15.Id);

        var job16 = await jobService.CreateJobAsync(
            "Solutions Architect",
            "Help enterprise customers design and implement solutions on AWS. Be a trusted technical advisor.",
            emp4.Id, amazon.Id);
        job16.Requirements = "AWS certifications, 5+ years solution architecture, customer-facing experience";
        job16.SalaryMin = 170000; job16.SalaryMax = 240000;
        job16.EmploymentType = "Full-time"; job16.Category = "Solutions Architecture";
        await jobService.UpdateJobAsync(job16);
        await jobService.PublishJobAsync(job16.Id);

        // Meta Jobs (3)
        var job17 = await jobService.CreateJobAsync(
            "Software Engineer, Mobile",
            "Build mobile experiences for billions of users on Facebook, Instagram, and WhatsApp. Work with React Native and native platforms.",
            emp5.Id, meta.Id);
        job17.Requirements = "3+ years mobile development, iOS/Android, React Native, strong CS fundamentals";
        job17.SalaryMin = 170000; job17.SalaryMax = 250000;
        job17.EmploymentType = "Full-time"; job17.Category = "Mobile Development";
        await jobService.UpdateJobAsync(job17);
        await jobService.PublishJobAsync(job17.Id);

        var job18 = await jobService.CreateJobAsync(
            "Data Engineer",
            "Build and maintain data infrastructure supporting Meta's products. Process petabytes of data daily.",
            emp5.Id, meta.Id);
        job18.Requirements = "4+ years data engineering, Spark, Hadoop, SQL, Python, distributed systems";
        job18.SalaryMin = 160000; job18.SalaryMax = 230000;
        job18.EmploymentType = "Full-time"; job18.Category = "Data Engineering";
        await jobService.UpdateJobAsync(job18);
        await jobService.PublishJobAsync(job18.Id);

        var job19 = await jobService.CreateJobAsync(
            "VR/AR Engineer",
            "Shape the future of the metaverse. Build immersive VR and AR experiences for Meta Quest and future products.",
            emp5.Id, meta.Id);
        job19.Requirements = "Unity/Unreal Engine, C++, computer graphics, 3+ years VR/AR development";
        job19.SalaryMin = 175000; job19.SalaryMax = 255000;
        job19.EmploymentType = "Full-time"; job19.Category = "VR/AR Development";
        await jobService.UpdateJobAsync(job19);
        await jobService.PublishJobAsync(job19.Id);

        // Netflix Jobs (3)
        var job20 = await jobService.CreateJobAsync(
            "Senior Backend Engineer",
            "Build microservices powering Netflix's streaming platform. Handle massive scale serving 230M+ members globally.",
            emp6.Id, netflix.Id);
        job20.Requirements = "5+ years backend, Java/Kotlin/Go, microservices, AWS, distributed systems";
        job20.SalaryMin = 180000; job20.SalaryMax = 260000;
        job20.EmploymentType = "Full-time"; job20.Category = "Backend Development";
        await jobService.UpdateJobAsync(job20);
        await jobService.PublishJobAsync(job20.Id);

        var job21 = await jobService.CreateJobAsync(
            "Content Recommendation Engineer",
            "Build ML models that recommend content to Netflix members. Drive engagement and member satisfaction.",
            emp6.Id, netflix.Id);
        job21.Requirements = "ML/AI experience, Python, TensorFlow, recommendation systems, 4+ years";
        job21.SalaryMin = 175000; job21.SalaryMax = 250000;
        job21.EmploymentType = "Full-time"; job21.Category = "Machine Learning";
        await jobService.UpdateJobAsync(job21);
        await jobService.PublishJobAsync(job21.Id);

        var job22 = await jobService.CreateJobAsync(
            "UI Engineer",
            "Create stunning UI experiences for Netflix on all devices. Work with React and modern web technologies.",
            emp6.Id, netflix.Id);
        job22.Requirements = "4+ years frontend, React, TypeScript, CSS, responsive design, performance optimization";
        job22.SalaryMin = 155000; job22.SalaryMax = 220000;
        job22.EmploymentType = "Full-time"; job22.Category = "Frontend Development";
        await jobService.UpdateJobAsync(job22);
        await jobService.PublishJobAsync(job22.Id);

        // Airbnb Jobs (3)
        var job23 = await jobService.CreateJobAsync(
            "Full Stack Engineer",
            "Build features for Airbnb's platform connecting hosts and guests worldwide. Work across the entire stack.",
            emp7.Id, airbnb.Id);
        job23.Requirements = "3+ years full-stack, React, Node.js, Ruby on Rails, PostgreSQL, AWS";
        job23.SalaryMin = 160000; job23.SalaryMax = 230000;
        job23.EmploymentType = "Full-time"; job23.Category = "Full Stack Development";
        await jobService.UpdateJobAsync(job23);
        await jobService.PublishJobAsync(job23.Id);

        var job24 = await jobService.CreateJobAsync(
            "Product Designer",
            "Design delightful experiences for Airbnb's community. Create user-centered designs that solve real problems.",
            emp7.Id, airbnb.Id);
        job24.Requirements = "4+ years product design, Figma, user research, prototyping, design systems";
        job24.SalaryMin = 150000; job24.SalaryMax = 210000;
        job24.EmploymentType = "Full-time"; job24.Category = "Product Design";
        await jobService.UpdateJobAsync(job24);
        await jobService.PublishJobAsync(job24.Id);

        var job25 = await jobService.CreateJobAsync(
            "Analytics Engineer",
            "Build data infrastructure and analytics tools. Empower teams with data-driven insights.",
            emp7.Id, airbnb.Id);
        job25.Requirements = "3+ years analytics engineering, SQL, Python, dbt, data modeling, Airflow";
        job25.SalaryMin = 145000; job25.SalaryMax = 200000;
        job25.EmploymentType = "Full-time"; job25.Category = "Analytics";
        await jobService.UpdateJobAsync(job25);
        await jobService.PublishJobAsync(job25.Id);

        // Tesla Jobs (4)
        var job26 = await jobService.CreateJobAsync(
            "Embedded Software Engineer",
            "Develop embedded software for Tesla vehicles. Work on Autopilot, battery management, and vehicle systems.",
            emp8.Id, tesla.Id);
        job26.Requirements = "C/C++, embedded Linux, real-time systems, CAN bus, 4+ years embedded development";
        job26.SalaryMin = 140000; job26.SalaryMax = 210000;
        job26.EmploymentType = "Full-time"; job26.Category = "Embedded Systems";
        await jobService.UpdateJobAsync(job26);
        await jobService.PublishJobAsync(job26.Id);

        var job27 = await jobService.CreateJobAsync(
            "Computer Vision Engineer",
            "Build perception systems for Tesla's Autopilot. Work with camera and sensor data for autonomous driving.",
            emp8.Id, tesla.Id);
        job27.Requirements = "Deep learning, computer vision, Python, C++, CUDA, autonomous systems";
        job27.SalaryMin = 165000; job27.SalaryMax = 240000;
        job27.EmploymentType = "Full-time"; job27.Category = "Computer Vision";
        await jobService.UpdateJobAsync(job27);
        await jobService.PublishJobAsync(job27.Id);

        var job28 = await jobService.CreateJobAsync(
            "Manufacturing Engineer",
            "Optimize production processes for Tesla vehicles. Drive manufacturing efficiency and quality.",
            emp8.Id, tesla.Id);
        job28.Requirements = "Mechanical/Industrial Engineering degree, lean manufacturing, 3+ years experience";
        job28.SalaryMin = 110000; job28.SalaryMax = 160000;
        job28.EmploymentType = "Full-time"; job28.Category = "Manufacturing";
        await jobService.UpdateJobAsync(job28);
        await jobService.PublishJobAsync(job28.Id);

        var job29 = await jobService.CreateJobAsync(
            "Battery Engineer",
            "Design and develop next-generation battery systems. Push the boundaries of energy storage technology.",
            emp8.Id, tesla.Id);
        job29.Requirements = "MS/PhD in EE/Materials, battery systems, electrochemistry, 4+ years experience";
        job29.SalaryMin = 130000; job29.SalaryMax = 190000;
        job29.EmploymentType = "Full-time"; job29.Category = "Electrical Engineering";
        await jobService.UpdateJobAsync(job29);
        await jobService.PublishJobAsync(job29.Id);

        // Goldman Sachs Jobs (3)
        var job30 = await jobService.CreateJobAsync(
            "Quantitative Developer",
            "Build trading systems and risk management platforms. Work with cutting-edge financial technology.",
            emp9.Id, goldman.Id);
        job30.Requirements = "CS/Math degree, C++/Java, financial markets knowledge, algorithms, 3+ years";
        job30.SalaryMin = 170000; job30.SalaryMax = 250000;
        job30.EmploymentType = "Full-time"; job30.Category = "Quantitative Development";
        await jobService.UpdateJobAsync(job30);
        await jobService.PublishJobAsync(job30.Id);

        var job31 = await jobService.CreateJobAsync(
            "Risk Analyst",
            "Analyze and manage financial risk across Goldman Sachs' portfolios. Build risk models and monitoring systems.",
            emp9.Id, goldman.Id);
        job31.Requirements = "Finance/Math degree, VaR modeling, Python/R, SQL, financial risk, 2+ years";
        job31.SalaryMin = 120000; job31.SalaryMax = 180000;
        job31.EmploymentType = "Full-time"; job31.Category = "Risk Management";
        await jobService.UpdateJobAsync(job31);
        await jobService.PublishJobAsync(job31.Id);

        var job32 = await jobService.CreateJobAsync(
            "Investment Banking Analyst",
            "Work on M&A transactions, IPOs, and strategic advisory. Gain exposure to high-profile deals.",
            emp9.Id, goldman.Id);
        job32.Requirements = "Top university degree, finance knowledge, Excel/PowerPoint mastery, analytical skills";
        job32.SalaryMin = 110000; job32.SalaryMax = 150000;
        job32.EmploymentType = "Full-time"; job32.Category = "Investment Banking";
        await jobService.UpdateJobAsync(job32);
        await jobService.PublishJobAsync(job32.Id);

        // JPMorgan Jobs (3)
        var job33 = await jobService.CreateJobAsync(
            "Full Stack Java Developer",
            "Build enterprise banking applications serving millions of customers. Work with Java, Spring, and React.",
            emp10.Id, jpmorgan.Id);
        job33.Requirements = "4+ years Java development, Spring Boot, React, microservices, SQL, cloud";
        job33.SalaryMin = 130000; job33.SalaryMax = 190000;
        job33.EmploymentType = "Full-time"; job33.Category = "Software Development";
        await jobService.UpdateJobAsync(job33);
        await jobService.PublishJobAsync(job33.Id);

        var job34 = await jobService.CreateJobAsync(
            "Cybersecurity Engineer",
            "Protect JPMorgan's systems and customer data. Implement security controls and respond to threats.",
            emp10.Id, jpmorgan.Id);
        job34.Requirements = "Cybersecurity certifications, penetration testing, SIEM, incident response, 3+ years";
        job34.SalaryMin = 125000; job34.SalaryMax = 185000;
        job34.EmploymentType = "Full-time"; job34.Category = "Cybersecurity";
        await jobService.UpdateJobAsync(job34);
        await jobService.PublishJobAsync(job34.Id);

        var job35 = await jobService.CreateJobAsync(
            "Business Analyst",
            "Bridge technology and business. Gather requirements and drive digital transformation initiatives.",
            emp10.Id, jpmorgan.Id);
        job35.Requirements = "Business/Tech background, requirements gathering, SQL, Agile, 2+ years BA experience";
        job35.SalaryMin = 95000; job35.SalaryMax = 140000;
        job35.EmploymentType = "Full-time"; job35.Category = "Business Analysis";
        await jobService.UpdateJobAsync(job35);
        await jobService.PublishJobAsync(job35.Id);

        // Salesforce Jobs (3)
        var job36 = await jobService.CreateJobAsync(
            "Salesforce Developer",
            "Build custom solutions on the Salesforce platform. Work with Apex, Lightning, and integrations.",
            emp11.Id, salesforce.Id);
        job36.Requirements = "Salesforce certifications, Apex, Lightning Web Components, 3+ years Salesforce dev";
        job36.SalaryMin = 120000; job36.SalaryMax = 170000;
        job36.EmploymentType = "Full-time"; job36.Category = "Salesforce Development";
        await jobService.UpdateJobAsync(job36);
        await jobService.PublishJobAsync(job36.Id);

        var job37 = await jobService.CreateJobAsync(
            "Technical Architect",
            "Design enterprise Salesforce solutions. Lead technical implementations for Fortune 500 customers.",
            emp11.Id, salesforce.Id);
        job37.Requirements = "Multiple Salesforce certifications, solution architecture, 6+ years experience";
        job37.SalaryMin = 160000; job37.SalaryMax = 230000;
        job37.EmploymentType = "Full-time"; job37.Category = "Solution Architecture";
        await jobService.UpdateJobAsync(job37);
        await jobService.PublishJobAsync(job37.Id);

        var job38 = await jobService.CreateJobAsync(
            "Customer Success Manager",
            "Help customers succeed with Salesforce. Drive adoption and customer satisfaction.",
            emp11.Id, salesforce.Id);
        job38.Requirements = "Salesforce knowledge, customer-facing experience, excellent communication, 2+ years CSM";
        job38.SalaryMin = 90000; job38.SalaryMax = 130000;
        job38.EmploymentType = "Full-time"; job38.Category = "Customer Success";
        await jobService.UpdateJobAsync(job38);
        await jobService.PublishJobAsync(job38.Id);

        // Adobe Jobs (3)
        var job39 = await jobService.CreateJobAsync(
            "Senior Software Engineer - Creative Cloud",
            "Build features for Adobe Creative Cloud products. Work on tools used by millions of creatives worldwide.",
            emp12.Id, adobe.Id);
        job39.Requirements = "5+ years software development, C++, JavaScript, graphics programming, algorithms";
        job39.SalaryMin = 165000; job39.SalaryMax = 230000;
        job39.EmploymentType = "Full-time"; job39.Category = "Software Development";
        await jobService.UpdateJobAsync(job39);
        await jobService.PublishJobAsync(job39.Id);

        var job40 = await jobService.CreateJobAsync(
            "Graphics Engineer",
            "Develop rendering and graphics technology for Adobe products. Push the boundaries of digital creativity.",
            emp12.Id, adobe.Id);
        job40.Requirements = "Computer graphics, OpenGL/DirectX, C++, shaders, image processing, 4+ years";
        job40.SalaryMin = 155000; job40.SalaryMax = 220000;
        job40.EmploymentType = "Full-time"; job40.Category = "Graphics Programming";
        await jobService.UpdateJobAsync(job40);
        await jobService.PublishJobAsync(job40.Id);

        var job41 = await jobService.CreateJobAsync(
            "UX Designer - Adobe XD",
            "Design intuitive UX for Adobe XD. Create tools that empower designers to do their best work.",
            emp12.Id, adobe.Id);
        job41.Requirements = "4+ years UX design, Adobe XD/Figma, prototyping, user research, design systems";
        job41.SalaryMin = 130000; job41.SalaryMax = 180000;
        job41.EmploymentType = "Full-time"; job41.Category = "UX Design";
        await jobService.UpdateJobAsync(job41);
        await jobService.PublishJobAsync(job41.Id);

        // Uber Jobs (3)
        var job42 = await jobService.CreateJobAsync(
            "Backend Engineer - Maps",
            "Build Uber's mapping and routing systems. Handle billions of location data points daily.",
            emp13.Id, uber.Id);
        job42.Requirements = "3+ years backend, Go/Java/Python, distributed systems, geospatial algorithms, databases";
        job42.SalaryMin = 155000; job42.SalaryMax = 220000;
        job42.EmploymentType = "Full-time"; job42.Category = "Backend Development";
        await jobService.UpdateJobAsync(job42);
        await jobService.PublishJobAsync(job42.Id);

        var job43 = await jobService.CreateJobAsync(
            "iOS Engineer - Rider App",
            "Build and improve Uber's rider mobile app. Create seamless experiences for millions of riders.",
            emp13.Id, uber.Id);
        job43.Requirements = "3+ years iOS, Swift, UIKit/SwiftUI, mobile architecture, testing, CI/CD";
        job43.SalaryMin = 150000; job43.SalaryMax = 210000;
        job43.EmploymentType = "Full-time"; job43.Category = "Mobile Development";
        await jobService.UpdateJobAsync(job43);
        await jobService.PublishJobAsync(job43.Id);

        var job44 = await jobService.CreateJobAsync(
            "Data Scientist - Pricing",
            "Build ML models for dynamic pricing. Optimize marketplace efficiency and driver-rider matching.",
            emp13.Id, uber.Id);
        job44.Requirements = "MS/PhD, Python, machine learning, optimization, causal inference, 3+ years";
        job44.SalaryMin = 160000; job44.SalaryMax = 230000;
        job44.EmploymentType = "Full-time"; job44.Category = "Data Science";
        await jobService.UpdateJobAsync(job44);
        await jobService.PublishJobAsync(job44.Id);

        // Stripe Jobs (4)
        var job45 = await jobService.CreateJobAsync(
            "Software Engineer - Payments",
            "Build payment infrastructure powering millions of businesses. Work on mission-critical financial systems.",
            emp14.Id, stripe.Id);
        job45.Requirements = "3+ years backend development, Ruby/Java/Go, distributed systems, databases, APIs";
        job45.SalaryMin = 170000; job45.SalaryMax = 250000;
        job45.EmploymentType = "Full-time"; job45.Category = "Backend Development";
        await jobService.UpdateJobAsync(job45);
        await jobService.PublishJobAsync(job45.Id);

        var job46 = await jobService.CreateJobAsync(
            "Security Engineer",
            "Secure Stripe's payment systems. Build security infrastructure protecting billions in transactions.",
            emp14.Id, stripe.Id);
        job46.Requirements = "Security engineering, cryptography, Ruby/Go, threat modeling, 4+ years";
        job46.SalaryMin = 175000; job46.SalaryMax = 255000;
        job46.EmploymentType = "Full-time"; job46.Category = "Security Engineering";
        await jobService.UpdateJobAsync(job46);
        await jobService.PublishJobAsync(job46.Id);

        var job47 = await jobService.CreateJobAsync(
            "Product Manager - Developer Tools",
            "Build products that developers love. Shape Stripe's developer experience and API design.",
            emp14.Id, stripe.Id);
        job47.Requirements = "Technical background, 3+ years PM, developer tools, APIs, strong communication";
        job47.SalaryMin = 165000; job47.SalaryMax = 240000;
        job47.EmploymentType = "Full-time"; job47.Category = "Product Management";
        await jobService.UpdateJobAsync(job47);
        await jobService.PublishJobAsync(job47.Id);

        var job48 = await jobService.CreateJobAsync(
            "Frontend Engineer",
            "Build Stripe's dashboard and developer tools. Create beautiful, fast web applications.",
            emp14.Id, stripe.Id);
        job48.Requirements = "4+ years frontend, React, TypeScript, performance, accessibility, design systems";
        job48.SalaryMin = 160000; job48.SalaryMax = 230000;
        job48.EmploymentType = "Full-time"; job48.Category = "Frontend Development";
        await jobService.UpdateJobAsync(job48);
        await jobService.PublishJobAsync(job48.Id);

        // Shopify Jobs (4)
        var job49 = await jobService.CreateJobAsync(
            "Full Stack Developer - Commerce",
            "Build e-commerce features powering millions of merchants. Work with Ruby on Rails and React.",
            emp15.Id, shopify.Id);
        job49.Requirements = "3+ years full-stack, Ruby on Rails, React, GraphQL, PostgreSQL, Redis";
        job49.SalaryMin = 130000; job49.SalaryMax = 190000;
        job49.EmploymentType = "Full-time"; job49.Category = "Full Stack Development";
        await jobService.UpdateJobAsync(job49);
        await jobService.PublishJobAsync(job49.Id);

        var job50 = await jobService.CreateJobAsync(
            "Mobile Engineer - Shop App",
            "Build Shopify's consumer shopping app. Create delightful mobile shopping experiences.",
            emp15.Id, shopify.Id);
        job50.Requirements = "3+ years mobile, iOS/Android or React Native, mobile architecture, performance";
        job50.SalaryMin = 140000; job50.SalaryMax = 200000;
        job50.EmploymentType = "Full-time"; job50.Category = "Mobile Development";
        await jobService.UpdateJobAsync(job50);
        await jobService.PublishJobAsync(job50.Id);

        var job51 = await jobService.CreateJobAsync(
            "Senior Product Designer",
            "Design merchant and consumer experiences for Shopify's platform. Impact millions of users.",
            emp15.Id, shopify.Id);
        job51.Requirements = "5+ years product design, e-commerce experience, Figma, user research, systems thinking";
        job51.SalaryMin = 135000; job51.SalaryMax = 190000;
        job51.EmploymentType = "Full-time"; job51.Category = "Product Design";
        await jobService.UpdateJobAsync(job51);
        await jobService.PublishJobAsync(job51.Id);

        var job52 = await jobService.CreateJobAsync(
            "Infrastructure Engineer",
            "Build and scale Shopify's infrastructure. Handle traffic for millions of online stores.",
            emp15.Id, shopify.Id);
        job52.Requirements = "Kubernetes, Docker, Ruby, Go, cloud infrastructure, monitoring, 4+ years";
        job52.SalaryMin = 145000; job52.SalaryMax = 210000;
        job52.EmploymentType = "Full-time"; job52.Category = "Infrastructure";
        await jobService.UpdateJobAsync(job52);
        await jobService.PublishJobAsync(job52.Id);

        Console.WriteLine($"✓ Created {52} job postings");

        // ============================================
        // CREATE SOME APPLICATIONS
        // ============================================

        await applicationService.SubmitApplicationAsync(jobSeeker1.Id, job1.Id, "I'm very excited about this iOS Engineer position at Apple. With my 5 years of Swift development experience and passion for creating intuitive user experiences, I believe I would be a great addition to your team.");
        await applicationService.SubmitApplicationAsync(jobSeeker2.Id, job5.Id, "I'm interested in joining Google as a Senior Software Engineer. My background in distributed systems and experience with large-scale applications aligns perfectly with this role.");
        await applicationService.SubmitApplicationAsync(jobSeeker3.Id, job9.Id, "I'm excited to apply for the Cloud Solution Architect role at Microsoft. My Azure certifications and 6 years of cloud architecture experience make me an ideal candidate.");
        await applicationService.SubmitApplicationAsync(jobSeeker4.Id, job13.Id, "I would love to join Amazon as a Software Development Engineer. My strong CS fundamentals and AWS experience would allow me to contribute immediately.");
        await applicationService.SubmitApplicationAsync(jobSeeker5.Id, job17.Id, "I'm applying for the Mobile Engineer position at Meta. My React Native expertise and experience building apps for millions of users would be valuable to your team.");
        await applicationService.SubmitApplicationAsync(jobSeeker6.Id, job20.Id, "I'm very interested in the Senior Backend Engineer role at Netflix. My microservices architecture experience and passion for scalable systems align perfectly.");
        await applicationService.SubmitApplicationAsync(jobSeeker7.Id, job23.Id, "I would love to join Airbnb as a Full Stack Engineer. My experience with React and Ruby on Rails matches your tech stack perfectly.");
        await applicationService.SubmitApplicationAsync(jobSeeker8.Id, job26.Id, "I'm excited about the Embedded Software Engineer position at Tesla. My background in automotive software and real-time systems would be a great fit.");
        await applicationService.SubmitApplicationAsync(jobSeeker9.Id, job30.Id, "I'm applying for the Quantitative Developer role at Goldman Sachs. My strong math background and C++ expertise align with your requirements.");
        await applicationService.SubmitApplicationAsync(jobSeeker10.Id, job33.Id, "I'm interested in the Full Stack Java Developer position at JPMorgan. My enterprise Java experience and financial domain knowledge would be valuable.");
        await applicationService.SubmitApplicationAsync(jobSeeker11.Id, job36.Id, "I would love to join Salesforce as a Salesforce Developer. My multiple certifications and 4 years of platform experience make me a strong candidate.");
        await applicationService.SubmitApplicationAsync(jobSeeker12.Id, job39.Id, "I'm excited about the Senior Software Engineer role at Adobe. My graphics programming background and passion for creative tools align perfectly.");
        await applicationService.SubmitApplicationAsync(jobSeeker13.Id, job42.Id, "I'm applying for the Backend Engineer position at Uber. My experience with geospatial algorithms and distributed systems would be highly valuable.");
        await applicationService.SubmitApplicationAsync(jobSeeker14.Id, job45.Id, "I would love to join Stripe as a Software Engineer. Building payment infrastructure at scale is exactly what I'm passionate about.");
        await applicationService.SubmitApplicationAsync(jobSeeker15.Id, job49.Id, "I'm interested in the Full Stack Developer role at Shopify. My Ruby on Rails and React experience matches your stack perfectly.");

        Console.WriteLine($"✓ Created {15} job applications");

        Console.WriteLine("\n✅ Database seeded successfully!");
        Console.WriteLine($"📊 Summary: {15} companies, {20} job seekers, {16} employers, {52} jobs, {15} applications\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}