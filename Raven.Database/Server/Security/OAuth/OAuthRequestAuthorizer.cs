using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Linq;
using Raven35.Abstractions.Data;
using Raven35.Database.Server.Controllers;

namespace Raven35.Database.Server.Security.OAuth
{
    public class OAuthRequestAuthorizer : AbstractRequestAuthorizer
    {
        public bool TryAuthorize(RavenBaseApiController controller, bool hasApiKey, bool ignoreDbAccess, out HttpResponseMessage msg)
        {
            var isGetRequest = IsGetRequest(controller);
            var allowUnauthenticatedUsers = // we need to auth even if we don't have to, for bundles that want the user 
                Settings.AnonymousUserAccessMode == AnonymousUserAccessMode.All ||
                Settings.AnonymousUserAccessMode == AnonymousUserAccessMode.Admin ||
                    Settings.AnonymousUserAccessMode == AnonymousUserAccessMode.Get &&
                    isGetRequest;

            var token = GetToken(controller);

            if (token == null)
            {
                if (allowUnauthenticatedUsers)
                {
                    msg = controller.GetEmptyMessage();
                    return true;
                }

                msg = WriteAuthorizationChallenge(controller, hasApiKey ? 412 : 401, "invalid_request", "The access token is required");

                return false;
            }

            AccessTokenBody tokenBody;
            if (!AccessToken.TryParseBody(Settings.OAuthTokenKey, token, out tokenBody))
            {
                if (allowUnauthenticatedUsers)
                {
                    msg = controller.GetEmptyMessage();
                    return true;
                }

                msg = WriteAuthorizationChallenge(controller, 401, "invalid_token", "The access token is invalid");

                return false;
            }

            if (tokenBody.IsExpired())
            {
                if (allowUnauthenticatedUsers)
                {
                    msg = controller.GetEmptyMessage();
                    return true;
                }

                msg = WriteAuthorizationChallenge(controller, 401, "invalid_token", "The access token is expired");

                return false;
            }

            var writeAccess = isGetRequest == false;
            if (!tokenBody.IsAuthorized(controller.ResourceName, writeAccess))
            {
                if (allowUnauthenticatedUsers || ignoreDbAccess)
                {
                    msg = controller.GetEmptyMessage();
                    return true;
                }

                msg = WriteAuthorizationChallenge(controller, 403, "insufficient_scope",
                    writeAccess ?
                    "Not authorized for read/write access for tenant " + controller.ResourceName :
                    "Not authorized for tenant " + controller.ResourceName);

                return false;
            }
            
            controller.User = new OAuthPrincipal(tokenBody, controller.ResourceName);
            CurrentOperationContext.User.Value = controller.User;
            msg = controller.GetEmptyMessage();

            return true;
        }

        public List<string> GetApprovedResources(IPrincipal user)
        {
            var oAuthUser = user as OAuthPrincipal;
            if (oAuthUser == null)
                return new List<string>();
            return oAuthUser.GetApprovedResources();
        }

        public override void Dispose()
        {

        }

