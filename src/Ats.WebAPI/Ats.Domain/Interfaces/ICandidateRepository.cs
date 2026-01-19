using Ats.Domain.Common;
using Ats.Domain.Entities;

namespace Ats.Domain.Interfaces;

public interface ICandidateRepository
{
    Task AddAsync(Candidate candidate, CancellationToken cancellationToken = default);
    Task UpdateAsync(Candidate candidate);
    Task<Candidate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Candidate>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default);
}