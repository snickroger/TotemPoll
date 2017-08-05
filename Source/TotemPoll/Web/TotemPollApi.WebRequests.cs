using Nancy.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Totem;
using Totem.Web;

namespace TotemPoll.Web
{
  public abstract class WebApiAnonymousRequest : WebApiRequest
  {
    public sealed override bool Authorize() => true;
  }

  public class VoteRequest : WebApiAnonymousRequest
  {
    public Id PollId;
    public Id ChoiceId;

    SavingVote Start() => new SavingVote(PollId, ChoiceId);

    void When(VoteSaved e) => RespondOK("Vote Successful");
    void When(VoteNotSaved e) => Respond(TotemPollApi.GetErrorResponse(e));
  }
}
