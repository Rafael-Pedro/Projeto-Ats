using Ats.Domain.Common;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ats.Infrastructure.AtsDatabase.Repositories;

public class CadidateRepository : ICandidateRepository
{
    private readonly IMongoCollection<Candidate> _collection;

    public CadidateRepository(IMongoClient client, IOptions<MongoSettings> options)
    {
        var db = client.GetDatabase(options.Value.DatabaseName);
        _collection = db.GetCollection<Candidate>("candidates");
    }

    public async Task AddAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(candidate, null, cancellationToken);
    }

    public async Task<PagedResult<Candidate>> GetAllPaginatedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var filter = Builders<Candidate>.Filter.Eq(c => c.IsDeleted, false);

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);

        var skip = (page - 1) * pageSize;

        var items = await _collection.Find(filter)
                                     .Skip(skip)
                                     .Limit(pageSize)
                                     .ToListAsync(ct);

        return new PagedResult<Candidate>(items, totalCount, page, pageSize);
    }
    public async Task<Candidate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var candidate = await _collection
                    .Find(c => c.Id == id && !c.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);

        return candidate;
    }

    public async Task UpdateAsync(Candidate candidate, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Candidate>.Filter.Eq(c => c.Id, candidate.Id);
        await _collection.ReplaceOneAsync(filter, candidate, new ReplaceOptions { IsUpsert = false });
    }
}
