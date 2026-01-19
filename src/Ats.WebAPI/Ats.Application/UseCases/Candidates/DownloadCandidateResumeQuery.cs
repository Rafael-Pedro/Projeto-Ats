using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Candidates;

public record DownloadCandidateResumeQuery(Guid Id) : IRequest<Result<DownloadCandidateResumeResponse>>;

public record DownloadCandidateResumeResponse(string FileName, byte[] FileContent);

public class DownloadCandidateResumeQueryHandler : IRequestHandler<DownloadCandidateResumeQuery, Result<DownloadCandidateResumeResponse>>
{
    private readonly ICandidateRepository _candidateRepository;

    public DownloadCandidateResumeQueryHandler(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }

    public async ValueTask<Result<DownloadCandidateResumeResponse>> Handle(DownloadCandidateResumeQuery request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByIdAsync(request.Id, cancellationToken);

        if (candidate is null)
            return Result.Fail(new NotFoundError("Candidato não encontrado."));

        if (candidate.ResumeFile is null || candidate.ResumeFile.Length == 0)
            return Result.Fail(new NotFoundError("Este candidato não possui currículo anexado."));

        return Result.Ok(new DownloadCandidateResumeResponse(
            candidate.ResumeFileName ?? "documento_sem_nome",
            candidate.ResumeFile
        ));
    }
}