using System.IO;
using System.Security.Principal;
using Nancy.Security;
using Totem;
using Totem.Web;

namespace TotemPoll.Web
{
  public class TotemPollSecureApi : WebApi
  {
    public TotemPollSecureApi()
    {
      this.RequiresAuthentication();
      Get("/create", _ => View["create.sshtml", new { PageTitle = "Totem Poll | Create Poll" }]);

      Post<NewPollRequest>("/api/poll", request => ProcessNewPollRequest(request, Request.Body, Context.CurrentUser));
      Delete<DeletePollRequest>("/api/poll/{pollId}", request => ProcessDeletePollRequest(request, Context.Parameters.pollId.Value, Context.CurrentUser));
    }

    private static NewPollRequest ProcessNewPollRequest(NewPollRequest req, Stream postBody, IPrincipal principal)
    {
      var jsonBody = TotemPollApi.ParseJsonBody(postBody);
      req.PostBody = jsonBody.ToString();
      req.Username = principal?.Identity?.Name;
      return req;
    }

    private static DeletePollRequest ProcessDeletePollRequest(DeletePollRequest req, string pollId, IPrincipal principal)
    {
      req.PollId = Id.From(pollId);
      req.Username = principal?.Identity?.Name;
      return req;
    }
  }
}
