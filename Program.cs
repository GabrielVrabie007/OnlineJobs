using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Services;
using OnlineJobs.Domain.Entities;
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


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<CompanyService>();

var app = builder.Build();

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

        Console.WriteLine("✓ Initial data seeded successfully");
        Console.WriteLine($"✓ Created {2} companies");
        Console.WriteLine($"✓ Created {4} users (2 job seekers, 2 employers)");
        Console.WriteLine($"✓ Created {3} active job postings");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}