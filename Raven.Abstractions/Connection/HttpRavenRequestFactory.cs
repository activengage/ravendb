#if !DNXCORE50
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Raven35.Abstractions.Data;
using Raven35.Abstractions.Extensions;
using Raven35.Abstractions.OAuth;

namespace Raven35.Abstractions.Connection
{
    [Obsolete]
    public class HttpRavenRequestFactory
    {
        public int? RequestTimeoutInMs { get; set; }

        public X509Certificate2 Certificate { get; set; }

        readonly ConcurrentDictionary<Tuple<string, string>, AbstractAuthenticator> authenticators = new ConcurrentDictionary<Tuple<string, string>, AbstractAuthenticator>();

        private static bool setSecurityProtocol;

        public void ConfigureRequest(RavenConnectionStringOptions options, HttpWebRequest request)
        {
            if (RequestTimeoutInMs.HasValue)
                request.Timeout = RequestTimeoutInMs.Value;

            if (options.ApiKey == null)
            {
                ICredentials credentialsToUse = CredentialCache.DefaultNetworkCredentials;
                if (options.Credentials != null)
                {
                    var networkCredentials = options.Credentials as NetworkCredential;
                    if (networkCredentials != null && options.AuthenticationScheme != null)
                    {
                        var credentialCache = new CredentialCache();
                        var uri = new Uri(options.Url);
                        credentialCache.Add(new Uri(string.Format("{0}://{1}:{2}/", uri.Scheme, uri.Host, uri.Port)), options.AuthenticationScheme, networkCredentials);

                        credentialsToUse = credentialCache;
                    }
                    else
                    {
                        credentialsToUse = options.Credentials;
                    }
                }

                if (Certificate != null)
                {
                    if (setSecurityProtocol == false)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        setSecurityProtocol = true;
                    }

                    request.ClientCertificates = new X509Certificate2Collection(Certificate);
                }

                request.Credentials = credentialsToUse;
                return;
            }

            var webRequestEventArgs = new WebRequestEventArgs { Request = request, Credentials = new OperationCredentials(options.ApiKey, options.Credentials)};

            AbstractAuthenticator existingAuthenticator;
            if (authenticators.TryGetValue(GetCacheKey(options), out existingAuthenticator))
            {
                existingAuthenticator.ConfigureRequest(this, webRequestEventArgs);
            }
            else
            {
                var basicAuthenticator = new BasicAuthenticator(enableBasicAuthenticationOverUnsecuredHttp: false);
                var securedAuthenticator = new SecuredAuthenticator(autoRefreshToken: false);

                basicAuthenticator.ConfigureRequest(this, webRequestEventArgs);
                securedAuthenticator.ConfigureRequest(this, webRequestEventArgs);
            }
        }

        private static Tuple<string, string> GetCacheKey(RavenConnectionStringOptions options)
        {
            return Tuple.Create(options.Url, options.ApiKey);
        }

        public HttpRavenRequest Create(string url, HttpMethod httpMethod, RavenConnectionStringOptions connectionStringOptions, bool? allowWriteStreamBuffering = null)
        {
            return new HttpRavenRequest(url, httpMethod, ConfigureRequest, HandleUnauthorizedResponse, connectionStringOptions, allowWriteStreamBuffering);
        }

        private Action<HttpWebRequest> HandleUnauthorizedResponse(RavenConnectionStringOptions options, WebResponse webResponse)
        {
            if (options.ApiKey == null)
                return null;

            var oauthSource = webResponse.Headers["OAuth-Source"];

            var useBasicAuthenticator =
                string.IsNullOrEmpty(oauthSource) == false &&
                oauthSource.EndsWith("/OAuth/API-Key", StringComparison.CurrentCultureIgnoreCase) == false;

            if (string.IsNullOrEmpty(oauthSource))
                oauthSource = options.Url + "/OAuth/API-Key";

            var authenticator = authenticators.GetOrAdd(
                GetCacheKey(options),
                _ =>
                {
                    if (useBasicAuthenticator)
                    {
                        return new BasicAuthenticator(enableBasicAuthenticationOverUnsecuredHttp: false);
                    }

                    return new SecuredAuthenticator(autoRefreshToken: false);
                });

            return authenticator.DoOAuthRequest(oauthSource, options.ApiKey);
        }

        public static IDisposable Expect100Continue(string url)
        {
            var servicePoint = ServicePointManager.FindServicePoint(new Uri(url));
            servicePoint.Expect100Continue = true;
            return new DisposableAction(() => servicePoint.Expect100Continue = false);
        }
    }
}
#endif