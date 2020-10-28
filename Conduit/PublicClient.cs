using Blazored.LocalStorage;
using Conduit.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Conduit
{
    /// <summary>
    /// For requests that do not require authentication
    /// </summary>
    public class PublicClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authenticationService;
        private readonly ILocalStorageService _localStorage;

        public PublicClient(HttpClient httpClient,
                           AuthService authenticationService,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationService = authenticationService;
            _localStorage = localStorage;
        }

        //Others
        // 401 - unauth
        // 403 - forbidden
        // 404 - not found
        // 500 - internal error (undefined in documentation, but expect)

        public async Task<ConduitApiResponse<List<string>>> GetTags()
        {
            var response = await _httpClient.GetAsync("api/tags");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<List<string>> result;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                result = JsonSerializer.Deserialize<ConduitApiResponse<List<string>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.Success = false;

                return result;
            }

            // If successful, deserialize the response into a Profile object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var profileObject = JsonExtensions.SearchJsonRoot<List<string>>(content, "tags");
            result = new ConduitApiResponse<List<string>> { Success = true, ReponseObject = profileObject };

            return result;
        }

        public async Task<ConduitApiResponse<User>> RegisterUser(Register registrationInfo)
        {
            // Conduit API expects a specific JSON format, so wrap the login data before serializing
            var dataWrapper = new { User = registrationInfo };

            var response = await _httpClient.PostAsJsonAsync("api/users", dataWrapper);
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

            // Set the app up to make use of the returned JWT
            await _authenticationService.LogUserIn(registerResult.ReponseObject);

            return registerResult;
        }

        public async Task<ConduitApiResponse<User>> Login(Login loginModel)
        {
            // Conduit API expects a specific JSON format, so wrap the login data before serializing
            var dataWrapper = new { User = loginModel };

            var response = await _httpClient.PostAsJsonAsync("api/users/login", dataWrapper);
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

            // Set the app up to make use of the returned JWT
            await _authenticationService.LogUserIn(loginResult.ReponseObject);

            return loginResult;
        }
    }
}
