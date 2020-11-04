using Conduit.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<ConduitApiResponse<Article>> GetArticle(string slug) {
            var response = await _httpClient.GetAsync($"api/articles/{slug}");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Article> result;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                result = JsonSerializer.Deserialize<ConduitApiResponse<Article>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.Success = false;

                return result;
            }

            // If successful, deserialize the response into an Article object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var articleObject = JsonExtensions.SearchJsonRoot<Article>(content, "article");
            result = new ConduitApiResponse<Article> { Success = true, ReponseObject = articleObject };

            return result;
        }

        public async Task<ConduitApiResponse<ArticleList>> GetArticles(string tag = null, string author = null, string favorited = null, int? limit = null, int? offset = null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(tag)) { query["tag"] = tag; }
            if (!string.IsNullOrEmpty(author)) { query["author"] = author; }
            if (!string.IsNullOrEmpty(favorited)) { query["favorited"] = favorited; }
            if (limit != null) { query["limit"] = limit?.ToString(); }
            if (offset != null) { query["offset"] = offset?.ToString(); }

            string queryString = $"?{query}";

            var response = await _httpClient.GetAsync($"api/articles{queryString}");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<ArticleList> result;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                result = JsonSerializer.Deserialize<ConduitApiResponse<ArticleList>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.Success = false;

                return result;
            }

            // If successful, deserialize the response into a ArticleList response object
            var articleListObject = JsonSerializer.Deserialize<ArticleList>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            result = new ConduitApiResponse<ArticleList> { Success = true, ReponseObject = articleListObject };

            return result;
        }

        public async Task<ConduitApiResponse<ArticleList>> GetFeed(int? limit = null, int? offset = null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (limit != null) { query["limit"] = limit?.ToString(); }
            if (offset != null) { query["offset"] = offset?.ToString(); }

            string queryString = $"?{query}";

            var response = await _httpClient.GetAsync($"api/articles/feed{queryString}");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<ArticleList> result;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                result = JsonSerializer.Deserialize<ConduitApiResponse<ArticleList>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.Success = false;

                return result;
            }

            // If successful, deserialize the response into a ArticleList response object
            var articleListObject = JsonSerializer.Deserialize<ArticleList>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            result = new ConduitApiResponse<ArticleList> { Success = true, ReponseObject = articleListObject };

            return result;
        }

        public async Task<ConduitApiResponse<Article>> FavoriteArticle(string articleSlug)
        {
            var response = await _httpClient.PostAsync($"api/articles/{articleSlug}/favorite", null);
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Article> apiRequestResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                apiRequestResult = JsonSerializer.Deserialize<ConduitApiResponse<Article>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiRequestResult.Success = false;

                return apiRequestResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var returnedObject = JsonExtensions.SearchJsonRoot<Article>(content, "article");
            apiRequestResult = new ConduitApiResponse<Article> { Success = true, ReponseObject = returnedObject };

            return apiRequestResult;
        }

        public async Task<ConduitApiResponse<Article>> UnfavoriteArticle(string articleSlug)
        {
            var response = await _httpClient.DeleteAsync($"api/articles/{articleSlug}/favorite");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Article> apiRequestResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                apiRequestResult = JsonSerializer.Deserialize<ConduitApiResponse<Article>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiRequestResult.Success = false;

                return apiRequestResult;
            }

            // If successful, deserialize the response into a User object
            // Ideally, we'd just use JsonSerializer.Deserialize on the reponse, but that doesn't work with our models, 
            // so the JsonExtensions method below parses the response for us
            var returnedObject = JsonExtensions.SearchJsonRoot<Article>(content, "article");
            apiRequestResult = new ConduitApiResponse<Article> { Success = true, ReponseObject = returnedObject };

            return apiRequestResult;
        }

        public async Task<ConduitApiResponse<List<Comment>>> GetComments(string slug)
        {
            var response = await _httpClient.GetAsync($"api/articles/{slug}/comments");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<List<Comment>> result;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                result = JsonSerializer.Deserialize<ConduitApiResponse<List<Comment>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                result.Success = false;

                return result;
            }

            // If successful, deserialize the response into a List<Comment> response object
            var commentListObject = JsonExtensions.SearchJsonRoot<List<Comment>>(content, "comments");
            result = new ConduitApiResponse<List<Comment>> { Success = true, ReponseObject = commentListObject };

            return result;
        }

        public async Task<ConduitApiResponse<Comment>> PostComment(string slug, Comment commentModel)
        {
            // Conduit API expects a specific JSON format, so wrap the comment data before serializing
            var dataWrapper = new { Comment = commentModel };

            var response = await _httpClient.PostAsJsonAsync($"api/articles/{slug}/comments/", dataWrapper);
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<Comment> apiRequestResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                apiRequestResult = JsonSerializer.Deserialize<ConduitApiResponse<Comment>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiRequestResult.Success = false;

                return apiRequestResult;
            }

            var commentObject = JsonExtensions.SearchJsonRoot<Comment>(content, "comment");
            apiRequestResult = new ConduitApiResponse<Comment> { Success = true, ReponseObject = commentObject };

            return apiRequestResult;
        }

        public async Task<ConduitApiResponse<bool>> DeleteComment(string slug, int commentId)
        {
            var response = await _httpClient.DeleteAsync($"api/articles/{slug}/comments/{commentId}");
            var content = await response.Content.ReadAsStringAsync();

            ConduitApiResponse<bool> apiRequestResult;

            if (!response.IsSuccessStatusCode)
            {
                // If the call fails, deserialize the response into the Errors field of ConduitApiResponse
                apiRequestResult = JsonSerializer.Deserialize<ConduitApiResponse<bool>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                apiRequestResult.Success = false;

                return apiRequestResult;
            }

            // This call doesn't return anything on success, so just wrap a bool and return that
            apiRequestResult = new ConduitApiResponse<bool> { Success = true, ReponseObject = true };

            return apiRequestResult;
        }

    }
}