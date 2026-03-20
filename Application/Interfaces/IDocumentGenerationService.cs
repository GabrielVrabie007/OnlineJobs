using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces;


public interface IDocumentGenerationService
{

    string GenerateResume(JobSeeker jobSeeker);
    string GenerateCoverLetter(JobSeeker jobSeeker, JobPosting jobPosting, string customMessage = null);
    string GenerateJobPosting(JobPosting jobPosting);
    string GenerateApplicationReport(JobPosting jobPosting, IEnumerable<JobApplication> applications = null);
    
    IDocument GetDocument(IDocumentFactory factory, object primaryEntity, object secondaryEntity = null, bool isProfile = true);
}