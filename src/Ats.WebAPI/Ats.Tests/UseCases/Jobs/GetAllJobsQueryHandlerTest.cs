using Ats.Application.UseCases.Jobs;
using Ats.Domain.Common;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Jobs;

public class GetAllJobsQueryHandlerTest
{
    private readonly IJobRepository _repository;
    private readonly GetAllJobsQueryHandler _handler;

    public GetAllJobsQueryHandlerTest()
    {
        _repository = Substitute.For<IJobRepository>();
        _handler = new GetAllJobsQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenOnlyActiveIsFalse_ShouldCallGetAllPaginatedAsync()
    {
        // Arrange
        var query = new GetAllJobsQuery(Page: 1, PageSize: 10, OnlyActive: false);

        var emptyPagedResult = new PagedResult<Job>(Enumerable.Empty<Job>(), 0, 1, 10);

        _repository.GetAllPaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
                   .Returns(emptyPagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).GetAllPaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>());

        await _repository.DidNotReceive().GetAllActivePaginatedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOnlyActiveIsTrue_ShouldCallGetAllActivePaginatedAsync()
    {
        // Arrange
        var query = new GetAllJobsQuery(Page: 1, PageSize: 10, OnlyActive: true);

        var emptyPagedResult = new PagedResult<Job>(Enumerable.Empty<Job>(), 0, 1, 10);

        _repository.GetAllActivePaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
                   .Returns(emptyPagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _repository.Received(1).GetAllActivePaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>());

        await _repository.DidNotReceive().GetAllPaginatedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldMapEntitiesToResponseAndPreservePaginationMetadata()
    {
        // Arrange
        var query = new GetAllJobsQuery(Page: 2, PageSize: 5, OnlyActive: false);

        var jobs = new List<Job>
        {
            new("Vaga 1", "Desc 1", 1000m),
            new("Vaga 2", "Desc 2", 2000m)
        };

        var pagedResult = new PagedResult<Job>(jobs, 20, 2, 5);

        _repository.GetAllPaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
                   .Returns(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var response = result.Value;

        response.TotalCount.Should().Be(20);
        response.Page.Should().Be(2);
        response.PageSize.Should().Be(5);
        response.Items.Should().HaveCount(2);

        var firstItem = response.Items.First();
        firstItem.Title.Should().Be("Vaga 1");
        firstItem.Description.Should().Be("Desc 1");
        firstItem.Salary.Should().Be(1000m);
        firstItem.Id.Should().NotBeEmpty();
    }
}