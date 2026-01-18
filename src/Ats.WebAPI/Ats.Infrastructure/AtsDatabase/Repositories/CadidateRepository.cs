using Ats.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static Ats.Domain.Entities.Candidate;

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

    public async Task<IEnumerable<Candidate>> GetAllAsync()
    {
        return await _collection.Find(c => !c.IsDeleted).ToListAsync();
    }

    public async Task<Candidate> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _collection
                    .Find(c => c.Id == id && !c.IsDeleted)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(Candidate candidate)
    {
        await _collection.ReplaceOneAsync(c => c.Id == candidate.Id, candidate);
    }
}
