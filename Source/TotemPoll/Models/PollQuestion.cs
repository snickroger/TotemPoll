using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Totem;

namespace TotemPoll.Models
{
  public class PollQuestion
  {
    public Id Id { get; set; }
    public string CreatedBy { get; set; }
    public SelectionType SelectionType { get; set; }
    public string Question { get; set; }
    public DateTime? Expires { get; set; }
    public List<PollChoice> Choices { get; set; }
    public int TotalVotes { get; private set; }
    public string Location => $"/api/poll/{Id.ToText()}";
    [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
    public object ChartData => GetChartData();

    public static PollQuestion From(PollQuestionDto dto, string username)
    {
      return new PollQuestion
      {
        Id = Id.FromGuid(),
        CreatedBy = username,
        SelectionType = dto.AllowMultiple ? SelectionType.MultipleChoice : SelectionType.SingleChoice,
        Question = dto.Question,
        Choices = dto.Choices.Select(c => new PollChoice(c)).ToList(),
        Expires = dto.Expires.HasValue ? (DateTime?)(DateTime.UtcNow + TimeSpan.FromSeconds(dto.Expires.Value)) : null,
        TotalVotes = 0
      };
    }

    private PollQuestion()
    {
      
    }

    internal void IncrementTotalVotes()
    {
      TotalVotes++;
    }

    internal object GetChartData()
    {
      return new
      {
        data = Choices.Select(a => a.Votes).ToArray(),
        labels = Choices.Select(a => a.Text).ToArray()
      };
    }

  }

  public enum SelectionType
  {
    SingleChoice,
    MultipleChoice
  }

}
