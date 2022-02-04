namespace Atamai.Slice.Validation;

public interface IValidatable
{
    void Validate(ValidationContext context);
}