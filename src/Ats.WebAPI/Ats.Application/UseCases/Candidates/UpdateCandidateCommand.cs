using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Candidates;

public record UpdateCandidateCommand(
    Guid Id,
    string? Name,
    string? Email,
    int? Age,
    string? Resume
) : IRequest<Result>;

public class UpdateCandidateCommandHandler : IRequestHandler<UpdateCandidateCommand, Result>
{
    private readonly ICandidateRepository _candidateRepository;

    public UpdateCandidateCommandHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result> Handle(UpdateCandidateCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (candidate is null)
            return Result.Fail(new NotFoundError("Candidato não encontrado"));

        candidate.UpdateInfo(
            request.Name ?? candidate.Name,
            request.Email ?? candidate.Email,
            request.Age ?? candidate.Age,
            request.Resume ?? candidate.Resume
        );

        await _candidateRepository.UpdateAsync(candidate);
        return Result.Ok();
    }
}