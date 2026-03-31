namespace VeloPass.Application.Identity;

public sealed record ValidatedExternalIdentity(
    ExternalIdentityProvider Provider,
    string Subject,
    string Email,
    string Name,
    bool EmailVerified);