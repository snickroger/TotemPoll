using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Nancy.Authentication.Basic;
using Totem;
using Totem.Http;
using Totem.Runtime;
using Totem.Web;

namespace TotemPoll.Web
{
  public interface IMockUserValidator : IUserValidator
  {
    ClaimsPrincipal Validate(HttpAuthorization header);
  }

  public class MockUserValidator : IMockUserValidator
  {
    public ClaimsPrincipal Validate(string username, string password)
    {
      if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
      {
        return null;
      }

      // any non-empty password will validate successfully in this mock validator
      var identity = new GenericIdentity(username);
      var principal = new ClaimsPrincipal(identity);

      return principal;
    }

    public ClaimsPrincipal Validate(HttpAuthorization header)
    {
      var encoding = Encoding.GetEncoding("iso-8859-1");
      var usernamePassword = encoding.GetString(Convert.FromBase64String(header.Credentials));
      var colonPos = usernamePassword.IndexOf(':');

      var username = usernamePassword.Substring(0, colonPos);
      var password = usernamePassword.Substring(colonPos + 1);

      return Validate(username, password);
    }
  }

  public class MockUserDb : IWebUserDb
  {
    private readonly IMockUserValidator _userValidator;

    public MockUserDb(IMockUserValidator userValidator)
    {
      _userValidator = userValidator;
    }

    public Task<User> Authenticate(HttpAuthorization header)
    {
      if (string.IsNullOrWhiteSpace(header.Credentials))
      {
        // unauthenticated user
        return Task.FromResult(new User());
      }

      var principal = _userValidator.Validate(header);

      if (principal?.Identity?.Name == null)
      {
        // unauthenticated user
        return Task.FromResult(new User());
      }

      var user = new User(Id.From(principal.Identity.Name), principal);
      return Task.FromResult(user);
    }
  }
}
