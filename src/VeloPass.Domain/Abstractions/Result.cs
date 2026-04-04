using System.Diagnostics.CodeAnalysis;

namespace VeloPass.Domain.Abstractions;

public static class Result
{
    public static Result<T> FromT<T>(T value) => Result<T>.Ok(value);

    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

    public static Result<T> ValidationFailed<T>(IEnumerable<ValidationError> errors) 
        => Result<T>.ValidationFailed(errors);

    public static Result<T> NotFound<T>(string error) 
        => Result<T>.Fail(DomainError.NotFound, error);

    public static Result<T> Invalid<T>(string error) 
        => Result<T>.Fail(DomainError.Invalid, error);

    public static Result<T> InternalServerError<T>(string error) 
        => Result<T>.Fail(DomainError.InternalServerError, error);
}

public sealed class Result<T>
{
    public T? Value { get; }
    public ErrorResponse? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
    }

    private Result(ErrorResponse error)
    {
        Error = error;
        IsSuccess = false;
    }
    
    internal static Result<T> Ok(T value) => new(value);
    
    internal static Result<T> Fail(DomainError type, string message) 
        => new(new ErrorResponse(message, type));

    internal static Result<T> ValidationFailed(IEnumerable<ValidationError> errors) 
        => new(new ErrorResponse("One or more validation errors occurred.", DomainError.Invalid, errors));
}