using System.Net;

namespace MinimalApi.Data {
    public class APIResponse {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public bool IsSuccess { get; set; } = false;
        public object Result { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
