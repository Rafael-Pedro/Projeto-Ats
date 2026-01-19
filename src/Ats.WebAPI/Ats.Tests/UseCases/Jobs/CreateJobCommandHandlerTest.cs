using Ats.Application.UseCases.Jobs;
using Ats.Domain.Entities;
using Ats.Domain.Exceptions;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Jobs;

public class CreateJobCommandHandlerTest
{
    private readonly IJobRepository _repository;
    private readonly CreateJobCommandHandler _handler;

    public CreateJobCommandHandlerTest()
    {
        _repository = Substitute.For<IJobRepository>();
        _handler = new CreateJobCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateJobAndReturnSuccess()
    {
        // Arrange
        var command = new CreateJobCommand(
            "Desenvolvedor Backend .NET",
            "Atuar com C#, Clean Architecture e DDD",
            8500.00m
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();

        await _repository.Received(1).AddAsync(Arg.Any<Job>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectDataToRepository()
    {
        // Arrange
        var command = new CreateJobCommand(
            "Product Owner",
            "Gestão de Backlog",
            12000m
        );

        Job? capturedJob = null;

        await _repository.AddAsync(Arg.Do<Job>(job => capturedJob = job), Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedJob.Should().NotBeNull();
        capturedJob!.Title.Should().Be(command.Title);
        capturedJob.Description.Should().Be(command.Description);
        capturedJob.Salary.Should().Be(command.Salary);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateJobCommand("DevOps", "CI/CD Pipelines", 9000m);

        _repository.When(x => x.AddAsync(Arg.Any<Job>(), Arg.Any<CancellationToken>()))
            .Do(x => throw new Exception("Database connection error"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database connection error");
    }

    [Fact]
    public async Task Handle_WhenDomainValidationFails_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateJobCommand("", "Description", 1000m);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>();

        await _repository.DidNotReceive().AddAsync(Arg.Any<Job>(), Arg.Any<CancellationToken>());
    }
}