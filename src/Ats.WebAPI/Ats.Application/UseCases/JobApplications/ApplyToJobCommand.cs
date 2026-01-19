using Ats.Application.FluentResultExtensions;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.JobApplications;

public record ApplyToJobCommand(
    Guid CandidateId,
    Guid JobId
) : IRequest<Result<ApplyToJobResponse>>;

public class ApplyToJobCommandHandler : IRequestHandler<ApplyToJobCommand, Result<ApplyToJobResponse>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IJobRepository _jobRepository;

    public ApplyToJobCommandHandler(
        IJobApplicationRepository applicationRepository,
        ICandidateRepository candidateRepository,
        IJobRepository jobRepository)
    {
        _applicationRepository = applicationRepository;
        _candidateRepository = candidateRepository;
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result<ApplyToJobResponse>> Handle(ApplyToJobCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetByIdAsync(request.CandidateId, cancellationToken);
        if (candidate is null)
            return Result.Fail(new NotFoundError("Candidato não encontrado."));

        var job = await _jobRepository.GetByIdAsync(request.JobId, cancellationToken);
        if (job is null)
            return Result.Fail(new NotFoundError("Vaga não encontrada."));

        if (!job.IsActive)
            return Result.Fail(new ValidationError("Esta vaga está encerrada e não aceita novas candidaturas."));

        var exists = await _applicationRepository.ExistsAsync(request.JobId, request.CandidateId, cancellationToken);
        if (exists)
            return Result.Fail(new ValidationError("O candidato já se aplicou para esta vaga."));

        var application = new JobApplication(job.Id, candidate.Id);

        await _applicationRepository.AddAsync(application, cancellationToken);

        return Result.Ok(new ApplyToJobResponse(application.Id));
    }
}

public record ApplyToJobResponse(Guid Id);