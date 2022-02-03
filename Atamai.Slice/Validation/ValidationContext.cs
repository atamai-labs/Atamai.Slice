using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Atamai.Slice.Validation;

public class ValidationContext
{
    internal ValidationContext()
    {
    }

    public Dictionary<string, string[]>? Errors { get; private set; }

    public bool IsValid => Errors is null;

    private void AddError(string path, string error)
    {
        Errors ??= new();
        path = JsonNamingPolicy.CamelCase.ConvertName(path);
        error = string.Format(error, path);
        ref var errors = ref CollectionsMarshal.GetValueRefOrAddDefault(Errors, path, out var exists)!;
        errors = !exists ? new[] { error } : Add(errors, error);
    }

    static T[] Add<T>(ReadOnlySpan<T> source, T add)
    {
        var newArray = new T[source.Length + 1];
        source.CopyTo(newArray);
        newArray[^1] = add;
        return newArray;
    }

    public ValidationContext? NotNull<T>(T? value, string? message = default, [CallerArgumentExpression("value")] string path = "")
    {
        if (value is null)
        {
            AddError(path, message ?? "{0} is required");
            return null;
        }

        return this;
    }

    public ValidationContext? Custom<T>(T value, Func<T, bool> customValidator, string? message = default,
        [CallerArgumentExpression("value")] string path = "")
    {
        if (!customValidator(value))
        {
            AddError(path, message ?? "{0} is not valid");
            return null;
        }

        return this;
    }
}