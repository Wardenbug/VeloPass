namespace VeloPass.Domain.Abstractions;

public sealed record ErrorResponse(
    string Message, 
    DomainError DomainError, 
    IEnumerable<ValidationError>? ValidationErrors = null);