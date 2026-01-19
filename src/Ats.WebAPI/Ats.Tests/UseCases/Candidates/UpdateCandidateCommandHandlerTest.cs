using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Candidates;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

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

        var existingCandidate = new Candidate(
            "Nome Antigo",
            "antigo@email.com",
            25,
            "linkedin-antigo",
            null,
            null
        );

        var command = new UpdateCandidateCommand(
            candidateId,
            "Nome Novo",
            null,
            null,
            null,
            null,
            null
        );

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(existingCandidate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        existingCandidate.Name.Should().Be("Nome Novo");
        existingCandidate.Email.Should().Be("antigo@email.com");
        existingCandidate.Age.Should().Be(25);
        existingCandidate.LinkedInProfile.Should().Be("linkedin-antigo");
        existingCandidate.ResumeFile.Should().BeNull();

        await _repository.Received(1).UpdateAsync(existingCandidate, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var candidateId = Guid.NewGuid();

        var command = new UpdateCandidateCommand(
            candidateId,
            "Novo Nome",
            "novo@email.com",
            30,
            "linkedin",
            null,
            null
        );

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);

        await _repository.DidNotReceive().UpdateAsync(Arg.Any<Candidate>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenFileIsProvided_ShouldUpdateResumeFile()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var existingCandidate = new Candidate("João", "joao@email.com", 25, "linkedin", null, null);

        var newFileContent = new byte[] { 1, 2, 3, 4, 5 };
        var newFileName = "novo_cv.pdf";

        var command = new UpdateCandidateCommand(
            candidateId,
            null,
            null,
            null,
            null,
            newFileContent,
            newFileName
        );

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(existingCandidate);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        existingCandidate.ResumeFile.Should().BeEquivalentTo(newFileContent);
        existingCandidate.ResumeFileName.Should().Be(newFileName);

        existingCandidate.Name.Should().Be("João");

        await _repository.Received(1).UpdateAsync(existingCandidate, Arg.Any<CancellationToken>());
    }
}