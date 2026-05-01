-- Clear existing data
DELETE FROM JobApplications;
DELETE FROM JobPostings;
DELETE FROM CompanyReveals;
DELETE FROM PaymentTransactions;
DELETE FROM JobCategories;
DELETE FROM Users;
DELETE FROM Companies;

-- =====================================================
-- INSERT COMPANIES (15 Companies)
-- =====================================================
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
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'JPMorgan Chase & Co.', 'New York, NY', 'Global financial services firm and one of the largest banking institutions', 'https://jpmorganchase.com', 'Banking', 293723, datetime('now')),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01', 'Salesforce', 'San Francisco, CA', 'Cloud-based software company providing customer relationship management (CRM) service', 'https://salesforce.com', 'Software (CRM)', 79000, datetime('now')),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02', 'Adobe Inc.', 'San Jose, CA', 'Multinational computer software company known for multimedia and creativity software products', 'https://adobe.com', 'Software', 29239, datetime('now')),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03', 'Uber Technologies', 'San Francisco, CA', 'Mobility as a service provider offering ride-hailing, food delivery, and freight transport', 'https://uber.com', 'Transportation', 32800, datetime('now')),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04', 'Stripe', 'San Francisco, CA', 'Financial services and SaaS company offering payment processing for internet businesses', 'https://stripe.com', 'Fintech', 8000, datetime('now')),
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05', 'Shopify', 'Ottawa, Canada', 'E-commerce platform helping businesses sell online, in-store, and everywhere in between', 'https://shopify.com', 'E-commerce', 11600, datetime('now'));

-- =====================================================
-- INSERT JOB CATEGORIES (25 Categories)
-- =====================================================
INSERT INTO JobCategories (Id, Name, Description, CategoryType, ParentId) VALUES
-- Root Categories (Composite)
('caa00000-0000-0000-0000-000000000001', 'Technology', 'All technology-related positions', 'CategoryComposite', NULL),
('caa00000-0000-0000-0000-000000000002', 'Business', 'Business and management roles', 'CategoryComposite', NULL),
('caa00000-0000-0000-0000-000000000003', 'Design', 'Creative and design positions', 'CategoryComposite', NULL),
('caa00000-0000-0000-0000-000000000004', 'Finance', 'Financial and accounting roles', 'CategoryComposite', NULL),
('caa00000-0000-0000-0000-000000000005', 'Operations', 'Operations and logistics roles', 'CategoryComposite', NULL),

