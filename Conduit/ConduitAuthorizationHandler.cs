using Blazored.LocalStorage;
using System.Net;
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

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                // TODO: What to do if JWT can't be retrieved, redirect to login or ?
                var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                if(string.IsNullOrEmpty(savedToken))
                {
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                else
                {
                    request.Headers.Add("Authorization", $"Token {savedToken}");
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
