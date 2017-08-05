using System.Linq;
using Totem;
using Totem.Runtime.Timeline;
using TotemPoll.Models;

namespace TotemPoll.Views
{
  public class SinglePollView : View
  {
    static Id RouteFirst(PollCreated e) => e.CreatedPoll.Id;
    static Id Route(VoteSaved e) => e.PollId;
    static Id Route(PollDeleted e) => e.PollId;

    // State
    public PollQuestion Poll { get; private set; }

    void When(PollCreated e)
    {
      Poll = e.CreatedPoll;
    }

    void When(VoteSaved e)
    {
      Poll.Choices.First(a => a.Id.Equals(e.ChoiceId)).Votes++;
    }

    void When(PollDeleted e)
    {
      ThenDone();
    }

  }
}
