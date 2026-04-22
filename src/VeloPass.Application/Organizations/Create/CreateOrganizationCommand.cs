namespace VeloPass.Application.Organizations.Create;

public record CreateOrganizationCommand(string Name, Guid OwnerId);