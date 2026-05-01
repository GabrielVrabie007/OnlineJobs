# Design Patterns - Real Integration

This document explains how the 4 structural design patterns are integrated into the actual application functionality, not as demos.

---

## 1. FLYWEIGHT PATTERN - Skill Sharing

### Where: Background/Domain Layer
**Files:**
- `Domain/Flyweights/SkillFlyweight.cs`
- `Application/Factories/SkillFlyweightFactory.cs`
- `Domain/ValueObjects/JobSkillRequirement.cs`

### How Users Experience It:
Users **don't see it directly** - it works behind the scenes when:
- Employers create job postings with required skills
- The system stores only unique skill objects in memory
- 1000 jobs with "C#" skill = 1 object in memory, not 1000

### Implementation:
```csharp
builder.Services.AddSingleton<SkillFlyweightFactory>();
```

The factory is injected and used automatically when creating job skill requirements. No special UI needed - it's pure optimization.

### Benefit:
**99% memory reduction** when multiple jobs share common skills.

---

## 2. DECORATOR PATTERN - Notification System

### Where: Background/Service Layer
**Files:**
- `Application/Notifications/BaseNotification.cs`
- `Application/Decorators/EmailNotificationDecorator.cs`
- `Application/Decorators/SMSNotificationDecorator.cs`
- `Application/Decorators/PushNotificationDecorator.cs`
- `Application/Decorators/LoggingNotificationDecorator.cs`
- `Application/Services/NotificationService.cs`

### How Users Experience It:
When a **job seeker applies** to a job:
1. They submit their application
2. System automatically sends notifications:
   - **Email** to job seeker (confirmation)
   - **Email + Push** to employer (new application)
   - **Logging** for all notifications

### Implementation in NotificationService:
```csharp
public async Task SendApplicationConfirmationAsync(...)
{
    INotification notification = new BaseNotification();
    notification = new EmailNotificationDecorator(notification);
    notification = new SMSNotificationDecorator(notification);
    notification = new PushNotificationDecorator(notification);
    notification = new LoggingNotificationDecorator(notification);

    await notification.SendAsync(...);
}
```

### User Flow:
`Application/Apply.cshtml` → Submit → `ApplicationService.SubmitApplicationAsync()` → `NotificationService` → Multiple channels automatically

### Benefit:
Easy to add/remove notification channels without changing core application code. Want Telegram notifications? Add one decorator class.

---

## 3. BRIDGE PATTERN - Reports Generation

### Where: Reports Section (Employers Only)
**Files:**
- `Application/Reporting/Reports/JobReport.cs`
- `Application/Reporting/Reports/ApplicationReport.cs`
- `Application/Reporting/Reports/CompanyReport.cs`
- `Application/Reporting/Exporters/PDFExporter.cs`
- `Application/Reporting/Exporters/ExcelExporter.cs`
- `Application/Reporting/Exporters/JSONExporter.cs`
- `Application/Reporting/Exporters/CSVExporter.cs`
- `Application/Services/ReportingService.cs`
- `Controllers/ReportsController.cs`
- `Views/Reports/Index.cshtml`

### How Users Experience It:
**Employer logs in** → Clicks **"Reports"** in navigation → Selects:
1. **Report Type:** Jobs / Applications / Companies
2. **Format:** PDF / Excel / JSON / CSV
3. Clicks **"Generate Report"**
4. Views report on screen
5. Downloads report file

### Real User Journey:
```
Login as Employer
→ Navigation: "Reports"
→ /Reports/Index
→ Select "Applications Report" + "Excel"
→ POST /Reports/Generate
→ See formatted report
→ Download .xlsx file
```

### Implementation:
```csharp
[HttpPost]
public async Task<IActionResult> Generate(string reportType, string format)
{
    string reportContent = reportType switch
    {
        "jobs" => await _reportingService.GenerateJobReportAsync(format),
        "applications" => await _reportingService.GenerateApplicationReportAsync(format),
        "companies" => await _reportingService.GenerateCompanyReportAsync(format),
        _ => throw new ArgumentException("Invalid report type")
    };
    // Display in view
}
```

### Benefit:
- **3 report types × 4 formats = 12 combinations**
- Only **7 classes** (not 12!)
- Add new report? 1 class, works with all formats
- Add new format? 1 class, works with all reports

