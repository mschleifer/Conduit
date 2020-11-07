using Blazored.LocalStorage;
using Conduit.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// TODO: currently replicating login on the public client, but we should probably be using this to do all the token management stuff
namespace Conduit
{
    /// <summary>
    /// https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/
    /// </summary>
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        /// <summary>
        /// Add the User's token (JWT) to local storage, mark the user authenticated with the auth state provider, and 
        /// add the bearer header on our default HttpClient so that the JWT is sent with all requests.
        /// </summary>
        public async Task LogUserIn(User userModel)
        {
            await _localStorage.SetItemAsync("authToken", userModel.Token);
            await _localStorage.SetItemAsync("profileImage", userModel.Image);
            ((ConduitAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(userModel.Username, userModel.Image);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", userModel.Token);
        }

        public async Task LogUserOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ConduitAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