        static string GetToken(RavenBaseApiController controller)
        {
            const string bearerPrefix = "Bearer ";

            var auth = controller.GetHeader("Authorization");
            if (auth == null)
            {
                auth = controller.GetCookie("OAuth-Token");
                if (auth != null)
                    auth = Uri.UnescapeDataString(auth);
            }
            if (auth == null || auth.Length <= bearerPrefix.Length ||
                !auth.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                return null;

            var token = auth.Substring(bearerPrefix.Length, auth.Length - bearerPrefix.Length);

            return token;
        }

        HttpResponseMessage WriteAuthorizationChallenge(RavenBaseApiController controller, int statusCode, string error, string errorDescription)
        {
            var msg = controller.GetEmptyMessage();
            var systemConfiguration = controller.SystemConfiguration;
            if (string.IsNullOrEmpty(systemConfiguration.OAuthTokenServer) == false)
            {
                if (systemConfiguration.UseDefaultOAuthTokenServer == false)
                {
                    controller.AddHeader("OAuth-Source", systemConfiguration.OAuthTokenServer, msg);
                }
                else
                {
                    controller.AddHeader("OAuth-Source", new UriBuilder(systemConfiguration.OAuthTokenServer)
                    {
                        Scheme = controller.InnerRequest.RequestUri.Scheme,
                        Host = controller.InnerRequest.RequestUri.Host,
                        Port = controller.InnerRequest.RequestUri.Port,
                    }.Uri.ToString(), msg);
                }
            }
            msg.StatusCode = (HttpStatusCode)statusCode;
 
            msg.Headers.Add("WWW-Authenticate", string.Format("Bearer realm=\"Raven\", error=\"{0}\",error_description=\"{1}\"", error, errorDescription));
            msg.Headers.Add("Access-Control-Expose-Headers", "WWW-Authenticate, OAuth-Source");
            return msg;
        }

        public IPrincipal GetUser(RavenBaseApiController controller, bool hasApiKey)
        {
            var token = GetToken(controller);

            if (token == null)
            {
                WriteAuthorizationChallenge(controller, hasApiKey ? 412 : 401, "invalid_request", "The access token is required");

                return null;
            }

            AccessTokenBody tokenBody;
            if (!AccessToken.TryParseBody(controller.DatabasesLandlord.SystemConfiguration.OAuthTokenKey, token, out tokenBody))
            {
                WriteAuthorizationChallenge(controller, 401, "invalid_token", "The access token is invalid");

                return null;
            }

            return new OAuthPrincipal(tokenBody, null);
        }
    }
}

public class OAuthPrincipal : IPrincipal, IIdentity
{
    private readonly AccessTokenBody tokenBody;
    private readonly string tenantId;

    public OAuthPrincipal(AccessTokenBody tokenBody, string tenantId)
    {
        this.tokenBody = tokenBody;
        this.tenantId = tenantId;
        AdminDatabases = new HashSet<string>(this.tokenBody.AuthorizedDatabases.Where(db => db.Admin).Select(db => db.TenantId));
        ReadOnlyDatabases = new HashSet<string>(this.tokenBody.AuthorizedDatabases.Where(db => db.ReadOnly).Select(db => db.TenantId));
        ReadWriteDatabases = new HashSet<string>(this.tokenBody.AuthorizedDatabases.Where(db => db.ReadOnly == false).Select(db => db.TenantId));
    }

    public bool IsInRole(string role)
    {
        if ("Administrators".Equals(role, StringComparison.OrdinalIgnoreCase) == false)
            return false;

        var databaseAccess = tokenBody.AuthorizedDatabases
            .Where(x =>
                string.Equals(x.TenantId, tenantId, StringComparison.OrdinalIgnoreCase) ||
                x.TenantId == "*");

        return databaseAccess.Any(access => access.Admin);
    }

    public IIdentity Identity
    {
        get { return this; }
    }

    public string Name
    {
        get { return tokenBody.UserId; }
    }

    public string AuthenticationType
    {
        get { return "OAuth"; }
    }

    public bool IsAuthenticated
    {
        get { return true; }
    }

    public List<string> GetApprovedResources()
    {
        return tokenBody.AuthorizedDatabases.Select(access => access.TenantId).ToList();
    }
    public AccessTokenBody TokenBody
    {
        get { return tokenBody; }
    }

    public bool IsGlobalAdmin()
    {
        var databaseAccess = tokenBody.AuthorizedDatabases
            .Where(x => string.Equals(x.TenantId, Constants.SystemDatabase, StringComparison.OrdinalIgnoreCase));

        return databaseAccess.Any(access => access.Admin);
    }
    public HashSet<string> AdminDatabases { get; private set; }
    public HashSet<string> ReadOnlyDatabases { get; private set; }
    public HashSet<string> ReadWriteDatabases { get; private set; }
}
