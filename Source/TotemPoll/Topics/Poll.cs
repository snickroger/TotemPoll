using System.Collections.Generic;
using System.Linq;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Totem;
using Totem.Runtime.Timeline;
using TotemPoll.Models;

namespace TotemPoll.Topics
{
  public class Poll : Topic
  {
    static Id RouteFirst(CreatingPoll e) => e.PollId;
	  static Id Route(PollCreated e) => e.CreatedPoll.Id;
    static Id Route(PollDeleted e) => e.PollId;
    static Id Route(PollExpired e) => e.PollId;
    static Id Route(SavingVote e) => e.PollId;

	  void When(CreatingPoll e)
	  {
	    var newPoll = JsonConvert.DeserializeObject<PollQuestion>(e.PostBody);
	    newPoll.CreatedBy = e.Username;

	    if (!newPoll.IsValid())
	    {
	      Then(new PollNotCreated(e.PollId, newPoll.AllErrors(), HttpStatusCode.UnprocessableEntity));
	      return;
	    }

	    newPoll.Id = e.PollId;

	    if (newPoll.Expires.HasValue)
	    {
	      ThenSchedule(new PollExpired(newPoll.Id), newPoll.Expires.Value);
	    }

	    Then(new PollCreated(newPoll, e.Username));
	  }

    void When(SavingVote e)
    {
      if (_isDeleted)
      {
        Then(new VoteNotSaved(e.PollId, $"Poll '{e.PollId}' not found", HttpStatusCode.NotFound));
        return;
      }

      if (_isExpired)
      {
        Then(new VoteNotSaved(e.PollId, $"Poll '{e.PollId}' has expired", HttpStatusCode.BadRequest));
        return;
      }

      PollChoiceDto choices;
      try
      {
        choices = JsonConvert.DeserializeObject<PollChoiceDto>(e.PostBody);
      }
      catch (JsonException ex)
      {
        Then(new VoteNotSaved(e.PollId, "Could not read list of choices from post body.", HttpStatusCode.BadRequest));
        return;
      }

      var choiceIds = new List<Id>();
      foreach (var choice in choices.Choices)
      {
        var id = Id.From(choice);

        if (!_choices.Any(a => a.Equals(id)))
        {
          Then(new VoteNotSaved(e.PollId, $"Poll '{e.PollId}' does not contain choice '{choice}", HttpStatusCode.NotFound));
          return;
        }

        choiceIds.Add(id);
      }

      if (choiceIds.Count == 0)
      {
        Then(new VoteNotSaved(e.PollId, "At least one choice must be supplied.", HttpStatusCode.BadRequest));
        return;
      }

      if (!_allowMultiple && choiceIds.Count > 1)
      {
        Then(new VoteNotSaved(e.PollId, "This poll does not allow multiple choices to be selected at once.", HttpStatusCode.BadRequest));
        return;
      }

      Then(new VoteSaved(e.PollId, choiceIds));
    }

    void When(PollDeleted e)
    {
      ThenDone();
    }

    bool _isDeleted;
    bool _isExpired;
    bool _allowMultiple;
    List<Id> _choices;

    void Given(PollDeleted e)
    {
      _isDeleted = true;
    }

    void Given(PollExpired e)
    {
      _isExpired = true;
    }

    void Given(PollCreated e)
    {
      _choices = new List<Id>();
      _choices.AddRange(e.CreatedPoll.Choices.Select(a=>a.Id));
      _allowMultiple = e.CreatedPoll.SelectionType == SelectionType.MultipleChoice;
    }
  }
}
