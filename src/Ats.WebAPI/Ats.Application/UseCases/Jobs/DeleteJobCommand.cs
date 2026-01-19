using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Jobs;

public record DeleteJobCommand(Guid Id) : IRequest<Result>;

public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand, Result>
{
    private readonly IJobRepository _jobRepository;

    public DeleteJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id, cancellationToken);

        if (job is null)
            return Result.Fail(new NotFoundError("Vaga não encontrada."));

        job.Deactivate();

        await _jobRepository.UpdateAsync(job);

        return Result.Ok();
    }
}