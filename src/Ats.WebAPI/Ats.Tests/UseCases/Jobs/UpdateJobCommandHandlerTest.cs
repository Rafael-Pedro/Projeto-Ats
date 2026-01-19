using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Jobs;
using Ats.Domain.Entities;
using Ats.Domain.Exceptions;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Jobs;

public class UpdateJobCommandHandlerTest
{
    private readonly IJobRepository _repository;
    private readonly UpdateJobCommandHandler _handler;

    public UpdateJobCommandHandlerTest()
    {
        _repository = Substitute.For<IJobRepository>();
        _handler = new UpdateJobCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenJobExistsAndDataIsValid_ShouldUpdateAndSave()
    {
        // Arrange
        var jobId = Guid.NewGuid();

        var job = new Job("Título Antigo", "Descrição Antiga", 5000m);

        var command = new UpdateJobCommand(
            jobId,
            "Título Novo",
            "Descrição Nova",
            8000m
        );

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                   .Returns(job);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).UpdateAsync(job);

        job.Title.Should().Be(command.Title);
        job.Description.Should().Be(command.Description);
        job.Salary.Should().Be(command.Salary);
    }

    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new UpdateJobCommand(Guid.NewGuid(), "Titulo", "Desc", 1000m);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                   .Returns((Job?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Vaga não encontrada.");
        result.Errors.First().Should().BeOfType<NotFoundError>();

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Job>());
    }

    [Fact]
    public async Task Handle_WhenDomainValidationFails_ShouldThrowExceptionAndNotSave()
    {
        // Arrange
        var job = new Job("Título Ok", "Desc Ok", 5000m);

        var command = new UpdateJobCommand(Guid.NewGuid(), "", "Desc Nova", 5000m);

        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
                   .Returns(job);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Job>());
    }
}