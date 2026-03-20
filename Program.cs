using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Services;
using OnlineJobs.Application.Builders;
using OnlineJobs.Application.Configuration;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.ValueObjects;
using OnlineJobs.Infrastructure.Repositories;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();
builder.Services.AddSingleton<IRepository<JobSeeker>, InMemoryRepository<JobSeeker>>();
builder.Services.AddSingleton<IRepository<Employer>, InMemoryRepository<Employer>>();
builder.Services.AddSingleton<IRepository<Company>, InMemoryRepository<Company>>();
builder.Services.AddSingleton<IRepository<JobPosting>, InMemoryRepository<JobPosting>>();
builder.Services.AddSingleton<IRepository<JobApplication>, InMemoryRepository<JobApplication>>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IDocumentGenerationService, DocumentGenerationService>(); // Abstract Factory Pattern

// Builder Pattern - Lab 3
builder.Services.AddScoped<IJobSeekerProfileBuilder, JobSeekerProfileBuilder>();
builder.Services.AddScoped<JobSeekerProfileDirector>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<CompanyService>();

var app = builder.Build();

// Initialize and configure the Singleton Configuration Manager (Lab 3)
var config = ApplicationConfiguration.Instance;
Console.WriteLine($"✓ {config}");
Console.WriteLine($"  - Job Expiry: {config.JobExpiryDays} days");
Console.WriteLine($"  - Max Applications per User: {config.MaxActiveApplicationsPerUser}");
Console.WriteLine($"  - Max Job Postings per Employer: {config.MaxActiveJobPostingsPerEmployer}");

await SeedDataAsync(app.Services);

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

