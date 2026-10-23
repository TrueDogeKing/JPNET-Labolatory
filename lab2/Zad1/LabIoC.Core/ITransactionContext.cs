namespace LabIoC.Core;

public interface ITransactionContext
{
    Guid TransactionId { get; }
}

public class TransactionContext : ITransactionContext
{
    public Guid TransactionId { get; } = Guid.NewGuid();
}

public class StepService(ITransactionContext context)
{
    public Guid GetCurrentTransaction() => context.TransactionId;
}