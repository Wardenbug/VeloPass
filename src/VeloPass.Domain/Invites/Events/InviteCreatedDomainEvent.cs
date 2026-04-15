using VeloPass.Domain.Abstractions;

namespace VeloPass.Domain.Invites.Events;

public sealed record InviteCreatedDomainEvent(string Email, string OrganizationId): IDomainEvent;