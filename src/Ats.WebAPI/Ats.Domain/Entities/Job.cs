using Ats.Domain.Exceptions;

namespace Ats.Domain.Entities;

public class Job : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal? Salary { get; private set; }
    public bool IsActive { get; private set; }

    public Job(string title, string description, decimal? salary) : base()
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título da vaga é obrigatório.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição da vaga é obrigatória.");

        if (salary.HasValue && salary < 0)
            throw new DomainException("O salário não pode ser negativo.");

        Title = title;
        Description = description;
        Salary = salary;
        IsActive = true;
    }

    public void UpdateInfo(string title, string description, decimal? salary)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Título obrigatório.");

        Title = title;
        Description = description;
        Salary = salary;
        UpdateTimestamp();
    }

    public void Close()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Reopen()
    {
        IsActive = true;
        UpdateTimestamp();
    }
}