async Task SeedDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

    var companyService = scope.ServiceProvider.GetRequiredService<CompanyService>();
    var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();

    try
    {
        var techCorp = await companyService.CreateCompanyAsync(
            "TechCorp Solutions",
            "San Francisco, CA"
        );
        techCorp.Description = "Leading technology solutions provider";
        techCorp.Website = "https://techcorp.example.com";
        techCorp.Industry = "Technology";
        await companyService.UpdateCompanyAsync(techCorp);

        var innovateInc = await companyService.CreateCompanyAsync(
            "Innovate Inc",
            "New York, NY"
        );
        innovateInc.Description = "Innovation-driven software company";
        innovateInc.Industry = "Software Development";
        await companyService.UpdateCompanyAsync(innovateInc);

        // Create job seekers with basic registration (legacy method)
        var jobSeeker1 = await userService.RegisterJobSeekerAsync(
            "john.doe@example.com",
            "John",
            "Doe",
            "password123"
        );

        var jobSeeker2 = await userService.RegisterJobSeekerAsync(
            "jane.smith@example.com",
            "Jane",
            "Smith",
            "password123"
        );

        // Demonstrate Builder Pattern (Lab 3) - Create a complete profile for jobSeeker1
        var builder = new JobSeekerProfileBuilder();
        var director = new JobSeekerProfileDirector(builder);

        var seniorDeveloper = director.ConstructSeniorTechnicalProfile(
            "sarah.wilson@example.com",
            "Sarah",
            "Wilson",
            "+1-555-0123",
            "Experienced full-stack developer with 8+ years of expertise in building scalable web applications. Passionate about clean code, system design, and mentoring junior developers.",
            new List<WorkExperience>
            {
                new WorkExperience(
                    "Google",
                    "Senior Software Engineer",
                    "Mountain View, CA",
                    new DateTime(2020, 1, 1),
                    null,
                    "Leading development of cloud-based solutions",
                    new List<string> { "Design and implement microservices", "Mentor junior developers", "Code reviews" },
                    new List<string> { "Reduced API latency by 40%", "Led team of 5 engineers" }
                ),
                new WorkExperience(
                    "Microsoft",
                    "Software Engineer",
                    "Seattle, WA",
                    new DateTime(2016, 6, 1),
                    new DateTime(2019, 12, 31),
                    "Developed enterprise applications",
                    new List<string> { "Backend development", "Database optimization", "CI/CD implementation" },
                    new List<string> { "Improved deployment efficiency by 60%" }
                )
            },
            new List<Skill>
            {
                new Skill("C#", SkillProficiency.Expert, 8, "Backend"),
                new Skill("ASP.NET Core", SkillProficiency.Expert, 6, "Backend"),
                new Skill("React", SkillProficiency.Advanced, 5, "Frontend"),
                new Skill("SQL Server", SkillProficiency.Advanced, 7, "Database"),
                new Skill("Azure", SkillProficiency.Advanced, 4, "Cloud"),
                new Skill("Docker", SkillProficiency.Intermediate, 3, "DevOps")
            },
            "https://github.com/sarahwilson",
            "https://sarahwilson.dev"
        );

        // Register the complete profile (this would normally be done through a service)
        var jobSeekerRepo = scope.ServiceProvider.GetRequiredService<IRepository<JobSeeker>>();
        await jobSeekerRepo.AddAsync(seniorDeveloper);

        Console.WriteLine($"✓ Created detailed profile for {seniorDeveloper.GetFullName()} using Builder Pattern");
        Console.WriteLine($"  - Work Experience: {seniorDeveloper.WorkHistory.Count} positions");
        Console.WriteLine($"  - Skills: {seniorDeveloper.SkillSet.Count} skills");
        Console.WriteLine($"  - Total Experience: {seniorDeveloper.GetTotalYearsOfExperience()} years");

        var employer1 = await userService.RegisterEmployerAsync(
            "hr@techcorp.example.com",
            "Alice",
            "Johnson",
            "password123",
            techCorp.Id
        );

        var employer2 = await userService.RegisterEmployerAsync(
            "recruiter@innovate.example.com",
            "Bob",
            "Williams",
            "password123",
            innovateInc.Id
        );

        var job1 = await jobService.CreateJobAsync(
            "Senior Software Engineer",
            "We are looking for an experienced software engineer to join our team. You will work on cutting-edge technologies and solve challenging problems.",
            employer1.Id,
            techCorp.Id
        );
        job1.Requirements = "5+ years experience in C#, ASP.NET Core, SQL Server";
        job1.SalaryMin = 100000;
        job1.SalaryMax = 150000;
        job1.EmploymentType = "Full-time";
        job1.Category = "Software Development";
        await jobService.UpdateJobAsync(job1);
        await jobService.PublishJobAsync(job1.Id);

        var job2 = await jobService.CreateJobAsync(
            "Frontend Developer",
            "Join our dynamic team as a frontend developer. Work with React, TypeScript, and modern web technologies.",
            employer1.Id,
            techCorp.Id
        );
        job2.Requirements = "3+ years experience with React, TypeScript, HTML/CSS";
        job2.SalaryMin = 80000;
        job2.SalaryMax = 120000;
        job2.EmploymentType = "Full-time";
        job2.Category = "Frontend Development";
        await jobService.UpdateJobAsync(job2);
        await jobService.PublishJobAsync(job2.Id);

        var job3 = await jobService.CreateJobAsync(
            "DevOps Engineer",
            "We need a skilled DevOps engineer to manage our cloud infrastructure and CI/CD pipelines.",
            employer2.Id,
            innovateInc.Id
        );
        job3.Requirements = "Experience with AWS, Docker, Kubernetes, Jenkins";
        job3.SalaryMin = 90000;
        job3.SalaryMax = 140000;
        job3.EmploymentType = "Full-time";
        job3.Category = "DevOps";
        await jobService.UpdateJobAsync(job3);
        await jobService.PublishJobAsync(job3.Id);

        // Demonstrate Prototype Pattern (Lab 3) - Clone a job posting
        var clonedJob = job1.Clone();
        clonedJob.Title = "Lead Software Engineer";
        clonedJob.Description = "We are looking for a lead software engineer to drive technical initiatives. Build on our existing stack and lead the team.";
        clonedJob.SalaryMin = 120000;
        clonedJob.SalaryMax = 180000;
        await jobService.UpdateJobAsync(clonedJob);
        await jobService.PublishJobAsync(clonedJob.Id);

        Console.WriteLine($"✓ Cloned job posting using Prototype Pattern: '{clonedJob.Title}'");

        // Demonstrate Prototype Pattern - Clone a company
        var techCorpSubsidiary = techCorp.CloneAsSubsidiary("TechCorp Europe", "London, UK");
        techCorpSubsidiary.Description = "European branch of TechCorp Solutions";
        await companyService.UpdateCompanyAsync(techCorpSubsidiary);

        Console.WriteLine($"✓ Cloned company using Prototype Pattern: '{techCorpSubsidiary.Name}' in {techCorpSubsidiary.Location}");

        Console.WriteLine("\n✓ Initial data seeded successfully");
        Console.WriteLine($"✓ Created {3} companies (including 1 cloned subsidiary)");
        Console.WriteLine($"✓ Created {5} users (3 job seekers, 2 employers)");
        Console.WriteLine($"✓ Created {4} active job postings (including 1 cloned)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}