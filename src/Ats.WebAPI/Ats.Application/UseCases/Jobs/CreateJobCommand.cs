using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Jobs;

public record CreateJobCommand(
    string Title,
    string Description,
    decimal? Salary
    ) : IRequest<Result<CreateJobCommandResponse>>;

public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<CreateJobCommandResponse>>
{
    private readonly IJobRepository _jobRepository;

    public CreateJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result<CreateJobCommandResponse>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var job = new Job(
            request.Title,
            request.Description,
            request.Salary
        );

        await _jobRepository.AddAsync(job, cancellationToken);

        var response = new CreateJobCommandResponse(job.Id);

        return Result.Ok(response);
    }
}

public record CreateJobCommandResponse(Guid Id);