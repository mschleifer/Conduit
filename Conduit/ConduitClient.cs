using Conduit.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Conduit
{
    public class ConduitClient
    {
        private readonly HttpClient httpClient;

        public ConduitClient(HttpClient httpClient)
        {
            //client.BaseAddress = new Uri("https://api.github.com/");
            //// GitHub API versioning
            //client.DefaultRequestHeaders.Add("Accept",
            //    "application/vnd.github.v3+json");
            //// GitHub requires a user-agent
            //client.DefaultRequestHeaders.Add("User-Agent",
            //    "HttpClientFactory-Sample");
            this.httpClient = httpClient;
        }

        public async Task<User> GetCurrentUser()
        {
            var response = await httpClient.GetAsync("api/user");
            var content = await response.Content.ReadAsStringAsync();
            var userObject = JsonExtensions.SearchJsonRoot<User>(content, "user");

            return userObject;
        }

        public async Task<ConduitApiResponse<User>> UpdateUser(Settings updateInfo)
        {
            // Conduit API expects a specific JSON format, so wrap the login data before serializing
            var dataWrapper = new { User = updateInfo };

            var response = await httpClient.PutAsJsonAsync("api/user", dataWrapper);
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<User> updateResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                updateResult = JsonSerializer.Deserialize<ConduitApiResponse<User>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                updateResult.Success = false;

                return updateResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var userObject = JsonExtensions.SearchJsonRoot<User>(content, "user");
            updateResult = new ConduitApiResponse<User> { Success = true, ReponseObject = userObject };

            return updateResult;
        }

        public async Task<Profile> FollowUser(string username)
        {
            var response = await httpClient.PostAsJsonAsync("api/profiles/:username/follow", username);
            response.EnsureSuccessStatusCode();
            var profile = await response.Content.ReadFromJsonAsync<Profile>();
            return profile;
        }

        public async Task<Profile> UnfollowUser(string username)
        {
            var response = await httpClient.DeleteAsync($"api/profiles/{username}/follow");
            response.EnsureSuccessStatusCode();
            var profile = await response.Content.ReadFromJsonAsync<Profile>();
            return profile;
        }
    }
}