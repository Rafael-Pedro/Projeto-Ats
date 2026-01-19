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
    string? LinkedIn,
    byte[]? ResumeFile,
    string? ResumeFileName
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
            request.LinkedIn ?? candidate.LinkedInProfile
        );

        if (request.ResumeFile != null && request.ResumeFile.Length > 0)
        {
            candidate.UploadResume(request.ResumeFile, request.ResumeFileName!);
        }

        await _candidateRepository.UpdateAsync(candidate, cancellationToken);
        return Result.Ok();
    }
}