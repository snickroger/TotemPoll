using System;
using System.IO;
using System.Linq;
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
      Post<VoteRequest>("/api/poll/{pollId}/vote", request => ProcessVoteRequest(request, Request.Body, Context.Parameters.pollId.Value));
    }

    private static VoteRequest ProcessVoteRequest(VoteRequest req, Stream postBody, string pollId)
    {
      var jsonBody = ParseJsonBody<JObject>(postBody);
      req.PollId = Id.From(pollId);
      req.PostBody = jsonBody.ToString();
      return req;
    }

    public static TextResponse GetErrorResponse(ErrorEvent e)
    {
      var errorObject = new JObject { { "error", e.Error } };
      var errorStr = errorObject.ToString(Formatting.Indented);
      return new TextResponse(errorStr, "application/json") { StatusCode = e.StatusCode };
    }

    public static T ParseJsonBody<T>(Stream requestBody) where T : JContainer
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
        var result = JToken.Parse(body) as T;
        if (result == null)
        {
          throw new FormatException($"A JSON request body of type '{typeof(T).Name}' is required.");
        }
        return result;
      }
      catch (JsonException ex)
      {
        throw new FormatException("Could not parse JSON request body.", ex);
      }
    }

  }
}
