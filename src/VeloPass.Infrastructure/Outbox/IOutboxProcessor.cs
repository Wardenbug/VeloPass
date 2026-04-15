namespace VeloPass.Infrastructure.Outbox;

public interface IOutboxProcessor
{
    Task ProcessAsync();
}