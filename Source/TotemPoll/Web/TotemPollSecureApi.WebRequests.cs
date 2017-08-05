using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totem;
using Totem.Web;

namespace TotemPoll.Web
{
  public abstract class WebApiSecureRequest : WebApiRequest
  {
    public string Username;
    public sealed override bool Authorize()
    {
      return !string.IsNullOrWhiteSpace(Username);
    }
  }

  public class NewPollRequest : WebApiSecureRequest
  {
    public string PostBody;

    CreatingPoll Start() => new CreatingPoll(PostBody, Username);

    void When(PollCreated e) => RespondCreated("Poll Created", e.CreatedPoll.Location);
    void When(PollNotCreated e) => Respond(TotemPollApi.GetErrorResponse(e));
  }

  public class DeletePollRequest : WebApiSecureRequest
  {
    public Id PollId;

    DeletingPoll Start() => new DeletingPoll(PollId, Username);

    void When(PollDeleted e) => RespondNoContent("Poll Deleted");
    void When(PollNotDeleted e) => Respond(TotemPollApi.GetErrorResponse(e));
  }
}
