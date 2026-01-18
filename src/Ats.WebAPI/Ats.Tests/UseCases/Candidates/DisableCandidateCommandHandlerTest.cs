using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Candidates;
using Ats.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using static Ats.Domain.Entities.Candidate;

namespace Ats.Tests.UseCases.Candidates;

public class DisableCandidateCommandHandlerTest
{
    private readonly ICandidateRepository _repository;
    private readonly DisableCandidateCommandHandler _handler;

    public DisableCandidateCommandHandlerTest()
    {
        _repository = Substitute.For<ICandidateRepository>();
        _handler = new DisableCandidateCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var command = new DisableCandidateCommand(candidateId);

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                       .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);
        result.Errors.First().Message.Should().Contain(candidateId.ToString());

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Candidate>());
    }

    [Fact]
    public async Task Handle_WhenCandidateExists_ShouldDeactivateAndSave()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var command = new DisableCandidateCommand(candidateId);
        var candidate = new Candidate("Teste", "teste@email.com", 25, null);

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(candidate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        candidate.IsDeleted.Should().BeTrue();

        await _repository.Received(1).UpdateAsync(Arg.Is<Candidate>(c => c.Id == candidate.Id && c.IsDeleted));
    }


}
