using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Conduit
{
    /// <summary>
    /// Provides a custom version of GetAuthenticationStateAsync for <CascadingAuthenticationState> as well as utility
    /// methods for setup and teardown of identity claims
    /// </summary>
    /// <remarks>
    ///  Based on the work at 
    ///  https://chrissainty.com/securing-your-blazor-apps-authentication-with-clientside-blazor-using-webapi-aspnet-core-identity
    /// /</remarks>
    public class ConduitAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ConduitAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        /// <summary>
        /// Check local storage for a JWT. If no token exists, return AuthenticationState with a blank claims prinicpal 
        /// (equivalent to saying that no user is logged in). Otherwise, set the JWT as the default auth header for HttpClient,
        /// extract the claims from the JWT, and return an AuthenticationState with those claims
        /// </summary>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            var profileImage = await _localStorage.GetItemAsync<string>("profileImage");

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            // Expected claims from Conduit JWT are "id", "username", and "exp"
            var claims = ParseClaimsFromJwt(savedToken);

            // Need to copy the "username" claim from Conduit JWT into a "Name" claim because .NET allows working with
            // the specific claim "Name" much more easily than "username" (i.e. User.Identity.Name)
            var username = claims.Single(x => x.Type == "username").Value;
            claims = claims.Append(new Claim(ClaimTypes.Name, username));

            if (!string.IsNullOrEmpty(profileImage))
            {
                claims = claims.Append(new Claim("ProfileImage", profileImage));
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt")));
        }

        /// <summary>
        ///  Constructs a new ClaimsPrincipal and corresponding AuthenticationState. Invokes
        ///  NotifyAuthenticationStateChanged() method which fires an event that will update any 
        ///  <CascadingAuthenticationState> components with the new AuthenticationState.
        /// </summary>
        public void MarkUserAsAuthenticated(string username, string profileImage)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username), new Claim("ProfileImage", profileImage) }, "apiauth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}