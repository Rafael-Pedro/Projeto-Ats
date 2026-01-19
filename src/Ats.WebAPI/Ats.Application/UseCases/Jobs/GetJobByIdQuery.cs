using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.Jobs;

public record GetJobByIdQuery(Guid Id) : IRequest<Result<GetJobByIdResponse>>;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, Result<GetJobByIdResponse>>
{
    private readonly IJobRepository _jobRepository;

    public GetJobByIdQueryHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async ValueTask<Result<GetJobByIdResponse>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id, cancellationToken);

        if (job is null)
            return Result.Fail(new NotFoundError($"Vaga não encontrada."));

        var response = new GetJobByIdResponse(
            job.Id,
            job.Title,
            job.Description,
            job.Salary,
            job.IsActive,
            job.CreatedAt,
            job.UpdatedAt,
            job.DeletedAt,
            job.IsDeleted
        );

        return Result.Ok(response);
    }
}

public record GetJobByIdResponse(
    Guid Id,
    string Title,
    string Description,
    decimal? Salary,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? DeletedAt,
    bool IsDeleted
);