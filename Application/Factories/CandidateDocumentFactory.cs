using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Documents;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Factories;


public class CandidateDocumentFactory : IDocumentFactory
{
    public string FactoryType => "Candidate Documents";

    public IDocument CreateProfileDocument(object entity)
    {
        if (entity is not JobSeeker jobSeeker)
        {
            throw new ArgumentException("Entity must be a JobSeeker", nameof(entity));
        }

        return new ResumeDocument(jobSeeker);
    }

    public IDocument CreateApplicationDocument(object primaryEntity, object secondaryEntity = null)
    {
        if (primaryEntity is not JobSeeker jobSeeker)
        {
            throw new ArgumentException("Primary entity must be a JobSeeker", nameof(primaryEntity));
        }

        JobPosting jobPosting = null;
        string customMessage = null;

        if (secondaryEntity is JobPosting posting)
        {
            jobPosting = posting;
        }
        else if (secondaryEntity is (JobPosting jp, string msg))
        {
            jobPosting = jp;
            customMessage = msg;
        }
        else if (secondaryEntity != null)
        {
            throw new ArgumentException("Secondary entity must be a JobPosting or tuple of (JobPosting, string)", nameof(secondaryEntity));
        }

        if (jobPosting == null)
        {
            throw new ArgumentException("JobPosting is required for cover letter", nameof(secondaryEntity));
        }

        return new CoverLetterDocument(jobSeeker, jobPosting, customMessage);
    }
}