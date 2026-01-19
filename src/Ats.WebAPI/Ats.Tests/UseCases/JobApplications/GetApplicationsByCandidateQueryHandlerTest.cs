using Ats.Application.UseCases.JobApplications;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.JobApplications;

public class GetApplicationsByCandidateQueryHandlerTest
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly GetApplicationsByCandidateQueryHandler _handler;

    public GetApplicationsByCandidateQueryHandlerTest()
    {
        _applicationRepository = Substitute.For<IJobApplicationRepository>();
        _jobRepository = Substitute.For<IJobRepository>();

        _handler = new GetApplicationsByCandidateQueryHandler(
            _applicationRepository,
            _jobRepository
        );
    }

    [Fact]
    public async Task Handle_WhenCandidateHasNoApplications_ShouldReturnEmptyList()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var query = new GetApplicationsByCandidateQuery(candidateId);

        _applicationRepository.GetByCandidateIdAsync(candidateId, Arg.Any<CancellationToken>())
            .Returns(new List<JobApplication>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        await _jobRepository.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenApplicationsAndJobsExist_ShouldReturnEnrichedData()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var query = new GetApplicationsByCandidateQuery(candidateId);

        var application = new JobApplication(jobId, candidateId);

        var applicationsList = new List<JobApplication> { application };

        _applicationRepository.GetByCandidateIdAsync(candidateId, Arg.Any<CancellationToken>())
            .Returns(applicationsList);

        var job = new Job("Engenheiro de Dados", "ETL e Big Data", 12000m);

        _jobRepository.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
            .Returns(job);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);

        var responseItem = result.Value.First();

        responseItem.ApplicationId.Should().Be(application.Id);
        responseItem.Status.Should().Be(application.Status);
        responseItem.AppliedAt.Should().Be(application.CreatedAt);

        responseItem.JobId.Should().Be(jobId);
        responseItem.JobTitle.Should().Be("Engenheiro de Dados");
        responseItem.IsJobActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenJobIsMissingForApplication_ShouldSkipItemInResponse()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var jobIdInexistente = Guid.NewGuid();
        var query = new GetApplicationsByCandidateQuery(candidateId);

        var application = new JobApplication(jobIdInexistente, candidateId);
        var applicationsList = new List<JobApplication> { application };

        _applicationRepository.GetByCandidateIdAsync(candidateId, Arg.Any<CancellationToken>())
            .Returns(applicationsList);

        _jobRepository.GetByIdAsync(jobIdInexistente, Arg.Any<CancellationToken>())
            .Returns((Job?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().BeEmpty();
    }
}