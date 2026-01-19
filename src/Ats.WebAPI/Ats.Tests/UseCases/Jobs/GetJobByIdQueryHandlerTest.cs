using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Jobs;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Jobs;

public class GetJobByIdQueryHandlerTest
{
    private readonly IJobRepository _repository;
    private readonly GetJobByIdQueryHandler _handler;

    public GetJobByIdQueryHandlerTest()
    {
        _repository = Substitute.For<IJobRepository>();
        _handler = new GetJobByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenJobExists_ShouldReturnSuccessAndMappedData()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var query = new GetJobByIdQuery(jobId);

        var job = new Job("Arquiteto de Software", "Definição de padrões", 15000m);

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
                   .Returns(job);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var response = result.Value;
        response.Id.Should().Be(job.Id);
        response.Title.Should().Be(job.Title);
        response.Description.Should().Be(job.Description);
        response.Salary.Should().Be(job.Salary);
        response.IsActive.Should().Be(job.IsActive);
        response.CreatedAt.Should().Be(job.CreatedAt);

        response.UpdatedAt.Should().Be(job.UpdatedAt);
        response.DeletedAt.Should().Be(job.DeletedAt);
        response.IsDeleted.Should().Be(job.IsDeleted);
    }

    [Fact]
    public async Task Handle_WhenJobDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var query = new GetJobByIdQuery(jobId);

        _repository.GetByIdAsync(jobId, Arg.Any<CancellationToken>())
                   .Returns((Job?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.Should().Contain(e => e.Message == "Vaga não encontrada.");

        result.Errors.First().Should().BeOfType<NotFoundError>();
    }
}