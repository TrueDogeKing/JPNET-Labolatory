namespace LabIoC.Core;

public interface IUnitOfWork
{
    Guid Id { get; }
}

public class UnitOfWork : IUnitOfWork
{
    public Guid Id { get; } = Guid.NewGuid();
}