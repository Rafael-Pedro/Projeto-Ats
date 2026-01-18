using Ats.Domain.Common;

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
            throw new ArgumentException("O nome não pode estar vazio.", nameof(name));

        if (age < 18 || age > 65)
            throw new ArgumentException("A idade deve estar entre 18 e 65 anos.", nameof(age));

        if (!email.Contains('@'))
            throw new ArgumentException("E-mail inválido.", nameof(email));

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

    public interface ICandidateRepository
    {
        Task AddAsync(Candidate candidate, CancellationToken cancellationToken = default);
        Task UpdateAsync(Candidate candidate);
        Task<Candidate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<Candidate>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}