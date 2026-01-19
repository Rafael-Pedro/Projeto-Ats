using Ats.Application.UseCases.Jobs;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Jobs;

public class DeleteJobCommandHandlerTest
{
    private readonly IJobRepository _repository;
    private readonly DeleteJobCommandHandler _handler;

    public DeleteJobCommandHandlerTest()
    {
        _repository = Substitute.For<IJobRepository>();
        _handler = new DeleteJobCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenJobExists_ShouldDeactivateAndUpdate()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new DeleteJobCommand(jobId);

        var job = new Job("Título Teste", "Descrição Teste", 5000m);

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
                   .Returns(job);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).GetByIdAsync(jobId, Arg.Any<CancellationToken>());

        await _repository.Received(1).UpdateAsync(job);
    }

    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var command = new DeleteJobCommand(jobId);

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
                   .Returns((Job?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should().Contain(e => e.Message == "Vaga não encontrada.");

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Job>());
    }
}