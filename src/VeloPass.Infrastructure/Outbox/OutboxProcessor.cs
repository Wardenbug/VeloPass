using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VeloPass.Domain.Abstractions;
using VeloPass.Infrastructure.Data;
using Wolverine;

namespace VeloPass.Infrastructure.Outbox;

public class OutboxProcessor(
    ApplicationDbContext applicationDbContext,
    IMessageBus messageBus,
    ILogger<OutboxProcessor> logger) : IOutboxProcessor
{
    private const int BatchSize = 10;
    
    public async Task ProcessAsync()
    {
        var messages = await applicationDbContext.Set<OutboxMessage>()
            .Where(om => om.ProcessedOnUtc == null)
            .Take(BatchSize)
            .ToListAsync();

        if (messages.Count == 0)
        {
           logger.LogInformation("No outbox messages found");
           return;
        }

        foreach (var outboxMessage in messages)
        {
            var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            if (domainEvent is not null)
            {
                await messageBus.PublishAsync(domainEvent);
                outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
            }
        }
        
        await applicationDbContext.SaveChangesAsync();
    }
}