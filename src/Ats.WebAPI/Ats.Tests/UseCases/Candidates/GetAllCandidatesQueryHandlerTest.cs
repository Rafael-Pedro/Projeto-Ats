using Ats.Application.UseCases.Candidates;
using Ats.Domain.Common;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Candidates;

public class GetAllCandidatesQueryHandlerTest
{
    private readonly ICandidateRepository _repository;
    private readonly GetAllCandidatesQueryHandler _handler;

    public GetAllCandidatesQueryHandlerTest()
    {
        _repository = Substitute.For<ICandidateRepository>();
        _handler = new GetAllCandidatesQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCandidatesExist_ShouldReturnPagedDtos()
    {
        // Arrange
        var query = new GetAllCandidatesQuery(Page: 1, PageSize: 10);
        var candidates = new List<Candidate>
        {
            new("Candidato 1", "c1@email.com", 20, "Resume 1"),
            new("Candidato 2", "c2@email.com", 30, "Resume 2")
        };

        var pagedResult = new PagedResult<Candidate>(candidates, TotalCount: 2, Page: 1, PageSize: 10);

        _repository.GetAllPaginatedAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
                   .Returns(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);

        var firstDto = result.Value.Items.First();
        firstDto.Name.Should().Be("Candidato 1");
        firstDto.Email.Should().Be("c1@email.com");

        await _repository.Received(1).GetAllPaginatedAsync(1, 10, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenNoCandidatesFound_ShouldReturnEmptyPagedResult()
    {
        // Arrange
        var query = new GetAllCandidatesQuery(1, 10);
        var emptyPagedResult = new PagedResult<Candidate>(Enumerable.Empty<Candidate>(), 0, 1, 10);

        _repository.GetAllPaginatedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
                   .Returns(emptyPagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }
}
