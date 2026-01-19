using Ats.Domain.Enums;
using Ats.Domain.Exceptions;

namespace Ats.Domain.Entities;

public class JobApplication : Entity
{
    public Guid JobId { get; private set; }
    public Guid CandidateId { get; private set; }
    public ApplicationStatus Status { get; private set; }

    public JobApplication(Guid jobId, Guid candidateId) : base()
    {
        if (jobId == Guid.Empty) throw new DomainException("ID da vaga inválido.");
        if (candidateId == Guid.Empty) throw new DomainException("ID do candidato inválido.");

        JobId = jobId;
        CandidateId = candidateId;
        Status = ApplicationStatus.Applied;
    }

    public void MoveToReview()
    {
        if (Status == ApplicationStatus.Rejected || Status == ApplicationStatus.Hired)
            throw new DomainException("Não é possível mover uma candidatura finalizada.");

        Status = ApplicationStatus.Reviewing;
        UpdateTimestamp();
    }

    public void MoveToInterview()
    {
        if (Status == ApplicationStatus.Rejected)
            throw new DomainException("Candidato já reprovado.");

        Status = ApplicationStatus.Interviewing;
        UpdateTimestamp();
    }

    public void Reject()
    {
        if (Status == ApplicationStatus.Hired)
            throw new DomainException("Não é possível rejeitar um candidato já contratado.");

        Status = ApplicationStatus.Rejected;
        UpdateTimestamp();
    }

    public void Hire()
    {
        if (Status == ApplicationStatus.Rejected)
            throw new DomainException("Não é possível contratar um candidato rejeitado.");

        Status = ApplicationStatus.Hired;
        UpdateTimestamp();
    }
}