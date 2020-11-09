using Blazored.LocalStorage;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    /// <summary>
    /// HttpMessageHandler that can be added to a named HttpClient at startup to retrieve JWT and place it in the 
    /// Authorization header of the request if it doesn't already exist
    /// </summary>
    public class ConduitAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public ConduitAuthorizationHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                // If the token cannot be retrieved, send the request anyway
                // We do this because some of Conduit's API endpoints are "token optional" e.g. GET /profiles/<username>
                // Calling that without a token returns the profile, calling with a token returns a Profile + info
                // about whether the current logged in user has favorited the profile
                // API will return a 401 if a token was required, we'll let the calling function deal with that
                var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrEmpty(savedToken))
                {
                    request.Headers.Add("Authorization", $"Token {savedToken}");
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
