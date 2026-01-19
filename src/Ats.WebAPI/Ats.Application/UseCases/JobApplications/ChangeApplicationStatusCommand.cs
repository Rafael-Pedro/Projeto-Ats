using Ats.Application.FluentResultExtensions;
using Ats.Domain.Interfaces;
using FluentResults;
using Mediator;

namespace Ats.Application.UseCases.JobApplications;

public record ChangeApplicationStatusCommand(Guid ApplicationId, string Action) : IRequest<Result>;

public class ChangeApplicationStatusCommandHandler : IRequestHandler<ChangeApplicationStatusCommand, Result>
{
    private readonly IJobApplicationRepository _repository;

    public ChangeApplicationStatusCommandHandler(IJobApplicationRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<Result> Handle(ChangeApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (application is null) return Result.Fail(new NotFoundError("Candidatura não encontrada."));

        try
        {
            switch (request.Action.ToLower())
            {
                case "interview": application.MoveToInterview(); break;
                case "reject": application.Reject(); break;
                //TODO: Adicionar Hire() e/ou MoveToReview()

                default: return Result.Fail("Ação inválida.");
            }
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }

        await _repository.UpdateAsync(application, cancellationToken);
        return Result.Ok();
    }
}