using Ats.Domain.Enums;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.JobApplications;

public record GetApplicationsByCandidateQuery(Guid CandidateId)
    : IRequest<Result<IEnumerable<GetApplicationsByCandidateResponse>>>;

public class GetApplicationsByCandidateQueryHandler
    : IRequestHandler<GetApplicationsByCandidateQuery, Result<IEnumerable<GetApplicationsByCandidateResponse>>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;

    public GetApplicationsByCandidateQueryHandler(
        IJobApplicationRepository applicationRepository,
        IJobRepository jobRepository)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result<IEnumerable<GetApplicationsByCandidateResponse>>> Handle(
        GetApplicationsByCandidateQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetByCandidateIdAsync(request.CandidateId, cancellationToken);

        if (!applications.Any())
        {
            return Result.Ok(Enumerable.Empty<GetApplicationsByCandidateResponse>());
        }

        var responseList = new List<GetApplicationsByCandidateResponse>();

        foreach (var app in applications)
        {
            var job = await _jobRepository.GetByIdAsync(app.JobId, cancellationToken);

            if (job != null)
            {
                responseList.Add(new GetApplicationsByCandidateResponse(
                    app.Id,
                    app.JobId,
                    job.Title,
                    job.IsActive,
                    app.Status,
                    app.CreatedAt
                ));
            }
        }

        return Result.Ok((IEnumerable<GetApplicationsByCandidateResponse>)responseList);
    }
}

public record GetApplicationsByCandidateResponse(
    Guid ApplicationId,
    Guid JobId,
    string JobTitle,
    bool IsJobActive,
    ApplicationStatus Status,
    DateTime AppliedAt
);