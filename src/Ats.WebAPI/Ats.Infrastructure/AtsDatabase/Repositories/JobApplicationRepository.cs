using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ats.Infrastructure.AtsDatabase.Repositories;

public class JobApplicationRepository : IJobApplicationRepository
{
    private readonly IMongoCollection<JobApplication> _collection;

    public JobApplicationRepository(IOptions<MongoSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _collection = mongoDatabase.GetCollection<JobApplication>("job_applications");
    }

    public async Task AddAsync(JobApplication application, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(application, cancellationToken: ct);
    }

    public async Task UpdateAsync(JobApplication application, CancellationToken ct = default)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == application.Id,
            application,
            cancellationToken: ct);
    }

    public async Task<bool> ExistsAsync(Guid jobId, Guid candidateId, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.JobId == jobId &&
                       x.CandidateId == candidateId &&
                       !x.IsDeleted)
            .AnyAsync(ct);
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.Id == id && !x.IsDeleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.JobId == jobId && !x.IsDeleted)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<JobApplication>> GetByCandidateIdAsync(Guid candidateId, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.CandidateId == candidateId && !x.IsDeleted)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(ct);
    }
}