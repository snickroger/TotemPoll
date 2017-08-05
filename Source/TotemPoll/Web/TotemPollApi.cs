using System;
using System.IO;
using Nancy.Responses;
using Nancy.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Totem;
using Totem.Web;
using TotemPoll.Views;

namespace TotemPoll.Web
{
  public class TotemPollApi : WebApi
  {
    public TotemPollApi()
    {
      Get("/", _ => View["home.sshtml", new { PageTitle = "Totem Poll", HomeActive = true }]);

      Get<AllPollsView>("/api/polls");
      Get<SinglePollView>("/api/poll/{pollId}", a => a.pollId);
      Post<VoteRequest>("/api/poll/{pollId}/choice/{choiceId}/vote", request => ProcessVoteRequest(request, Context.Parameters.pollId.Value, Context.Parameters.choiceId.Value));
    }

    private static VoteRequest ProcessVoteRequest(VoteRequest req, string pollId, string choiceId)
    {
      req.PollId = Id.From(pollId);
      req.ChoiceId = Id.From(choiceId);
      return req;
    }

    public static TextResponse GetErrorResponse(ErrorEvent e)
    {
      var errorObject = new JObject { { "error", e.Error } };
      var errorStr = errorObject.ToString(Formatting.Indented);
      return new TextResponse(errorStr, "application/json") { StatusCode = e.StatusCode };
    }

    public static JObject ParseJsonBody(Stream requestBody)
    {
      string body;
      using (var sr = new StreamReader(requestBody))
      {
        body = sr.ReadToEnd();
      }

      if (string.IsNullOrWhiteSpace(body))
      {
        throw new FormatException("Request body must not be empty.");
      }

      try
      {
        return JObject.Parse(body);
      }
      catch (JsonException ex)
      {
        throw new FormatException("Could not parse JSON request body.", ex);
      }
    }

  }
}
