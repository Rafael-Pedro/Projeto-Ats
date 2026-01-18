using Ats.Application.FluentResultExtensions;
using FluentResults;
using Mediator;
using static Ats.Domain.Entities.Candidate;

namespace Ats.Application.UseCases.Candidates;

public record DeleteCandidateCommand(Guid Id) : IRequest<Result>;

public class DeleteCandidateCommandHandler : IRequestHandler<DeleteCandidateCommand, Result>
{
    private readonly ICandidateRepository _candidateRepository;

    public DeleteCandidateCommandHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result> Handle(DeleteCandidateCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (candidate is null)
            return Result.Fail(new NotFoundError($"Candidato com ID {request.Id} não encontrado."));

        candidate.Deactivate();

        await _candidateRepository.UpdateAsync(candidate);

        return Result.Ok();
    }
}