using Ats.Application.UseCases.Candidates;
using Ats.Domain.Entities;
using Ats.Domain.Exceptions;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Candidates;

public class CreateCandidateCommandHandlerTest
{
    private readonly ICandidateRepository _repository;
    private readonly CreateCandidateCommandHandler _handler;

    public CreateCandidateCommandHandlerTest()
    {
        _repository = Substitute.For<ICandidateRepository>();
        _handler = new CreateCandidateCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateCandidateAndReturnSuccess()
    {
        // Arrange
        var fileContent = new byte[] { 0x25, 0x50, 0x44, 0x46 };

        var command = new CreateCandidateCommand(
            "João Silva",
            "joao@email.com",
            30,
            "https://linkedin.com/in/joao",
            fileContent,
            "curriculo.pdf"
        );
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBeEmpty();

        await _repository.Received(1).AddAsync(Arg.Any<Candidate>(), cancellationToken);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectDataToRepository()
    {
        // Arrange
        var fileContent = new byte[] { 1, 2, 3 };
        var command = new CreateCandidateCommand(
            "Maria Souza",
            "maria@email.com",
            25,
            "https://linkedin.com/in/maria",
            fileContent,
            "cv_maria.pdf"
        );

        Candidate? capturedCandidate = null;

        await _repository.AddAsync(Arg.Do<Candidate>(candidate => capturedCandidate = candidate), Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedCandidate.Should().NotBeNull();
        capturedCandidate!.Name.Should().Be(command.Name);
        capturedCandidate.Email.Should().Be(command.Email);
        capturedCandidate.Age.Should().Be(command.Age);

        capturedCandidate.LinkedInProfile.Should().Be(command.LinkedIn);
        capturedCandidate.ResumeFileName.Should().Be(command.ResumeFileName);
        capturedCandidate.ResumeFile.Should().BeEquivalentTo(command.ResumeFile);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ShouldPropagateException()
    {
        // Arrange
        var command = new CreateCandidateCommand("Erro", "erro@test.com", 20, null, null, null);

        _repository.When(x => x.AddAsync(Arg.Any<Candidate>(), Arg.Any<CancellationToken>()))
            .Do(x => throw new Exception("Database failure"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database failure");
    }

    [Fact]
    public async Task Handle_WhenDomainValidationFails_ShouldThrowDomainException()
    {
        // Arrange
        var command = new CreateCandidateCommand("Nome Válido", "email-invalido", 30, null, null, null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<DomainException>().WithMessage("E-mail inválido.");

        await _repository.DidNotReceive().AddAsync(Arg.Any<Candidate>(), Arg.Any<CancellationToken>());
    }
}