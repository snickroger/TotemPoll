using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotemPoll.Models
{
  public class PollChoiceDto : IValidatable
  {
    public string[] Choices;

    public ValidationResult Validate()
    {
      var errors = new List<string>();
      if (!Choices.Any())
      {
        errors.Add("At least one choice is required for a vote.");
      }

      return new ValidationResult(errors);
    }
  }
}
