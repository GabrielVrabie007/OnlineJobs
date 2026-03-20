using OnlineJobs.Domain.Entities;
using OnlineJobs.Domain.ValueObjects;

namespace OnlineJobs.Application.Interfaces
{

    public interface IJobSeekerProfileBuilder
    {
   
        IJobSeekerProfileBuilder WithBasicInfo(string email, string firstName, string lastName, string? phoneNumber = null);

   
        IJobSeekerProfileBuilder WithPersonalDetails(DateTime? dateOfBirth, string? address);


        IJobSeekerProfileBuilder WithProfessionalSummary(string summary);

 
        IJobSeekerProfileBuilder WithOnlinePresence(string? linkedInUrl = null, string? gitHubUrl = null, string? portfolioUrl = null);

   
        IJobSeekerProfileBuilder AddEducation(Education education);


        IJobSeekerProfileBuilder AddEducation(IEnumerable<Education> educations);

    
        IJobSeekerProfileBuilder AddWorkExperience(WorkExperience experience);

    
        IJobSeekerProfileBuilder AddWorkExperience(IEnumerable<WorkExperience> experiences);

    
        IJobSeekerProfileBuilder AddSkill(Skill skill);

        IJobSeekerProfileBuilder AddSkills(IEnumerable<Skill> skills);

   
        IJobSeekerProfileBuilder AddCertification(Certification certification);

     
        IJobSeekerProfileBuilder AddCertifications(IEnumerable<Certification> certifications);

 
        void Reset();
        
        JobSeeker Build();
    }
}
