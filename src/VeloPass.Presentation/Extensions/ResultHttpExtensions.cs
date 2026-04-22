using VeloPass.Domain.Abstractions;

namespace VeloPass.Presentation.Extensions;

internal static class ResultHttpExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        ArgumentNullException.ThrowIfNull(result);
        
        if (result.IsSuccess)
            return Results.Ok(result.Value);
        
        var err = result.Error!;
        
        if (err.ValidationErrors is { } errors)
        {
            var dict = errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());
            return Results.ValidationProblem(
                errors: dict,
                title: err.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (err.DomainError == DomainError.NotFound)
        {
            return Results.NotFound(new { message = err.Message });
        }

        if (err.DomainError == DomainError.InternalServerError)
        {
            return Results.Problem(
                title: err.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }

        if (err.DomainError == DomainError.Unauthorized)
        {
            return Results.Json(
                new { message = err.Message },
                statusCode: StatusCodes.Status401Unauthorized);
        }
        
        return Results.BadRequest(new { message = err.Message });
    }
}