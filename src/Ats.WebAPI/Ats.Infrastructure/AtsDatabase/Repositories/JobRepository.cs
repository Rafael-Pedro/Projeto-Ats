using Ats.Domain.Common;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ats.Infrastructure.AtsDatabase.Repositories;

public class JobRepository : IJobRepository
{
    private readonly IMongoCollection<Job> _collection;

    public JobRepository(IOptions<MongoSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<Job>("jobs");
    }

    public async Task AddAsync(Job job, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(job, cancellationToken: ct);
    }

    public async Task UpdateAsync(Job job)
    {
        await _collection.ReplaceOneAsync(x => x.Id == job.Id, job);
    }

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
    }

    // Admin (Vê vagas abertas e fechadas)
    public async Task<PagedResult<Job>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var filter = Builders<Job>.Filter.Eq(x => x.IsDeleted, false);

        return await ExecutePagination(filter, page, pageSize, ct);
    }


    // Público (Vê apenas vagas ATIVAS)
    public async Task<PagedResult<Job>> GetAllActivePaginatedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var builder = Builders<Job>.Filter;
        var filter = builder.And(
            builder.Eq(x => x.IsDeleted, false),
            builder.Eq(x => x.IsActive, true)
        );

        return await ExecutePagination(filter, page, pageSize, ct);
    }

    private async Task<PagedResult<Job>> ExecutePagination(FilterDefinition<Job> filter, int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var items = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Job>(items, totalCount, page, pageSize);
    }
}