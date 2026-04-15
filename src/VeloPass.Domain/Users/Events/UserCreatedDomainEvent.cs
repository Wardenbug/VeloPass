using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Users.Events;

public record UserCreatedDomainEvent(string Email): IDomainEvent;