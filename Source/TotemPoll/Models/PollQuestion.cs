using System;
using System.Collections.Generic;
using System.Linq;
using Totem;

namespace TotemPoll.Models
{
  public class PollQuestion : IValidatable
  {
    public Id Id { get; set; }
    public string CreatedBy { get; set; }
    public SelectionType SelectionType { get; set; }
    public string Question { get; set; }
    public DateTime? Expires { get; set; }
    public List<PollChoice> Choices { get; set; }
    public int TotalVotes { get; private set;  }
    public string Location => $"/api/poll/{Id.ToText()}";

    public ValidationResult Validate()
    {
      var errors = new List<string>();
      if (string.IsNullOrWhiteSpace(Question))
      {
        errors.Add($"'{nameof(Question)}' is required.");
      }

      if (string.IsNullOrWhiteSpace(CreatedBy))
      {
        errors.Add("'Created polls require a valid user.");
      }

      if (Expires.HasValue && Expires.Value < DateTime.UtcNow)
      {
        errors.Add("Expiration time must be a time in the future.");
      }

      if (Choices?.Any() != true)
      {
        errors.Add("At least one choice for this poll question must be provided.");
      }

      return new ValidationResult(errors);
    }

    internal void IncrementTotalVotes()
    {
      TotalVotes++;
    }
  }

  public enum SelectionType
  {
    SingleChoice,
    MultipleChoice
  }

}
