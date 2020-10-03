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

        public async Task<Article> GetArticle(string slug) =>
            await _httpClient.GetFromJsonAsync<Article>($"api/articles/:{slug}");

        public async Task<List<Article>> GetArticles() =>
            (await _httpClient.GetFromJsonAsync<ArticleList>("api/articles")).Articles;

        public async Task<List<string>> GetTags() =>
            (await _httpClient.GetFromJsonAsync<TagList>("api/tags")).Tags;

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
            _authenticationService.LogUserIn(registerResult.ReponseObject);

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
            _authenticationService.LogUserIn(loginResult.ReponseObject);

            return loginResult;
        }

        public async Task<Profile> GetProfile(string username) =>
            await _httpClient.GetFromJsonAsync<Profile>($"api/profiles/{username}");

    }
}