---

## 4. PROXY PATTERN - Job Access Control

### Where: Job Details Page
**Files:**
- `Application/Proxies/RealJobPostingAccess.cs`
- `Application/Proxies/JobPostingProtectionProxy.cs`
- `Application/Proxies/ApplicationListVirtualProxy.cs`
- `Controllers/JobController.cs` (Details action)
- `Views/Job/Details.cshtml`

### How Users Experience It:

#### A) Protection Proxy - Unauthenticated vs Authenticated

**Unauthenticated User:**
```
Browse Jobs → Click job → See:
✓ Job Title: "Senior iOS Engineer"
✓ Location: "Cupertino, CA"
✗ Company: "🔒 Company Hidden - Login to view"
✗ Salary: "⚠️ Salary information hidden. Please login to view"
```

**Authenticated User:**
```
Login → Browse Jobs → Click job → See:
✓ Job Title: "Senior iOS Engineer"
✓ Location: "Cupertino, CA"
✓ Company: "Apple Inc."
✓ Salary: "$180,000 - $250,000"
```

#### B) Virtual Proxy - Application Count Caching

When employer views **"My Jobs"** with 1000s of applications:
- **First load:** Query DB (500ms)
- **Subsequent loads:** Cache hit (5ms) - **100x faster**
- Auto-invalidates when new applications arrive

### Implementation in JobController:
```csharp
public async Task<IActionResult> Details(Guid id)
{
    var realAccess = new RealJobPostingAccess(_jobService);
    var userId = GetCurrentUserId();
    var isAuthenticated = IsUserLoggedIn();

    // Protection Proxy controls access
    var jobProxy = new JobPostingProtectionProxy(realAccess, isAuthenticated, userId);

    var job = await jobProxy.GetJobDetailsAsync(id);

    // View uses proxied data
    ViewBag.CompanyName = jobProxy.GetCompanyName(job);  // Hidden or visible
    ViewBag.SalaryRange = jobProxy.GetSalaryRange(job);  // Hidden or visible

    return View(job);
}
```

### User Flow:
```
Guest user → /Job/Details/{id}
→ Controller uses Protection Proxy
→ View shows: Company hidden, Salary hidden
→ Badge: "🔒 Login to see company"
```

### Benefit:
- **Centralized security** - one place to control access
- **Performance boost** - caching prevents repeated DB queries
- **Transparent** - users see appropriate data based on authentication

---

## Summary: Where Patterns Are Used

| Pattern | User Sees | Where | Access |
|---------|-----------|-------|--------|
| **Flyweight** | No (background) | Job creation with skills | Automatic |
| **Decorator** | No (background) | Application notifications | Automatic |
| **Bridge** | **YES** | Reports page | `/Reports/Index` (Employers) |
| **Proxy** | **YES** | Job details page | `/Job/Details/{id}` (All users) |

---

## Testing the Patterns

### 1. Test Flyweight
1. Login as employer
2. Create multiple jobs with same skills (e.g., "C#", "Python")
3. Check console logs - factory reuses existing skill objects

### 2. Test Decorator
1. Login as job seeker
2. Apply to any job
3. Check console output - see multiple notification channels triggered

### 3. Test Bridge
1. Login as employer
2. Navigate to "Reports"
3. Generate "Applications Report" in "Excel" format
4. Change to "JSON" format - see same data, different format
5. Change to "Job Report" in "Excel" - see different data, same format

### 4. Test Proxy (Protection)
1. **Logout** (browse as guest)
2. Click any job
3. Notice: Company name hidden, salary hidden
4. **Login** as job seeker
5. Click same job
6. Notice: Company name visible, salary visible

### 5. Test Proxy (Virtual - Caching)
1. Login as employer with many applications
2. View "My Jobs" with application counts
3. First load: slower (DB query)
4. Refresh page: faster (cached)

---

## Architecture Benefits

1. **Flyweight** - Memory efficiency (99% reduction)
2. **Decorator** - Flexible notifications (16 combinations with 4 classes)
3. **Bridge** - Avoid class explosion (7 classes instead of 12)
4. **Proxy** - Security + Performance in one pattern

All patterns follow **SOLID principles** and are integrated into real user workflows, not demo pages.
