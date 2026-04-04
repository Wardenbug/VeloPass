namespace VeloPass.Domain.Abstractions;

public record ValidationError(string PropertyName, string ErrorMessage);