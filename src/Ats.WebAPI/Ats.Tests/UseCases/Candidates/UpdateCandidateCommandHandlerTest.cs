using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Candidates;
using Ats.Domain.Entities;
using FluentAssertions;
using NSubstitute;
using static Ats.Domain.Entities.Candidate;

namespace Ats.Tests.UseCases.Candidates;

public class UpdateCandidateCommandHandlerTest
{
    private readonly ICandidateRepository _repository;
    private readonly UpdateCandidateCommandHandler _handler;

    public UpdateCandidateCommandHandlerTest()
    {
        _repository = Substitute.For<ICandidateRepository>();
        _handler = new UpdateCandidateCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCandidateExists_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var existingCandidate = new Candidate("Nome Antigo", "antigo@email.com", 25, "Resume Antigo");

        var command = new UpdateCandidateCommand(candidateId, "Nome Novo", null, null, null);

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(existingCandidate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        existingCandidate.Name.Should().Be("Nome Novo");
        existingCandidate.Email.Should().Be("antigo@email.com");
        existingCandidate.Age.Should().Be(25);

        await _repository.Received(1).UpdateAsync(existingCandidate);
    }

    [Fact]
    public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var command = new UpdateCandidateCommand(candidateId, "Novo Nome", "novo@email.com", 30, null);

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Candidate>());
    }
}
