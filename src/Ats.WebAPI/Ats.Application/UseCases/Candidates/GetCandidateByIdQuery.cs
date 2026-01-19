using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Candidates;

public record GetCandidateByIdQuery(
    Guid Id
    ) : IRequest<Result<GetCandidateByIdQueryResponse>>;
public class GetCandidateByIdQueryHandler : IRequestHandler<GetCandidateByIdQuery, Result<GetCandidateByIdQueryResponse>>
{
    private readonly ICandidateRepository _candidateRepository;

    public GetCandidateByIdQueryHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result<GetCandidateByIdQueryResponse>> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByIdAsync(request.Id, cancellationToken);

        if (candidate is null)
            return Result.Fail(new NotFoundError($"Candidato não encontrado."));

        var response = new GetCandidateByIdQueryResponse(
            candidate.Name,
            candidate.Email,
            candidate.Age,
            candidate.Resume,
            candidate.CreatedAt,
            candidate.UpdatedAt,
            candidate.DeletedAt,
            candidate.IsDeleted
        );

        return Result.Ok(response);
    }
}

public record GetCandidateByIdQueryResponse(
    string Name,
    string Email,
    int Age,
    string? Resume,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsDeleted
);