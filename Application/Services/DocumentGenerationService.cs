using OnlineJobs.Application.Interfaces;
using OnlineJobs.Application.Factories;
using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Services;


public class DocumentGenerationService : IDocumentGenerationService
{
    private readonly IDocumentFactory _candidateFactory;
    private readonly IDocumentFactory _employerFactory;

    public DocumentGenerationService()
    {
        _candidateFactory = new CandidateDocumentFactory();
        _employerFactory = new EmployerDocumentFactory();
    }
    
    public string GenerateResume(JobSeeker jobSeeker)
    {
        if (jobSeeker == null)
            throw new ArgumentNullException(nameof(jobSeeker));

        var document = _candidateFactory.CreateProfileDocument(jobSeeker);
        return document.Generate();
    }

    public string GenerateCoverLetter(JobSeeker jobSeeker, JobPosting jobPosting, string customMessage = null)
    {
        if (jobSeeker == null)
            throw new ArgumentNullException(nameof(jobSeeker));
        if (jobPosting == null)
            throw new ArgumentNullException(nameof(jobPosting));

        var document = _candidateFactory.CreateApplicationDocument(jobSeeker, jobPosting);
        return document.Generate();
    }


    public string GenerateJobPosting(JobPosting jobPosting)
    {
        if (jobPosting == null)
            throw new ArgumentNullException(nameof(jobPosting));

        var document = _employerFactory.CreateProfileDocument(jobPosting);
        return document.Generate();
    }


    public string GenerateApplicationReport(JobPosting jobPosting, IEnumerable<JobApplication> applications = null)
    {
        if (jobPosting == null)
            throw new ArgumentNullException(nameof(jobPosting));

        var document = _employerFactory.CreateApplicationDocument(jobPosting, applications);
        return document.Generate();
    }

    public IDocument GetDocument(IDocumentFactory factory, object primaryEntity, object secondaryEntity = null, bool isProfile = true)
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        if (primaryEntity == null)
            throw new ArgumentNullException(nameof(primaryEntity));

        return isProfile
            ? factory.CreateProfileDocument(primaryEntity)
            : factory.CreateApplicationDocument(primaryEntity, secondaryEntity);
    }
}