using Ats.Domain.Enums;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.JobApplications;

public record GetApplicationsByJobQuery(Guid JobId)
    : IRequest<Result<IEnumerable<GetApplicationsByJobResponse>>>;

public class GetApplicationsByJobQueryHandler
    : IRequestHandler<GetApplicationsByJobQuery, Result<IEnumerable<GetApplicationsByJobResponse>>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;

    public GetApplicationsByJobQueryHandler(
        IJobApplicationRepository applicationRepository,
        ICandidateRepository candidateRepository)
    {
        _applicationRepository = applicationRepository;
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result<IEnumerable<GetApplicationsByJobResponse>>> Handle(
        GetApplicationsByJobQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetByJobIdAsync(request.JobId, cancellationToken);

        if (!applications.Any())
        {
            return Result.Ok(Enumerable.Empty<GetApplicationsByJobResponse>());
        }

        var responseList = new List<GetApplicationsByJobResponse>();

        foreach (var app in applications)
        {
            var candidate = await _candidateRepository.GetByIdAsync(app.CandidateId, cancellationToken);

            if (candidate != null)
            {
                responseList.Add(new GetApplicationsByJobResponse(
                    app.Id,
                    app.CandidateId,
                    candidate.Name,
                    candidate.Email,
                    app.Status,
                    app.CreatedAt
                ));
            }
        }

        return Result.Ok((IEnumerable<GetApplicationsByJobResponse>)responseList);
    }
}

public record GetApplicationsByJobResponse(
    Guid ApplicationId,
    Guid CandidateId,
    string CandidateName,
    string CandidateEmail,
    ApplicationStatus Status,
    DateTime AppliedAt
);