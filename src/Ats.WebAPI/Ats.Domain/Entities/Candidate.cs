using Ats.Domain.Exceptions;

namespace Ats.Domain.Entities;

public class Candidate : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public int Age { get; private set; }
    public string? LinkedInProfile { get; private set; }
    public byte[]? ResumeFile { get; private set; }
    public string? ResumeFileName { get; private set; }

    public Candidate(string name, string email, int age, string? linkedInProfile, byte[]? resumeFile, string? resumeFileName) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome não pode estar vazio.");

        if (age < 18 || age > 65)
            throw new DomainException("A idade deve estar entre 18 e 65 anos.");

        if (!email.Contains('@'))
            throw new DomainException("E-mail inválido.");

        Name = name;
        Email = email;
        Age = age;
        LinkedInProfile = linkedInProfile;
        ResumeFile = resumeFile;
        ResumeFileName = resumeFileName;
    }

    public void UpdateInfo(string name, string email, int age, string? linkedInProfile)
    {
        Name = name;
        Email = email;
        Age = age;
        LinkedInProfile = linkedInProfile;
        UpdateTimestamp();
    }

    public void UploadResume(byte[] fileContent, string fileName)
    {
        if (fileContent == null || fileContent.Length == 0)
            throw new DomainException("O arquivo está vazio.");

        ResumeFile = fileContent;
        ResumeFileName = fileName;
        UpdateTimestamp();
    }
}