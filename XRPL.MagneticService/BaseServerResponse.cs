using Newtonsoft.Json;

namespace XRPL.MagneticService
{
    public class BaseServerResponse<T>/*: IResponse*/
    {
        public HttpResponseMessage Response { get; set; }
        [JsonProperty("data")]
        public T Data { get; set; }
    }

    //public interface IResponse
    //{
    //    public HttpResponseMessage Response { get; set; }
    //}
}
