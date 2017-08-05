using System.Collections.Generic;
using System.Linq;
using Nancy;
using Totem;
using Totem.Runtime.Timeline;
using Totem.Web.Push;
using TotemPoll.Models;

namespace TotemPoll.Topics
{
  public class AllPolls : Topic
  {
    void When(DeletingPoll e)
    {
      if (!PollExists(e.PollId))
      {
        Then(new PollNotDeleted(e.PollId, $"Poll '{e.PollId}' not found", HttpStatusCode.NotFound));
        return;
      }

      ThenDone(new PollDeleted(e.PollId, e.Username));
    }

    void When(SavingVote e)
    {
      if (!PollExists(e.PollId))
      {
        Then(new VoteNotSaved(e.PollId, $"Poll '{e.PollId}' not found", HttpStatusCode.NotFound));
      }
    }

    void When(PollCreated e, IPushChannel push)
    {
      push.PushToAll(e);
    }

    // State
    readonly List<PollQuestion> _polls = new List<PollQuestion>();
    readonly List<PollQuestion> _activePolls = new List<PollQuestion>();

    void Given(PollCreated e)
    {
      _polls.Add(e.CreatedPoll);
      _activePolls.Add(e.CreatedPoll);
    }

    void Given(PollDeleted e)
    {
      _polls.RemoveAll(a => a.Id.Equals(e.PollId));
      _activePolls.RemoveAll(a => a.Id.Equals(e.PollId));
    }

    void Given(PollExpired e)
    {
      _activePolls.RemoveAll(a => a.Id.Equals(e.PollId));
    }

    // Private methods
    private bool PollExists(Id pollId)
    {
      return _polls.Any(a => a.Id.Equals(pollId));
    }
  }
}
