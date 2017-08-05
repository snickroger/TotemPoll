using System.Collections.Generic;
using Nancy;
using Totem;
using TotemPoll.Models;

namespace TotemPoll
{
  public abstract class ErrorEvent : Event
  {
    public Id PollId { get; protected set; }
    public string Error { get; protected set; }
    public HttpStatusCode StatusCode { get; protected set; }
  }

  public class CreatingPoll : Event
  {
    public Id PollId { get; }
    public string PostBody { get; }
    public string Username { get; }

    public CreatingPoll(string postBody, string username)
    {
      PostBody = postBody;
      Username = username;
      PollId = Id.FromGuid();
    }
  }

  public class PollCreated : Event
  {
    public PollQuestion CreatedPoll { get; }
    public string CreatedBy { get; }

    public PollCreated(PollQuestion createdPoll, string createdBy)
    {
      CreatedPoll = createdPoll;
      CreatedBy = createdBy;
    }
  }

  public class PollNotCreated : ErrorEvent
  {
    public PollNotCreated(Id id, string error, HttpStatusCode statusCode)
    {
      PollId = id;
      Error = error;
      StatusCode = statusCode;
    }
  }

  public class DeletingPoll : Event
  {
    public Id PollId { get; }
    public string Username { get; }

    public DeletingPoll(Id id, string username)
    {
      PollId = id;
      Username = username;
    }
  }

  public class PollDeleted : Event
  {
    public Id PollId { get; }
    public string DeletedBy { get; }

    public PollDeleted(Id id, string deletedBy)
    {
      PollId = id;
      DeletedBy = deletedBy;
    }
  }

  public class PollNotDeleted : ErrorEvent
  {
    public PollNotDeleted(Id id, string error, HttpStatusCode statusCode)
    {
      PollId = id;
      Error = error;
      StatusCode = statusCode;
    }
  }

  public class PollExpired : Event
  {
    public Id PollId { get; }
    public PollExpired(Id id)
    {
      PollId = id;
    }
  }

  public class SavingVote : Event
  {
    public Id PollId { get; }
    public string PostBody { get; }

    public SavingVote(Id pollId, string postBody)
    {
      PollId = pollId;
      PostBody = postBody;
    }
  }

  public class VoteSaved : Event
  {
    public Id PollId { get; }
    public IEnumerable<Id> ChoiceIds { get; }

    public VoteSaved(Id pollId, IEnumerable<Id> choiceIds)
    {
      PollId = pollId;
      ChoiceIds = choiceIds;
    }
  }

  public class VoteNotSaved : ErrorEvent
  {
    public VoteNotSaved(Id id, string error, HttpStatusCode statusCode)
    {
      PollId = id;
      Error = error;
      StatusCode = statusCode;
    }
  }

}
