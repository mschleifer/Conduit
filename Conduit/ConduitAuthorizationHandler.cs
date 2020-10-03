using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Conduit
{
    // TBD if this is needed or if setting default headers in auth service/public client login does the trick
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
                request.Headers.Add("Authorization", $"Token {savedToken}");

            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
