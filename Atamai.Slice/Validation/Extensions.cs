using Microsoft.AspNetCore.Http;

namespace Atamai.Slice.Validation;

public static class Extensions
{
    public static IResult? Validate<T>(this T validatable) where T : IValidatable
    {
        var validationContext = new ValidationContext();
        validatable.Validate(in validationContext);

        if (!validationContext.IsValid)
            return Results.ValidationProblem(validationContext.Errors!, string.Empty, string.Empty, StatusCodes.Status400BadRequest);

        return default;
    }
}