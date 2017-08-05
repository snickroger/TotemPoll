using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Timeline;
using TotemPoll.Models;

namespace TotemPoll.Views
{
  public class AllPollsView : View
  {
    // State
    public List<PollQuestion> Polls { get; } = new List<PollQuestion>();

    void When(PollCreated e)
    {
      Polls.Add(e.CreatedPoll);
    }

    void When(PollDeleted e)
    {
      Polls.RemoveAll(a => a.Id.Equals(e.PollId));
    }

    void When(VoteSaved e)
    {
      Polls.First(a => a.Id.Equals(e.PollId)).IncrementTotalVotes();
    }
  }
}
