

namespace inventory.core.common
{
    public class CoreController : ControllerBase
    {
        protected string GetHeaderValue(string key)
        {
            if(Request.Headers.TryGetValue(key, out StringValues value))
            {
                IEnumerable<string> headerDetails = (IEnumerable<string>)value;
                return headerDetails.First();
            }
            return value;
        }

        private string ClientIdHeaderValue
        {
            get { return GetHeaderValue("ClientId"); }
        }

        private async Task<T> GetRequest<T>() where T : class
        {
            T requestObject;

            using(StreamReader reader = new StreamReader(Request.Body))
            {
                string rawRequest = await reader.ReadToEndAsync();
                requestObject = rawRequest as T;
            }

            return requestObject;
        }

        private async Task<T> GetRequest<T>(T request) where T : class
        {
            if (request == null)
            {
                request = await GetRequest<T>();

                if (request == null)
                {
                    throw new System.Exception();
                }
            }

            if (request is RequestBase)
            {
                (request as RequestBase).ClientId = ClientIdHeaderValue;
            }

            return request;
        }

        protected async Task<T> ValidateRequest<T>(T request) where T : class
        {
            request = await GetRequest<T>(request);
            return request;
        }

        protected async Task<string> GetRawRequest()
        {
            using(StreamReader reader = new StreamReader(Request.Body))
            {
                return await reader.ReadToEndAsync();
            }
        }

        protected async Task<ObjectResult> CreateResponse<T>(HttpStatusCode statusCode, T responseObject) where T : ResponseBase
        {
            responseObject.HttpStatusCode = statusCode;

            return new ObjectResult(responseObject);
        }
    
        
    }
}