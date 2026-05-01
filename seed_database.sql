-- Clear existing data
DELETE FROM JobApplications;
DELETE FROM JobPostings;
DELETE FROM CompanyReveals;
DELETE FROM PaymentTransactions;
DELETE FROM Users;
DELETE FROM Companies;

-- Insert Companies
INSERT INTO Companies (Id, Name, Location, Description, Website, Industry, EmployeeCount, CreatedAt) VALUES
('a1111111-1111-1111-1111-111111111111', 'Apple Inc.', 'Cupertino, CA', 'Technology company designing and manufacturing consumer electronics, software, and online services', 'https://apple.com', 'Technology', 164000, datetime('now')),
('a2222222-2222-2222-2222-222222222222', 'Google', 'Mountain View, CA', 'Multinational technology company specializing in Internet-related services and products', 'https://google.com', 'Technology', 190000, datetime('now')),
('a3333333-3333-3333-3333-333333333333', 'Microsoft Corporation', 'Redmond, WA', 'Leading platform and productivity company for the mobile-first, cloud-first world', 'https://microsoft.com', 'Technology', 221000, datetime('now')),
('a4444444-4444-4444-4444-444444444444', 'Amazon', 'Seattle, WA', 'E-commerce, cloud computing, digital streaming, and artificial intelligence company', 'https://amazon.com', 'E-commerce & Cloud', 1540000, datetime('now')),
('a5555555-5555-5555-5555-555555555555', 'Meta (Facebook)', 'Menlo Park, CA', 'Social technology company building the metaverse and connecting people worldwide', 'https://meta.com', 'Social Media', 86000, datetime('now')),
('a6666666-6666-6666-6666-666666666666', 'Netflix', 'Los Gatos, CA', 'World''s leading streaming entertainment service with over 230 million paid memberships', 'https://netflix.com', 'Entertainment', 12800, datetime('now')),
('a7777777-7777-7777-7777-777777777777', 'Airbnb', 'San Francisco, CA', 'Online marketplace for lodging, primarily homestays for vacation rentals and tourism', 'https://airbnb.com', 'Travel & Hospitality', 6800, datetime('now')),
('a8888888-8888-8888-8888-888888888888', 'Tesla Inc.', 'Austin, TX', 'Electric vehicle and clean energy company designing and manufacturing electric cars', 'https://tesla.com', 'Automotive & Energy', 127855, datetime('now')),
('a9999999-9999-9999-9999-999999999999', 'Goldman Sachs', 'New York, NY', 'Leading global investment banking, securities and investment management firm', 'https://goldmansachs.com', 'Financial Services', 48500, datetime('now')),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'JPMorgan Chase & Co.', 'New York, NY', 'Global financial services firm and one of the largest banking institutions', 'https://jpmorganchase.com', 'Banking', 293723, datetime('now'));

