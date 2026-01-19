using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Candidates;

public record DisableCandidateCommand(Guid Id) : IRequest<Result>;

public class DisableCandidateCommandHandler : IRequestHandler<DisableCandidateCommand, Result>
{
    private readonly ICandidateRepository _candidateRepository;

    public DisableCandidateCommandHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result> Handle(DisableCandidateCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (candidate is null)
            return Result.Fail(new NotFoundError($"Candidato com ID {request.Id} não encontrado."));

        candidate.Deactivate();

        await _candidateRepository.UpdateAsync(candidate);

        return Result.Ok();
    }
}