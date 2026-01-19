using Ats.Domain.Entities;

namespace Ats.Domain.Interfaces;

public interface IJobApplicationRepository
{
    Task AddAsync(JobApplication application, CancellationToken ct = default);

    Task UpdateAsync(JobApplication application, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid jobId, Guid candidateId, CancellationToken ct = default);

    Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId, CancellationToken ct = default);
    Task<IEnumerable<JobApplication>> GetByCandidateIdAsync(Guid candidateId, CancellationToken ct = default);
}