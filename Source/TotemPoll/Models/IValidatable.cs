using System.Collections.Generic;
using System.Linq;

namespace TotemPoll.Models
{
  public interface IValidatable
  {
    ValidationResult Validate();
  }

  public class ValidationResult
  {
    public bool IsValid => !Errors.Any();
    public List<string> Errors { get; }

    public ValidationResult(List<string> errors)
    {
      Errors = errors;
    }
  }

  public static class ValidationExtensions
  {
    public static bool IsValid(this IValidatable validatable)
    {
      return validatable.Validate().IsValid;
    }

    public static string AllErrors(this IValidatable validatable)
    {
      return string.Join(" ", validatable.Validate().Errors);
    }
  }

}
