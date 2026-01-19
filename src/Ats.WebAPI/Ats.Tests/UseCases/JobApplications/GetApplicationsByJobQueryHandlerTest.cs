using Ats.Application.UseCases.JobApplications;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.JobApplications;

public class GetApplicationsByJobQueryHandlerTest
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly GetApplicationsByJobQueryHandler _handler;

    public GetApplicationsByJobQueryHandlerTest()
    {
        _applicationRepository = Substitute.For<IJobApplicationRepository>();
        _candidateRepository = Substitute.For<ICandidateRepository>();

        _handler = new GetApplicationsByJobQueryHandler(
            _applicationRepository,
            _candidateRepository
        );
    }

    [Fact]
    public async Task Handle_WhenJobHasNoApplications_ShouldReturnEmptyList()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var query = new GetApplicationsByJobQuery(jobId);

        _applicationRepository.GetByJobIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(new List<JobApplication>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        await _candidateRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenApplicationsAndCandidatesExist_ShouldReturnEnrichedData()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var candidateId = Guid.NewGuid();
        var query = new GetApplicationsByJobQuery(jobId);

        var application = new JobApplication(jobId, candidateId);

        var applicationsList = new List<JobApplication> { application };

        _applicationRepository.GetByJobIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(applicationsList);

        var candidate = new Candidate("João Silva", "joao@email.com", 30, "linkedin.com", null, null);

        _candidateRepository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
            .Returns(candidate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);

        var responseItem = result.Value.First();

        responseItem.ApplicationId.Should().Be(application.Id);
        responseItem.Status.Should().Be(application.Status);
        responseItem.AppliedAt.Should().Be(application.CreatedAt);

        responseItem.CandidateId.Should().Be(candidateId);
        responseItem.CandidateName.Should().Be("João Silva");
        responseItem.CandidateEmail.Should().Be("joao@email.com");
    }

    [Fact]
    public async Task Handle_WhenCandidateIsMissingForApplication_ShouldSkipItemInResponse()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var candidateIdInexistente = Guid.NewGuid();
        var query = new GetApplicationsByJobQuery(jobId);

        var application = new JobApplication(jobId, candidateIdInexistente);
        var applicationsList = new List<JobApplication> { application };

        _applicationRepository.GetByJobIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(applicationsList);

        _candidateRepository.GetByIdAsync(candidateIdInexistente, Arg.Any<CancellationToken>())
            .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().BeEmpty();
    }
}