using Ats.Domain.Common;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Jobs;

public record GetAllJobsQuery(
    int Page = 1,
    int PageSize = 10,
    bool OnlyActive = false
    ) : IRequest<Result<PagedResult<GetAllJobsQueryResponse>>>;

public class GetAllJobsQueryHandler : IRequestHandler<GetAllJobsQuery, Result<PagedResult<GetAllJobsQueryResponse>>>
{
    private readonly IJobRepository _jobRepository;

    public GetAllJobsQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result<PagedResult<GetAllJobsQueryResponse>>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
    {
        PagedResult<Job> pagedJobs;

        if (request.OnlyActive)
            pagedJobs = await _jobRepository.GetAllActivePaginatedAsync(request.Page, request.PageSize, cancellationToken);

        else
            pagedJobs = await _jobRepository.GetAllPaginatedAsync(request.Page, request.PageSize, cancellationToken);

        var dtos = pagedJobs.Items.Select(job => new GetAllJobsQueryResponse(
            job.Id,
            job.Title,
            job.Description,
            job.Salary,
            job.IsActive,
            job.CreatedAt
        ));

        var response = new PagedResult<GetAllJobsQueryResponse>(
            dtos,
            pagedJobs.TotalCount,
            pagedJobs.Page,
            pagedJobs.PageSize
        );

        return Result.Ok(response);
    }
}

public record GetAllJobsQueryResponse(
    Guid Id,
    string Title,
    string Description,
    decimal? Salary,
    bool IsActive,
    DateTime CreatedAt
);