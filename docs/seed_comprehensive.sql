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
('95b8706b-3787-4202-8180-424c6045f975', 'Apple Inc.', 'Cupertino, CA', 'Technology company designing and manufacturing consumer electronics, software, and online services', 'https://apple.com', 'Technology', 164000, datetime('now')),
('9d4b41f2-3574-4c82-bc55-fe5e3007a83d', 'Google', 'Mountain View, CA', 'Multinational technology company specializing in Internet-related services and products', 'https://google.com', 'Technology', 190000, datetime('now')),
('07a2e842-9d86-45aa-9748-a5c3db434173', 'Microsoft Corporation', 'Redmond, WA', 'Leading platform and productivity company for the mobile-first, cloud-first world', 'https://microsoft.com', 'Technology', 221000, datetime('now')),
('fcb9b214-66be-4a23-8dea-8f164fde02db', 'Amazon', 'Seattle, WA', 'E-commerce, cloud computing, digital streaming, and artificial intelligence company', 'https://amazon.com', 'E-commerce & Cloud', 1540000, datetime('now')),
('e1f0b643-0bf8-4d43-9fe1-0875edf9d1d9', 'Meta (Facebook)', 'Menlo Park, CA', 'Social technology company building the metaverse and connecting people worldwide', 'https://meta.com', 'Social Media', 86000, datetime('now')),
('387e9f46-d816-40ff-8727-8575cd04692c', 'Netflix', 'Los Gatos, CA', 'World''s leading streaming entertainment service with over 230 million paid memberships', 'https://netflix.com', 'Entertainment', 12800, datetime('now')),
('2d74aaf8-4915-4a39-88c3-f519ad37f17c', 'Airbnb', 'San Francisco, CA', 'Online marketplace for lodging, primarily homestays for vacation rentals and tourism', 'https://airbnb.com', 'Travel & Hospitality', 6800, datetime('now')),
('eb0136e2-a84f-4753-9fe5-d4c59ef576e1', 'Tesla Inc.', 'Austin, TX', 'Electric vehicle and clean energy company designing and manufacturing electric cars', 'https://tesla.com', 'Automotive & Energy', 127855, datetime('now')),
('4f3f0d98-3a5e-448a-bbd9-b7d514a5fb36', 'Goldman Sachs', 'New York, NY', 'Leading global investment banking, securities and investment management firm', 'https://goldmansachs.com', 'Financial Services', 48500, datetime('now')),
('237319df-9e47-4b32-ace8-76866c10ebd0', 'JPMorgan Chase & Co.', 'New York, NY', 'Global financial services firm and one of the largest banking institutions', 'https://jpmorganchase.com', 'Banking', 293723, datetime('now')),
('eb684036-c737-4a6d-b073-878f8869c31e', 'Salesforce', 'San Francisco, CA', 'Cloud-based software company providing customer relationship management (CRM) service', 'https://salesforce.com', 'Software (CRM)', 79000, datetime('now')),
('47658486-12cb-4bd0-a07c-98ba40f476ed', 'Adobe Inc.', 'San Jose, CA', 'Multinational computer software company known for multimedia and creativity software products', 'https://adobe.com', 'Software', 29239, datetime('now')),
('3cef231d-0a29-4d44-8eed-7847e6c6e198', 'Uber Technologies', 'San Francisco, CA', 'Mobility as a service provider offering ride-hailing, food delivery, and freight transport', 'https://uber.com', 'Transportation', 32800, datetime('now')),
('467c2d1e-2eff-4cbf-9d04-62b4054a1cf0', 'Stripe', 'San Francisco, CA', 'Financial services and SaaS company offering payment processing for internet businesses', 'https://stripe.com', 'Fintech', 8000, datetime('now')),
('a70631db-4ab7-486d-9288-5ac6e5ef9513', 'Shopify', 'Ottawa, Canada', 'E-commerce platform helping businesses sell online, in-store, and everywhere in between', 'https://shopify.com', 'E-commerce', 11600, datetime('now'));

-- =====================================================
-- INSERT JOB CATEGORIES (25 Categories)
-- =====================================================
INSERT INTO JobCategories (Id, Name, Description, CategoryType, ParentId) VALUES
-- Root Categories (Composite)
('725927dc-52ce-4ad4-a11e-6eee44873d36', 'Technology', 'All technology-related positions', 'CategoryComposite', NULL),
('b0f91ad0-57ed-48d4-b664-4bc2ed6b7932', 'Business', 'Business and management roles', 'CategoryComposite', NULL),
('a99cf29a-b03e-476b-a6c4-c2ebc908a2c8', 'Design', 'Creative and design positions', 'CategoryComposite', NULL),
('f2c9aa6e-c550-4268-a350-e9ac889b3854', 'Finance', 'Financial and accounting roles', 'CategoryComposite', NULL),
('72aae634-ada1-4043-bc2c-78b72770a5de', 'Operations', 'Operations and logistics roles', 'CategoryComposite', NULL),

