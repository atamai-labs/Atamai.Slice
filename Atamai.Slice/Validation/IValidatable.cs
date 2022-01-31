namespace Atamai.Slice.Validation;

public interface IValidatable
{
    void Validate(in ValidationContext context);
}