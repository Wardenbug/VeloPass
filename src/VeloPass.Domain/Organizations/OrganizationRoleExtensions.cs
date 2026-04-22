namespace VeloPass.Domain.Organizations;

public static class OrganizationRoleExtensions
{
    public static bool IsPrivileged(this OrganizationRole role) =>
        role is OrganizationRole.Admin or OrganizationRole.Owner;
}