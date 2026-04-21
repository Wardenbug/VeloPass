using VeloPass.Domain.Organizations;

namespace VeloPass.Presentation.Organizations;

internal sealed record ChangeRoleRequest(OrganizationRole Role);