using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Application.Builders
{

    public class JobSeekerProfileBuilder : IJobSeekerProfileBuilder
    {
        private JobSeeker _jobSeeker;

        public JobSeekerProfileBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _jobSeeker = new JobSeeker();
        }

        public IJobSeekerProfileBuilder WithBasicInfo(string email, string firstName, string lastName, string? phoneNumber = null)
        {
            _jobSeeker.Email = email;
            _jobSeeker.FirstName = firstName;
            _jobSeeker.LastName = lastName;
            _jobSeeker.PhoneNumber = phoneNumber;
            return this;
        }

        public IJobSeekerProfileBuilder WithPersonalDetails(DateTime? dateOfBirth, string? address)
        {
            _jobSeeker.DateOfBirth = dateOfBirth;
            _jobSeeker.Address = address;
            return this;
        }

        public IJobSeekerProfileBuilder WithProfessionalSummary(string summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
                throw new ArgumentException("Professional summary cannot be empty", nameof(summary));

            if (summary.Length < 50)
                throw new ArgumentException("Professional summary should be at least 50 characters", nameof(summary));

            _jobSeeker.ProfessionalSummary = summary;
            return this;
        }

        public IJobSeekerProfileBuilder WithOnlinePresence(string? linkedInUrl = null, string? gitHubUrl = null, string? portfolioUrl = null)
        {
            _jobSeeker.LinkedInUrl = linkedInUrl;
            _jobSeeker.GitHubUrl = gitHubUrl;
            _jobSeeker.PortfolioUrl = portfolioUrl;
            return this;
        }

        public IJobSeekerProfileBuilder AddEducation(Education education)
        {
            if (education == null)
                throw new ArgumentNullException(nameof(education));

            _jobSeeker.EducationHistory.Add(education);
            return this;
        }

        public IJobSeekerProfileBuilder AddEducation(IEnumerable<Education> educations)
        {
            if (educations == null)
                throw new ArgumentNullException(nameof(educations));

            foreach (var education in educations)
            {
                AddEducation(education);
            }
            return this;
        }

        public IJobSeekerProfileBuilder AddWorkExperience(WorkExperience experience)
        {
            if (experience == null)
                throw new ArgumentNullException(nameof(experience));

            _jobSeeker.WorkHistory.Add(experience);
            return this;
        }

        public IJobSeekerProfileBuilder AddWorkExperience(IEnumerable<WorkExperience> experiences)
        {
            if (experiences == null)
                throw new ArgumentNullException(nameof(experiences));

            foreach (var experience in experiences)
            {
                AddWorkExperience(experience);
            }
            return this;
        }

        public IJobSeekerProfileBuilder AddSkill(Skill skill)
        {
            if (skill == null)
                throw new ArgumentNullException(nameof(skill));

            // Prevent duplicate skills
            if (_jobSeeker.SkillSet.Any(s => s.Equals(skill)))
                throw new InvalidOperationException($"Skill '{skill.Name}' already exists in the profile");

            _jobSeeker.SkillSet.Add(skill);
            return this;
        }

        public IJobSeekerProfileBuilder AddSkills(IEnumerable<Skill> skills)
        {
            if (skills == null)
                throw new ArgumentNullException(nameof(skills));

            foreach (var skill in skills)
            {
                AddSkill(skill);
            }
            return this;
        }

        public IJobSeekerProfileBuilder AddCertification(Certification certification)
        {
            if (certification == null)
                throw new ArgumentNullException(nameof(certification));

            _jobSeeker.Certifications.Add(certification);
            return this;
        }

        public IJobSeekerProfileBuilder AddCertifications(IEnumerable<Certification> certifications)
        {
            if (certifications == null)
                throw new ArgumentNullException(nameof(certifications));

            foreach (var certification in certifications)
            {
                AddCertification(certification);
            }
            return this;
        }

        public JobSeeker Build()
        {
            // Validation before building
            if (string.IsNullOrWhiteSpace(_jobSeeker.Email))
                throw new InvalidOperationException("Cannot build JobSeeker profile without email");

            if (string.IsNullOrWhiteSpace(_jobSeeker.FirstName))
                throw new InvalidOperationException("Cannot build JobSeeker profile without first name");

            if (string.IsNullOrWhiteSpace(_jobSeeker.LastName))
                throw new InvalidOperationException("Cannot build JobSeeker profile without last name");

            // Return the built object
            var result = _jobSeeker;

            // Reset for next build
            Reset();

            return result;
        }
    }
}
