using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Documents;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Factories;


public class EmployerDocumentFactory : IDocumentFactory
{
    public string FactoryType => "Employer Documents";
    
    public IDocument CreateProfileDocument(object entity)
    {
        if (entity is not JobPosting jobPosting)
        {
            throw new ArgumentException("Entity must be a JobPosting", nameof(entity));
        }

        return new JobPostingDocument(jobPosting);
    }

    public IDocument CreateApplicationDocument(object primaryEntity, object secondaryEntity = null)
    {
        if (primaryEntity is not JobPosting jobPosting)
        {
            throw new ArgumentException("Primary entity must be a JobPosting", nameof(primaryEntity));
        }

        IEnumerable<JobApplication> applications;

        if (secondaryEntity == null)
        {
            applications = jobPosting.Applications ?? new List<JobApplication>();
        }
        else if (secondaryEntity is IEnumerable<JobApplication> apps)
        {
            applications = apps;
        }
        else
        {
            throw new ArgumentException("Secondary entity must be an IEnumerable<JobApplication>", nameof(secondaryEntity));
        }

        return new ApplicationReportDocument(jobPosting, applications);
    }
}