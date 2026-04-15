using VeloPass.Application.Abstractions;
using VeloPass.Domain.Users.Events;

namespace VeloPass.Application.Users.UserCreated;

public class UserCreatedDomainEventHandler(
    IEmailSender emailSender)
{
    public async Task Handle(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        
        await emailSender.SendTemplatedAsync(domainEvent.Email, "d-ef0a9edf88d542209d706f1ce39d4439", new
        {
            email = domainEvent.Email
        }, cancellationToken);
    }
}