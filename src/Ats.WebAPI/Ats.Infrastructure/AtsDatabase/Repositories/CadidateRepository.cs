using Ats.Domain.Entities;
using static Ats.Domain.Entities.Candidate;

namespace Ats.Infrastructure.AtsDatabase.Repositories;

internal class CadidateRepository : ICandidateRepository
{
    public Task AddAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Candidate>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Candidate> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Candidate candidate)
    {
        throw new NotImplementedException();
    }
}
