using Ats.Domain.Common;
using Ats.Domain.Entities;

namespace Ats.Domain.Interfaces;

public interface IJobRepository
{
    Task AddAsync(Job job, CancellationToken ct = default);
    Task UpdateAsync(Job job);

    Task<Job?> GetByIdAsync(Guid id, CancellationToken ct = default);

    // Listagem geral (para a área administrativa)
    Task<PagedResult<Job>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default);

    // Listagem pública (somente vagas ativas para os candidatos verem)
    Task<PagedResult<Job>> GetAllActivePaginatedAsync(int page, int pageSize, CancellationToken ct = default);
}
