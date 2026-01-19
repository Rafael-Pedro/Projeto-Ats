using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.JobApplications;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.JobApplications;

public class ChangeApplicationStatusCommandHandlerTest
{
    private readonly IJobApplicationRepository _repository;
    private readonly ChangeApplicationStatusCommandHandler _handler;

    public ChangeApplicationStatusCommandHandlerTest()
    {
        _repository = Substitute.For<IJobApplicationRepository>();
        _handler = new ChangeApplicationStatusCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenActionIsInterview_ShouldUpdateStatusAndSave()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var command = new ChangeApplicationStatusCommand(appId, "Interview");

        var application = new JobApplication(Guid.NewGuid(), Guid.NewGuid());

        _repository.GetByIdAsync(appId, Arg.Any<CancellationToken>())
                   .Returns(application);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).UpdateAsync(application, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenActionIsReject_ShouldUpdateStatusAndSave()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var command = new ChangeApplicationStatusCommand(appId, "reject");

        var application = new JobApplication(Guid.NewGuid(), Guid.NewGuid());

        _repository.GetByIdAsync(appId, Arg.Any<CancellationToken>())
                   .Returns(application);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _repository.Received(1).UpdateAsync(application, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenApplicationNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new ChangeApplicationStatusCommand(Guid.NewGuid(), "interview");

        _repository.GetByIdAsync(command.ApplicationId, Arg.Any<CancellationToken>())
                   .Returns((JobApplication?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Candidatura não encontrada.");
        result.Errors.First().Should().BeOfType<NotFoundError>();

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenActionIsInvalid_ShouldReturnFail()
    {
        // Arrange
        var appId = Guid.NewGuid();
        var command = new ChangeApplicationStatusCommand(appId, "banana");

        var application = new JobApplication(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(appId, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Ação inválida.");

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenDomainLogicThrowsException_ShouldCatchAndReturnFail()
    {
        // Arrange
        var appId = Guid.NewGuid();

        var command = new ChangeApplicationStatusCommand(appId, "interview");

        var application = new JobApplication(Guid.NewGuid(), Guid.NewGuid());

        application.Reject();


        _repository.GetByIdAsync(appId, Arg.Any<CancellationToken>()).Returns(application);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should().NotBeEmpty();

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }
}