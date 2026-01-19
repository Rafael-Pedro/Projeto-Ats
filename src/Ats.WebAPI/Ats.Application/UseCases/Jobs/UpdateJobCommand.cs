using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Jobs;

public record UpdateJobCommand(
    Guid Id,
    string Title,
    string Description,
    decimal? Salary
) : IRequest<Result>;

public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, Result>
{
    private readonly IJobRepository _jobRepository;

    public UpdateJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id, cancellationToken);

        if (job is null)
            return Result.Fail(new NotFoundError("Vaga não encontrada."));

        job.UpdateInfo(request.Title, request.Description, request.Salary);

        await _jobRepository.UpdateAsync(job);

        return Result.Ok();
    }
}