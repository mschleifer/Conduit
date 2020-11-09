using Blazored.LocalStorage;
using Conduit.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Conduit.Services
{
    /// <summary>
    /// Handles calls to Login and Register API endpoints as well as set up and tear down of local storage items
    /// </summary>
    /// <remarks>
    /// In conjunction with ConduitAuthenticationStateProvider see 
    /// https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity/
    /// </remarks>
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

        public async Task<ConduitApiResponse<User>> RegisterUser(Register registrationInfo)
        {
            // Conduit API expects a specific JSON format, so wrap the login data before serializing
            var dataWrapper = new { User = registrationInfo };

            var response = await _httpClient.PostAsJsonAsync("users", dataWrapper);
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<User> registerResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                registerResult = JsonSerializer.Deserialize<ConduitApiResponse<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                registerResult.Success = false;

                return registerResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var userObject = JsonExtensions.SearchJsonRoot<User>(content, "user");
            registerResult = new ConduitApiResponse<User> { Success = true, ReponseObject = userObject };

            return registerResult;
        }

        public async Task<ConduitApiResponse<User>> LogUserIn(Login loginModel)
        {
            // Conduit API expects a specific JSON format, so wrap the login data before serializing
            var dataWrapper = new { User = loginModel };

            var response = await _httpClient.PostAsJsonAsync("users/login", dataWrapper);
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<User> loginResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                loginResult = JsonSerializer.Deserialize<ConduitApiResponse<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                loginResult.Success = false;

                return loginResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var userObject = JsonExtensions.SearchJsonRoot<User>(content, "user");
            loginResult = new ConduitApiResponse<User> { Success = true, ReponseObject = userObject };            

            return loginResult;
        }

        /// <summary>
        /// Add the User's token (JWT) to local storage, mark the user authenticated with the auth state provider, and 
        /// add the bearer header on our default HttpClient so that the JWT is sent with all requests.
        /// </summary>
        public async Task SetupUserIdentity(User userModel)
        {
            await _localStorage.SetItemAsync("authToken", userModel.Token);
            await _localStorage.SetItemAsync("profileImage", userModel.Image);
            ((ConduitAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(userModel.Username, userModel.Image);
        }

        public async Task LogUserOut()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("profileImage");
            ((ConduitAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
