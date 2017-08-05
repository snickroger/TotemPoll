using System.Collections.Generic;
using Totem;

namespace TotemPoll.Models
{
  public class PollChoice : IValidatable
  {
    public Id Id { get; set; }
    public string Text { get; set; }
    public int Votes { get; set; }

    public PollChoice(string text)
    {
      Id = Id.FromGuid();
      Text = text;
      Votes = 0;
    }

    public ValidationResult Validate()
    {
      var errors = new List<string>();
      if (string.IsNullOrWhiteSpace(Text))
      {
        errors.Add($"'{nameof(Text)}' is required.");
      }

      return new ValidationResult(errors);
    }
  }
}