-- Insert Job Seekers (Discriminator = 'JobSeeker', UserType = 1)
INSERT INTO Users (Id, Email, FirstName, LastName, PasswordHash, CreatedAt, IsActive, UserType, Discriminator, PhoneNumber, LastLoginAt, Resume, Skills, Address, DateOfBirth, ProfessionalSummary, LinkedInUrl, GitHubUrl, PortfolioUrl) VALUES
('b1111111-1111-1111-1111-111111111111', 'emily.chen@email.com', 'Emily', 'Chen', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b2222222-2222-2222-2222-222222222222', 'michael.rodriguez@email.com', 'Michael', 'Rodriguez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b3333333-3333-3333-3333-333333333333', 'sarah.johnson@email.com', 'Sarah', 'Johnson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b4444444-4444-4444-4444-444444444444', 'david.kim@email.com', 'David', 'Kim', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b5555555-5555-5555-5555-555555555555', 'jessica.taylor@email.com', 'Jessica', 'Taylor', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

-- Insert Employers (Discriminator = 'Employer', UserType = 2)
INSERT INTO Users (Id, Email, FirstName, LastName, PasswordHash, CreatedAt, IsActive, UserType, Discriminator, CompanyId, Position, PhoneNumber, LastLoginAt) VALUES
('c1111111-1111-1111-1111-111111111111', 'hr.apple@company.com', 'Jennifer', 'Smith', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a1111111-1111-1111-1111-111111111111', 'HR Manager', NULL, NULL),
('c2222222-2222-2222-2222-222222222222', 'recruiting.google@company.com', 'Robert', 'Johnson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a2222222-2222-2222-2222-222222222222', 'Recruiting Manager', NULL, NULL),
('c3333333-3333-3333-3333-333333333333', 'talent.microsoft@company.com', 'Maria', 'Garcia', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a3333333-3333-3333-3333-333333333333', 'Talent Acquisition', NULL, NULL),
('c4444444-4444-4444-4444-444444444444', 'hiring.amazon@company.com', 'William', 'Brown', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a4444444-4444-4444-4444-444444444444', 'Hiring Manager', NULL, NULL),
('c5555555-5555-5555-5555-555555555555', 'careers.meta@company.com', 'Patricia', 'Davis', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a5555555-5555-5555-5555-555555555555', 'Careers Lead', NULL, NULL);

-- Insert Job Postings
INSERT INTO JobPostings (Id, Title, Description, Requirements, SalaryMin, SalaryMax, Location, EmploymentType, Category, EmployerId, CompanyId, Status, PostedDate, ClosedDate, ExpiryDate, ExperienceLevel, IsCompanyRevealed, CategoryId) VALUES
('d1111111-1111-1111-1111-111111111111', 'Senior iOS Engineer', 'Join Apple''s world-class iOS team to build the next generation of iOS experiences. Work on features used by millions of users daily.', '5+ years iOS development, Swift, SwiftUI, Objective-C, strong CS fundamentals', '180000', '250000', 'Cupertino, CA', 'Full-time', 'Software Development', 'c1111111-1111-1111-1111-111111111111', 'a1111111-1111-1111-1111-111111111111', 1, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, NULL),
('d2222222-2222-2222-2222-222222222222', 'Machine Learning Engineer', 'Be part of Apple''s ML team developing cutting-edge machine learning solutions for products like Siri, Photos, and more.', 'PhD or MS in CS/ML, TensorFlow, PyTorch, Python, 3+ years ML experience', '200000', '280000', 'Cupertino, CA', 'Full-time', 'Machine Learning', 'c1111111-1111-1111-1111-111111111111', 'a1111111-1111-1111-1111-111111111111', 1, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, NULL),
('d3333333-3333-3333-3333-333333333333', 'Senior Software Engineer', 'Build technologies that help billions of people connect, explore, and interact with information at Google.', 'BS in CS, 5+ years software development, Java/C++/Python, distributed systems', '190000', '270000', 'Mountain View, CA', 'Full-time', 'Software Development', 'c2222222-2222-2222-2222-222222222222', 'a2222222-2222-2222-2222-222222222222', 1, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, NULL),
('d4444444-4444-4444-4444-444444444444', 'Cloud Solution Architect', 'Help customers architect and deploy solutions on Microsoft Azure. Lead technical engagements.', 'Azure certifications, 5+ years cloud architecture, C#/.NET, microservices', '165000', '230000', 'Redmond, WA', 'Full-time', 'Cloud Architecture', 'c3333333-3333-3333-3333-333333333333', 'a3333333-3333-3333-3333-333333333333', 1, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, NULL),
('d5555555-5555-5555-5555-555555555555', 'Software Development Engineer', 'Build and scale Amazon''s e-commerce platform. Work on systems handling millions of transactions daily.', 'CS degree, 3+ years Java/Python/C++, algorithms, data structures, AWS', '150000', '220000', 'Seattle, WA', 'Full-time', 'Software Development', 'c4444444-4444-4444-4444-444444444444', 'a4444444-4444-4444-4444-444444444444', 1, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, NULL);

-- Insert Job Applications
INSERT INTO JobApplications (Id, JobPostingId, JobSeekerId, CoverLetter, ResumeUrl, Status, AppliedDate, ReviewedDate, ReviewNotes, ExpectedSalary, PortfolioLink, AvailableStartDate, AdditionalInfo) VALUES
('e1111111-1111-1111-1111-111111111111', 'd1111111-1111-1111-1111-111111111111', 'b1111111-1111-1111-1111-111111111111', 'I''m very excited about this iOS Engineer position at Apple. With my 5 years of Swift development experience and passion for creating intuitive user experiences, I believe I would be a great addition to your team.', NULL, 0, datetime('now'), NULL, NULL, NULL, NULL, NULL, NULL),
('e2222222-2222-2222-2222-222222222222', 'd3333333-3333-3333-3333-333333333333', 'b2222222-2222-2222-2222-222222222222', 'I''m interested in joining Google as a Senior Software Engineer. My background in distributed systems and experience with large-scale applications aligns perfectly with this role.', NULL, 0, datetime('now'), NULL, NULL, NULL, NULL, NULL, NULL),
('e3333333-3333-3333-3333-333333333333', 'd4444444-4444-4444-4444-444444444444', 'b3333333-3333-3333-3333-333333333333', 'I''m excited to apply for the Cloud Solution Architect role at Microsoft. My Azure certifications and 6 years of cloud architecture experience make me an ideal candidate.', NULL, 0, datetime('now'), NULL, NULL, NULL, NULL, NULL, NULL);
