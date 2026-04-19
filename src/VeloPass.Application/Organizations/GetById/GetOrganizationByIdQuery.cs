namespace VeloPass.Application.Organizations.GetById;

public sealed record GetOrganizationByIdQuery(Guid OrganizationId, Guid UserId);