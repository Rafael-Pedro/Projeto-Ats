using Mediator;
using FluentResults;
using Ats.Domain.Common;
using static Ats.Domain.Entities.Candidate;

namespace Ats.Application.UseCases.Candidates;

public record GetAllCandidatesQuery(
    int Page = 1,
    int PageSize = 10
    ) : IRequest<Result<PagedResult<GetAllCandidatesQueryResponse>>>;
public class GetAllCandidatesQueryHandler : IRequestHandler<GetAllCandidatesQuery, Result<PagedResult<GetAllCandidatesQueryResponse>>>
{
    private readonly ICandidateRepository _candidateRepository;

    public GetAllCandidatesQueryHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result<PagedResult<GetAllCandidatesQueryResponse>>> Handle(GetAllCandidatesQuery request, CancellationToken cancellationToken)
    {
        var pagedCandidates = await _candidateRepository.GetAllPaginatedAsync(request.Page, request.PageSize, cancellationToken);

        var dtos = pagedCandidates.Items.Select(candidate => new GetAllCandidatesQueryResponse(
            candidate.Id,
            candidate.Name,
            candidate.Email,
            candidate.Age,
            candidate.Resume
        ));

        var response = new PagedResult<GetAllCandidatesQueryResponse>(
        dtos,
        pagedCandidates.TotalCount,
        pagedCandidates.Page,
        pagedCandidates.PageSize
        );

        return Result.Ok(response);
    }
}

public record GetAllCandidatesQueryResponse(
    Guid Id,
    string Name,
    string Email,
    int Age,
    string? Resume
);
