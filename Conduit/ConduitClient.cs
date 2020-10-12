using Conduit.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Conduit
{
    public class ConduitClient
    {
        private readonly HttpClient _httpClient;

        public ConduitClient(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<User> GetCurrentUser()
        {
            var response = await _httpClient.GetAsync("api/user");
            var content = await response.Content.ReadAsStringAsync();
            var userObject = JsonExtensions.SearchJsonRoot<User>(content, "user");

            return userObject;
        }

        public async Task<ConduitApiResponse<Profile>> GetProfile(string username)
        {
            var response = await _httpClient.GetAsync($"api/profiles/{username}");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Profile> result;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                result = JsonSerializer.Deserialize<ConduitApiResponse<Profile>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.Success = false;

                return result;
            }

            // If successful, deserialize the response into a Profile object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var profileObject = JsonExtensions.SearchJsonRoot<Profile>(content, "profile");
            result = new ConduitApiResponse<Profile> { Success = true, ReponseObject = profileObject };

            return result;
        }

        public async Task<ConduitApiResponse<User>> UpdateUser(Settings updateInfo)
        {
            // Conduit API expects a specific JSON format, so wrap the login data before serializing
            var dataWrapper = new { User = updateInfo };

            var response = await _httpClient.PutAsJsonAsync("api/user", dataWrapper);
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

        public async Task<ConduitApiResponse<Profile>> FollowUser(string username)
        {
            var response = await _httpClient.PostAsync($"api/profiles/{username}/follow", null);
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Profile> apiRequestResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                apiRequestResult = JsonSerializer.Deserialize<ConduitApiResponse<Profile>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiRequestResult.Success = false;

                return apiRequestResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var returnedObject = JsonExtensions.SearchJsonRoot<Profile>(content, "profile");
            apiRequestResult = new ConduitApiResponse<Profile> { Success = true, ReponseObject = returnedObject };

            return apiRequestResult;
        }

        public async Task<ConduitApiResponse<Profile>> UnfollowUser(string username)
        {
            var response = await _httpClient.DeleteAsync($"api/profiles/{username}/follow");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Profile> apiRequestResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                apiRequestResult = JsonSerializer.Deserialize<ConduitApiResponse<Profile>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiRequestResult.Success = false;

                return apiRequestResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var returnedObject = JsonExtensions.SearchJsonRoot<Profile>(content, "profile");
            apiRequestResult = new ConduitApiResponse<Profile> { Success = true, ReponseObject = returnedObject };

            return apiRequestResult;
        }
    }
}