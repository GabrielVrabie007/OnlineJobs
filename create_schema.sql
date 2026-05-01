-- OnlineJobs Database Schema Creation
-- Generated from InitialCreate migration

-- Create migrations history table
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

-- Insert migration record
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260330183057_InitialCreate', '8.0.0');

-- Create Companies table
CREATE TABLE "Companies" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Companies" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT NULL,
    "Website" TEXT NULL,
    "Location" TEXT NULL,
    "Industry" TEXT NULL,
    "EmployeeCount" INTEGER NULL,
    "CreatedAt" TEXT NOT NULL
);

-- Create JobCategories table
CREATE TABLE "JobCategories" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_JobCategories" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CategoryType" TEXT NOT NULL,
    "ParentId" TEXT NULL,
    CONSTRAINT "FK_JobCategories_JobCategories_ParentId" FOREIGN KEY ("ParentId") REFERENCES "JobCategories" ("Id") ON DELETE CASCADE
);

-- Create PaymentTransactions table
CREATE TABLE "PaymentTransactions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_PaymentTransactions" PRIMARY KEY,
    "UserId" TEXT NOT NULL,
    "Amount" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "Gateway" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    "TransactionDate" TEXT NOT NULL,
    "ExternalTransactionId" TEXT NULL,
    "Description" TEXT NULL,
    "ErrorMessage" TEXT NULL
);

-- Create Users table
CREATE TABLE "Users" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Users" PRIMARY KEY,
    "Email" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "LastLoginAt" TEXT NULL,
    "IsActive" INTEGER NOT NULL,
    "PhoneNumber" TEXT NULL,
    "UserType" INTEGER NOT NULL,
    "Discriminator" TEXT NOT NULL,
    "CompanyId" TEXT NULL,
    "Position" TEXT NULL,
    "Resume" TEXT NULL,
    "Skills" TEXT NULL,
    "Address" TEXT NULL,
    "DateOfBirth" TEXT NULL,
    "ProfessionalSummary" TEXT NULL,
    "LinkedInUrl" TEXT NULL,
    "GitHubUrl" TEXT NULL,
    "PortfolioUrl" TEXT NULL,
    CONSTRAINT "FK_Users_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE RESTRICT
);

-- Create JobPostings table
CREATE TABLE "JobPostings" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_JobPostings" PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Requirements" TEXT NOT NULL,
    "SalaryMin" TEXT NULL,
    "SalaryMax" TEXT NULL,
    "Location" TEXT NOT NULL,
    "EmploymentType" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "EmployerId" TEXT NOT NULL,
    "CompanyId" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "PostedDate" TEXT NOT NULL,
    "ClosedDate" TEXT NULL,
    "ExpiryDate" TEXT NULL,
    "ExperienceLevel" TEXT NULL,
    "IsCompanyRevealed" INTEGER NOT NULL,
    "CategoryId" TEXT NULL,
    CONSTRAINT "FK_JobPostings_Companies_CompanyId" FOREIGN KEY ("CompanyId") REFERENCES "Companies" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostings_JobCategories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "JobCategories" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_JobPostings_Users_EmployerId" FOREIGN KEY ("EmployerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Create CompanyReveals table
CREATE TABLE "CompanyReveals" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_CompanyReveals" PRIMARY KEY,
    "JobSeekerId" TEXT NOT NULL,
    "JobPostingId" TEXT NOT NULL,
    "PaymentTransactionId" TEXT NOT NULL,
    "RevealedDate" TEXT NOT NULL,
    CONSTRAINT "FK_CompanyReveals_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyReveals_PaymentTransactions_PaymentTransactionId" FOREIGN KEY ("PaymentTransactionId") REFERENCES "PaymentTransactions" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CompanyReveals_Users_JobSeekerId" FOREIGN KEY ("JobSeekerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Create JobApplications table
CREATE TABLE "JobApplications" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_JobApplications" PRIMARY KEY,
    "JobPostingId" TEXT NOT NULL,
    "JobSeekerId" TEXT NOT NULL,
    "CoverLetter" TEXT NOT NULL,
    "ResumeUrl" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "AppliedDate" TEXT NOT NULL,
    "ReviewedDate" TEXT NULL,
    "ReviewNotes" TEXT NULL,
    "ExpectedSalary" TEXT NULL,
    "PortfolioLink" TEXT NULL,
    "AvailableStartDate" TEXT NULL,
    "AdditionalInfo" TEXT NULL,
    CONSTRAINT "FK_JobApplications_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobApplications_Users_JobSeekerId" FOREIGN KEY ("JobSeekerId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Create Indexes
CREATE INDEX "IX_CompanyReveals_JobPostingId" ON "CompanyReveals" ("JobPostingId");
CREATE INDEX "IX_CompanyReveals_JobSeekerId" ON "CompanyReveals" ("JobSeekerId");
CREATE INDEX "IX_CompanyReveals_PaymentTransactionId" ON "CompanyReveals" ("PaymentTransactionId");
CREATE INDEX "IX_JobApplications_JobPostingId" ON "JobApplications" ("JobPostingId");
CREATE INDEX "IX_JobApplications_JobSeekerId" ON "JobApplications" ("JobSeekerId");
CREATE INDEX "IX_JobCategories_ParentId" ON "JobCategories" ("ParentId");
CREATE INDEX "IX_JobPostings_CategoryId" ON "JobPostings" ("CategoryId");
CREATE INDEX "IX_JobPostings_CompanyId" ON "JobPostings" ("CompanyId");
CREATE INDEX "IX_JobPostings_EmployerId" ON "JobPostings" ("EmployerId");
CREATE INDEX "IX_Users_CompanyId" ON "Users" ("CompanyId");
