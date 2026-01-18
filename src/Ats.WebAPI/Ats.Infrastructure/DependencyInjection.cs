using Ats.Infrastructure.AtsDatabase;
using Ats.Infrastructure.AtsDatabase.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static Ats.Domain.Entities.Candidate;

namespace Ats.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            var connectionString = settings.ConnectionString;
            return new MongoClient(connectionString);
        });

        services.AddScoped<ICandidateRepository, CadidateRepository>();

        return services;
    }
}
