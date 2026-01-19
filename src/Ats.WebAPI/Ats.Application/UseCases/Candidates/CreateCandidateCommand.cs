using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Candidates;

public record CreateCandidateCommand(
    string Name,
    string Email,
    int Age,
    string? LinkedIn,
    byte[]? ResumeFile,
    string? ResumeFileName
) : IRequest<Result<CreateCandidateCommandResponse>>;

public class CreateCandidateCommandHandler : IRequestHandler<CreateCandidateCommand, Result<CreateCandidateCommandResponse>>
{
    private readonly ICandidateRepository _candidateRepository;

    public CreateCandidateCommandHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result<CreateCandidateCommandResponse>> Handle(CreateCandidateCommand request, CancellationToken cancellationToken)
    {
         var candidate = new Candidate(
            request.Name,
            request.Email,
            request.Age,
            request.LinkedIn,
            request.ResumeFile,
            request.ResumeFileName
        );

        await _candidateRepository.AddAsync(candidate, cancellationToken);

        var response = new CreateCandidateCommandResponse(candidate.Id);

        return Result.Ok(response);
    }
}

// 3. A Resposta
public record CreateCandidateCommandResponse(Guid Id);