using OnlineJobs.Domain.Entities;

namespace OnlineJobs.Application.Interfaces;


public interface IDocumentFactory
{
    IDocument CreateProfileDocument(object entity);
    IDocument CreateApplicationDocument(object primaryEntity, object secondaryEntity = null);
    string FactoryType { get; }
}