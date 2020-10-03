using System.Collections.Generic;

namespace Conduit.Models
{
    /// <summary>
    /// Wrapper class for the responses returned by the Conduit API. If the call is successful, ResponseObject holds
    /// the result otherwise Errors will contain any validation or server errors passed.
    /// </summary>
    public class ConduitApiResponse<T>
    {
        public bool Success { get; set; }
        public T ReponseObject { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}
