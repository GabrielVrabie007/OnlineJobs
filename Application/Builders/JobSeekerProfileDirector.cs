using OnlineJobs.Application.Interfaces;
using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Application.Builders
{

    public class JobSeekerProfileDirector
    {
        private readonly IJobSeekerProfileBuilder _builder;

        public JobSeekerProfileDirector(IJobSeekerProfileBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

    
        public JobSeeker ConstructEntryLevelProfile(
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            Education education,
            string summary,
            List<Skill> skills)
        {
            return _builder
                .WithBasicInfo(email, firstName, lastName, phoneNumber)
                .AddEducation(education)
                .WithProfessionalSummary(summary)
                .AddSkills(skills)
                .Build();
        }


        public JobSeeker ConstructExperiencedProfile(
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            DateTime dateOfBirth,
            string address,
            string summary,
            List<Education> educations,
            List<WorkExperience> experiences,
            List<Skill> skills,
            List<Certification> certifications,
            string? linkedInUrl = null,
            string? gitHubUrl = null,
            string? portfolioUrl = null)
        {
            return _builder
                .WithBasicInfo(email, firstName, lastName, phoneNumber)
                .WithPersonalDetails(dateOfBirth, address)
                .WithProfessionalSummary(summary)
                .WithOnlinePresence(linkedInUrl, gitHubUrl, portfolioUrl)
                .AddEducation(educations)
                .AddWorkExperience(experiences)
                .AddSkills(skills)
                .AddCertifications(certifications)
                .Build();
        }

        public JobSeeker ConstructSeniorTechnicalProfile(
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            string summary,
            List<WorkExperience> experiences,
            List<Skill> technicalSkills,
            string gitHubUrl,
            string portfolioUrl)
        {
            return _builder
                .WithBasicInfo(email, firstName, lastName, phoneNumber)
                .WithProfessionalSummary(summary)
                .WithOnlinePresence(gitHubUrl: gitHubUrl, portfolioUrl: portfolioUrl)
                .AddWorkExperience(experiences)
                .AddSkills(technicalSkills)
                .Build();
        }
        
        public JobSeeker ConstructCareerChangerProfile(
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            string summary,
            List<Education> educations,
            List<WorkExperience> previousExperiences,
            List<Skill> transferableSkills,
            List<Certification> relevantCertifications)
        {
            return _builder
                .WithBasicInfo(email, firstName, lastName, phoneNumber)
                .WithProfessionalSummary(summary)
                .AddEducation(educations)
                .AddWorkExperience(previousExperiences)
                .AddSkills(transferableSkills)
                .AddCertifications(relevantCertifications)
                .Build();
        }
        
        public JobSeeker ConstructCompleteProfile(
            string email,
            string firstName,
            string lastName,
            string phoneNumber,
            DateTime dateOfBirth,
            string address,
            string summary,
            string linkedInUrl,
            string? gitHubUrl,
            string? portfolioUrl,
            List<Education> educations,
            List<WorkExperience> experiences,
            List<Skill> skills,
            List<Certification> certifications)
        {
            return _builder
                .WithBasicInfo(email, firstName, lastName, phoneNumber)
                .WithPersonalDetails(dateOfBirth, address)
                .WithProfessionalSummary(summary)
                .WithOnlinePresence(linkedInUrl, gitHubUrl, portfolioUrl)
                .AddEducation(educations)
                .AddWorkExperience(experiences)
                .AddSkills(skills)
                .AddCertifications(certifications)
                .Build();
        }
    }
}