-- Technology Sub-categories (Leaf)
('caa00000-0000-0000-0000-000000000011', 'Software Engineering', 'Software development and engineering roles', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000012', 'Data Science', 'Data analysis and machine learning roles', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000013', 'DevOps & Infrastructure', 'DevOps, SRE, and infrastructure roles', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000014', 'Mobile Development', 'iOS and Android development', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000015', 'Frontend Development', 'Web frontend and UI development', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000016', 'Backend Development', 'Server-side and API development', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000017', 'Security Engineering', 'Cybersecurity and information security', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),
('caa00000-0000-0000-0000-000000000018', 'Cloud Architecture', 'Cloud infrastructure and architecture', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000001'),

-- Business Sub-categories (Leaf)
('caa00000-0000-0000-0000-000000000021', 'Product Management', 'Product strategy and management', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000002'),
('caa00000-0000-0000-0000-000000000022', 'Project Management', 'Project planning and execution', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000002'),
('caa00000-0000-0000-0000-000000000023', 'Sales', 'Sales and business development', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000002'),
('caa00000-0000-0000-0000-000000000024', 'Marketing', 'Marketing and growth roles', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000002'),
('caa00000-0000-0000-0000-000000000025', 'Business Analysis', 'Business analysis and strategy', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000002'),

-- Design Sub-categories (Leaf)
('caa00000-0000-0000-0000-000000000031', 'UX/UI Design', 'User experience and interface design', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000003'),
('caa00000-0000-0000-0000-000000000032', 'Product Design', 'Product and interaction design', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000003'),
('caa00000-0000-0000-0000-000000000033', 'Graphic Design', 'Visual and graphic design', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000003'),

-- Finance Sub-categories (Leaf)
('caa00000-0000-0000-0000-000000000041', 'Financial Analysis', 'Financial planning and analysis', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000004'),
('caa00000-0000-0000-0000-000000000042', 'Accounting', 'Accounting and bookkeeping roles', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000004'),
('caa00000-0000-0000-0000-000000000043', 'Investment Banking', 'Investment banking and M&A', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000004'),

-- Operations Sub-categories (Leaf)
('caa00000-0000-0000-0000-000000000051', 'Supply Chain', 'Supply chain and logistics', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000005'),
('caa00000-0000-0000-0000-000000000052', 'Quality Assurance', 'QA and testing roles', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000005'),
('caa00000-0000-0000-0000-000000000053', 'Customer Support', 'Customer service and support', 'CategoryLeaf', 'caa00000-0000-0000-0000-000000000005');

-- =====================================================
-- INSERT JOB SEEKERS (20 Job Seekers)
-- =====================================================
INSERT INTO Users (Id, Email, FirstName, LastName, PasswordHash, CreatedAt, IsActive, UserType, Discriminator, PhoneNumber, LastLoginAt, Resume, Skills, Address, DateOfBirth, ProfessionalSummary, LinkedInUrl, GitHubUrl, PortfolioUrl) VALUES
('b1111111-1111-1111-1111-111111111111', 'emily.chen@email.com', 'Emily', 'Chen', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b2222222-2222-2222-2222-222222222222', 'michael.rodriguez@email.com', 'Michael', 'Rodriguez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b3333333-3333-3333-3333-333333333333', 'sarah.johnson@email.com', 'Sarah', 'Johnson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b4444444-4444-4444-4444-444444444444', 'david.kim@email.com', 'David', 'Kim', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b5555555-5555-5555-5555-555555555555', 'jessica.taylor@email.com', 'Jessica', 'Taylor', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b6666666-6666-6666-6666-666666666666', 'james.anderson@email.com', 'James', 'Anderson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b7777777-7777-7777-7777-777777777777', 'amanda.martinez@email.com', 'Amanda', 'Martinez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b8888888-8888-8888-8888-888888888888', 'ryan.thomas@email.com', 'Ryan', 'Thomas', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b9999999-9999-9999-9999-999999999999', 'nicole.jackson@email.com', 'Nicole', 'Jackson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01', 'kevin.white@email.com', 'Kevin', 'White', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02', 'lisa.harris@email.com', 'Lisa', 'Harris', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03', 'daniel.clark@email.com', 'Daniel', 'Clark', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04', 'rachel.lewis@email.com', 'Rachel', 'Lewis', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05', 'chris.walker@email.com', 'Chris', 'Walker', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06', 'sophia.hall@email.com', 'Sophia', 'Hall', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb07', 'brandon.allen@email.com', 'Brandon', 'Allen', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb08', 'olivia.young@email.com', 'Olivia', 'Young', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb09', 'matthew.king@email.com', 'Matthew', 'King', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb10', 'ashley.wright@email.com', 'Ashley', 'Wright', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb11', 'joshua.lopez@email.com', 'Joshua', 'Lopez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

-- =====================================================
-- INSERT EMPLOYERS (15 Employers - one per company)
-- =====================================================
INSERT INTO Users (Id, Email, FirstName, LastName, PasswordHash, CreatedAt, IsActive, UserType, Discriminator, CompanyId, Position, PhoneNumber, LastLoginAt) VALUES
('c1111111-1111-1111-1111-111111111111', 'hr.apple@company.com', 'Jennifer', 'Smith', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a1111111-1111-1111-1111-111111111111', 'HR Manager', NULL, NULL),
('c2222222-2222-2222-2222-222222222222', 'recruiting.google@company.com', 'Robert', 'Johnson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a2222222-2222-2222-2222-222222222222', 'Recruiting Manager', NULL, NULL),
('c3333333-3333-3333-3333-333333333333', 'talent.microsoft@company.com', 'Maria', 'Garcia', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a3333333-3333-3333-3333-333333333333', 'Talent Acquisition', NULL, NULL),
('c4444444-4444-4444-4444-444444444444', 'hiring.amazon@company.com', 'William', 'Brown', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a4444444-4444-4444-4444-444444444444', 'Hiring Manager', NULL, NULL),
('c5555555-5555-5555-5555-555555555555', 'careers.meta@company.com', 'Patricia', 'Davis', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a5555555-5555-5555-5555-555555555555', 'Careers Lead', NULL, NULL),
('c6666666-6666-6666-6666-666666666666', 'jobs.netflix@company.com', 'Richard', 'Miller', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a6666666-6666-6666-6666-666666666666', 'Talent Manager', NULL, NULL),
('c7777777-7777-7777-7777-777777777777', 'talent.airbnb@company.com', 'Linda', 'Wilson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a7777777-7777-7777-7777-777777777777', 'Recruitment Lead', NULL, NULL),
('c8888888-8888-8888-8888-888888888888', 'hr.tesla@company.com', 'Thomas', 'Moore', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a8888888-8888-8888-8888-888888888888', 'HR Director', NULL, NULL),
('c9999999-9999-9999-9999-999999999999', 'recruiting.goldman@company.com', 'Barbara', 'Taylor', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a9999999-9999-9999-9999-999999999999', 'Recruiting Director', NULL, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccc01', 'hr.jpmorgan@company.com', 'Christopher', 'Anderson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'VP Talent', NULL, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccc02', 'talent.salesforce@company.com', 'Nancy', 'Thomas', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa01', 'Talent Acquisition', NULL, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccc03', 'careers.adobe@company.com', 'Steven', 'Jackson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa02', 'Hiring Manager', NULL, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccc04', 'jobs.uber@company.com', 'Karen', 'White', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa03', 'Recruiting Lead', NULL, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccc05', 'hiring.stripe@company.com', 'Kevin', 'Harris', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04', 'Head of Talent', NULL, NULL),
('cccccccc-cccc-cccc-cccc-cccccccccc06', 'talent.shopify@company.com', 'Lisa', 'Martin', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05', 'Recruitment Manager', NULL, NULL);

-- =====================================================
-- INSERT JOB POSTINGS (30 Job Postings)
-- =====================================================
INSERT INTO JobPostings (Id, Title, Description, Requirements, SalaryMin, SalaryMax, Location, EmploymentType, Category, EmployerId, CompanyId, Status, PostedDate, ClosedDate, ExpiryDate, ExperienceLevel, IsCompanyRevealed, CategoryId) VALUES
-- Apple Jobs
('d1111111-1111-1111-1111-111111111111', 'Senior iOS Engineer', 'Join Apple''s world-class iOS team to build the next generation of iOS experiences used by millions daily.', '5+ years iOS development, Swift, SwiftUI, Objective-C, strong CS fundamentals', '180000', '250000', 'Cupertino, CA', 'Full-time', 'Software Development', 'c1111111-1111-1111-1111-111111111111', 'a1111111-1111-1111-1111-111111111111', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000014'),
('d1111111-1111-1111-1111-111111111112', 'Machine Learning Engineer', 'Be part of Apple''s ML team developing cutting-edge solutions for Siri, Photos, and more.', 'PhD or MS in CS/ML, TensorFlow, PyTorch, Python, 3+ years ML experience', '200000', '280000', 'Cupertino, CA', 'Full-time', 'Machine Learning', 'c1111111-1111-1111-1111-111111111111', 'a1111111-1111-1111-1111-111111111111', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000012'),

-- Google Jobs
('d2222222-2222-2222-2222-222222222221', 'Senior Software Engineer', 'Build technologies that help billions of people connect, explore, and interact with information.', 'BS in CS, 5+ years software development, Java/C++/Python, distributed systems', '190000', '270000', 'Mountain View, CA', 'Full-time', 'Software Development', 'c2222222-2222-2222-2222-222222222222', 'a2222222-2222-2222-2222-222222222222', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000011'),
('d2222222-2222-2222-2222-222222222222', 'Site Reliability Engineer', 'Ensure Google''s services are reliable and scalable with 99.99% uptime.', 'Linux/Unix, Python/Go, networking, 4+ years SRE/DevOps experience', '170000', '240000', 'Mountain View, CA', 'Full-time', 'DevOps/SRE', 'c2222222-2222-2222-2222-222222222222', 'a2222222-2222-2222-2222-222222222222', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000013'),
('d2222222-2222-2222-2222-222222222223', 'UX Researcher', 'Lead research initiatives to understand user needs and behaviors. Shape product decisions.', 'MS/PhD in HCI, 3+ years UX research, quantitative/qualitative methods', '140000', '190000', 'Mountain View, CA', 'Full-time', 'Research', 'c2222222-2222-2222-2222-222222222222', 'a2222222-2222-2222-2222-222222222222', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000031'),
('d2222222-2222-2222-2222-222222222224', 'Product Manager', 'Drive product strategy and roadmap for key Google products with cross-functional teams.', 'MBA or 5+ years PM experience, technical background, data-driven', '175000', '260000', 'Mountain View, CA', 'Full-time', 'Product Management', 'c2222222-2222-2222-2222-222222222222', 'a2222222-2222-2222-2222-222222222222', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000021'),

-- Microsoft Jobs
('d3333333-3333-3333-3333-333333333331', 'Cloud Solution Architect', 'Help customers architect and deploy solutions on Microsoft Azure. Lead technical engagements.', 'Azure certifications, 5+ years cloud architecture, C#/.NET, microservices', '165000', '230000', 'Redmond, WA', 'Full-time', 'Cloud Architecture', 'c3333333-3333-3333-3333-333333333333', 'a3333333-3333-3333-3333-333333333333', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000018'),
('d3333333-3333-3333-3333-333333333332', 'Principal Software Engineer', 'Lead engineering teams building the next generation of Microsoft 365. Drive technical excellence.', '10+ years software development, C#, TypeScript, distributed systems, leadership', '210000', '300000', 'Redmond, WA', 'Full-time', 'Software Development', 'c3333333-3333-3333-3333-333333333333', 'a3333333-3333-3333-3333-333333333333', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Staff', 1, 'caa00000-0000-0000-0000-000000000011'),
('d3333333-3333-3333-3333-333333333333', 'AI Research Scientist', 'Conduct cutting-edge AI research at Microsoft Research. Publish papers and develop AI tech.', 'PhD in CS/ML/AI, published research, deep learning, NLP or computer vision', '190000', '280000', 'Redmond, WA', 'Full-time', 'Research', 'c3333333-3333-3333-3333-333333333333', 'a3333333-3333-3333-3333-333333333333', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000012'),
('d3333333-3333-3333-3333-333333333334', 'Full Stack Developer', 'Build modern web applications for Microsoft''s cloud services with React, Node.js, and Azure.', '4+ years full-stack development, React, Node.js, SQL, REST APIs', '140000', '190000', 'Redmond, WA', 'Full-time', 'Web Development', 'c3333333-3333-3333-3333-333333333333', 'a3333333-3333-3333-3333-333333333333', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000011'),

-- Amazon Jobs
('d4444444-4444-4444-4444-444444444441', 'Software Development Engineer', 'Build and scale Amazon''s e-commerce platform handling millions of transactions daily.', 'CS degree, 3+ years Java/Python/C++, algorithms, data structures, AWS', '150000', '220000', 'Seattle, WA', 'Full-time', 'Software Development', 'c4444444-4444-4444-4444-444444444444', 'a4444444-4444-4444-4444-444444444444', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000011'),
('d4444444-4444-4444-4444-444444444442', 'Data Scientist', 'Drive data-driven decisions across Amazon. Build ML models for recommendations and pricing.', 'MS/PhD, Python, R, SQL, machine learning, statistics, 3+ years', '160000', '230000', 'Seattle, WA', 'Full-time', 'Data Science', 'c4444444-4444-4444-4444-444444444444', 'a4444444-4444-4444-4444-444444444444', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000012'),
('d4444444-4444-4444-4444-444444444443', 'DevOps Engineer', 'Build and maintain infrastructure for Amazon''s services. Automate deployments.', 'AWS, Docker, Kubernetes, CI/CD, Python/Bash, 4+ years DevOps', '145000', '200000', 'Seattle, WA', 'Full-time', 'DevOps', 'c4444444-4444-4444-4444-444444444444', 'a4444444-4444-4444-4444-444444444444', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000013'),
('d4444444-4444-4444-4444-444444444444', 'Solutions Architect', 'Help enterprise customers design and implement solutions on AWS as a technical advisor.', 'AWS certifications, 5+ years solution architecture, customer-facing', '170000', '240000', 'Seattle, WA', 'Full-time', 'Solutions Architecture', 'c4444444-4444-4444-4444-444444444444', 'a4444444-4444-4444-4444-444444444444', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, 'caa00000-0000-0000-0000-000000000018'),

-- Meta Jobs
('d5555555-5555-5555-5555-555555555551', 'Software Engineer, Mobile', 'Build mobile experiences for billions on Facebook, Instagram, WhatsApp with React Native.', '3+ years mobile development, iOS/Android, React Native, CS fundamentals', '170000', '250000', 'Menlo Park, CA', 'Full-time', 'Mobile Development', 'c5555555-5555-5555-5555-555555555555', 'a5555555-5555-5555-5555-555555555555', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000014'),
('d5555555-5555-5555-5555-555555555552', 'Data Engineer', 'Build and maintain data infrastructure supporting Meta''s products. Process petabytes daily.', '4+ years data engineering, Spark, Hadoop, SQL, Python, distributed systems', '160000', '230000', 'Menlo Park, CA', 'Full-time', 'Data Engineering', 'c5555555-5555-5555-5555-555555555555', 'a5555555-5555-5555-5555-555555555555', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, 'caa00000-0000-0000-0000-000000000012'),
('d5555555-5555-5555-5555-555555555553', 'VR/AR Engineer', 'Shape the future of the metaverse. Build immersive VR/AR for Meta Quest.', 'Unity/Unreal Engine, C++, computer graphics, 3+ years VR/AR', '175000', '255000', 'Menlo Park, CA', 'Full-time', 'VR/AR Development', 'c5555555-5555-5555-5555-555555555555', 'a5555555-5555-5555-5555-555555555555', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000011'),

-- Netflix Jobs
('d6666666-6666-6666-6666-666666666661', 'Senior Backend Engineer', 'Build microservices powering Netflix''s streaming for 230M+ members globally.', '5+ years backend, Java/Kotlin/Go, microservices, AWS, distributed systems', '180000', '260000', 'Los Gatos, CA', 'Full-time', 'Backend Development', 'c6666666-6666-6666-6666-666666666666', 'a6666666-6666-6666-6666-666666666666', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, 'caa00000-0000-0000-0000-000000000016'),
('d6666666-6666-6666-6666-666666666662', 'Content Recommendation Engineer', 'Build ML models recommending content to Netflix members. Drive engagement.', 'ML/AI experience, Python, TensorFlow, recommendation systems, 4+ years', '175000', '250000', 'Los Gatos, CA', 'Full-time', 'Machine Learning', 'c6666666-6666-6666-6666-666666666666', 'a6666666-6666-6666-6666-666666666666', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, 'caa00000-0000-0000-0000-000000000012'),
('d6666666-6666-6666-6666-666666666663', 'UI Engineer', 'Create stunning UI experiences for Netflix on all devices with React.', '4+ years frontend, React, TypeScript, CSS, responsive design, performance', '155000', '220000', 'Los Gatos, CA', 'Full-time', 'Frontend Development', 'c6666666-6666-6666-6666-666666666666', 'a6666666-6666-6666-6666-666666666666', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000015'),

-- Airbnb Jobs
('d7777777-7777-7777-7777-777777777771', 'Full Stack Engineer', 'Build features for Airbnb''s platform connecting hosts and guests worldwide.', '3+ years full-stack, React, Node.js, Ruby on Rails, PostgreSQL, AWS', '160000', '230000', 'San Francisco, CA', 'Full-time', 'Full Stack', 'c7777777-7777-7777-7777-777777777777', 'a7777777-7777-7777-7777-777777777777', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000011'),
('d7777777-7777-7777-7777-777777777772', 'Product Designer', 'Design delightful experiences for Airbnb''s community. Create user-centered designs.', '4+ years product design, Figma, user research, prototyping, design systems', '150000', '210000', 'San Francisco, CA', 'Full-time', 'Product Design', 'c7777777-7777-7777-7777-777777777777', 'a7777777-7777-7777-7777-777777777777', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000032'),

-- Tesla Jobs
('d8888888-8888-8888-8888-888888888881', 'Embedded Software Engineer', 'Develop embedded software for Tesla vehicles. Work on Autopilot and battery management.', 'C/C++, embedded Linux, real-time systems, CAN bus, 4+ years embedded', '140000', '210000', 'Austin, TX', 'Full-time', 'Embedded Systems', 'c8888888-8888-8888-8888-888888888888', 'a8888888-8888-8888-8888-888888888888', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000011'),
('d8888888-8888-8888-8888-888888888882', 'Computer Vision Engineer', 'Build perception systems for Tesla''s Autopilot with camera and sensor data.', 'Deep learning, computer vision, Python, C++, CUDA, autonomous systems', '165000', '240000', 'Austin, TX', 'Full-time', 'Computer Vision', 'c8888888-8888-8888-8888-888888888888', 'a8888888-8888-8888-8888-888888888888', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, 'caa00000-0000-0000-0000-000000000012'),

-- Goldman Sachs Jobs
('d9999999-9999-9999-9999-999999999991', 'Quantitative Developer', 'Build trading systems and risk management platforms with cutting-edge fintech.', 'CS/Math degree, C++/Java, financial markets, algorithms, 3+ years', '170000', '250000', 'New York, NY', 'Full-time', 'Quantitative Dev', 'c9999999-9999-9999-9999-999999999999', 'a9999999-9999-9999-9999-999999999999', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000011'),
('d9999999-9999-9999-9999-999999999992', 'Investment Banking Analyst', 'Work on M&A transactions, IPOs, and strategic advisory. High-profile deals.', 'Top university degree, finance knowledge, Excel/PowerPoint, analytical', '110000', '150000', 'New York, NY', 'Full-time', 'Investment Banking', 'c9999999-9999-9999-9999-999999999999', 'a9999999-9999-9999-9999-999999999999', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Entry-Level', 0, 'caa00000-0000-0000-0000-000000000043'),

-- Remaining companies
('dddddddd-dddd-dddd-dddd-dddddddddd01', 'Cybersecurity Engineer', 'Protect JPMorgan''s systems and customer data. Implement security controls.', 'Security certifications, penetration testing, SIEM, incident response, 3+', '125000', '185000', 'New York, NY', 'Full-time', 'Cybersecurity', 'cccccccc-cccc-cccc-cccc-cccccccccc01', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000017'),
('dddddddd-dddd-dddd-dddd-dddddddddd02', 'Frontend Engineer', 'Build Stripe''s dashboard and developer tools. Create beautiful, fast web apps.', '4+ years frontend, React, TypeScript, performance, accessibility, design', '160000', '230000', 'San Francisco, CA', 'Full-time', 'Frontend', 'cccccccc-cccc-cccc-cccc-cccccccccc05', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa04', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'caa00000-0000-0000-0000-000000000015'),
('dddddddd-dddd-dddd-dddd-dddddddddd03', 'Senior Product Designer', 'Design merchant and consumer experiences for Shopify''s platform.', '5+ years product design, e-commerce, Figma, user research, systems thinking', '135000', '190000', 'Ottawa, Canada', 'Full-time', 'Product Design', 'cccccccc-cccc-cccc-cccc-cccccccccc06', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaa05', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, 'caa00000-0000-0000-0000-000000000032');

-- =====================================================
-- INSERT JOB APPLICATIONS (25 Applications)
-- =====================================================
INSERT INTO JobApplications (Id, JobPostingId, JobSeekerId, CoverLetter, ResumeUrl, Status, AppliedDate, ReviewedDate, ReviewNotes, ExpectedSalary, PortfolioLink, AvailableStartDate, AdditionalInfo) VALUES
('e0000000-0000-0000-0000-000000000001', 'd1111111-1111-1111-1111-111111111111', 'b1111111-1111-1111-1111-111111111111', 'I''m very excited about the iOS Engineer position at Apple. My 5 years of Swift development and passion for intuitive UX make me a perfect fit.', NULL, 0, datetime('now', '-5 days'), NULL, NULL, '200000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000002', 'd2222222-2222-2222-2222-222222222221', 'b2222222-2222-2222-2222-222222222222', 'I''m interested in joining Google as a Senior Software Engineer. My distributed systems experience aligns perfectly with this role.', NULL, 0, datetime('now', '-4 days'), NULL, NULL, '220000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000003', 'd3333333-3333-3333-3333-333333333331', 'b3333333-3333-3333-3333-333333333333', 'I''m excited to apply for the Cloud Solution Architect role at Microsoft. My Azure certifications and 6 years of cloud experience are ideal.', NULL, 2, datetime('now', '-10 days'), datetime('now', '-2 days'), 'Strong technical background', '190000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000004', 'd4444444-4444-4444-4444-444444444441', 'b4444444-4444-4444-4444-444444444444', 'I would love to join Amazon as an SDE. My CS fundamentals and AWS experience would let me contribute immediately.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '180000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000005', 'd5555555-5555-5555-5555-555555555551', 'b5555555-5555-5555-5555-555555555555', 'Applying for Mobile Engineer at Meta. My React Native expertise and experience with millions of users would be valuable.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '210000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000006', 'd6666666-6666-6666-6666-666666666661', 'b6666666-6666-6666-6666-666666666666', 'Very interested in the Senior Backend Engineer role at Netflix. My microservices architecture experience aligns perfectly.', NULL, 2, datetime('now', '-15 days'), datetime('now', '-5 days'), 'Excellent candidate, moving to final round', '220000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000007', 'd7777777-7777-7777-7777-777777777771', 'b7777777-7777-7777-7777-777777777777', 'Would love to join Airbnb as a Full Stack Engineer. My React and Rails experience matches your tech stack perfectly.', NULL, 0, datetime('now', '-1 day'), NULL, NULL, '190000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000008', 'd8888888-8888-8888-8888-888888888881', 'b8888888-8888-8888-8888-888888888888', 'Excited about the Embedded Software Engineer position at Tesla. My automotive software background is a great fit.', NULL, 0, datetime('now', '-6 days'), NULL, NULL, '170000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000009', 'd9999999-9999-9999-9999-999999999991', 'b9999999-9999-9999-9999-999999999999', 'Applying for Quantitative Developer at Goldman Sachs. My strong math and C++ skills align with requirements.', NULL, 2, datetime('now', '-8 days'), datetime('now', '-1 day'), 'Good technical skills', '200000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000010', 'd1111111-1111-1111-1111-111111111112', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01', 'Interested in the Machine Learning Engineer position at Apple. My PhD in ML and TensorFlow expertise are perfect.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '240000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000011', 'd2222222-2222-2222-2222-222222222222', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02', 'Applying for SRE at Google. My 5 years of DevOps and system reliability experience would be valuable.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '200000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000012', 'd3333333-3333-3333-3333-333333333332', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03', 'Would love the Principal Engineer role at Microsoft. My 12 years of experience and leadership skills are a match.', NULL, 2, datetime('now', '-12 days'), datetime('now', '-3 days'), 'Strong leadership qualities', '270000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000013', 'd4444444-4444-4444-4444-444444444442', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04', 'Excited to apply for Data Scientist at Amazon. My ML models have driven significant business impact.', NULL, 0, datetime('now', '-4 days'), NULL, NULL, '190000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000014', 'd5555555-5555-5555-5555-555555555552', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05', 'Applying for Data Engineer at Meta. My experience with petabyte-scale data processing is relevant.', NULL, 0, datetime('now', '-1 day'), NULL, NULL, '190000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000015', 'd6666666-6666-6666-6666-666666666662', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06', 'Very interested in the Recommendation Engineer role at Netflix. Building recommendation systems is my passion.', NULL, 0, datetime('now', '-7 days'), NULL, NULL, '210000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000016', 'd7777777-7777-7777-7777-777777777772', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb07', 'Would love to join Airbnb as a Product Designer. My user-centered design approach aligns with your values.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '180000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000017', 'd8888888-8888-8888-8888-888888888882', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb08', 'Excited about the Computer Vision Engineer role at Tesla. Autonomous driving is where I want to make an impact.', NULL, 2, datetime('now', '-20 days'), datetime('now', '-8 days'), 'Excellent CV background, final interview scheduled', '200000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000018', 'd2222222-2222-2222-2222-222222222223', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb09', 'Applying for UX Researcher at Google. My qualitative and quantitative research skills would be valuable.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '160000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000019', 'd3333333-3333-3333-3333-333333333333', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb10', 'Very interested in the AI Research Scientist role at Microsoft. My published research in NLP aligns perfectly.', NULL, 2, datetime('now', '-9 days'), datetime('now', '-2 days'), 'Strong research portfolio', '250000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000020', 'd4444444-4444-4444-4444-444444444443', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb11', 'Excited to apply for DevOps Engineer at Amazon. My AWS and Kubernetes experience is extensive.', NULL, 0, datetime('now', '-5 days'), NULL, NULL, '170000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000021', 'd2222222-2222-2222-2222-222222222224', 'b1111111-1111-1111-1111-111111111111', 'Interested in the Product Manager role at Google. My technical background and PM experience are ideal.', NULL, 0, datetime('now', '-4 days'), NULL, NULL, '220000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000022', 'd3333333-3333-3333-3333-333333333334', 'b2222222-2222-2222-2222-222222222222', 'Would love the Full Stack Developer role at Microsoft. My React and Node.js skills match perfectly.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '165000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000023', 'd4444444-4444-4444-4444-444444444444', 'b3333333-3333-3333-3333-333333333333', 'Applying for Solutions Architect at Amazon. My customer-facing experience and AWS knowledge are strong.', NULL, 0, datetime('now', '-6 days'), NULL, NULL, '200000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000024', 'd5555555-5555-5555-5555-555555555553', 'b4444444-4444-4444-4444-444444444444', 'Excited about the VR/AR Engineer position at Meta. Building the metaverse is my dream job.', NULL, 0, datetime('now', '-1 day'), NULL, NULL, '210000', NULL, NULL, NULL),
('e0000000-0000-0000-0000-000000000025', 'd6666666-6666-6666-6666-666666666663', 'b5555555-5555-5555-5555-555555555555', 'Interested in the UI Engineer role at Netflix. My React and performance optimization skills are excellent.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '185000', NULL, NULL, NULL);

-- =====================================================
-- INSERT PAYMENT TRANSACTIONS (25 Transactions)
-- =====================================================
INSERT INTO PaymentTransactions (Id, UserId, Amount, Currency, Gateway, Status, TransactionDate, ExternalTransactionId, Description, ErrorMessage) VALUES
('pt000000-0000-0000-0000-000000000001', 'b1111111-1111-1111-1111-111111111111', '9.99', 'USD', 0, 2, datetime('now', '-5 days'), 'stripe_ch_1A2B3C4D', 'Company reveal payment for Apple Inc.', NULL),
('pt000000-0000-0000-0000-000000000002', 'b2222222-2222-2222-2222-222222222222', '9.99', 'USD', 1, 2, datetime('now', '-4 days'), 'paypal_tx_5E6F7G8H', 'Company reveal payment for Google', NULL),
('pt000000-0000-0000-0000-000000000003', 'b3333333-3333-3333-3333-333333333333', '9.99', 'USD', 0, 2, datetime('now', '-10 days'), 'stripe_ch_9I0J1K2L', 'Company reveal payment for Microsoft', NULL),
('pt000000-0000-0000-0000-000000000004', 'b4444444-4444-4444-4444-444444444444', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_3M4N5O6P', 'Company reveal payment for Amazon', NULL),
('pt000000-0000-0000-0000-000000000005', 'b5555555-5555-5555-5555-555555555555', '9.99', 'USD', 1, 2, datetime('now', '-2 days'), 'paypal_tx_7Q8R9S0T', 'Company reveal payment for Meta', NULL),
('pt000000-0000-0000-0000-000000000006', 'b6666666-6666-6666-6666-666666666666', '9.99', 'USD', 0, 2, datetime('now', '-15 days'), 'stripe_ch_1U2V3W4X', 'Company reveal payment for Netflix', NULL),
('pt000000-0000-0000-0000-000000000007', 'b7777777-7777-7777-7777-777777777777', '9.99', 'USD', 0, 2, datetime('now', '-1 day'), 'stripe_ch_5Y6Z7A8B', 'Company reveal payment for Airbnb', NULL),
('pt000000-0000-0000-0000-000000000008', 'b8888888-8888-8888-8888-888888888888', '9.99', 'USD', 1, 2, datetime('now', '-6 days'), 'paypal_tx_9C0D1E2F', 'Company reveal payment for Tesla', NULL),
('pt000000-0000-0000-0000-000000000009', 'b9999999-9999-9999-9999-999999999999', '9.99', 'USD', 0, 2, datetime('now', '-8 days'), 'stripe_ch_3G4H5I6J', 'Company reveal payment for Goldman Sachs', NULL),
('pt000000-0000-0000-0000-000000000010', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_7K8L9M0N', 'Company reveal payment for Apple Inc.', NULL),
('pt000000-0000-0000-0000-000000000011', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02', '9.99', 'USD', 1, 2, datetime('now', '-2 days'), 'paypal_tx_1O2P3Q4R', 'Company reveal payment for Google', NULL),
('pt000000-0000-0000-0000-000000000012', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03', '9.99', 'USD', 0, 2, datetime('now', '-12 days'), 'stripe_ch_5S6T7U8V', 'Company reveal payment for Microsoft', NULL),
('pt000000-0000-0000-0000-000000000013', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04', '9.99', 'USD', 0, 2, datetime('now', '-4 days'), 'stripe_ch_9W0X1Y2Z', 'Company reveal payment for Amazon', NULL),
('pt000000-0000-0000-0000-000000000014', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05', '9.99', 'USD', 1, 2, datetime('now', '-1 day'), 'paypal_tx_3A4B5C6D', 'Company reveal payment for Meta', NULL),
('pt000000-0000-0000-0000-000000000015', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06', '9.99', 'USD', 0, 2, datetime('now', '-7 days'), 'stripe_ch_7E8F9G0H', 'Company reveal payment for Netflix', NULL),
('pt000000-0000-0000-0000-000000000016', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb07', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_1I2J3K4L', 'Company reveal payment for Airbnb', NULL),
('pt000000-0000-0000-0000-000000000017', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb08', '9.99', 'USD', 1, 2, datetime('now', '-20 days'), 'paypal_tx_5M6N7O8P', 'Company reveal payment for Tesla', NULL),
('pt000000-0000-0000-0000-000000000018', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb09', '9.99', 'USD', 0, 2, datetime('now', '-2 days'), 'stripe_ch_9Q0R1S2T', 'Company reveal payment for Google', NULL),
('pt000000-0000-0000-0000-000000000019', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb10', '9.99', 'USD', 0, 2, datetime('now', '-9 days'), 'stripe_ch_3U4V5W6X', 'Company reveal payment for Microsoft', NULL),
('pt000000-0000-0000-0000-000000000020', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb11', '9.99', 'USD', 1, 2, datetime('now', '-5 days'), 'paypal_tx_7Y8Z9A0B', 'Company reveal payment for Amazon', NULL),
('pt000000-0000-0000-0000-000000000021', 'b1111111-1111-1111-1111-111111111111', '9.99', 'USD', 0, 2, datetime('now', '-4 days'), 'stripe_ch_1C2D3E4F', 'Company reveal payment for Google', NULL),
('pt000000-0000-0000-0000-000000000022', 'b2222222-2222-2222-2222-222222222222', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_5G6H7I8J', 'Company reveal payment for Microsoft', NULL),
('pt000000-0000-0000-0000-000000000023', 'b3333333-3333-3333-3333-333333333333', '9.99', 'USD', 1, 2, datetime('now', '-6 days'), 'paypal_tx_9K0L1M2N', 'Company reveal payment for Amazon', NULL),
('pt000000-0000-0000-0000-000000000024', 'b4444444-4444-4444-4444-444444444444', '9.99', 'USD', 0, 2, datetime('now', '-1 day'), 'stripe_ch_3O4P5Q6R', 'Company reveal payment for Meta', NULL),
('pt000000-0000-0000-0000-000000000025', 'b5555555-5555-5555-5555-555555555555', '9.99', 'USD', 0, 2, datetime('now', '-2 days'), 'stripe_ch_7S8T9U0V', 'Company reveal payment for Netflix', NULL);

-- =====================================================
-- INSERT COMPANY REVEALS (25 Company Reveals)
-- =====================================================
INSERT INTO CompanyReveals (Id, JobSeekerId, JobPostingId, PaymentTransactionId, RevealedDate) VALUES
('cr000000-0000-0000-0000-000000000001', 'b1111111-1111-1111-1111-111111111111', 'd1111111-1111-1111-1111-111111111111', 'pt000000-0000-0000-0000-000000000001', datetime('now', '-5 days')),
('cr000000-0000-0000-0000-000000000002', 'b2222222-2222-2222-2222-222222222222', 'd2222222-2222-2222-2222-222222222221', 'pt000000-0000-0000-0000-000000000002', datetime('now', '-4 days')),
('cr000000-0000-0000-0000-000000000003', 'b3333333-3333-3333-3333-333333333333', 'd3333333-3333-3333-3333-333333333331', 'pt000000-0000-0000-0000-000000000003', datetime('now', '-10 days')),
('cr000000-0000-0000-0000-000000000004', 'b4444444-4444-4444-4444-444444444444', 'd4444444-4444-4444-4444-444444444441', 'pt000000-0000-0000-0000-000000000004', datetime('now', '-3 days')),
('cr000000-0000-0000-0000-000000000005', 'b5555555-5555-5555-5555-555555555555', 'd5555555-5555-5555-5555-555555555551', 'pt000000-0000-0000-0000-000000000005', datetime('now', '-2 days')),
('cr000000-0000-0000-0000-000000000006', 'b6666666-6666-6666-6666-666666666666', 'd6666666-6666-6666-6666-666666666661', 'pt000000-0000-0000-0000-000000000006', datetime('now', '-15 days')),
('cr000000-0000-0000-0000-000000000007', 'b7777777-7777-7777-7777-777777777777', 'd7777777-7777-7777-7777-777777777771', 'pt000000-0000-0000-0000-000000000007', datetime('now', '-1 day')),
('cr000000-0000-0000-0000-000000000008', 'b8888888-8888-8888-8888-888888888888', 'd8888888-8888-8888-8888-888888888881', 'pt000000-0000-0000-0000-000000000008', datetime('now', '-6 days')),
('cr000000-0000-0000-0000-000000000009', 'b9999999-9999-9999-9999-999999999999', 'd9999999-9999-9999-9999-999999999991', 'pt000000-0000-0000-0000-000000000009', datetime('now', '-8 days')),
('cr000000-0000-0000-0000-000000000010', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb01', 'd1111111-1111-1111-1111-111111111112', 'pt000000-0000-0000-0000-000000000010', datetime('now', '-3 days')),
('cr000000-0000-0000-0000-000000000011', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb02', 'd2222222-2222-2222-2222-222222222222', 'pt000000-0000-0000-0000-000000000011', datetime('now', '-2 days')),
('cr000000-0000-0000-0000-000000000012', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb03', 'd3333333-3333-3333-3333-333333333332', 'pt000000-0000-0000-0000-000000000012', datetime('now', '-12 days')),
('cr000000-0000-0000-0000-000000000013', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb04', 'd4444444-4444-4444-4444-444444444442', 'pt000000-0000-0000-0000-000000000013', datetime('now', '-4 days')),
('cr000000-0000-0000-0000-000000000014', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb05', 'd5555555-5555-5555-5555-555555555552', 'pt000000-0000-0000-0000-000000000014', datetime('now', '-1 day')),
('cr000000-0000-0000-0000-000000000015', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb06', 'd6666666-6666-6666-6666-666666666662', 'pt000000-0000-0000-0000-000000000015', datetime('now', '-7 days')),
('cr000000-0000-0000-0000-000000000016', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb07', 'd7777777-7777-7777-7777-777777777772', 'pt000000-0000-0000-0000-000000000016', datetime('now', '-3 days')),
('cr000000-0000-0000-0000-000000000017', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb08', 'd8888888-8888-8888-8888-888888888882', 'pt000000-0000-0000-0000-000000000017', datetime('now', '-20 days')),
('cr000000-0000-0000-0000-000000000018', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb09', 'd2222222-2222-2222-2222-222222222223', 'pt000000-0000-0000-0000-000000000018', datetime('now', '-2 days')),
('cr000000-0000-0000-0000-000000000019', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb10', 'd3333333-3333-3333-3333-333333333333', 'pt000000-0000-0000-0000-000000000019', datetime('now', '-9 days')),
('cr000000-0000-0000-0000-000000000020', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbb11', 'd4444444-4444-4444-4444-444444444443', 'pt000000-0000-0000-0000-000000000020', datetime('now', '-5 days')),
('cr000000-0000-0000-0000-000000000021', 'b1111111-1111-1111-1111-111111111111', 'd2222222-2222-2222-2222-222222222224', 'pt000000-0000-0000-0000-000000000021', datetime('now', '-4 days')),
('cr000000-0000-0000-0000-000000000022', 'b2222222-2222-2222-2222-222222222222', 'd3333333-3333-3333-3333-333333333334', 'pt000000-0000-0000-0000-000000000022', datetime('now', '-3 days')),
('cr000000-0000-0000-0000-000000000023', 'b3333333-3333-3333-3333-333333333333', 'd4444444-4444-4444-4444-444444444444', 'pt000000-0000-0000-0000-000000000023', datetime('now', '-6 days')),
('cr000000-0000-0000-0000-000000000024', 'b4444444-4444-4444-4444-444444444444', 'd5555555-5555-5555-5555-555555555553', 'pt000000-0000-0000-0000-000000000024', datetime('now', '-1 day')),
('cr000000-0000-0000-0000-000000000025', 'b5555555-5555-5555-5555-555555555555', 'd6666666-6666-6666-6666-666666666663', 'pt000000-0000-0000-0000-000000000025', datetime('now', '-2 days'));
