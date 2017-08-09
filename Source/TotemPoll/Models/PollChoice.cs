using System.Collections.Generic;
using Totem;

namespace TotemPoll.Models
{
  public class PollChoice
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
  }
}
