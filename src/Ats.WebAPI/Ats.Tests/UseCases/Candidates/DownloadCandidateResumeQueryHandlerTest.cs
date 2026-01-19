using Ats.Application.FluentResultExtensions;
using Ats.Application.UseCases.Candidates;
using Ats.Domain.Entities;
using Ats.Domain.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace Ats.Tests.UseCases.Candidates;

public class DownloadCandidateResumeQueryHandlerTest
{
    private readonly ICandidateRepository _repository;
    private readonly DownloadCandidateResumeQueryHandler _handler;

    public DownloadCandidateResumeQueryHandlerTest()
    {
        _repository = Substitute.For<ICandidateRepository>();
        _handler = new DownloadCandidateResumeQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenCandidateDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var query = new DownloadCandidateResumeQuery(candidateId);

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns((Candidate?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);
        result.Errors.First().Message.Should().Be("Candidato não encontrado.");
    }

    [Fact]
    public async Task Handle_WhenCandidateExistsButHasNoFile_ShouldReturnError()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var query = new DownloadCandidateResumeQuery(candidateId);

        var candidate = new Candidate(
            "Sem Arquivo",
            "sem@email.com",
            25,
            "linkedin",
            null, // <--- Sem bytes
            null  // <--- Sem nome
        );

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(candidate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is NotFoundError);
        result.Errors.First().Message.Should().Be("Este candidato não possui currículo anexado.");
    }

    [Fact]
    public async Task Handle_WhenCandidateHasFile_ShouldReturnFileContentAndName()
    {
        // Arrange
        var candidateId = Guid.NewGuid();
        var query = new DownloadCandidateResumeQuery(candidateId);

        var expectedBytes = new byte[] { 0xCA, 0xFE, 0xBA, 0xBE };
        var expectedFileName = "meu_curriculo_final.pdf";

        var candidate = new Candidate(
            "Com Arquivo",
            "com@email.com",
            25,
            "linkedin",
            expectedBytes,
            expectedFileName
        );

        _repository.GetByIdAsync(candidateId, Arg.Any<CancellationToken>())
                   .Returns(candidate);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.FileName.Should().Be(expectedFileName);
        result.Value.FileContent.Should().BeEquivalentTo(expectedBytes);
    }
}