-- Technology Sub-categories (Leaf)
('592ed168-0483-40e7-ac48-7b36f2f58095', 'Software Engineering', 'Software development and engineering roles', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('786394cf-4081-43b0-9bd2-80316429b7c5', 'Data Science', 'Data analysis and machine learning roles', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('2b314afe-614c-4547-8ba7-1a1c20bbfa1c', 'DevOps & Infrastructure', 'DevOps, SRE, and infrastructure roles', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('6fe031d2-5e0d-4269-984f-93f7b152ef21', 'Mobile Development', 'iOS and Android development', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('89542706-42cb-4ddf-abc3-d46925f36779', 'Frontend Development', 'Web frontend and UI development', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('a7ce5f5d-8eb5-45fc-95ac-ed174178e2fc', 'Backend Development', 'Server-side and API development', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('e324c5ef-341f-4f6b-a8b2-f5f69a937fda', 'Security Engineering', 'Cybersecurity and information security', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),
('0f12daad-a623-498a-9a14-9eefb38be0f2', 'Cloud Architecture', 'Cloud infrastructure and architecture', 'CategoryLeaf', '725927dc-52ce-4ad4-a11e-6eee44873d36'),

-- Business Sub-categories (Leaf)
('047dc3bf-f05b-4aa7-b51e-a058783c5d18', 'Product Management', 'Product strategy and management', 'CategoryLeaf', 'b0f91ad0-57ed-48d4-b664-4bc2ed6b7932'),
('9afe3bf9-f740-4cd2-b0c0-6a891bb67264', 'Project Management', 'Project planning and execution', 'CategoryLeaf', 'b0f91ad0-57ed-48d4-b664-4bc2ed6b7932'),
('e8423a00-e3fc-4dee-b26b-3992d2b1227e', 'Sales', 'Sales and business development', 'CategoryLeaf', 'b0f91ad0-57ed-48d4-b664-4bc2ed6b7932'),
('9251a129-5393-4b50-b8cc-73c7dd499c72', 'Marketing', 'Marketing and growth roles', 'CategoryLeaf', 'b0f91ad0-57ed-48d4-b664-4bc2ed6b7932'),
('51fc0d42-284b-4803-be32-073de897179f', 'Business Analysis', 'Business analysis and strategy', 'CategoryLeaf', 'b0f91ad0-57ed-48d4-b664-4bc2ed6b7932'),

-- Design Sub-categories (Leaf)
('0030dbf8-d0af-4eb8-86ef-38e5cd63bc6c', 'UX/UI Design', 'User experience and interface design', 'CategoryLeaf', 'a99cf29a-b03e-476b-a6c4-c2ebc908a2c8'),
('6dc55785-aede-4971-a703-de56722b5ee3', 'Product Design', 'Product and interaction design', 'CategoryLeaf', 'a99cf29a-b03e-476b-a6c4-c2ebc908a2c8'),
('090820a7-f4d8-476f-8791-f86a94c9b819', 'Graphic Design', 'Visual and graphic design', 'CategoryLeaf', 'a99cf29a-b03e-476b-a6c4-c2ebc908a2c8'),

-- Finance Sub-categories (Leaf)
('872e8b94-343c-4d9f-b1bf-a0d2cb7a9060', 'Financial Analysis', 'Financial planning and analysis', 'CategoryLeaf', 'f2c9aa6e-c550-4268-a350-e9ac889b3854'),
('6cedc5e4-db50-4a55-b941-9e766ce667d1', 'Accounting', 'Accounting and bookkeeping roles', 'CategoryLeaf', 'f2c9aa6e-c550-4268-a350-e9ac889b3854'),
('1093817a-3d40-459b-8a77-2dd2743030af', 'Investment Banking', 'Investment banking and M&A', 'CategoryLeaf', 'f2c9aa6e-c550-4268-a350-e9ac889b3854'),

-- Operations Sub-categories (Leaf)
('dfb72408-271d-4eac-97b9-33f7a2828dff', 'Supply Chain', 'Supply chain and logistics', 'CategoryLeaf', '72aae634-ada1-4043-bc2c-78b72770a5de'),
('5fec9a50-c800-4f9a-8be8-19327ccfec2a', 'Quality Assurance', 'QA and testing roles', 'CategoryLeaf', '72aae634-ada1-4043-bc2c-78b72770a5de'),
('72cc410f-3bfd-4c33-bcad-c28d75dddca3', 'Customer Support', 'Customer service and support', 'CategoryLeaf', '72aae634-ada1-4043-bc2c-78b72770a5de');

-- =====================================================
-- INSERT JOB SEEKERS (20 Job Seekers)
-- =====================================================
INSERT INTO Users (Id, Email, FirstName, LastName, PasswordHash, CreatedAt, IsActive, UserType, Discriminator, PhoneNumber, LastLoginAt, Resume, Skills, Address, DateOfBirth, ProfessionalSummary, LinkedInUrl, GitHubUrl, PortfolioUrl) VALUES
('8eef28c3-d76c-4f70-8a20-bf4fe669650a', 'emily.chen@email.com', 'Emily', 'Chen', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('4a558171-abf6-42f3-bc91-b8ce2a12c621', 'michael.rodriguez@email.com', 'Michael', 'Rodriguez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('dacafacb-44bb-4213-8db4-b2c5c1f455c9', 'sarah.johnson@email.com', 'Sarah', 'Johnson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('edb363a6-237a-4bb5-be80-698a63a9d34e', 'david.kim@email.com', 'David', 'Kim', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('99daaccd-f55c-41fb-a533-adccbc48c232', 'jessica.taylor@email.com', 'Jessica', 'Taylor', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('c2341773-25c9-4c15-bae8-b5a8f93768ae', 'james.anderson@email.com', 'James', 'Anderson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('91057f3d-1e5c-4f40-945f-640ac892de0c', 'amanda.martinez@email.com', 'Amanda', 'Martinez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('9936992a-5e5c-4d92-9caf-9635cdfc2224', 'ryan.thomas@email.com', 'Ryan', 'Thomas', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('41b53468-a703-44ff-ba6c-f359ca9e56cc', 'nicole.jackson@email.com', 'Nicole', 'Jackson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('2c913984-35c7-4908-9969-f35c848af42e', 'kevin.white@email.com', 'Kevin', 'White', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('a5d624a1-a995-41e8-aa78-dcbf614615f0', 'lisa.harris@email.com', 'Lisa', 'Harris', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('d9c4b1f2-fc90-489a-a5f1-fa8bd130a2c1', 'daniel.clark@email.com', 'Daniel', 'Clark', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('97c9de39-034a-4bf2-9089-7a32a0ab15ff', 'rachel.lewis@email.com', 'Rachel', 'Lewis', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('22960242-b24f-419c-80bf-33917142e146', 'chris.walker@email.com', 'Chris', 'Walker', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('bbd201a5-aee5-4b95-b0d5-05c5b6d71aaf', 'sophia.hall@email.com', 'Sophia', 'Hall', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('e3bb78da-4214-4f07-b279-32b594ca4cdc', 'brandon.allen@email.com', 'Brandon', 'Allen', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('93f99443-0467-42b4-927c-f14a88d5a9b8', 'olivia.young@email.com', 'Olivia', 'Young', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('10db06d7-28f7-47de-889d-02260eaa48c7', 'matthew.king@email.com', 'Matthew', 'King', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('b3d92585-05b7-47d6-a05b-bd806b2752cc', 'ashley.wright@email.com', 'Ashley', 'Wright', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL),
('920ba6fd-f058-495a-a400-e99c9272b7ce', 'joshua.lopez@email.com', 'Joshua', 'Lopez', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 1, 'JobSeeker', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);

-- =====================================================
-- INSERT EMPLOYERS (15 Employers - one per company)
-- =====================================================
INSERT INTO Users (Id, Email, FirstName, LastName, PasswordHash, CreatedAt, IsActive, UserType, Discriminator, CompanyId, Position, PhoneNumber, LastLoginAt) VALUES
('024058d7-0fb4-4933-80e5-bf4f3d48aed2', 'hr.apple@company.com', 'Jennifer', 'Smith', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '95b8706b-3787-4202-8180-424c6045f975', 'HR Manager', NULL, NULL),
('5074ca2f-67f3-401e-aff5-2eff82fc5dfa', 'recruiting.google@company.com', 'Robert', 'Johnson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '9d4b41f2-3574-4c82-bc55-fe5e3007a83d', 'Recruiting Manager', NULL, NULL),
('0e2dc1b3-7892-4fe5-8621-30c82100ba1e', 'talent.microsoft@company.com', 'Maria', 'Garcia', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '07a2e842-9d86-45aa-9748-a5c3db434173', 'Talent Acquisition', NULL, NULL),
('e9eb6bb1-7b28-41c9-9f7c-96b63164d1f0', 'hiring.amazon@company.com', 'William', 'Brown', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'fcb9b214-66be-4a23-8dea-8f164fde02db', 'Hiring Manager', NULL, NULL),
('06061db6-cbf6-4ee8-a314-9c2a867210fd', 'careers.meta@company.com', 'Patricia', 'Davis', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'e1f0b643-0bf8-4d43-9fe1-0875edf9d1d9', 'Careers Lead', NULL, NULL),
('05386418-39ad-4389-9fcd-3a7dd0ec2c42', 'jobs.netflix@company.com', 'Richard', 'Miller', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '387e9f46-d816-40ff-8727-8575cd04692c', 'Talent Manager', NULL, NULL),
('5d01e617-7a2e-4bcb-9843-1c2454d6fb7d', 'talent.airbnb@company.com', 'Linda', 'Wilson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '2d74aaf8-4915-4a39-88c3-f519ad37f17c', 'Recruitment Lead', NULL, NULL),
('d1cdd453-3b5f-46b5-a0b5-bb9ece52d2f4', 'hr.tesla@company.com', 'Thomas', 'Moore', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'eb0136e2-a84f-4753-9fe5-d4c59ef576e1', 'HR Director', NULL, NULL),
('b96bba64-d1a9-425d-9b95-81dc9782a8c9', 'recruiting.goldman@company.com', 'Barbara', 'Taylor', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '4f3f0d98-3a5e-448a-bbd9-b7d514a5fb36', 'Recruiting Director', NULL, NULL),
('1adf35fc-387f-4ad3-b275-ea5495101188', 'hr.jpmorgan@company.com', 'Christopher', 'Anderson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '237319df-9e47-4b32-ace8-76866c10ebd0', 'VP Talent', NULL, NULL),
('ea67a317-dd49-4fd8-a84d-01a0935a2c9a', 'talent.salesforce@company.com', 'Nancy', 'Thomas', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'eb684036-c737-4a6d-b073-878f8869c31e', 'Talent Acquisition', NULL, NULL),
('c0613a94-c7a2-4bea-b0d1-d225526d4bfe', 'careers.adobe@company.com', 'Steven', 'Jackson', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '47658486-12cb-4bd0-a07c-98ba40f476ed', 'Hiring Manager', NULL, NULL),
('0542a479-484b-4a5f-ab65-7b784c12f893', 'jobs.uber@company.com', 'Karen', 'White', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '3cef231d-0a29-4d44-8eed-7847e6c6e198', 'Recruiting Lead', NULL, NULL),
('1c63f5e1-c6a3-409a-96e9-6acf5f14747d', 'hiring.stripe@company.com', 'Kevin', 'Harris', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', '467c2d1e-2eff-4cbf-9d04-62b4054a1cf0', 'Head of Talent', NULL, NULL),
('53fae379-75bf-4ea5-b10b-ade3918244c7', 'talent.shopify@company.com', 'Lisa', 'Martin', 'cGFzc3dvcmQxMjM=', datetime('now'), 1, 2, 'Employer', 'a70631db-4ab7-486d-9288-5ac6e5ef9513', 'Recruitment Manager', NULL, NULL);

-- =====================================================
-- INSERT JOB POSTINGS (30 Job Postings)
-- =====================================================
INSERT INTO JobPostings (Id, Title, Description, Requirements, SalaryMin, SalaryMax, Location, EmploymentType, Category, EmployerId, CompanyId, Status, PostedDate, ClosedDate, ExpiryDate, ExperienceLevel, IsCompanyRevealed, CategoryId) VALUES
-- Apple Jobs
('399f35ed-3d17-407e-bd84-ff2de9cdeca2', 'Senior iOS Engineer', 'Join Apple''s world-class iOS team to build the next generation of iOS experiences used by millions daily.', '5+ years iOS development, Swift, SwiftUI, Objective-C, strong CS fundamentals', '180000', '250000', 'Cupertino, CA', 'Full-time', 'Software Development', '024058d7-0fb4-4933-80e5-bf4f3d48aed2', '95b8706b-3787-4202-8180-424c6045f975', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '6fe031d2-5e0d-4269-984f-93f7b152ef21'),
('7442e153-e5cf-46c5-843f-80410e47aeaf', 'Machine Learning Engineer', 'Be part of Apple''s ML team developing cutting-edge solutions for Siri, Photos, and more.', 'PhD or MS in CS/ML, TensorFlow, PyTorch, Python, 3+ years ML experience', '200000', '280000', 'Cupertino, CA', 'Full-time', 'Machine Learning', '024058d7-0fb4-4933-80e5-bf4f3d48aed2', '95b8706b-3787-4202-8180-424c6045f975', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '786394cf-4081-43b0-9bd2-80316429b7c5'),

-- Google Jobs
('e556bacd-8986-457d-857c-6a774c6cd309', 'Senior Software Engineer', 'Build technologies that help billions of people connect, explore, and interact with information.', 'BS in CS, 5+ years software development, Java/C++/Python, distributed systems', '190000', '270000', 'Mountain View, CA', 'Full-time', 'Software Development', '5074ca2f-67f3-401e-aff5-2eff82fc5dfa', '9d4b41f2-3574-4c82-bc55-fe5e3007a83d', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '592ed168-0483-40e7-ac48-7b36f2f58095'),
('40e50630-c055-4393-acaa-59b05b9483e5', 'Site Reliability Engineer', 'Ensure Google''s services are reliable and scalable with 99.99% uptime.', 'Linux/Unix, Python/Go, networking, 4+ years SRE/DevOps experience', '170000', '240000', 'Mountain View, CA', 'Full-time', 'DevOps/SRE', '5074ca2f-67f3-401e-aff5-2eff82fc5dfa', '9d4b41f2-3574-4c82-bc55-fe5e3007a83d', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '2b314afe-614c-4547-8ba7-1a1c20bbfa1c'),
('24f7b6f2-1612-41e9-b04d-8db25a0b3d1c', 'UX Researcher', 'Lead research initiatives to understand user needs and behaviors. Shape product decisions.', 'MS/PhD in HCI, 3+ years UX research, quantitative/qualitative methods', '140000', '190000', 'Mountain View, CA', 'Full-time', 'Research', '5074ca2f-67f3-401e-aff5-2eff82fc5dfa', '9d4b41f2-3574-4c82-bc55-fe5e3007a83d', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '0030dbf8-d0af-4eb8-86ef-38e5cd63bc6c'),
('5dcd57a1-b7a4-48c8-8893-61db43d0c98a', 'Product Manager', 'Drive product strategy and roadmap for key Google products with cross-functional teams.', 'MBA or 5+ years PM experience, technical background, data-driven', '175000', '260000', 'Mountain View, CA', 'Full-time', 'Product Management', '5074ca2f-67f3-401e-aff5-2eff82fc5dfa', '9d4b41f2-3574-4c82-bc55-fe5e3007a83d', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '047dc3bf-f05b-4aa7-b51e-a058783c5d18'),

-- Microsoft Jobs
('14e5f869-603c-4724-a959-518d44700f12', 'Cloud Solution Architect', 'Help customers architect and deploy solutions on Microsoft Azure. Lead technical engagements.', 'Azure certifications, 5+ years cloud architecture, C#/.NET, microservices', '165000', '230000', 'Redmond, WA', 'Full-time', 'Cloud Architecture', '0e2dc1b3-7892-4fe5-8621-30c82100ba1e', '07a2e842-9d86-45aa-9748-a5c3db434173', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '0f12daad-a623-498a-9a14-9eefb38be0f2'),
('a62193bf-ed63-4b42-a5a4-ba2d1e8c1d19', 'Principal Software Engineer', 'Lead engineering teams building the next generation of Microsoft 365. Drive technical excellence.', '10+ years software development, C#, TypeScript, distributed systems, leadership', '210000', '300000', 'Redmond, WA', 'Full-time', 'Software Development', '0e2dc1b3-7892-4fe5-8621-30c82100ba1e', '07a2e842-9d86-45aa-9748-a5c3db434173', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Staff', 1, '592ed168-0483-40e7-ac48-7b36f2f58095'),
('36d63e46-173e-4964-8b22-9d32bc6507ff', 'AI Research Scientist', 'Conduct cutting-edge AI research at Microsoft Research. Publish papers and develop AI tech.', 'PhD in CS/ML/AI, published research, deep learning, NLP or computer vision', '190000', '280000', 'Redmond, WA', 'Full-time', 'Research', '0e2dc1b3-7892-4fe5-8621-30c82100ba1e', '07a2e842-9d86-45aa-9748-a5c3db434173', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '786394cf-4081-43b0-9bd2-80316429b7c5'),
('a60f725f-bbbf-4f87-9c68-892a5ae24603', 'Full Stack Developer', 'Build modern web applications for Microsoft''s cloud services with React, Node.js, and Azure.', '4+ years full-stack development, React, Node.js, SQL, REST APIs', '140000', '190000', 'Redmond, WA', 'Full-time', 'Web Development', '0e2dc1b3-7892-4fe5-8621-30c82100ba1e', '07a2e842-9d86-45aa-9748-a5c3db434173', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '592ed168-0483-40e7-ac48-7b36f2f58095'),

-- Amazon Jobs
('94c9e586-4103-4c37-bea4-e17771e03202', 'Software Development Engineer', 'Build and scale Amazon''s e-commerce platform handling millions of transactions daily.', 'CS degree, 3+ years Java/Python/C++, algorithms, data structures, AWS', '150000', '220000', 'Seattle, WA', 'Full-time', 'Software Development', 'e9eb6bb1-7b28-41c9-9f7c-96b63164d1f0', 'fcb9b214-66be-4a23-8dea-8f164fde02db', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '592ed168-0483-40e7-ac48-7b36f2f58095'),
('f900d9a6-f65f-4a25-8202-439556b3b002', 'Data Scientist', 'Drive data-driven decisions across Amazon. Build ML models for recommendations and pricing.', 'MS/PhD, Python, R, SQL, machine learning, statistics, 3+ years', '160000', '230000', 'Seattle, WA', 'Full-time', 'Data Science', 'e9eb6bb1-7b28-41c9-9f7c-96b63164d1f0', 'fcb9b214-66be-4a23-8dea-8f164fde02db', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '786394cf-4081-43b0-9bd2-80316429b7c5'),
('da0d7973-fad2-4e77-9dd3-748ec330f5c4', 'DevOps Engineer', 'Build and maintain infrastructure for Amazon''s services. Automate deployments.', 'AWS, Docker, Kubernetes, CI/CD, Python/Bash, 4+ years DevOps', '145000', '200000', 'Seattle, WA', 'Full-time', 'DevOps', 'e9eb6bb1-7b28-41c9-9f7c-96b63164d1f0', 'fcb9b214-66be-4a23-8dea-8f164fde02db', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '2b314afe-614c-4547-8ba7-1a1c20bbfa1c'),
('99bc67f4-16a7-41f4-9bf9-cefefa63a57c', 'Solutions Architect', 'Help enterprise customers design and implement solutions on AWS as a technical advisor.', 'AWS certifications, 5+ years solution architecture, customer-facing', '170000', '240000', 'Seattle, WA', 'Full-time', 'Solutions Architecture', 'e9eb6bb1-7b28-41c9-9f7c-96b63164d1f0', 'fcb9b214-66be-4a23-8dea-8f164fde02db', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 1, '0f12daad-a623-498a-9a14-9eefb38be0f2'),

-- Meta Jobs
('3398e7f2-5796-4a74-953e-7b3ef86ed227', 'Software Engineer, Mobile', 'Build mobile experiences for billions on Facebook, Instagram, WhatsApp with React Native.', '3+ years mobile development, iOS/Android, React Native, CS fundamentals', '170000', '250000', 'Menlo Park, CA', 'Full-time', 'Mobile Development', '06061db6-cbf6-4ee8-a314-9c2a867210fd', 'e1f0b643-0bf8-4d43-9fe1-0875edf9d1d9', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '6fe031d2-5e0d-4269-984f-93f7b152ef21'),
('b3e057f1-427b-4c35-b809-420f0fd8adee', 'Data Engineer', 'Build and maintain data infrastructure supporting Meta''s products. Process petabytes daily.', '4+ years data engineering, Spark, Hadoop, SQL, Python, distributed systems', '160000', '230000', 'Menlo Park, CA', 'Full-time', 'Data Engineering', '06061db6-cbf6-4ee8-a314-9c2a867210fd', 'e1f0b643-0bf8-4d43-9fe1-0875edf9d1d9', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 1, '786394cf-4081-43b0-9bd2-80316429b7c5'),
('77982ff2-a7b7-4b1f-8814-f575aabeb8f7', 'VR/AR Engineer', 'Shape the future of the metaverse. Build immersive VR/AR for Meta Quest.', 'Unity/Unreal Engine, C++, computer graphics, 3+ years VR/AR', '175000', '255000', 'Menlo Park, CA', 'Full-time', 'VR/AR Development', '06061db6-cbf6-4ee8-a314-9c2a867210fd', 'e1f0b643-0bf8-4d43-9fe1-0875edf9d1d9', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '592ed168-0483-40e7-ac48-7b36f2f58095'),

-- Netflix Jobs
('21157351-204a-4b3e-901b-fa2bde3a6eaf', 'Senior Backend Engineer', 'Build microservices powering Netflix''s streaming for 230M+ members globally.', '5+ years backend, Java/Kotlin/Go, microservices, AWS, distributed systems', '180000', '260000', 'Los Gatos, CA', 'Full-time', 'Backend Development', '05386418-39ad-4389-9fcd-3a7dd0ec2c42', '387e9f46-d816-40ff-8727-8575cd04692c', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, 'a7ce5f5d-8eb5-45fc-95ac-ed174178e2fc'),
('67d27190-5113-4412-b39d-73a281c4f3c7', 'Content Recommendation Engineer', 'Build ML models recommending content to Netflix members. Drive engagement.', 'ML/AI experience, Python, TensorFlow, recommendation systems, 4+ years', '175000', '250000', 'Los Gatos, CA', 'Full-time', 'Machine Learning', '05386418-39ad-4389-9fcd-3a7dd0ec2c42', '387e9f46-d816-40ff-8727-8575cd04692c', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, '786394cf-4081-43b0-9bd2-80316429b7c5'),
('ac436caf-bad2-46f2-83e8-85215b9e8f8d', 'UI Engineer', 'Create stunning UI experiences for Netflix on all devices with React.', '4+ years frontend, React, TypeScript, CSS, responsive design, performance', '155000', '220000', 'Los Gatos, CA', 'Full-time', 'Frontend Development', '05386418-39ad-4389-9fcd-3a7dd0ec2c42', '387e9f46-d816-40ff-8727-8575cd04692c', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '89542706-42cb-4ddf-abc3-d46925f36779'),

-- Airbnb Jobs
('00abc720-0026-4d55-bf4c-6af19c18b3ce', 'Full Stack Engineer', 'Build features for Airbnb''s platform connecting hosts and guests worldwide.', '3+ years full-stack, React, Node.js, Ruby on Rails, PostgreSQL, AWS', '160000', '230000', 'San Francisco, CA', 'Full-time', 'Full Stack', '5d01e617-7a2e-4bcb-9843-1c2454d6fb7d', '2d74aaf8-4915-4a39-88c3-f519ad37f17c', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '592ed168-0483-40e7-ac48-7b36f2f58095'),
('438d6108-bd0f-45be-a37e-04ec14145bb6', 'Product Designer', 'Design delightful experiences for Airbnb''s community. Create user-centered designs.', '4+ years product design, Figma, user research, prototyping, design systems', '150000', '210000', 'San Francisco, CA', 'Full-time', 'Product Design', '5d01e617-7a2e-4bcb-9843-1c2454d6fb7d', '2d74aaf8-4915-4a39-88c3-f519ad37f17c', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '6dc55785-aede-4971-a703-de56722b5ee3'),

-- Tesla Jobs
('98c893ea-dde1-4d6b-a316-42d812981bcb', 'Embedded Software Engineer', 'Develop embedded software for Tesla vehicles. Work on Autopilot and battery management.', 'C/C++, embedded Linux, real-time systems, CAN bus, 4+ years embedded', '140000', '210000', 'Austin, TX', 'Full-time', 'Embedded Systems', 'd1cdd453-3b5f-46b5-a0b5-bb9ece52d2f4', 'eb0136e2-a84f-4753-9fe5-d4c59ef576e1', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '592ed168-0483-40e7-ac48-7b36f2f58095'),
('0a97e325-ac9d-49ac-a017-fbf40af62d10', 'Computer Vision Engineer', 'Build perception systems for Tesla''s Autopilot with camera and sensor data.', 'Deep learning, computer vision, Python, C++, CUDA, autonomous systems', '165000', '240000', 'Austin, TX', 'Full-time', 'Computer Vision', 'd1cdd453-3b5f-46b5-a0b5-bb9ece52d2f4', 'eb0136e2-a84f-4753-9fe5-d4c59ef576e1', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, '786394cf-4081-43b0-9bd2-80316429b7c5'),

-- Goldman Sachs Jobs
('fd241ea7-8dee-4ee8-804f-a9ed99ff13ec', 'Quantitative Developer', 'Build trading systems and risk management platforms with cutting-edge fintech.', 'CS/Math degree, C++/Java, financial markets, algorithms, 3+ years', '170000', '250000', 'New York, NY', 'Full-time', 'Quantitative Dev', 'b96bba64-d1a9-425d-9b95-81dc9782a8c9', '4f3f0d98-3a5e-448a-bbd9-b7d514a5fb36', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '592ed168-0483-40e7-ac48-7b36f2f58095'),
('7a505f85-8e33-485d-b874-c8bf8ae4602e', 'Investment Banking Analyst', 'Work on M&A transactions, IPOs, and strategic advisory. High-profile deals.', 'Top university degree, finance knowledge, Excel/PowerPoint, analytical', '110000', '150000', 'New York, NY', 'Full-time', 'Investment Banking', 'b96bba64-d1a9-425d-9b95-81dc9782a8c9', '4f3f0d98-3a5e-448a-bbd9-b7d514a5fb36', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Entry-Level', 0, '1093817a-3d40-459b-8a77-2dd2743030af'),

-- Remaining companies
('f6ea05f4-f865-4e44-9a5c-f5169053e593', 'Cybersecurity Engineer', 'Protect JPMorgan''s systems and customer data. Implement security controls.', 'Security certifications, penetration testing, SIEM, incident response, 3+', '125000', '185000', 'New York, NY', 'Full-time', 'Cybersecurity', '1adf35fc-387f-4ad3-b275-ea5495101188', '237319df-9e47-4b32-ace8-76866c10ebd0', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, 'e324c5ef-341f-4f6b-a8b2-f5f69a937fda'),
('8a68f0c4-6559-497e-b272-2e4c51190fd7', 'Frontend Engineer', 'Build Stripe''s dashboard and developer tools. Create beautiful, fast web apps.', '4+ years frontend, React, TypeScript, performance, accessibility, design', '160000', '230000', 'San Francisco, CA', 'Full-time', 'Frontend', '1c63f5e1-c6a3-409a-96e9-6acf5f14747d', '467c2d1e-2eff-4cbf-9d04-62b4054a1cf0', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Mid-Level', 0, '89542706-42cb-4ddf-abc3-d46925f36779'),
('4bfc12b6-bbb8-4be2-8c35-0f697450547a', 'Senior Product Designer', 'Design merchant and consumer experiences for Shopify''s platform.', '5+ years product design, e-commerce, Figma, user research, systems thinking', '135000', '190000', 'Ottawa, Canada', 'Full-time', 'Product Design', '53fae379-75bf-4ea5-b10b-ade3918244c7', 'a70631db-4ab7-486d-9288-5ac6e5ef9513', 2, datetime('now'), NULL, datetime('now', '+30 days'), 'Senior', 0, '6dc55785-aede-4971-a703-de56722b5ee3');

-- =====================================================
-- INSERT JOB APPLICATIONS (25 Applications)
-- =====================================================
INSERT INTO JobApplications (Id, JobPostingId, JobSeekerId, CoverLetter, ResumeUrl, Status, AppliedDate, ReviewedDate, ReviewNotes, ExpectedSalary, PortfolioLink, AvailableStartDate, AdditionalInfo) VALUES
('df34a000-b35a-499a-b172-5e91500aa76d', '399f35ed-3d17-407e-bd84-ff2de9cdeca2', '8eef28c3-d76c-4f70-8a20-bf4fe669650a', 'I''m very excited about the iOS Engineer position at Apple. My 5 years of Swift development and passion for intuitive UX make me a perfect fit.', NULL, 0, datetime('now', '-5 days'), NULL, NULL, '200000', NULL, NULL, NULL),
('e0c120db-4c39-4e88-828c-8ef6e477995d', 'e556bacd-8986-457d-857c-6a774c6cd309', '4a558171-abf6-42f3-bc91-b8ce2a12c621', 'I''m interested in joining Google as a Senior Software Engineer. My distributed systems experience aligns perfectly with this role.', NULL, 0, datetime('now', '-4 days'), NULL, NULL, '220000', NULL, NULL, NULL),
('04717483-fda0-46fa-8991-c444315b6508', '14e5f869-603c-4724-a959-518d44700f12', 'dacafacb-44bb-4213-8db4-b2c5c1f455c9', 'I''m excited to apply for the Cloud Solution Architect role at Microsoft. My Azure certifications and 6 years of cloud experience are ideal.', NULL, 2, datetime('now', '-10 days'), datetime('now', '-2 days'), 'Strong technical background', '190000', NULL, NULL, NULL),
('6ed32770-9654-4216-bcac-c51938c71841', '94c9e586-4103-4c37-bea4-e17771e03202', 'edb363a6-237a-4bb5-be80-698a63a9d34e', 'I would love to join Amazon as an SDE. My CS fundamentals and AWS experience would let me contribute immediately.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '180000', NULL, NULL, NULL),
('8ebf7eff-ff09-4e09-a454-ab0aceb46ae3', '3398e7f2-5796-4a74-953e-7b3ef86ed227', '99daaccd-f55c-41fb-a533-adccbc48c232', 'Applying for Mobile Engineer at Meta. My React Native expertise and experience with millions of users would be valuable.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '210000', NULL, NULL, NULL),
('cc051782-aa25-478a-858f-62b4811ba177', '21157351-204a-4b3e-901b-fa2bde3a6eaf', 'c2341773-25c9-4c15-bae8-b5a8f93768ae', 'Very interested in the Senior Backend Engineer role at Netflix. My microservices architecture experience aligns perfectly.', NULL, 2, datetime('now', '-15 days'), datetime('now', '-5 days'), 'Excellent candidate, moving to final round', '220000', NULL, NULL, NULL),
('29b35dac-f2fd-481d-ba86-bff20d777bc9', '00abc720-0026-4d55-bf4c-6af19c18b3ce', '91057f3d-1e5c-4f40-945f-640ac892de0c', 'Would love to join Airbnb as a Full Stack Engineer. My React and Rails experience matches your tech stack perfectly.', NULL, 0, datetime('now', '-1 day'), NULL, NULL, '190000', NULL, NULL, NULL),
('d701197b-4991-442e-9fc9-3c6f09f63ed4', '98c893ea-dde1-4d6b-a316-42d812981bcb', '9936992a-5e5c-4d92-9caf-9635cdfc2224', 'Excited about the Embedded Software Engineer position at Tesla. My automotive software background is a great fit.', NULL, 0, datetime('now', '-6 days'), NULL, NULL, '170000', NULL, NULL, NULL),
('ce1c1016-3f44-401c-a132-2435121e197c', 'fd241ea7-8dee-4ee8-804f-a9ed99ff13ec', '41b53468-a703-44ff-ba6c-f359ca9e56cc', 'Applying for Quantitative Developer at Goldman Sachs. My strong math and C++ skills align with requirements.', NULL, 2, datetime('now', '-8 days'), datetime('now', '-1 day'), 'Good technical skills', '200000', NULL, NULL, NULL),
('551eb6b5-daee-4a05-b2d3-819a31ac424f', '7442e153-e5cf-46c5-843f-80410e47aeaf', '2c913984-35c7-4908-9969-f35c848af42e', 'Interested in the Machine Learning Engineer position at Apple. My PhD in ML and TensorFlow expertise are perfect.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '240000', NULL, NULL, NULL),
('e1a8c84d-9843-4034-afc3-a52a86db4da6', '40e50630-c055-4393-acaa-59b05b9483e5', 'a5d624a1-a995-41e8-aa78-dcbf614615f0', 'Applying for SRE at Google. My 5 years of DevOps and system reliability experience would be valuable.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '200000', NULL, NULL, NULL),
('f01ec484-7c5a-484c-afc8-4a47a21be59e', 'a62193bf-ed63-4b42-a5a4-ba2d1e8c1d19', 'd9c4b1f2-fc90-489a-a5f1-fa8bd130a2c1', 'Would love the Principal Engineer role at Microsoft. My 12 years of experience and leadership skills are a match.', NULL, 2, datetime('now', '-12 days'), datetime('now', '-3 days'), 'Strong leadership qualities', '270000', NULL, NULL, NULL),
('c2e5f2c7-5470-4e78-ad8c-ed7bc75030e3', 'f900d9a6-f65f-4a25-8202-439556b3b002', '97c9de39-034a-4bf2-9089-7a32a0ab15ff', 'Excited to apply for Data Scientist at Amazon. My ML models have driven significant business impact.', NULL, 0, datetime('now', '-4 days'), NULL, NULL, '190000', NULL, NULL, NULL),
('6cf58a3d-e4b2-44e8-a1d0-af4b3536d172', 'b3e057f1-427b-4c35-b809-420f0fd8adee', '22960242-b24f-419c-80bf-33917142e146', 'Applying for Data Engineer at Meta. My experience with petabyte-scale data processing is relevant.', NULL, 0, datetime('now', '-1 day'), NULL, NULL, '190000', NULL, NULL, NULL),
('f005ee08-591a-484d-9945-c8e5c7002fe2', '67d27190-5113-4412-b39d-73a281c4f3c7', 'bbd201a5-aee5-4b95-b0d5-05c5b6d71aaf', 'Very interested in the Recommendation Engineer role at Netflix. Building recommendation systems is my passion.', NULL, 0, datetime('now', '-7 days'), NULL, NULL, '210000', NULL, NULL, NULL),
('3b233cec-2598-4908-a924-422f91aac494', '438d6108-bd0f-45be-a37e-04ec14145bb6', 'e3bb78da-4214-4f07-b279-32b594ca4cdc', 'Would love to join Airbnb as a Product Designer. My user-centered design approach aligns with your values.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '180000', NULL, NULL, NULL),
('188e55b7-c63b-4aa0-b67e-44e0ce5bc3a1', '0a97e325-ac9d-49ac-a017-fbf40af62d10', '93f99443-0467-42b4-927c-f14a88d5a9b8', 'Excited about the Computer Vision Engineer role at Tesla. Autonomous driving is where I want to make an impact.', NULL, 2, datetime('now', '-20 days'), datetime('now', '-8 days'), 'Excellent CV background, final interview scheduled', '200000', NULL, NULL, NULL),
('e28d88dc-9c32-44c6-8673-66bba73b3c55', '24f7b6f2-1612-41e9-b04d-8db25a0b3d1c', '10db06d7-28f7-47de-889d-02260eaa48c7', 'Applying for UX Researcher at Google. My qualitative and quantitative research skills would be valuable.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '160000', NULL, NULL, NULL),
('52d34c2f-ec58-4f58-a0ec-44b62d64577b', '36d63e46-173e-4964-8b22-9d32bc6507ff', 'b3d92585-05b7-47d6-a05b-bd806b2752cc', 'Very interested in the AI Research Scientist role at Microsoft. My published research in NLP aligns perfectly.', NULL, 2, datetime('now', '-9 days'), datetime('now', '-2 days'), 'Strong research portfolio', '250000', NULL, NULL, NULL),
('3501c786-5cee-4ec9-beda-4035d85e05ed', 'da0d7973-fad2-4e77-9dd3-748ec330f5c4', '920ba6fd-f058-495a-a400-e99c9272b7ce', 'Excited to apply for DevOps Engineer at Amazon. My AWS and Kubernetes experience is extensive.', NULL, 0, datetime('now', '-5 days'), NULL, NULL, '170000', NULL, NULL, NULL),
('63c9fa9c-b567-4b20-a217-ce1212f7aebe', '5dcd57a1-b7a4-48c8-8893-61db43d0c98a', '8eef28c3-d76c-4f70-8a20-bf4fe669650a', 'Interested in the Product Manager role at Google. My technical background and PM experience are ideal.', NULL, 0, datetime('now', '-4 days'), NULL, NULL, '220000', NULL, NULL, NULL),
('ff7a46dd-eb50-4e2f-b30a-1b565a15dcb7', 'a60f725f-bbbf-4f87-9c68-892a5ae24603', '4a558171-abf6-42f3-bc91-b8ce2a12c621', 'Would love the Full Stack Developer role at Microsoft. My React and Node.js skills match perfectly.', NULL, 0, datetime('now', '-3 days'), NULL, NULL, '165000', NULL, NULL, NULL),
('a469ab2d-00fc-43e9-800a-31454b0f43c5', '99bc67f4-16a7-41f4-9bf9-cefefa63a57c', 'dacafacb-44bb-4213-8db4-b2c5c1f455c9', 'Applying for Solutions Architect at Amazon. My customer-facing experience and AWS knowledge are strong.', NULL, 0, datetime('now', '-6 days'), NULL, NULL, '200000', NULL, NULL, NULL),
('359d8ee6-5d09-46c6-9106-a83586dcb4e8', '77982ff2-a7b7-4b1f-8814-f575aabeb8f7', 'edb363a6-237a-4bb5-be80-698a63a9d34e', 'Excited about the VR/AR Engineer position at Meta. Building the metaverse is my dream job.', NULL, 0, datetime('now', '-1 day'), NULL, NULL, '210000', NULL, NULL, NULL),
('207cc1a6-3dd4-44bd-947a-23cb77d9eb3c', 'ac436caf-bad2-46f2-83e8-85215b9e8f8d', '99daaccd-f55c-41fb-a533-adccbc48c232', 'Interested in the UI Engineer role at Netflix. My React and performance optimization skills are excellent.', NULL, 0, datetime('now', '-2 days'), NULL, NULL, '185000', NULL, NULL, NULL);

-- =====================================================
-- INSERT PAYMENT TRANSACTIONS (25 Transactions)
-- =====================================================
INSERT INTO PaymentTransactions (Id, UserId, Amount, Currency, Gateway, Status, TransactionDate, ExternalTransactionId, Description, ErrorMessage) VALUES
('64c25fe5-2f2a-4993-9167-96b64e377419', '8eef28c3-d76c-4f70-8a20-bf4fe669650a', '9.99', 'USD', 0, 2, datetime('now', '-5 days'), 'stripe_ch_1A2B3C4D', 'Company reveal payment for Apple Inc.', NULL),
('1ecb1ad4-2d63-4e5d-81e0-cf175f003efb', '4a558171-abf6-42f3-bc91-b8ce2a12c621', '9.99', 'USD', 1, 2, datetime('now', '-4 days'), 'paypal_tx_5E6F7G8H', 'Company reveal payment for Google', NULL),
('2012422a-275e-49ba-8d39-feee0ec0bd8d', 'dacafacb-44bb-4213-8db4-b2c5c1f455c9', '9.99', 'USD', 0, 2, datetime('now', '-10 days'), 'stripe_ch_9I0J1K2L', 'Company reveal payment for Microsoft', NULL),
('9e7346a1-229d-4109-afbb-fe6d93fa1c50', 'edb363a6-237a-4bb5-be80-698a63a9d34e', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_3M4N5O6P', 'Company reveal payment for Amazon', NULL),
('985b1e94-0458-4bcb-b305-ac8e36087276', '99daaccd-f55c-41fb-a533-adccbc48c232', '9.99', 'USD', 1, 2, datetime('now', '-2 days'), 'paypal_tx_7Q8R9S0T', 'Company reveal payment for Meta', NULL),
('76198c88-890c-4f7c-9e93-bbb45e2721d0', 'c2341773-25c9-4c15-bae8-b5a8f93768ae', '9.99', 'USD', 0, 2, datetime('now', '-15 days'), 'stripe_ch_1U2V3W4X', 'Company reveal payment for Netflix', NULL),
('26fafaea-aa28-474e-9b47-361f689dde64', '91057f3d-1e5c-4f40-945f-640ac892de0c', '9.99', 'USD', 0, 2, datetime('now', '-1 day'), 'stripe_ch_5Y6Z7A8B', 'Company reveal payment for Airbnb', NULL),
('dbd918b9-28dd-491d-aa73-0f63ddba69e1', '9936992a-5e5c-4d92-9caf-9635cdfc2224', '9.99', 'USD', 1, 2, datetime('now', '-6 days'), 'paypal_tx_9C0D1E2F', 'Company reveal payment for Tesla', NULL),
('ed0f7442-ecad-4528-8b92-8cddc15546b9', '41b53468-a703-44ff-ba6c-f359ca9e56cc', '9.99', 'USD', 0, 2, datetime('now', '-8 days'), 'stripe_ch_3G4H5I6J', 'Company reveal payment for Goldman Sachs', NULL),
('c4a80969-7bb1-4eda-9885-25fdc994dec4', '2c913984-35c7-4908-9969-f35c848af42e', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_7K8L9M0N', 'Company reveal payment for Apple Inc.', NULL),
('93b04230-9abb-4f99-88a1-7e943d144333', 'a5d624a1-a995-41e8-aa78-dcbf614615f0', '9.99', 'USD', 1, 2, datetime('now', '-2 days'), 'paypal_tx_1O2P3Q4R', 'Company reveal payment for Google', NULL),
('3e41e164-8186-42a5-82be-59a9ee5f0f32', 'd9c4b1f2-fc90-489a-a5f1-fa8bd130a2c1', '9.99', 'USD', 0, 2, datetime('now', '-12 days'), 'stripe_ch_5S6T7U8V', 'Company reveal payment for Microsoft', NULL),
('6391b31a-5b2f-4013-a2d3-1f03217947c7', '97c9de39-034a-4bf2-9089-7a32a0ab15ff', '9.99', 'USD', 0, 2, datetime('now', '-4 days'), 'stripe_ch_9W0X1Y2Z', 'Company reveal payment for Amazon', NULL),
('1962f35b-8cb8-4e95-bb89-4967fe69fa44', '22960242-b24f-419c-80bf-33917142e146', '9.99', 'USD', 1, 2, datetime('now', '-1 day'), 'paypal_tx_3A4B5C6D', 'Company reveal payment for Meta', NULL),
('038a5c35-2d99-4c16-9b83-6c544fd72051', 'bbd201a5-aee5-4b95-b0d5-05c5b6d71aaf', '9.99', 'USD', 0, 2, datetime('now', '-7 days'), 'stripe_ch_7E8F9G0H', 'Company reveal payment for Netflix', NULL),
('0304c53f-043e-4b0b-9725-9013ab4fe76d', 'e3bb78da-4214-4f07-b279-32b594ca4cdc', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_1I2J3K4L', 'Company reveal payment for Airbnb', NULL),
('bf07314e-8286-42a4-898f-a21b413fced0', '93f99443-0467-42b4-927c-f14a88d5a9b8', '9.99', 'USD', 1, 2, datetime('now', '-20 days'), 'paypal_tx_5M6N7O8P', 'Company reveal payment for Tesla', NULL),
('9fa98334-9c83-4afc-9511-0da0d599df67', '10db06d7-28f7-47de-889d-02260eaa48c7', '9.99', 'USD', 0, 2, datetime('now', '-2 days'), 'stripe_ch_9Q0R1S2T', 'Company reveal payment for Google', NULL),
('e1021731-6876-446b-bb52-82b9bf7d670c', 'b3d92585-05b7-47d6-a05b-bd806b2752cc', '9.99', 'USD', 0, 2, datetime('now', '-9 days'), 'stripe_ch_3U4V5W6X', 'Company reveal payment for Microsoft', NULL),
('34214830-b975-42fd-917c-313239c9fc23', '920ba6fd-f058-495a-a400-e99c9272b7ce', '9.99', 'USD', 1, 2, datetime('now', '-5 days'), 'paypal_tx_7Y8Z9A0B', 'Company reveal payment for Amazon', NULL),
('5f08f4f2-5958-44d2-a8a2-e57b38adff88', '8eef28c3-d76c-4f70-8a20-bf4fe669650a', '9.99', 'USD', 0, 2, datetime('now', '-4 days'), 'stripe_ch_1C2D3E4F', 'Company reveal payment for Google', NULL),
('5defbbd5-587e-4ba0-9e83-334968ac9127', '4a558171-abf6-42f3-bc91-b8ce2a12c621', '9.99', 'USD', 0, 2, datetime('now', '-3 days'), 'stripe_ch_5G6H7I8J', 'Company reveal payment for Microsoft', NULL),
('f8a9a998-c911-48d4-b6d0-97df6ac80299', 'dacafacb-44bb-4213-8db4-b2c5c1f455c9', '9.99', 'USD', 1, 2, datetime('now', '-6 days'), 'paypal_tx_9K0L1M2N', 'Company reveal payment for Amazon', NULL),
('4bfec9ed-7ca9-4bbb-9fb5-74799c024294', 'edb363a6-237a-4bb5-be80-698a63a9d34e', '9.99', 'USD', 0, 2, datetime('now', '-1 day'), 'stripe_ch_3O4P5Q6R', 'Company reveal payment for Meta', NULL),
('1f0f0429-2544-419d-b6ce-bc29349f4e9f', '99daaccd-f55c-41fb-a533-adccbc48c232', '9.99', 'USD', 0, 2, datetime('now', '-2 days'), 'stripe_ch_7S8T9U0V', 'Company reveal payment for Netflix', NULL);

-- =====================================================
-- INSERT COMPANY REVEALS (25 Company Reveals)
-- =====================================================
INSERT INTO CompanyReveals (Id, JobSeekerId, JobPostingId, PaymentTransactionId, RevealedDate) VALUES
('d54688da-f657-4ab5-bc09-9d7742a6f965', '8eef28c3-d76c-4f70-8a20-bf4fe669650a', '399f35ed-3d17-407e-bd84-ff2de9cdeca2', '64c25fe5-2f2a-4993-9167-96b64e377419', datetime('now', '-5 days')),
('85ead778-37bf-45d3-bf4f-a920d3826672', '4a558171-abf6-42f3-bc91-b8ce2a12c621', 'e556bacd-8986-457d-857c-6a774c6cd309', '1ecb1ad4-2d63-4e5d-81e0-cf175f003efb', datetime('now', '-4 days')),
('0e0afc68-8f33-4550-8c38-cfbdb5f01374', 'dacafacb-44bb-4213-8db4-b2c5c1f455c9', '14e5f869-603c-4724-a959-518d44700f12', '2012422a-275e-49ba-8d39-feee0ec0bd8d', datetime('now', '-10 days')),
('d2513013-4d27-4e34-bd36-17786d54225a', 'edb363a6-237a-4bb5-be80-698a63a9d34e', '94c9e586-4103-4c37-bea4-e17771e03202', '9e7346a1-229d-4109-afbb-fe6d93fa1c50', datetime('now', '-3 days')),
('6e6a4ca8-9bc7-4b0e-a22a-bb89932bc8d8', '99daaccd-f55c-41fb-a533-adccbc48c232', '3398e7f2-5796-4a74-953e-7b3ef86ed227', '985b1e94-0458-4bcb-b305-ac8e36087276', datetime('now', '-2 days')),
('d1942707-0c30-43b0-b5e6-c0eb8c1206e6', 'c2341773-25c9-4c15-bae8-b5a8f93768ae', '21157351-204a-4b3e-901b-fa2bde3a6eaf', '76198c88-890c-4f7c-9e93-bbb45e2721d0', datetime('now', '-15 days')),
('2885dbaf-1da0-4a77-9390-c01dffa8797f', '91057f3d-1e5c-4f40-945f-640ac892de0c', '00abc720-0026-4d55-bf4c-6af19c18b3ce', '26fafaea-aa28-474e-9b47-361f689dde64', datetime('now', '-1 day')),
('72ea79e3-330e-402a-98fd-774a089bba14', '9936992a-5e5c-4d92-9caf-9635cdfc2224', '98c893ea-dde1-4d6b-a316-42d812981bcb', 'dbd918b9-28dd-491d-aa73-0f63ddba69e1', datetime('now', '-6 days')),
('bc944813-cf0a-4cf9-8443-55f71897023a', '41b53468-a703-44ff-ba6c-f359ca9e56cc', 'fd241ea7-8dee-4ee8-804f-a9ed99ff13ec', 'ed0f7442-ecad-4528-8b92-8cddc15546b9', datetime('now', '-8 days')),
('e8f26ac1-bae6-4536-988c-80fa2b686da7', '2c913984-35c7-4908-9969-f35c848af42e', '7442e153-e5cf-46c5-843f-80410e47aeaf', 'c4a80969-7bb1-4eda-9885-25fdc994dec4', datetime('now', '-3 days')),
('ccd1daa2-2552-47c2-a065-f99fe77b7b85', 'a5d624a1-a995-41e8-aa78-dcbf614615f0', '40e50630-c055-4393-acaa-59b05b9483e5', '93b04230-9abb-4f99-88a1-7e943d144333', datetime('now', '-2 days')),
('f2674ab2-9936-4f67-83bc-7570fae4c603', 'd9c4b1f2-fc90-489a-a5f1-fa8bd130a2c1', 'a62193bf-ed63-4b42-a5a4-ba2d1e8c1d19', '3e41e164-8186-42a5-82be-59a9ee5f0f32', datetime('now', '-12 days')),
('21c0e5c6-6096-4bf5-82ba-0126f3b4453f', '97c9de39-034a-4bf2-9089-7a32a0ab15ff', 'f900d9a6-f65f-4a25-8202-439556b3b002', '6391b31a-5b2f-4013-a2d3-1f03217947c7', datetime('now', '-4 days')),
('db68eb9f-424b-49c5-b356-4d1552187bbc', '22960242-b24f-419c-80bf-33917142e146', 'b3e057f1-427b-4c35-b809-420f0fd8adee', '1962f35b-8cb8-4e95-bb89-4967fe69fa44', datetime('now', '-1 day')),
('01707759-bbc2-4593-9831-81c8e80cd446', 'bbd201a5-aee5-4b95-b0d5-05c5b6d71aaf', '67d27190-5113-4412-b39d-73a281c4f3c7', '038a5c35-2d99-4c16-9b83-6c544fd72051', datetime('now', '-7 days')),
('89db4cd8-73ef-4190-b874-d2aa0c92d12c', 'e3bb78da-4214-4f07-b279-32b594ca4cdc', '438d6108-bd0f-45be-a37e-04ec14145bb6', '0304c53f-043e-4b0b-9725-9013ab4fe76d', datetime('now', '-3 days')),
('3a4119f2-02f5-418b-abe9-2ef714e4a56d', '93f99443-0467-42b4-927c-f14a88d5a9b8', '0a97e325-ac9d-49ac-a017-fbf40af62d10', 'bf07314e-8286-42a4-898f-a21b413fced0', datetime('now', '-20 days')),
('fc6ab006-4528-45d4-91eb-ca2d3a9f061b', '10db06d7-28f7-47de-889d-02260eaa48c7', '24f7b6f2-1612-41e9-b04d-8db25a0b3d1c', '9fa98334-9c83-4afc-9511-0da0d599df67', datetime('now', '-2 days')),
('1ebfcc30-d36a-415f-9b10-01df9616e63c', 'b3d92585-05b7-47d6-a05b-bd806b2752cc', '36d63e46-173e-4964-8b22-9d32bc6507ff', 'e1021731-6876-446b-bb52-82b9bf7d670c', datetime('now', '-9 days')),
('a2a599a7-8e5e-4fb6-991e-bf2a3ae32598', '920ba6fd-f058-495a-a400-e99c9272b7ce', 'da0d7973-fad2-4e77-9dd3-748ec330f5c4', '34214830-b975-42fd-917c-313239c9fc23', datetime('now', '-5 days')),
('dc2faea3-54b4-4bd5-b1b0-1ac9532be16d', '8eef28c3-d76c-4f70-8a20-bf4fe669650a', '5dcd57a1-b7a4-48c8-8893-61db43d0c98a', '5f08f4f2-5958-44d2-a8a2-e57b38adff88', datetime('now', '-4 days')),
('034ce9ca-83af-467b-8d76-8f2563b7916a', '4a558171-abf6-42f3-bc91-b8ce2a12c621', 'a60f725f-bbbf-4f87-9c68-892a5ae24603', '5defbbd5-587e-4ba0-9e83-334968ac9127', datetime('now', '-3 days')),
('2c229a95-edca-4ff3-bfd6-af0b3679ca92', 'dacafacb-44bb-4213-8db4-b2c5c1f455c9', '99bc67f4-16a7-41f4-9bf9-cefefa63a57c', 'f8a9a998-c911-48d4-b6d0-97df6ac80299', datetime('now', '-6 days')),
('c82bb698-f121-4767-85c7-966ae4ad26c4', 'edb363a6-237a-4bb5-be80-698a63a9d34e', '77982ff2-a7b7-4b1f-8814-f575aabeb8f7', '4bfec9ed-7ca9-4bbb-9fb5-74799c024294', datetime('now', '-1 day')),
('bd63463f-132c-45fe-918d-568815f0c2d4', '99daaccd-f55c-41fb-a533-adccbc48c232', 'ac436caf-bad2-46f2-83e8-85215b9e8f8d', '1f0f0429-2544-419d-b6ce-bc29349f4e9f', datetime('now', '-2 days'));
