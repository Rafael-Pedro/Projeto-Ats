using Ats.Domain.Common;
using Ats.Domain.Exceptions;

namespace Ats.Domain.Entities;

public class Candidate : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public int Age { get; private set; }
    public string? Resume { get; private set; }

    public Candidate(string name, string email, int age, string? resume) : base()
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
        Resume = resume;
    }

    public void UpdateInfo(string name, string email, int age, string? resume)
    {
        Name = name;
        Email = email;
        Age = age;
        Resume = resume;
        UpdateTimestamp();
    }
}