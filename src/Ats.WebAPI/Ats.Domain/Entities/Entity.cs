namespace Ats.Domain.Entities;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void Deactivate()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
    
    public void UpdateTimestamp() => UpdatedAt = DateTime.UtcNow;
}