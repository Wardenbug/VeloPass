namespace VeloPass.Domain.Abstractions;

public sealed record DomainError(string Code, string Name)
{
    public static readonly DomainError None = new(string.Empty, string.Empty);
    
    public static readonly DomainError NotFound = new("DomainError.NotFound", "Entity not found");
    
    public static readonly DomainError InternalServerError = new("DomainError.InternalServerError", "Internal server error");
    
    public static readonly DomainError Invalid = new("DomainError.Invalid", "Invalid error");
    
}