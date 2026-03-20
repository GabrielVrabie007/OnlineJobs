namespace OnlineJobs.Application.Interfaces;

public interface IDocument
{

    string DocumentType { get; }
    
    string Generate();
    
    bool Validate();
    string GetFileExtension();
}