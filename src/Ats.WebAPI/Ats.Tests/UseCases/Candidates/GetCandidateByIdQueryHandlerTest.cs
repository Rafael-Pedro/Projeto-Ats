using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Candidates;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Candidates;

public class GetCandidateByIdQueryHandlerTest
{
    private readonly ICandidateRepository _repository;
    private readonly GetCandidateByIdQueryHandler _handler;

    public GetCandidateByIdQueryHandlerTest()
    {
        _repository = Substitute.For<ICandidateRepository>();
        _handler = new GetCandidateByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCandidateExists_ShouldReturnMappedResponse()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var query = new GetCandidateByIdQuery(candidateId);

        var candidate = new Candidate("Candidato teste", "ct@email.com", 32, "Currículo teste");

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(candidate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(candidate.Name);
        result.Value.Email.Should().Be(candidate.Email);
        result.Value.Age.Should().Be(candidate.Age);
        result.Value.Resume.Should().Be(candidate.Resume);
        result.Value.IsDeleted.Should().Be(candidate.IsDeleted);

        await _repository.Received(1).GetByIdAsync(candidateId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var query = new GetCandidateByIdQuery(candidateId);

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);
        result.Errors.First().Message.Should().Be("Candidato não encontrado.");
    }
}
