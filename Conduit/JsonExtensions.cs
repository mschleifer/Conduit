using System.Text.Json;

namespace Conduit
{
    /// <summary>
    /// Methods for parsing the reponses returned by the Conduit API
    /// </summary>
    /// This could all be avoided by using Json.NET, but I wanted to try using System.Text.Json replacement for
    /// learning purposes. Another option would be to create a wrapper class for each of our models, but I disliked the 
    /// idea of bloating our model count just to accomodate the Conduit API.
    /// <remarks>
    /// </remarks>
    public static class JsonExtensions
    {
        /// <summary>
        /// Searches the root of the provided JSON for an element named <paramref name="jsonElementName"/> and 
        /// deserializes that element into an object of type T.
        /// </summary>
        public static T SearchJsonRoot<T>(string json, string jsonElementName)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            JsonElement userElement = root.GetProperty(jsonElementName);

            return userElement.ToObject<T>();
        }

        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static T ToObject<T>(this JsonDocument document)
        {
            var json = document.RootElement.GetRawText();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
