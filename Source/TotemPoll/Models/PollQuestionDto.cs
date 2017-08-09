using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotemPoll.Models
{
  public class PollQuestionDto : IValidatable
  {
    public bool AllowMultiple { get; set; }
    public string Question { get; set; }
    public int? Expires { get; set; }
    public string[] Choices { get; set; }

    public ValidationResult Validate()
    {
      var errors = new List<string>();
      if (string.IsNullOrWhiteSpace(Question))
      {
        errors.Add($"'{nameof(Question)}' is required.");
      }

      if (Expires.HasValue && Expires.Value <= 0)
      {
        errors.Add("Expiration time must be a positive number if provided.");
      }

      if (Choices?.Length < 2)
      {
        errors.Add("At least two choices for this poll question must be provided.");
      }

      return new ValidationResult(errors);
    }
  }
}
