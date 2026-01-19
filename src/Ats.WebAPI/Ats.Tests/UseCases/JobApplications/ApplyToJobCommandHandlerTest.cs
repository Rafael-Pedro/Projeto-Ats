using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.JobApplications;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.JobApplications;

public class ApplyToJobCommandHandlerTest
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ApplyToJobCommandHandler _handler;

    public ApplyToJobCommandHandlerTest()
    {
        _applicationRepository = Substitute.For<IJobApplicationRepository>();
        _candidateRepository = Substitute.For<ICandidateRepository>();
        _jobRepository = Substitute.For<IJobRepository>();

        _handler = new ApplyToJobCommandHandler(
            _applicationRepository,
            _candidateRepository,
            _jobRepository
        );
    }

    [Fact]
    public async Task Handle_WhenEverythingIsValid_ShouldCreateApplicationAndReturnSuccess()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var command = new ApplyToJobCommand(candidateId, jobId);

        var candidate = new Candidate("Cand", "email@email", 20, "link", null, null);

        _candidateRepository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
            .Returns(candidate);

        var job = new Job("Dev", "Desc", 5000m);
        _jobRepository.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(job);

        _applicationRepository.ExistsAsync(jobId, candidateId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();

        await _applicationRepository.Received(1).AddAsync(
            Arg.Is<JobApplication>(ja => ja.JobId == job.Id && ja.CandidateId == candidate.Id),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new ApplyToJobCommand(Guid.NewGuid(), Guid.NewGuid());

        _candidateRepository.GetByIdAsync(command.CandidateId, Arg.Any<CancellationToken>())
            .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Candidato não encontrado.");
        result.Errors.First().Should().BeOfType<NotFoundError>();

        await _jobRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _applicationRepository.DidNotReceive().AddAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new ApplyToJobCommand(Guid.NewGuid(), Guid.NewGuid());

        _candidateRepository.GetByIdAsync(command.CandidateId, Arg.Any<CancellationToken>())
            .Returns(new Candidate("Nome", "email@email", 20, null, null, null));

        _jobRepository.GetByIdAsync(command.JobId, Arg.Any<CancellationToken>())
            .Returns((Job?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Vaga não encontrada.");

        await _applicationRepository.DidNotReceive().AddAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenJobIsNotActive_ShouldReturnValidationError()
    {
        // Arrange
        var command = new ApplyToJobCommand(Guid.NewGuid(), Guid.NewGuid());

        _candidateRepository.GetByIdAsync(command.CandidateId, Arg.Any<CancellationToken>())
            .Returns(new Candidate("Nome", "email@email.com", 20, null, null, null));

        var inactiveJob = new Job("Dev", "Desc", 5000m);

        inactiveJob.Close();

        _jobRepository.GetByIdAsync(command.JobId, Arg.Any<CancellationToken>())
            .Returns(inactiveJob);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "Esta vaga está encerrada e não aceita novas candidaturas.");
        result.Errors.First().Should().BeOfType<ValidationError>();

        await _applicationRepository.DidNotReceive().AddAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenApplicationAlreadyExists_ShouldReturnValidationError()
    {
        // Arrange
        var command = new ApplyToJobCommand(Guid.NewGuid(), Guid.NewGuid());

        _candidateRepository.GetByIdAsync(command.CandidateId, Arg.Any<CancellationToken>())
            .Returns(new Candidate("Nome", "email@email", 20, null, null, null));

        _jobRepository.GetByIdAsync(command.JobId, Arg.Any<CancellationToken>())
            .Returns(new Job("Dev", "Desc", 5000m));

        _applicationRepository.ExistsAsync(command.JobId, command.CandidateId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message == "O candidato já se aplicou para esta vaga.");
        result.Errors.First().Should().BeOfType<ValidationError>();

        await _applicationRepository.DidNotReceive().AddAsync(Arg.Any<JobApplication>(), Arg.Any<CancellationToken>());
    }
}